using System;
using System.Collections;
using System.Collections.Generic;
using TestAssignment._Project.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TestAssignment._Project.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _movementSpeed = 3.0f;
        [SerializeField] private Animator _animator;
        [SerializeField] private Joystick _joystick;

        [SerializeField] private float _punchCooldown;
        [SerializeField] private float _kickCooldown;

        private bool _isInCooldown = false;
        private float _movementX;
        private float _movementXHash;
        private float _movementY;
        private bool _isMobile;
        private Vector3 _movementDirection = Vector3.zero;


        private void Awake()
        {
            _isMobile = Application.isMobilePlatform;
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            ReadInput();
            FlipIfBackwards();
            _controller.Move(_movementDirection * (_movementSpeed * Time.deltaTime));
            AnimateMovement();
        }

        private void FlipIfBackwards()
        {
            if (_movementX == 0)
                return;
            if (Mathf.Sign(_movementXHash) != Mathf.Sign(_movementX))
            {
                transform.Rotate(0, 180, 0);
            }
        }

        public void Punch()
        {
            if (!_isInCooldown)
            {
                _isInCooldown = true;
                _animator.SetTrigger("Punch");
                StartCoroutine(Cooldown(_punchCooldown));
            }
        }

        public void Kick()
        {
            if (!_isInCooldown)
            {
                _isInCooldown = true;
                _animator.SetTrigger("Kick");
                StartCoroutine(Cooldown(_kickCooldown));
            }
        }

        private void AnimateMovement()
        {
            _animator.SetFloat("VelocityX", Mathf.Abs(_movementX));
            _animator.SetFloat("VelocityY", _movementY * transform.forward.x);
        }

        private void ReadInput()
        {
            if (_movementX != 0)
                _movementXHash = _movementX;
            if (_isMobile)
            {
                _movementX = _joystick._joystickVec.x;
                _movementY = _joystick._joystickVec.y;
                _movementDirection = Vector3.ClampMagnitude(new Vector3(_movementX, 0.0f, _movementY), 1);
            }
            else
            {
                _movementX = Input.GetAxis("Horizontal");
                _movementY = Input.GetAxis("Vertical");
                _movementDirection = Vector3.ClampMagnitude(new Vector3(_movementX, 0.0f, _movementY), 1);
                if (Input.GetKeyDown(KeyCode.J))
                    Kick();
                if (Input.GetKeyDown(KeyCode.K))
                    Punch();
            }
        }

        private IEnumerator Cooldown(float delay)
        {
            var wait = new WaitForSeconds(delay);
            yield return wait;
            _isInCooldown = false;
        }
    }
}