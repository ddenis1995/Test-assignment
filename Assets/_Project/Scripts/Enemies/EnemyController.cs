using System;
using TestAssignment._Project.Scripts.Player;
using UnityEngine;

namespace TestAssignment._Project.Scripts.Enemies
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _player;
        [SerializeField] private float _movementSpeed;

        [SerializeField] private Vector3 _movementDirection;
        private float _movementXHash;

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
            _controller.Move(_movementDirection * (_movementSpeed * Time.deltaTime));
            AnimateMovement();
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
        }
    }
}