using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Chat : NetworkBehaviour
{
    public TMP_InputField TMP_ChatInput;
    public TMP_Text TMP_ChatOutput;


    void OnEnable()
    {
        TMP_ChatInput.onSubmit.AddListener(AddToChatOutput);

    }

    void OnDisable()
    {
        TMP_ChatInput.onSubmit.RemoveListener(AddToChatOutput);

    }

    void AddToChatOutput(string newText)
    {
        // Clear Input Field
        TMP_ChatInput.text = string.Empty;

        var timeNow = System.DateTime.Now;

        string message = "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" + timeNow.Second.ToString("d2") + "</color>] " + "[" + NetworkManager.singleton.userName + "]" + newText + "\n";

        CmdSendMessage(message);

        TMP_ChatInput.ActivateInputField();
    }

    [Command(ignoreAuthority = true)]
    private void CmdSendMessage(string message)
	{
        RpcHandleMessage(message);
	}

    [ClientRpc]
    private void RpcHandleMessage(string message)
	{
        TMP_ChatOutput.text += message;
    }

}
