using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class SyncHealth : MonoBehaviourPun, IPunObservable
{
    public float selfDamageTimeout = 0.5f;

    private float _selfDamageTime;

    public byte hp,maxHp;

    public Image hpBar;
    public ParticleSystem deathParticleSystem;
    
    
    private void Start()
    {
        if (!photonView.IsMine)
        {
            if(hpBar)
                hpBar.color = Color.red;
            return;
        }
        
        hp = 3;
        UpdateUi();
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
        _selfDamageTime -= Time.deltaTime;
        
    }

    [PunRPC]
    private void TryToDealDamage()
    {
        if (_selfDamageTime <= 0.0f)
        {
            TakeDamage();
            CheckDeath();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        
        Debug.Log("OnControllerColliderHit");
        // _selfDamageTime has a simple "cooldown" time
        if (_selfDamageTime <= 0.0f && hit.collider.CompareTag("Player"))
        {
            PhotonView other = hit.gameObject.GetComponent<PhotonView>();
            other.RPC(nameof(TryToDealDamage), RpcTarget.All);
            TakeDamage();
            CheckDeath();
        }
    }

    private void CheckDeath()
    {
        if (photonView.IsMine && hp <= 0)
        {
            Debug.Log("Death");
            //release death particles
            PhotonNetwork.InstantiateRoomObject("death_particle", transform.position, Quaternion.identity);
            //destroy & respawn
            FindObjectOfType<GameManager>()?.DestroyAndRespawnLocalPlayer();
        }
    }

    private void TakeDamage()
    {
        if (hp <= 0) return;
        _selfDamageTime = selfDamageTimeout;
        Debug.Log("reduce hp by 1");
        Debug.Log("Take Damage");
        hp -= 1;
        UpdateUi();
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }else
        {
            hp = (byte) stream.ReceiveNext();
            UpdateUi();
        }
    }

    private void UpdateUi()
    {
        if(hpBar)
            hpBar.fillAmount = hp / (float) maxHp;
    }
}
