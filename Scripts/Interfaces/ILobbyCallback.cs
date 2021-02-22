namespace WYUN
{
    public interface ILobbyCallback
    {
        void ServerError(string message);
        void UpdatedLobbyMember(string mList);
        void UpdatedRoomList(string rList);
        void JoinedLobby();
        void LeftLobby();
    }
}