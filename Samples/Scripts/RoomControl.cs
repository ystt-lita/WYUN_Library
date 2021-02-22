using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using WYUN.Deserialization;

public class RoomControl : MonoBehaviour, WYUN.IRoomCallback
{
    MemberList memberList, newMemberList;
    Text messageArea;
    InputField messageField;
    Dropdown tellTo;
    Queue<string> textToAdd;
    // Start is called before the first frame update
    void Awake()
    {
        WYUN.Core.AddRoomCallback(this);
        tellTo = transform.Find("TellTo").GetComponent<Dropdown>();
        messageArea = transform.parent.Find("DraggingArea/MessageView/Viewport/Content/Text").GetComponent<Text>();
        messageField = transform.Find("Message").GetComponent<InputField>();
        memberList = newMemberList = new MemberList();
        textToAdd = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (memberList != newMemberList)
        {
            foreach (var item in memberList.members)
            {
                if (!newMemberList.members.Contains(item))
                {
                    textToAdd.Enqueue(item.name + "さんが退出しました");
                    tellTo.options.Remove(tellTo.options.Find(match => match.text.Equals(item.name)));
                }
            }
            foreach (var item in newMemberList.members)
            {
                if (!memberList.members.Contains(item))
                {
                    textToAdd.Enqueue(item.name + "さんが参加しました");
                    if (item.name != WYUN.Core.settings.userName)
                    {
                        tellTo.options.Add(new Dropdown.OptionData(item.name));
                    }
                }
            }
            memberList = newMemberList;
        }
        while (textToAdd.Count > 0)
        {
            messageArea.text += "\n" + textToAdd.Dequeue();
        }
    }
    public void Exit()
    {
        WYUN.Core.LeaveRoom();
    }
    public void Submit()
    {
        string message = messageField.text;
        if (tellTo.value != 0)
        {
            WYUN.Core.Tell(tellTo.captionText.text, message);
        }
        else
        {
            WYUN.Core.Broad(message);
        }
    }

    public void JoinedRoom() { }
    public void LeftRoom() { }
    public void UpdatedRoomMember(string list)
    {
        newMemberList = UnityEngine.JsonUtility.FromJson<MemberList>(list);
        Debug.Log("newMemberListCount: " + newMemberList.members.Count);
    }
    public void UpdatedRoomOption(string list) { }
    public void MessageReceived(string msg)
    {
        textToAdd.Enqueue(msg.Split(',')[0] + ":" + msg.Split(',')[1]);
    }
    public void ServerError(string message) { }
}
