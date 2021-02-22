using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using WYUN.Queries;
using WYUN.Deserialization;

public class RoomInfo : MonoBehaviour, WYUN.IRoomCallback
{
    CreateQuery option, newOption;
    MemberList memberList;
    private void Awake()
    {
        WYUN.Core.AddRoomCallback(this);
    }
    Text roomName, memberNum;
    // Start is called before the first frame update
    void Start()
    {
        roomName = transform.Find("RoomName").GetComponent<Text>();
        memberNum = transform.Find("MemberNum").GetComponent<Text>();
        newOption = option = new CreateQuery("", 0, "");
        memberList = new MemberList();
        WYUN.Core.RequestRoomOption();
        WYUN.Core.RequestRoomMember();
    }

    // Update is called once per frame
    void Update()
    {
        if (option != newOption)
        {
            roomName.text = newOption.name;
            memberNum.text = memberNum.text.Split('/')[0] + "/" + newOption.limit;
            option = newOption;
        }
        if (!memberNum.text.Split('/')[0].Equals(memberList.members.Count.ToString()))
        {
            memberNum.text = memberList.members.Count.ToString() + "/" + memberNum.text.Split('/')[1];
        }
    }
    public void LeftRoom() { }
    public void UpdatedRoomMember(string member)
    {
        memberList = JsonUtility.FromJson<MemberList>(member);
        Debug.Log("memberNum: " + memberList.members.Count);
    }
    public void JoinedRoom() { }
    public void UpdatedRoomOption(string o)
    {
        newOption = JsonUtility.FromJson<CreateQuery>(o);
    }
    public void ServerError(string message) { }
    public void MessageReceived(string msg) { }
}
