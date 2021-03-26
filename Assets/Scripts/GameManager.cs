using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private GameObject _localPlayer;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    

    private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        GameObject respawnPoints = GameObject.FindGameObjectWithTag("Respawn");

        Vector3 randomPos = Vector3.zero;
        if (respawnPoints != null && respawnPoints.transform.childCount > 0)
            randomPos = respawnPoints.transform.GetChild(Random.Range(0, respawnPoints.transform.childCount - 1))
                .position;
        _localPlayer = PhotonNetwork.Instantiate("character_container", randomPos, Quaternion.identity);
    }


    public override void OnConnected()
    {
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room in region [" + PhotonNetwork.CloudRegion +
                  "]. Game is now running.");
        // move camera to main
        PhotonNetwork.LoadLevel(1);
    }

    public void DestroyAndRespawnLocalPlayer()
    {
        PhotonNetwork.Destroy(_localPlayer);
        SpawnPlayer();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("Quiting...");
            Application.Quit();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;

        if(SceneManager.GetActiveScene().buildIndex == 0) return;
        
        SceneManager.LoadScene(0);
        Destroy(gameObject, 2f);
    }
}