using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Lift : MonoBehaviour, IPunObservable, IToggle
{
    [SerializeField] private List<Transform> _points = new();
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isLoop;
    [SerializeField] private float _speed;
    private Sequence _liftSequence;
    private PhotonView _photonView;
    private Rigidbody2D _rigidbody2D;
    private Vector2 _lastPosition;
    private Vector2 _velocity;
    private Animator _animator;
    public void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _animator.SetBool("IsOn", _isMoving);
        if (!_photonView.IsMine) return;

        _lastPosition = transform.position;
        
        CreateSequence();
        if (_isMoving)
            _liftSequence.Play();
        else _liftSequence.Pause();
    }
     
    private void FixedUpdate()
    {
        if (_photonView.IsMine)
        {
            
            _velocity = ((Vector2)transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;
        }
        else
        {
            _rigidbody2D.velocity = _velocity;
        }
       
    }

    private void CreateSequence()
    {
        _liftSequence.Kill();
        _liftSequence = DOTween.Sequence();
        for (int i = 1; i < _points.Count; i++)
        {
            _liftSequence.Append(_rigidbody2D.
                DOMove(_points[i].position, Vector2.Distance(_points[i-1].position, _points[i].position)/_speed).SetEase(Ease.InOutQuad));
        }
        _liftSequence.onComplete += () =>
        {
            if (_isLoop)
                CreateReverseSequence();
        };
    }

    private void CreateReverseSequence()
    {
        _liftSequence.Kill();
        _liftSequence = DOTween.Sequence();
        for(int i = _points.Count-2; i >= 0; i--)
        {
            _liftSequence.Append(_rigidbody2D.DOMove(_points[i].position, Vector2.Distance(_points[i+1].position, _points[i].position)/_speed).SetEase(Ease.InOutQuad));
        }
        _liftSequence.onComplete += () =>
        {
            if (_isLoop)
                CreateSequence();
        };
    }
    public void ChangeState(bool state)
    {
        if (!_photonView.IsMine) return;
        _isMoving = state;
        _animator.SetBool("IsOn", _isMoving);
        if (state)
            _liftSequence.Play();
        else _liftSequence.Pause();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //sync transform and rigidbody
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_velocity);
            stream.SendNext(_isMoving);
            
        }
        else
        {
            transform.position = (Vector3) stream.ReceiveNext();
            transform.rotation = (Quaternion) stream.ReceiveNext();
            _velocity = (Vector2) stream.ReceiveNext();
            _isMoving = (bool) stream.ReceiveNext();
            _animator.SetBool("IsOn", _isMoving);
        }
    }

    public void Toggle(bool isOn)
    {
        ChangeState(isOn);
    }
}
