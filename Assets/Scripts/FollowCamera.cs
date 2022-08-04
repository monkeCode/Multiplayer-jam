using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    void Start()
    {
        GetComponent<CinemachineVirtualCamera>().Follow = Server.Instance.GetLookingObject();
    }
    
    void Update()
    {
        
    }
}
