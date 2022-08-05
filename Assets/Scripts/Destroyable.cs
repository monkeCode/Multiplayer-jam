using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
[RequireComponent(typeof(PhotonView))]
public class Destroyable : MonoBehaviourPun, IDamageable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;
        photonView.RPC(nameof(UpdateHp), RpcTarget.All, damage);
    }

    [PunRPC]
    private void UpdateHp(int hp)
    {
        _currentHealth -= hp;
        if (_currentHealth > 0) return;
        _currentHealth = 0;
        Dest();
    }
    public void Heal(int heal)
    {
        throw new System.NotImplementedException();
    }

    public void Kill()
    {
        throw new System.NotImplementedException();
    }
    
    private void Dest()
    {
        //if(photonView.IsMine) 
            //PhotonNetwork.Destroy(gameObject);
            Destroy(gameObject);
    }
}
