using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy.Movement {
    public class Patrolling : MonoBehaviour {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform[] _points;
        [SerializeField] private Vector2 _idleTime;
        [SerializeField] private Vector2 _moveSpeed;
        [SerializeField] private PatrolType _patrolType;
        [SerializeField] private float _offsetToDestination;
        [Space]
        [SerializeField] private Animator _animator;
        [SerializeField] private string _animatorSpeedPeramName;
        private int _speedParamId;

        private bool _isIdle;
        private int _currentCircleIndex;
        private int _currentFlipFlopIndex;
        private int _flipFlopIndexIncrement;

        private Vector3 _nextDestination;

        private void Awake() {
            _flipFlopIndexIncrement = 1;
            _speedParamId = _animator.parameters.First(p => p.name == _animatorSpeedPeramName)
                .nameHash;
        }

        private void Update() {
            _animator.SetFloat(_speedParamId, _agent.velocity.magnitude);
            // This probably can be placed in coroutine, but it may open a memory leak due to recursiveness...
            if (ReachedDestination() && !_isIdle) StartCoroutine(SetNextDestinationLater());
        }

        private Vector3 NextRandomDestination() {
            return _points[Random.Range(0, _points.Length)].position;
        }

        private Vector3 NextCircleDestination() {
            var nextPoint = _points[_currentCircleIndex].position;

            _currentCircleIndex++;
            if (_currentCircleIndex >= _points.Length) _currentCircleIndex = 0;
            _currentFlipFlopIndex = _currentCircleIndex;

            return nextPoint;
        }

        private Vector3 NextFlipFlopDestination() {
            var nextPoint = _points[_currentCircleIndex].position;

            _flipFlopIndexIncrement += _flipFlopIndexIncrement;
            if (_flipFlopIndexIncrement >= _points.Length) {
                _flipFlopIndexIncrement = -1;
                _currentFlipFlopIndex -= 2;

                // when only one point
                if (_currentFlipFlopIndex < 0) _currentFlipFlopIndex = 0;
            }
            else if (_currentFlipFlopIndex < 0) {
                _flipFlopIndexIncrement = 1;
                _currentFlipFlopIndex += 2;

                // when only one point
                if (_currentFlipFlopIndex >= _points.Length) _currentFlipFlopIndex = 0;
            }

            _currentCircleIndex = _currentFlipFlopIndex;

            return nextPoint;
        }

        private Vector3 NextDestination() {
            return _patrolType switch {
                PatrolType.Random => NextRandomDestination(),
                PatrolType.FlipFlop => NextFlipFlopDestination(),
                PatrolType.Circle => NextCircleDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private IEnumerator SetNextDestinationLater() {
            _isIdle = true;
            yield return new WaitForSeconds(Random.Range(_idleTime.x, _idleTime.y));
            _agent.speed = Random.Range(_moveSpeed.x, _moveSpeed.y);
            _agent.SetDestination(NextDestination());
            _isIdle = false;
        }

        private bool ReachedDestination() {
            return Vector3.Distance(_agent.destination, transform.position) < _offsetToDestination;
        }
    }
}