using UnityEngine;

namespace TestAssignment._Project.Scripts
{
    public class StartManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] _tutorials;
        private int _counter;
        private bool _isPaused = true;

        void Start()
        {
            if (_isPaused)
            {
                _tutorials[_counter].SetActive(true);
            }

            Time.timeScale = 0;
        }

        private void Update()
        {
            if (_isPaused)
                if (Input.anyKeyDown)
                    Proceed();
        }

        private void Proceed()
        {
            ++_counter;
            if (_counter >= _tutorials.Length)
            {
                _isPaused = false;
                Time.timeScale = 1f;
                _tutorials[_counter - 1].SetActive(false);
                return;
            }

            ShowTutorial();
        }

        private void ShowTutorial()
        {
            _tutorials[_counter - 1].SetActive(false);
            _tutorials[_counter].SetActive(true);
        }
    }
}