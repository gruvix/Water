using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Chat : NetworkBehaviour
{
    public TMP_InputField TMP_ChatInput;
    public TMP_Text TMP_ChatOutput;
    public LobbyHandler lobby;

    void OnEnable()
    {
        TMP_ChatInput.onSubmit.AddListener(AddToChatOutput);
    }

    void OnDisable()
    {
        TMP_ChatInput.onSubmit.RemoveListener(AddToChatOutput);
    }

	public void ChatWelcome()
	{
        string message = "A wild <#" + lobby.htmlColor + ">" + NetworkManager.singleton.userName + ":</color> joins the room!\n";
        CmdSendMessage(message);
        TMP_ChatOutput.text += $"Welcome to {lobby.hostName}'s Server!\n";
    }

	void AddToChatOutput(string newText)
    {
        // Clear Input Field
        TMP_ChatInput.text = string.Empty;

        var timeNow = System.DateTime.Now;

        string message = "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" + timeNow.Second.ToString("d2") + "</color>] " + "<#"+lobby.htmlColor+">" + NetworkManager.singleton.userName + ":</color> " + newText + "\n";

        CmdSendMessage(message);

        TMP_ChatInput.ActivateInputField();
    }

    [Command(ignoreAuthority = true)]
    public void CmdSendMessage(string message)
	{
        RpcHandleMessage(message);
	}

    [ClientRpc]
    private void RpcHandleMessage(string message)
	{
        TMP_ChatOutput.text += message;//Se podria pasar todo el string de arriba de AddToChatOutput a acá, para q se diferencie el sender del resto con un if
    }

}
