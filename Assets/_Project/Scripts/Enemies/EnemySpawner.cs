using System;
using System.Collections;
using System.Collections.Generic;
using TestAssignment._Project.Scripts.Player;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace TestAssignment._Project.Scripts.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [SerializeField] private GameObject[] _enemyPrefabs;
        [SerializeField] private float _delay;

        [SerializeField] private float _firstTimeMark;
        [SerializeField] private float _secondTimeMark;
        [SerializeField] private float _thirdTimeMark;

        [SerializeField] private float _firstPositionMark;
        [SerializeField] private float _secondPositionMark;
        [SerializeField] private float _thirdPositionMark;


        private Vector3[] _spawnPoints = new Vector3 [2];
        private int _currentEnemyLevel = 0;

        private void Awake()
        {
            for (int i = 0; i < _spawnPoints.Length-1; i++)
            {
                _spawnPoints[i] = new Vector3(10f, 0f,0f);
            }
            _player = FindObjectOfType<PlayerController>().gameObject;
        }

        private void Start()
        {
            StartCoroutine(Repeater(_delay));
        }

        private void Update()
        {
            GetSpawnPosition();

            if (Time.unscaledTime >= _firstTimeMark || _player.transform.position.x >= _firstPositionMark)
                _currentEnemyLevel = 1;
            if (Time.unscaledTime >= _secondTimeMark || _player.transform.position.x >= _secondPositionMark)
                _currentEnemyLevel = 2;
            if (Time.unscaledTime >= _thirdTimeMark || _player.transform.position.x >= _thirdPositionMark)
                _currentEnemyLevel = 3;
            if (_currentEnemyLevel == 3 && _player.transform.position.x >= _thirdPositionMark)
            {
                StopAllCoroutines();
                if(FindObjectOfType<EnemyController>() == null)
                    _player.GetComponent<PlayerController>().Win();
            }
        }


        private void GetSpawnPosition()
        {Random r = new Random();
            if (_player.transform.position.x <= 0)
            {
                _spawnPoints[0] = _player.transform.position + new Vector3(10f, 0f, r.Next(-5, 5));
                _spawnPoints[1] = _player.transform.position + new Vector3(11f, 0f, r.Next(-5, 5));
            }
            else if (_player.transform.position.x is > -7 and < 70)
            {
                _spawnPoints[0] = _player.transform.position + new Vector3(10f, 0f, r.Next(-5, 5));
                _spawnPoints[1] = _player.transform.position + new Vector3(-10f, 0f, r.Next(-5, 5));
            }
            else if (_player.transform.position.x is >= 70)
            {
                _spawnPoints[0] = _player.transform.position + new Vector3(-10f, 0f, r.Next(-5, 5));
                _spawnPoints[1] = _player.transform.position + new Vector3(-11f, 0f, r.Next(-5, 5));
            }
        }

        private void SpawnEnemy(int enemyLevel)
        {
            Random r = new Random();
            Instantiate(_enemyPrefabs[enemyLevel], _spawnPoints[r.Next(2)],
                _enemyPrefabs[enemyLevel].transform.rotation);
        }

        private IEnumerator Repeater(float delay)
        {
            for (;;)
            {
                var wait = new WaitForSeconds(delay);
                SpawnEnemy(_currentEnemyLevel);
                yield return wait;
            }
        }
    }
}