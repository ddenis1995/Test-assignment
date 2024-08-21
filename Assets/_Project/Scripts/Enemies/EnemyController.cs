using System;
using System.Collections;
using System.Net;
using TestAssignment._Project.Scripts.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace TestAssignment._Project.Scripts.Enemies
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _player;
        [SerializeField] private Health _health;

        [SerializeField] private float _movementSpeed;

        [SerializeField] private float _attackRadius;
        [SerializeField] private float _attackDamage;
        [SerializeField] private float _stunDuration;
        [SerializeField] private float _attackCooldown;

        private Vector3 _movementDirection;
        private float _movementXHash;
        private bool _isInCooldown;
        private WaitForSeconds _wait;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>().gameObject;
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
                CalculateMovement();
                FlipIfBackwards();
            if (!_isInCooldown)
            {
                _controller.Move(_movementDirection * (_movementSpeed * Time.deltaTime));
            }
                AnimateMovement();
                if (Vector3.Magnitude(_player.transform.position - transform.position) <= _attackRadius)
                    Hit();
        }

        private void FlipIfBackwards()
        {
            if (_movementDirection.x == 0)
                return;
            if (Mathf.Sign(_movementXHash) != Mathf.Sign(_movementDirection.x))
            {
                transform.Rotate(0, 180, 0);
            }
        }

        private void AnimateMovement()
        {
            _animator.SetFloat("VelocityX", Mathf.Abs(_movementDirection.x));
            _animator.SetFloat("VelocityY", _movementDirection.z * transform.forward.x * -1);
        }

        private void CalculateMovement()
        {
            _movementXHash = _movementDirection.x;
            _movementDirection = Vector3.Normalize(_player.transform.position - transform.position);
        }

        private void Hit()
        {
            if (!_isInCooldown)
            {
                _animator.SetTrigger("Attack");
                if(GetPlayerInRange())
                    GetPlayerInRange().TakeHit(_attackDamage);
                StartCoolDown(_attackCooldown);
            }
        }

        public void TakeHit(float damage)
        {
            _animator.SetTrigger("TakeHit");
            _health.TakeDamage(damage);
            StartCoolDown(_stunDuration);
        }

        private void StartCoolDown(float cd)
        {
            StopAllCoroutines();
            _wait = new WaitForSeconds(cd);
            _isInCooldown = true;
            StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            yield return _wait;
            _isInCooldown = false;
        }

        private PlayerController GetPlayerInRange()
        {
            Vector3 pos = transform.position + transform.forward * _attackRadius / 2 +
                          transform.up * (transform.localScale.y * 1.3f);
            Collider[] hits = Physics.OverlapSphere(pos, _attackRadius);

            PlayerController player = new();

            foreach (Collider hit in hits)
                if (hit.gameObject.GetComponent<PlayerController>())
                    player = hit.GetComponent<PlayerController>();
            return player;
        }

        public void Death()
        {
            _isInCooldown = true;
            _animator.SetTrigger("Death");
            Destroy(gameObject, 10);
            Destroy(gameObject.GetComponent<EnemyController>());
            Destroy(gameObject.GetComponent<Collider>());
        }
    }
}