using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Button : MonoBehaviour, IToggle
{
    [SerializeField] private bool isToggle;
    [SerializeField] private List<GameObject> _toggles;
    private bool _state;
    private PhotonView _photonView;
    private SpriteRenderer _spriteRenderer;
    private int countClick = 0;
    [SerializeField] private Sprite[] _sprites;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Toggle(bool isOn)
    {
        if (!_photonView.IsMine || countClick > 1) return;
        _state = isOn;
        _photonView.RPC(nameof(RPC_Toggle), RpcTarget.All, isOn);
    }
    [PunRPC]
    public void RPC_Toggle(bool isOn)
    {
        _state = isOn;
        _spriteRenderer.sprite = isOn?_sprites[0]:_sprites[1];
        foreach (var toggle in _toggles)
        {
            toggle.GetComponent<IToggle>()?.Toggle(isOn);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_photonView.IsMine)
            countClick++;
        Toggle(!_state);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(_photonView.IsMine)
            countClick--;
        if(countClick > 0)
            return;
        if(!isToggle)
            Toggle(!_state);
        
    }
}
