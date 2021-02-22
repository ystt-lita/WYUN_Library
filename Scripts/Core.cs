using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Utf8Json;
using Utf8Json.Resolvers;
using System;

namespace WYUN
{
    public class ServerState
    {
        public bool inLobby { get; internal set; }
        public bool inRoom { get; internal set; }
        public bool isConnected { get; internal set; }
        public string room
        {
            get { if (inRoom) return room; return null; }
            internal set { room = value; }
        }
    }
    public static class Core
    {
        public static ServerState state;
        public static AppSettings settings;
        static List<ILobbyCallback> lobbies;
        static List<IRoomCallback> rooms;
        static Socket server;
        static Thread Consumer;
        static Core()
        {
            JsonSerializer.SetDefaultResolver(StandardResolver.ExcludeNull);
            state = new ServerState();
            state.isConnected = false;
            state.inRoom = false;
            state.inLobby = false;
            lobbies = new List<ILobbyCallback>();
            rooms = new List<IRoomCallback>();
        }

        public static void AddLobbyCallback(ILobbyCallback l)
        {
            lobbies.Add(l);
        }
        public static void AddRoomCallback(IRoomCallback r)
        {
            rooms.Add(r);
        }
        static void Consume()
        {
            char[] split = { '\r', '\n' };
            int len;
            byte[] buff = new byte[1024];
            string[] received;
            while (state.isConnected)
            {
                len = server.Receive(buff);
                received = Encoding.UTF8.GetString(buff, 0, len).Split(split, System.StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in received)
                {
                    switch (item.Split(',')[0])
                    {
                        /*v Lobby Messages v*/
                        case "joinedLobby":
                            state.inLobby = true;
                            foreach (ILobbyCallback e in lobbies)
                            {
                                e.JoinedLobby();
                            }
                            break;
                        case "roomList":
                            if (!state.inLobby)
                            {
                                //サーバー側とステートがずれている
                                throw new ApplicationException("didnt received joinedLobby but received roomList.");
                            }
                            foreach (ILobbyCallback e in lobbies)
                            {
                                e.UpdatedRoomList(item.Substring(9));
                            }
                            break;
                        case "leftLobby":
                            state.inLobby = false;
                            foreach (ILobbyCallback e in lobbies)
                            {
                                e.LeftLobby();
                            }
                            break;
                        case "lobbyMember":
                            if (!state.inLobby)
                            {
                                //ずれてる
                                throw new ApplicationException("didnt received joinedLobby but received lobbyMember.");
                            }
                            foreach (ILobbyCallback e in lobbies)
                            {
                                e.UpdatedLobbyMember(item.Substring(12));
                            }
                            break;
                        /*^ Lobby Messages ^*/
                        /*v Room Messages v*/
                        case "joinedRoom":
                            state.inRoom = true;
                            foreach (IRoomCallback e in rooms)
                            {
                                e.JoinedRoom();
                            }
                            break;
                        case "roomOption":
                            if (!state.inRoom)
                            {
                                //ずれてる
                                throw new ApplicationException("didnt received JoinedRoom but received roomOption.");
                            }
                            foreach (IRoomCallback e in rooms)
                            {
                                e.UpdatedRoomOption(item.Substring(11));
                            }
                            break;
                        case "roomMember":
                            if (!state.inRoom)
                            {
                                //ずれてる
                                throw new ApplicationException("didnt received JoinedRoom but received roomMember.");
                            }
                            foreach (IRoomCallback e in rooms)
                            {
                                e.UpdatedRoomMember(item.Substring(11));
                            }
                            break;
                        case "leftRoom":
                            state.inRoom = false;
                            foreach (IRoomCallback e in rooms)
                            {
                                e.LeftRoom();
                            }
                            break;
                        case "tell":
                            if (!state.inRoom)
                            {
                                //ずれてる
                                throw new ApplicationException("didnt received JoinedRoom but received tell.");
                            }
                            foreach (IRoomCallback e in rooms)
                            {
                                e.MessageReceived(item.Substring(5));
                            }
                            break;
                        /*^ Room Messages ^*/
                        case "error":
                            if (state.inLobby)
                            {
                                foreach (ILobbyCallback e in lobbies)
                                {
                                    e.ServerError(item.Substring(6));
                                }
                            }
                            else if (state.inRoom)
                            {
                                foreach (IRoomCallback e in rooms)
                                {
                                    e.ServerError(item.Substring(6));
                                }
                            }
                            else
                            {
                                //UnityEngine.Debug.Log("Server returned error: " + item.Substring(6));
                            }
                            break;
                        case "exit":
                            state.isConnected = false;
                            return;
                    }
                }
            }
        }
        public static void Connect()
        {
            Connect(settings);
        }
        public static void Connect(AppSettings s)
        {
            settings = s;
            IPHostEntry ipInfo = Dns.GetHostEntry(Dns.GetHostAddresses(s.ip)[0]);
            server = new Socket(ipInfo.AddressList[0].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipInfo.AddressList[0], s.port);
            byte[] buff = new byte[1024]; server.Receive(buff);
            string received = Encoding.UTF8.GetString(buff).Split('\r')[0];
            if (!received.Equals("connected"))
            {
                //UnityEngine.Debug.LogError("something went wrong: serve sent " + received);
                return;
            }
            server.Send(Encoding.UTF8.GetBytes(s.userName + "\r\n"));
            server.Send(Encoding.UTF8.GetBytes(s.appID.ToString() + "\r\n"));
            state.isConnected = true;
            Consumer = new Thread(Consume);
            Consumer.Start();
        }
        public static void CreateAndJoinRoom(string name, long limit)
        {
            if (!state.inLobby)
            {
                //ロビーにいないのにルームに入ろうとしている
                throw new ApplicationException("you are not in lobby but are trying to join room");
            }
            byte[] message = JsonSerializer.Serialize(new Queries.CreateQuery(name, limit, settings.userName, true));
            //string message = UnityEngine.JsonUtility.ToJson(new Queries.CreateQuery(name, limit, settings.userName)).ToString().Insert(1, "\"query\":\"create\",") + "\r\n";
            server.Send(message);
            server.Send(new byte[] { 13, 10 });
        }
        public static void JoinRoom(string name)
        {
            if (!state.inLobby)
            {
                //ロビーにいない
                throw new ApplicationException("you are not in lobby but are trying to join room");
            }
            byte[] message = JsonSerializer.Serialize(new Queries.JoinQuery(name, true));
            server.Send(message);
            server.Send(new byte[] { 13, 10 });
        }
        public static void RefleshRoomList()
        {
            if (!state.inLobby)
            {
                //ロビーにいない
                throw new ApplicationException("you are not in lobby but are trying to get roomList");
            }
            server.Send(Encoding.UTF8.GetBytes("{\"query\":\"roomList\"}\r\n"));
        }
        public static void RequestRoomOption()
        {
            if (!state.inRoom)
            {
                //ルームにいない
                throw new ApplicationException("you are not in room but are trying to get current roomOption");
            }
            server.Send(Encoding.UTF8.GetBytes("{\"query\":\"roomOption\"}\r\n"));
        }
        public static void RequestRoomMember()
        {
            if (!state.inRoom)
            {
                //ルームにいない
                throw new ApplicationException("you are not in room but are trying to get roomMember");
            }
            server.Send(Encoding.UTF8.GetBytes("{\"query\":\"roomMember\"}\r\n"));
        }
        public static void LeaveRoom()
        {
            if (!state.inRoom)
            {
                //ルームにいない
                throw new ApplicationException("you are not in room but are trying to leave room");
            }
            server.Send(Encoding.UTF8.GetBytes("{\"query\":\"leave\"}\r\n"));
        }
        public static void Tell(string name, string message)
        {
            if (!state.inRoom)
            {
                //ルームにいない
                throw new ApplicationException("you are not in room but are trying to tell others");
            }
            server.Send(Encoding.UTF8.GetBytes("{\"query\":\"tell\",\"to\":\"" + name + "\",\"body\":\"" + message + "\"}\r\n"));
        }
        public static void Broad(string message)
        {
            if (!state.inRoom)
            {
                //ルームにいない
                throw new ApplicationException("you are not in room but are trying to tell others");
            }
            server.Send(Encoding.UTF8.GetBytes("{\"query\":\"broad\",\"body\":\"" + message + "\"}\r\n"));
        }
        public static void Exit()
        {
            if (state.isConnected)
            {
                server.Send(Encoding.UTF8.GetBytes("{\"query\":\"exit\"}\r\n"));
                Consumer.Join();
                server.Send(Encoding.UTF8.GetBytes("exit\r\n"));
                server.Close();

            }
        }
    }
}
