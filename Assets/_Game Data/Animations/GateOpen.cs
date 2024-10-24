using System;
using GAME;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<PlayerObject>() == null) return;
        
        _animator.SetBool("Open", true);
    }
}
