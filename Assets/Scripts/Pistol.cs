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
        var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - player.transform.position;
        Shoot(player.transform.position, mousePos.normalized );
        Reload();
    }

    private void Shoot(Vector2 pos, Vector2 dir)
    {
        var bullet = PhotonNetwork.Instantiate("Bullet", pos, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * _bulletSpeed;

    }
    private async void Reload()
    {
        _cantShoot = true;
        await Task.Delay((int) (shootDelay * 1000));
        _cantShoot = false;
    }
}
