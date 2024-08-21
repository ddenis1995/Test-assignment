using System;
using Unity.VisualScripting;
using UnityEngine;

namespace TestAssignment._Project.Scripts.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform _camera;

        private void Awake()
        {
            _camera = Camera.main.transform;
        }

        void Update()
        {
            transform.forward = -_camera.forward;
        }
    }
}