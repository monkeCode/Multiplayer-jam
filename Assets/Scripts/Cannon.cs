using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
[RequireComponent(typeof(PhotonView))]
public class Cannon : MonoBehaviourPun, IToggle, IPunObservable
{
    private Animator _animator;
    [SerializeField] private Transform _cannonBallSpawnPoint;
    [SerializeField] private float _shootForce;
    [SerializeField] private bool _activeOnStart;
    private bool _isOn
    {
        get => _animator.GetBool("IsOn");
        set => _animator.SetBool("IsOn", value);
    }
    void Start()
    {
        _animator = GetComponent<Animator>();
        _isOn = _activeOnStart;
    }

    private void Shoot()
    {
        if (photonView.IsMine)
        {
            Vector2 direction = _cannonBallSpawnPoint.position - transform.position;
            direction.Normalize();
            Bullet bullet = PhotonNetwork.Instantiate("CannonBall", _cannonBallSpawnPoint.position, Quaternion.identity).
                GetComponent<Bullet>();
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * _shootForce);
            bullet.SetCaster(gameObject);

        }
    }
    public void Toggle(bool isOn) => _isOn = isOn;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync isOn
        if (stream.IsWriting)
        {
            stream.SendNext(_isOn);
        }
        else
        {
            _isOn = (bool)stream.ReceiveNext();
        }
    }
}
