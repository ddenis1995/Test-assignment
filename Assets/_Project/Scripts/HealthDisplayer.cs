using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestAssignment._Project.Scripts
{
    public class HealthDisplayer : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private Slider _slider;
        private void OnEnable()
        {
            _health.Changed += TakeDamage;
        }

        private void OnDisable()
        {
            _health.Changed -= TakeDamage;
        }

        private void Start()
        {
            _slider.maxValue = _health.MaxHealth;
            _slider.value = _health.MaxHealth;
        }

        private void TakeDamage(float currentHealth)
        {
            _slider.value = currentHealth;
        }
    }
}
