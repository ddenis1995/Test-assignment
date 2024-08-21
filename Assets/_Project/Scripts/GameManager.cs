using System;
using TestAssignment._Project.Scripts.Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TestAssignment._Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private GameObject _winPopup;
        [SerializeField] private GameObject _losePopup;

        private bool _isAwaitingInput = false;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController>();
        }

        private void OnEnable()
        {
            _player.LevelFinished += ShowWinPopup;
            _player.PlayerDied += ShowLosePopup;
        }

        private void OnDisable()
        {
            _player.LevelFinished -= ShowWinPopup;
            _player.PlayerDied -= ShowLosePopup;
        }

        public void ShowWinPopup()
        {
            _winPopup.SetActive(true);
            _isAwaitingInput = true;
        }

        public void ShowLosePopup()
        {
            _losePopup.SetActive(true);
            _isAwaitingInput = true;
        }

        private void Update()
        {
            if (_isAwaitingInput)
                if (Input.anyKeyDown)
                {
                    ReloadGame();
                }
        }

        public void ReloadGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}