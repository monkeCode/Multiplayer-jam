using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class Lift : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new();
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isLoop;
    [SerializeField] private float _speed;
    private Sequence _liftSequence;
    private PhotonView _photonView;
    public void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine) return;
        
        CreateSequence();
        if (_isMoving)
            _liftSequence.Play();
        else _liftSequence.Pause();
    }

    private void CreateSequence()
    {
        _liftSequence.Kill();
        _liftSequence = DOTween.Sequence();
        for (int i = 1; i < _points.Count; i++)
        {
            _liftSequence.Append(transform.
                DOMove(_points[i].position, Vector2.Distance(_points[i-1].position, _points[i].position)/_speed).SetEase(Ease.Linear));
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
            _liftSequence.Append(transform.DOMove(_points[i].position, Vector2.Distance(_points[i+1].position, _points[i].position)/_speed).SetEase(Ease.Linear));
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
        if (state)
            _liftSequence.Play();
        else _liftSequence.Pause();
    }
}
