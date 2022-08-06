using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class Fan : MonoBehaviour,IToggle, IPunObservable
{
    List<Rigidbody2D> _rigidbodies = new();
    [SerializeField] [Range(-7, 7)] float _force;
    [SerializeField] private  bool _isOn;

    private Animator _animator;

    private static readonly int On = Animator.StringToHash("On");

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        Toggle(_isOn);
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbodies = _rigidbodies.Where(it => it != null).ToList();
        foreach (var rb in _rigidbodies)
        {
            var f = new Vector2(Mathf.Cos((transform.eulerAngles.z +90) * Mathf.Deg2Rad), Mathf.Sin((transform.eulerAngles.z +90) * Mathf.Deg2Rad) * _force);
            f*=Time.deltaTime * 225;
            rb.AddForce(f);
        }
            
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D x))
            _rigidbodies.Add(x);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D x))
            _rigidbodies.Remove(x);
    }

    public void Toggle(bool isOn)
    {
        if (isOn)
        {
            GetComponent<ParticleSystem>().Play();
            GetComponent<Collider2D>().enabled = true;
            _animator.SetBool(On, true);
            _isOn = true;
            return;
        }
        GetComponent<Collider2D>().enabled = false;
        GetComponent<ParticleSystem>().Stop();
        _animator.SetBool(On, false);
        _isOn = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isOn);
        }
        else
        {
            _isOn = (bool)stream.ReceiveNext();
            Toggle(_isOn);
        }
    }
}
