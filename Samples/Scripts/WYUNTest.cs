using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WYUNTest : MonoBehaviour, WYUN.ILobbyCallback, WYUN.IRoomCallback
{
    Stack<Scene> sceneHistory;
    string sceneLoad;
    // Start is called before the first frame update
    private void Awake()
    {
        sceneLoad = "";
        sceneHistory = new Stack<Scene>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("WYUN/Samples/Scenes/ConnectScene", LoadSceneMode.Additive);
    }
    private void OnDestroy()
    {
        WYUN.Core.Exit();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        sceneHistory.Push(scene);
    }
    void Start()
    {
        WYUN.Core.AddLobbyCallback(this);
        WYUN.Core.AddRoomCallback(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneLoad.Length > 0)
        {
            SceneManager.UnloadSceneAsync(sceneHistory.Pop());
            SceneManager.LoadScene(sceneLoad, LoadSceneMode.Additive);
            sceneLoad = "";
        }
    }

    public void JoinedLobby()
    {
        sceneLoad = "WYUN/Samples/Scenes/LobbyScene";
        Debug.Log("Joined Lobby");
    }
    public void LeftLobby()
    {
        Debug.Log("Left Lobby");
    }
    public void UpdatedRoomList(string rList)
    {
        Debug.Log("Updated RoomList: " + rList);
    }
    public void UpdatedLobbyMember(string mList)
    {
        Debug.Log("Updated LobbyMember: " + mList);
    }
    public void JoinedRoom()
    {
        sceneLoad = "WYUN/Samples/Scenes/RoomScene";
        Debug.Log("Joined Room");
    }
    public void UpdatedRoomOption(string o)
    {
        Debug.Log("Updated RoomOption: " + o);
    }
    public void LeftRoom()
    {
        Debug.Log("Left Room");
    }
    public void UpdatedRoomMember(string members)
    {
        Debug.Log("Updated RoomMember: " + members);
    }
    public void MessageReceived(string msg) { }
    public void ServerError(string message) { }
}
