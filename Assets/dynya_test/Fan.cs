using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();
    [SerializeField] [Range(-1, 1)] float force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var rb in rigidbodies)
        {
            rb.AddForce(new Vector2(Mathf.Cos((transform.rotation.z +90) * Mathf.Deg2Rad), Mathf.Sin((transform.rotation.z +90) * Mathf.Deg2Rad) * force));
        }
            
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D x))
            rigidbodies.Add(x);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D x))
            rigidbodies.Remove(x);
    }
}
