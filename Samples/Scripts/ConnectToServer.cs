using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConnectToServer : MonoBehaviour
{
    [SerializeField]
    InputField field_IP;
    [SerializeField]
    InputField field_name;
    // Start is called before the first frame update
    public void OnClick()
    {
        WYUN.Core.Connect(new WYUN.AppSettings(field_IP.text, 10100, field_name.text));
    }
}
