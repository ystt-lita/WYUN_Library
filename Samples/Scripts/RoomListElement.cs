using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListElement : MonoBehaviour
{
    Text roomName, roomOwner, memberLimit;
    Toggle select;
    // Start is called before the first frame update
    void Awake()
    {
        roomName = transform.Find("RoomName/Text").GetComponent<Text>();
        roomOwner = transform.Find("OwnerName/Text").GetComponent<Text>();
        memberLimit = transform.Find("MemberLimit/Text").GetComponent<Text>();
        select = transform.Find("Select").GetComponent<Toggle>();
    }
    public string GetRoomName()
    {
        return roomName.text;
    }
    public string GetRoomOwner()
    {
        return roomOwner.text;
    }
    public long GetMemberLimit()
    {
        return long.Parse(memberLimit.text);
    }
    public void CreateRoom(string n, long l, string o)
    {
        roomName.text = n;
        roomOwner.text = o;
        memberLimit.text = l.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
