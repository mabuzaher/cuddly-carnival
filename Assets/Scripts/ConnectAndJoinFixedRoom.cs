using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ConnectAndJoinFixedRoom : MonoBehaviourPunCallbacks
{
    public byte version = 1;

    public byte maxPlayers = 4;

    public int playerTtl = -1;

    public string fixedRoomName;

    public Text statusText;

    public Button joinButton;
    
    private void Start()
    {
        ResetUi();
    }

    public void ResetUi()
    {
        statusText.text = "";
        joinButton.interactable = true;
    }

    public void ConnectNow()
    {
        joinButton.interactable = false;

        Debug.Log("ConnectAndJoinFixedRoom.ConnectNow() will now call: PhotonNetwork.ConnectUsingSettings().");

        statusText.text = "Connecting...";

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = this.version + "." + SceneManagerHelper.ActiveSceneBuildIndex;
    }


    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected To Master...";
        Debug.Log("OnConnectedToMaster() was called by PUN. This client is now connected to Master Server in region [" +
                  PhotonNetwork.CloudRegion +
                  "] and can join a room. Calling: PhotonNetwork.JoinOrCreateRoom();");
        PhotonNetwork.JoinOrCreateRoom(fixedRoomName,
            new RoomOptions() {MaxPlayers = maxPlayers, PlayerTtl = playerTtl},
            TypedLobby.Default, null);
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby...";

        Debug.Log("OnJoinedLobby(). This client is now connected to Relay in region [" + PhotonNetwork.CloudRegion +
                  "]. This script now calls: PhotonNetwork.JoinOrCreateRoom();");
        PhotonNetwork.JoinOrCreateRoom(fixedRoomName,
            new RoomOptions() {MaxPlayers = maxPlayers, PlayerTtl = playerTtl},
            TypedLobby.Default, null);
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"OnJoinRoomFailed {message}");
        statusText.text = $"Join Room Failed...{message}";
        joinButton.interactable = true;

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        if (statusText)
            statusText.text = $"Disconnected...{cause}";
        Debug.Log($"OnDisconnected({cause})");
        joinButton.interactable = true;

    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Joined Room";
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room in region [" + PhotonNetwork.CloudRegion +
                  "]. Game is now running.");
        
    }
}