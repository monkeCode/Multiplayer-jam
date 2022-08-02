using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Photon.Pun;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using Task = System.Threading.Tasks.Task;
[CreateAssetMenu(menuName = "Items/Pistol")]
public class Pistol : InventoryItem
{
    [SerializeField] private float shootDelay;
    [SerializeField] private float _bulletSpeed;
    private bool _cantShoot;
    public override void Use(Player player)
    {
        if (_cantShoot || !player.GetComponent<PhotonView>().IsMine) return;
        _cantShoot = true;
        Shoot(player);
        Reload();
    }

    public Pistol()
    {
        _cantShoot = false;
    }

    private void Shoot(Component player)
    {
        var bullet = PhotonNetwork.Instantiate("Bullet", player.transform.position, Quaternion.identity);
        Vector2 force = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - player.transform.position;
        bullet.GetComponent<Rigidbody2D>().velocity = force.normalized * _bulletSpeed;
        bullet.GetComponent<Bullet>().SetCaster(player.gameObject);

    }
    private async void Reload()
    {
        _cantShoot = true;
        await Task.Delay((int) (shootDelay * 1000));
        _cantShoot = false;
    }
}
