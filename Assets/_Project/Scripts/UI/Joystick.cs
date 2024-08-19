using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace TestAssignment._Project.Scripts.UI
{
    public class Joystick : MonoBehaviour
    {
        [SerializeField] private GameObject _joystick;
        [SerializeField] private GameObject _joystickBG;
        [SerializeField] private float _joystickRadius;
        private Vector2 _joystickTouchPos;
        private Vector2 _joystickOriginalPos;
        public Vector2 _joystickVec;

        private void Awake()
        {
            _joystickOriginalPos = _joystick.transform.position;
        }

        public void PointerDown()
        {
            _joystickTouchPos = Input.mousePosition;
        }

        public void Drag(BaseEventData baseEventData)
        {
            PointerEventData pointerEventData = baseEventData as PointerEventData;
            Vector2 dragPos = pointerEventData.position;
            _joystickVec = (dragPos - _joystickOriginalPos).normalized;

            float joystickDist = Vector2.Distance(dragPos, _joystickOriginalPos);

            if (joystickDist < _joystickRadius)
            {
                _joystick.transform.position = _joystickOriginalPos + _joystickVec * joystickDist;
            }
            else
            {
                _joystick.transform.position = _joystickOriginalPos + _joystickVec * _joystickRadius;
            }
        }

        public void PointerUp()
        {
            _joystickVec = Vector2.zero;
            _joystick.transform.position = _joystickOriginalPos;
        }
    }
}