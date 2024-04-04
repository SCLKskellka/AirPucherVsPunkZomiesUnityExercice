using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScripte : MonoBehaviour , IDamagable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Collider _collider;

    public void TakeDamage(int damage) {
        _collider.enabled = false;
        if (_animator)_animator.SetBool("Destroy" , true);
    }
}
