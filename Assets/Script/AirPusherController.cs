using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AirPusherController : MonoBehaviour , IDamagable
{

    public event EventHandler<bool> OnKickButton;
    public event EventHandler<bool> OnPunchButton;
    public event EventHandler<bool> OnHeadBumpButton;
    public event EventHandler<bool> OnJumpButton;
    public event EventHandler<float> OnMoveButton; 

    [SerializeField] private float _speed = 1;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _gravity =-9.81f;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance=0.4f;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float JumpHeight = 1f;
    [Space(5)]
    [SerializeField] private float _projectileDelay = 0.2f;
    [SerializeField] private GameObject _prfabAirProjectile;
    [SerializeField] private Transform _projectilOrigin;
    
    private bool _isGRounded;
    private Vector3 _velocity;
    private Vector3 _moveVector;
    private float _movevalue;
    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }
    
    protected virtual void Update() {
        if( _moveVector.y>0.5f) DoJump(true);
        ManageMovement();
        
        
    }

    public void OnMove(InputValue value) {
        _moveVector = value.Get<Vector2>();
    }
    
    public void OnPunsh(InputValue value) {
        
        OnPunchButton?.Invoke(this, value.isPressed);
        if (!value.isPressed) return;
        ManageAirPunshWithDelay();
        //_animator.SetTrigger("Punsh");
    }

    protected virtual void DoJump(bool isPress) {
        Debug.Log("DoJump");
        if ( _isGRounded&&_velocity.y<0.5f) _velocity.y = Mathf.Sqrt(JumpHeight * -2f * _gravity);
        _characterController.Move(_velocity*Time.deltaTime);
    }

    protected virtual void ManageMovement() {
        _isGRounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        if (_isGRounded && _velocity.y < 0) {
            _velocity.y = -2f;
        }
        _velocity.y += _gravity * Time.deltaTime;
        
        _characterController.Move(_speed*Time.deltaTime*_moveVector);
        _characterController.Move(_velocity*Time.deltaTime);
        
        if (_animator)
        {
            _animator.SetFloat("MoveSpeed", _moveVector.x);
            _animator.SetBool("IsGrounded", _isGRounded);
            _animator.SetFloat("YVelocity", _velocity.y);
        }
    }

    private void ManageAirPunshWithDelay() {
        if (_animator) _animator.SetTrigger("Punsh");
        Invoke("ManageAirPunsh", _projectileDelay);
    }
    private void ManageAirPunsh() {
        Instantiate(_prfabAirProjectile, _projectilOrigin.position, quaternion.identity);
    }

    public void TakeDamage(int damage) {
        if (_animator)_animator.SetTrigger("Hit");
    }
}
