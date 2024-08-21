using System;
using System.Collections;
using System.Collections.Generic;
using TestAssignment._Project.Scripts.Enemies;
using TestAssignment._Project.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace TestAssignment._Project.Scripts.Player
{
    [RequireComponent(typeof(CharacterController), typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _movementSpeed = 3.0f;
        [SerializeField] private Animator _animator;
        [SerializeField] private Joystick _joystick;
        [SerializeField] private Health _health;

        [SerializeField] private float _punchCooldown;
        [SerializeField] private float _punchRadius;
        [SerializeField] private float _punchDamage;
        [SerializeField] private float _kickCooldown;
        [SerializeField] private float _kickRadius;
        [SerializeField] private float _kickDamage;
        [SerializeField] private float _stunDuration;
        
        [SerializeField] private bool _showHitZones;
        [SerializeField] private Material _hitZoneMaterial;
        private Mesh _hitzoneMesh;

        private bool _isInCooldown = false;
        private float _movementX;
        private float _movementXHash;
        private float _movementY;
        private bool _isMobile;
        private Vector3 _movementDirection = Vector3.zero;
        private WaitForSeconds _wait;

        public event Action LevelFinished;
        public event Action PlayerDied;

        private void Awake()
        {
            GameObject hitzoneObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hitzoneObj.transform.position = Vector3.down * 100;
            Destroy(hitzoneObj.GetComponent<Collider>());
            _hitzoneMesh = hitzoneObj.GetComponent<MeshFilter>().mesh;
            DestroyImmediate(hitzoneObj);
            
            _isMobile = Application.isMobilePlatform;
            _controller = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!_isInCooldown)
            {
                ReadInput();
                FlipIfBackwards();
                _controller.Move(_movementDirection * (_movementSpeed * Time.deltaTime));
                AnimateMovement();
            }
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
                _animator.SetTrigger("Punch");
                DamageEnemies(GetEnemiesInRange(_punchRadius), _punchDamage);
                if (_showHitZones)
                    CreateSphere(_punchRadius, _punchCooldown);
                StartCoolDown(_punchCooldown);
            }
        }

        public void Kick()
        {
            if (!_isInCooldown)
            {
                _animator.SetTrigger("Kick");
                DamageEnemies(GetEnemiesInRange(_kickRadius), _kickDamage);
                if (_showHitZones)
                    CreateSphere(_kickRadius, _kickCooldown);
                StartCoolDown(_kickCooldown);
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

        private List<EnemyController> GetEnemiesInRange(float range)
        {
            Vector3 pos = transform.position + transform.forward * range / 2 +
                          transform.up * (transform.localScale.y * 1.3f);
            Collider[] hits = Physics.OverlapSphere(pos, range);

            List<EnemyController> enemies = new();

            foreach (Collider hit in hits)
                if (hit.GetComponent<EnemyController>() != null)
                    enemies.Add(hit.GetComponent<EnemyController>());
            return enemies;
        }

        private void DamageEnemies(List<EnemyController> enemies, float damage)
        {
            foreach (EnemyController enemy in enemies)
            {
                enemy.TakeHit(damage);
            }
        }

        public void TakeHit(float damage)
        {
            _animator.SetTrigger("TakeHit");
            _health.TakeDamage(damage);
            StartCoolDown(_stunDuration);
        }

        public void Death()
        {
            Destroy(gameObject.GetComponent<PlayerController>());
            gameObject.GetComponent<Collider>().enabled = false;
            Debug.Log("I am player and I am dying");
            _animator.SetTrigger("Death");
            PlayerDied?.Invoke();
        }

        public void Win()
        {
            _isInCooldown = true;
            _animator.SetTrigger("Win");
            LevelFinished?.Invoke();
        }

        private GameObject CreateSphere(float range, float cooldown)
        {
            Vector3 pos = transform.position + transform.forward * range/2 + transform.up * (transform.localScale.y * 1.3f);
            GameObject sphere = new GameObject("Sphere");
            MeshFilter filter = sphere.AddComponent<MeshFilter>();
            filter.mesh = _hitzoneMesh;
            sphere.AddComponent<MeshRenderer>();
            sphere.GetComponent<Renderer>().material = _hitZoneMaterial;
            sphere.transform.position = pos;
            sphere.transform.localScale = Vector3.one * range;
            Destroy(sphere, cooldown);
            return sphere;
        }
    }
}