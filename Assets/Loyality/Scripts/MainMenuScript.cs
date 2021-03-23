using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuScript : MonoBehaviour
{

    [Tooltip("The text box containing the ip")]
    public TextMeshProUGUI ClientJoinServerField;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HostServer()
    {
        NetworkManager.singleton.StartHost();
    }

    public void StartClient()
    {
        string NetworkAddress = NetworkManager.singleton.networkAddress;

        NetworkAddress = NetworkAddress.Remove(NetworkAddress.Length - 1);
        NetworkManager.singleton.networkAddress = NetworkAddress;
        NetworkManager.singleton.StartClient();
    }

    public void StopClient()
    {
        NetworkManager.singleton.StopClient();
    }

    public void UpdateNetworkAddress()
    {
        NetworkManager.singleton.networkAddress = ClientJoinServerField.text;
    }

}
