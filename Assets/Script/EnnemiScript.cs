using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnnemiScript : MonoBehaviour , IDamagable
{


    public bool playertarget;
    
    [SerializeField] private Animator _animator;
    [SerializeField] private EnnemiStat _ennemiStat = EnnemiStat.Walk;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float _groundheight;
    [SerializeField] private Transform _groundChcker;

    [Space(5)] 
    [SerializeField] private Transform _rightRaycaster1;
    [SerializeField] private Transform _rightRaycaster2;
    [SerializeField] private Transform _leftRaycaster1;
    [SerializeField] private Transform _leftRaycaster2;
    [SerializeField] private float _wallDetection = 0.5f;
    [SerializeField] private float _groundDetection = 0.5f;
    [Space(5)]
    [SerializeField] private float _waitingTime;

    [Space(5)] 
    [SerializeField] private LayerMask _playerMask;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _projectileDelay = 0.2f;
    [SerializeField] private Projectile _prfabProjectile;
    [SerializeField] private Transform _projectilOrigin;
    [Space(5)] 
    [SerializeField] private int _hp = 1;

    private float _direction = 1;


    private float _waitingTimer;
    private float _firingTimer;
    
    private enum EnnemiStat {
        Walk , Wait , Aim  , Dead
    }
    void Update() {
        ManageGroundHeight();
        if (IsPlayerIsLineOfSite()) {
            if (_animator) {
                _animator.SetBool("Aim", true);
                _animator.SetBool("Walk", false);
            }

            if (_ennemiStat != EnnemiStat.Aim) _firingTimer = _fireRate / 2;
            _ennemiStat = EnnemiStat.Aim;
            playertarget = true;
        }
        else {
            if( _animator)_animator.SetBool("Aim", false);
            if( _ennemiStat==EnnemiStat.Aim)_ennemiStat = EnnemiStat.Walk;
            playertarget = false;
            
        }
        
        switch (_ennemiStat) {
            case EnnemiStat.Walk: ManageMoveState(); break;
            case EnnemiStat.Wait: ManageWaitingState(); break;
            case EnnemiStat.Aim: ManagerAimStat();break;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private void ManageMoveState() {
        if (!CheckEnnemiFront()) {
            _ennemiStat = EnnemiStat.Wait;
            _direction = _direction * -1;
            return;
        }

        if (_animator) {
            _animator.transform.forward = new Vector3(_direction, 0, 0);
            _animator.SetBool("Walk", true);
        }

        ManagerMove();
    }

    private bool CheckEnnemiFront() {
        if (_direction == 1) {
            if (Physics.Raycast(_rightRaycaster1.position, Vector3.right, _wallDetection, _groundLayerMask))
                return false;
            if (Physics.Raycast(_rightRaycaster2.position, Vector3.down, _groundDetection, _groundLayerMask))
                return true;
        }
        else if (_direction == -1) {
            if (Physics.Raycast(_leftRaycaster1.position, Vector3.left, _wallDetection, _groundLayerMask))
                return false;
            if (Physics.Raycast(_leftRaycaster2.position, Vector3.down, _groundDetection, _groundLayerMask))
                return true;
        }

        return false;
    }
    private void ManagerMove() {
        transform.position += new Vector3(_direction * Time.deltaTime * _moveSpeed, 0, 0);
    }

    private void ManageGroundHeight() {
        RaycastHit hit;
        if (Physics.Raycast(_groundChcker.position, Vector3.down, out hit, 3, _groundLayerMask)) {
            transform.position = new Vector3(transform.position.x, hit.point.y + _groundheight, transform.position.z);
        }
    }

    private void ManageWaitingState() {
        if( _animator)_animator.SetBool("Walk", false);
        _waitingTimer += Time.deltaTime;
        if (_waitingTimer >= _waitingTime) {
            _waitingTimer = 0;
            _ennemiStat = EnnemiStat.Walk;
        }
    }

    private bool IsPlayerIsLineOfSite() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, new Vector3(_direction, 0, 0), out hit, _playerMask)) {
            if (hit.collider.CompareTag("Player")) {
                
                return true;
            }
        }
        return false;
    }

    private void ManagerAimStat() {
        _firingTimer += Time.deltaTime;
        if( _animator)_animator.transform.forward = new Vector3(_direction, 0, 0);
        if (_firingTimer >= _fireRate) {
            _firingTimer = 0;
            if( _animator)_animator.SetTrigger("Fire");
            Invoke("ManagerFire", _projectileDelay);
            Debug.Log("Fire");
        }
    }

    private void ManagerFire() {
        Projectile projectile = Instantiate(_prfabProjectile,_projectilOrigin.position, Quaternion.identity);
        projectile.Speed = projectile.Speed * _direction;
    }

    public void TakeDamage(int damage) {
        _hp -= damage;
        if (_hp <= 0) {
            if( _animator)_animator.SetBool("Die", true);
            GetComponent<BoxCollider>().enabled = false;
            this.enabled = false;
        }
    }
    
}