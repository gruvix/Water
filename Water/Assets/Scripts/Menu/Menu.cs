using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Menu : MonoBehaviour
{
    public Button bnQuit;
    public GameObject MultiplayerMenuManager;

    void Start()
    {
        MultiplayerMenuManager.SetActive(true);
        bnQuit.onClick.AddListener(QuitGame);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
