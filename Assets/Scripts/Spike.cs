using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D (Collider2D other)
    {
        other.GetComponent<IDamageable>()?.Kill();
    }
}
