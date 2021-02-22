using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyButton : MonoBehaviour
{
    [SerializeField]
    InputField rName, oName, lNum;
    [SerializeField]
    Dropdown rComp, oComp, lComp;
    [SerializeField]
    RoomList list;
    [SerializeField]
    ToggleGroup roomSelection;
    private void Awake()
    {
    }
    public void Create()
    {
        if (rComp.value != 1)
        {
            Debug.Log("on Create RoomName comparing method must be Equals");
            return;
        }
        if (lComp.value != 1)
        {
            Debug.Log("on Create MemberLimit comparing method must be Equals");
            return;
        }
        WYUN.Core.CreateAndJoinRoom(rName.text, long.Parse(lNum.text));
    }
    public void Reflesh()
    {
        list.RefineRoomList(rName.text, rComp.value, oName.text, oComp.value, long.Parse(lNum.text.Length > 0 ? lNum.text : "0"), lComp.value - 1);
    }
    public void Join()
    {
        foreach (var item in roomSelection.ActiveToggles())
        {
            WYUN.Core.JoinRoom(item.transform.parent.GetComponent<RoomListElement>().GetRoomName());
            return;
        }
        Debug.Log("on Join any of the rooms must be selected");
    }
}
