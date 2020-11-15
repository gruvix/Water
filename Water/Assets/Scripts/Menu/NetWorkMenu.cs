using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class NetWorkMenu : MonoBehaviour
{
    public NetworkManager manager;
    public TMPro.TMP_InputField ipField;

    public void UpdateText()
    {
        manager.networkAddress = ipField.text;
        manager.isMenu = true;
    }
}
