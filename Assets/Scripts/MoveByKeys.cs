using Cinemachine;
using Photon.Pun;
using UnityEngine;

/// <summary>
/// Very basic component to move a GameObject by WASD and Space.
/// </summary>
/// <remarks>
/// Requires a PhotonView. 
/// Speed affects movement-speed. 
/// JumpForce defines how high the object "jumps". 
/// JumpTimeout defines after how many seconds you can jump again.
/// </remarks>
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]
public class MoveByKeys : MonoBehaviourPun//, IPunObservable
{
    public float playerSpeed = 10f;
    private Vector3 _playerVelocity;
    
    private CharacterController _characterController;

    public void Start()
    {
        
        //enabled = photonView.isMine;
        //add to camera targets group
        FindObjectOfType<CinemachineTargetGroup>()?.AddMember(transform,1,0);
        
        _characterController = GetComponent<CharacterController>();
    }


    // Update is called once per frame
    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (_playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _characterController.Move(Time.deltaTime * playerSpeed * move);
        

        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

   

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            
        }
        else
        {
            // Network player, receive data
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }*/
}