using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour,IToggle
{
    List<Rigidbody2D> _rigidbodies = new();
    [SerializeField] [Range(-7, 7)] float _force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rb in _rigidbodies)
        {
            rb.AddForce(new Vector2(Mathf.Cos((transform.eulerAngles.z +90) * Mathf.Deg2Rad), Mathf.Sin((transform.eulerAngles.z +90) * Mathf.Deg2Rad) * _force));
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
            return;
        }
        GetComponent<Collider2D>().enabled = false;
        GetComponent<ParticleSystem>().Stop();
    }
}
