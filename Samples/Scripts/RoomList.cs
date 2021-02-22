using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using rList = WYUN.Deserialization.RoomList;

public class RoomList : MonoBehaviour, WYUN.ILobbyCallback
{
    string rname;
    int nStrategy;
    string owner;
    int oStrategy;
    long limit;
    int lStrategy;
    public GameObject template;
    [SerializeField]
    rList list;
    rList newList;
    List<RoomListElement> roomListElements;
    // Start is called before the first frame update
    private void Awake()
    {
        WYUN.Core.AddLobbyCallback(this);
        rname = ""; nStrategy = 0; owner = ""; oStrategy = 0; limit = 0; lStrategy = 0;
    }
    void Start()
    {
        newList = list = new rList();
        roomListElements = new List<RoomListElement>();
        WYUN.Core.RefleshRoomList();
    }


    // Update is called once per frame
    void Update()
    {
        if (list != newList)
        {
            foreach (var item in list.rooms)
            {
                if (!newList.rooms.Contains(item))//古いのにあって新しいのになかったら削除された
                {
                    Debug.Log("room deleted");
                    var deleted = roomListElements.Find(e => e.GetRoomName().Equals(item.name));
                    roomListElements.Remove(deleted);
                    Destroy(deleted.gameObject);
                }
            }
            foreach (var item in newList.rooms)
            {
                if (!list.rooms.Contains(item))//新しいのにあって古いのになかったら追加された
                {
                    Debug.Log("new room appeared: " + item.name);
                    var tmp = Instantiate(template, transform).GetComponent<RoomListElement>();
                    tmp.transform.Find("Select").GetComponent<Toggle>().group = GetComponent<ToggleGroup>();
                    tmp.CreateRoom(item.name, item.limit, item.owner);
                    roomListElements.Add(tmp);
                }
            }
            list = newList;
        }
    }
    public void RefineRoomList(string n, int ns, string o, int os, long l, int ls)
    {
        Debug.Log("RefineQuery: " + n + "," + o + "," + l);
        rname = n; nStrategy = ns; owner = o; oStrategy = os; limit = l; lStrategy = ls;
        WYUN.Core.RefleshRoomList();
    }

    public void UpdatedLobbyMember(string mList) { }
    public void UpdatedRoomList(string rList)
    {
        newList = JsonUtility.FromJson<rList>(rList);
        if (rname.Length != 0)
        {
            if (nStrategy == 0)
            {
                newList.rooms.RemoveAll(e => !e.name.Contains(rname));//含まないやつを削除
            }
            else if (nStrategy == 1)
            {
                newList.rooms.RemoveAll(e => !e.name.Equals(rname));//一致しないやつを削除
            }
        }
        if (owner.Length != 0)
        {
            if (oStrategy == 0)
            {
                newList.rooms.RemoveAll(e => !e.owner.Contains(owner));//含まないやつを削除
            }
            else if (oStrategy == 1)
            {
                newList.rooms.RemoveAll(e => !e.owner.Equals(owner));//一致しないやつを削除
            }
        }
        if (limit != 0)
        {
            newList.rooms.RemoveAll(e => System.Math.Sign(e.limit - limit) != lStrategy);
        }
        Debug.Log(JsonUtility.ToJson(newList).ToString());
    }
    public void JoinedLobby() { }
    public void LeftLobby() { }
    public void ServerError(string message) { }
}
