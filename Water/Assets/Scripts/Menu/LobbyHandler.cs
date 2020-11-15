using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LobbyHandler : NetworkBehaviour
{
    public bool IsReady;
    public Button boton;

	private void Start()
	{
        boton.onClick.AddListener(BotonClick);
    }

	private void BotonClick()
	{
        if(isClientOnly)
		{
            NetworkManager.singleton.StopClient();
        }
        else
		{
            NetworkManager.singleton.StopHost();
		}
	}

	public void ReadyChange()
    {
        IsReady = !IsReady;
    }

	private void Update()
	{
	}
}
