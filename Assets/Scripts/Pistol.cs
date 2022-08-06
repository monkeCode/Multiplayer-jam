using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Task = System.Threading.Tasks.Task;
[CreateAssetMenu(menuName = "Items/Pistol")]
public class Pistol : InventoryItem
{
    [SerializeField] private float shootDelay;
    [SerializeField] private float _bulletSpeed;
    private bool _cantShoot;
    private DateTime lastTimeShoot;
    public override void Use(Player player)
    {
        if (_cantShoot || !player.GetComponent<PhotonView>().IsMine) return;
        var currentTime = DateTime.Now;
        if((currentTime - lastTimeShoot).TotalSeconds < shootDelay) return;
        _cantShoot = true;
        lastTimeShoot = currentTime;
        Shoot(player);
    }

    public Pistol()
    {
        _cantShoot = false;
    }

    private void Shoot(Component player)
    {
        var bullet = PhotonNetwork.Instantiate("Bullet", player.transform.position, Quaternion.identity);
        var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 force = mousePos - player.transform.position;
        Debug.Log(mousePos);
        Debug.Log(player.transform.position);
        bullet.GetComponent<Rigidbody2D>().velocity = force.normalized * _bulletSpeed;
        bullet.GetComponent<Bullet>().SetCaster(player.gameObject);

    }
}
