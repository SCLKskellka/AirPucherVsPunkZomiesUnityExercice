using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {
    public float Speed = 10;
    public int Damage = 1;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _visual;
    void Start() {
        _rigidbody.AddForce(new Vector3(Speed,0,0), ForceMode.Impulse);
    }

    private void Update() {
        if (_visual == null) return;
        _visual.right = -_rigidbody.velocity;
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<IDamagable>() != null) {
            other.gameObject.GetComponent<IDamagable>().TakeDamage(Damage);
        }
        Destroy(gameObject);
    }
}
