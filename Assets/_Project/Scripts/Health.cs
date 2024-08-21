using System;
using TestAssignment._Project.Scripts.Enemies;
using TestAssignment._Project.Scripts.Player;
using UnityEngine;

namespace TestAssignment._Project.Scripts
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _maxHealth;

        private float _currentHealth;

        public float MaxHealth => _maxHealth;
        public event Action<float> Changed;

        private void Start()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float damage)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
            Changed?.Invoke(_currentHealth);
            if(_currentHealth == 0)
                Die();
        }

        private void Die()
        {
            GameObject character = transform.gameObject;
            if (character.GetComponent<PlayerController>() != null)
            {
                character.GetComponent<PlayerController>().Death();
            }
            else if(character.GetComponent<EnemyController>() != null)
            {
                character.GetComponent<EnemyController>().Death();
                transform.Find("HPCanvas").gameObject.SetActive(false);
            }
        }
    }
}
