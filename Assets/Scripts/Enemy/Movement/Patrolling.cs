using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy.Movement {
    public class Patrolling : MonoBehaviour {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Transform[] points;
        [SerializeField] private Vector2 idleTime;
        [SerializeField] private PatrolType patrolType;
        [SerializeField] private float offsetToDestination;
        [Space]
        [SerializeField] private Animator animator;
        [SerializeField] private float animationCrossFade = 0.2f;
        [SerializeField] private AnimationClip idleAnimation;
        [SerializeField] private AnimationClip walkAnimation;

        private bool _isIdle;
        private int _currentCircleIndex;
        private int _currentFlipFlopIndex;
        private int _flipFlopIndexIncrement;

        private Vector3 _nextDestination;

        private void Awake() => _flipFlopIndexIncrement = 1;

        private void Update() {
            // This probably can be placed in coroutine, but it may open a memory leak due to recursiveness...
            if (ReachedDestination() && !_isIdle) StartCoroutine(SetNextDestinationLater());
        }

        private Vector3 NextRandomDestination() {
            return points[Random.Range(0, points.Length)].position;
        }

        private Vector3 NextCircleDestination() {
            var nextPoint = points[_currentCircleIndex].position;
            
            _currentCircleIndex++;
            if (_currentCircleIndex >= points.Length) _currentCircleIndex = 0;
            _currentFlipFlopIndex = _currentCircleIndex;

            return nextPoint;
        }

        private Vector3 NextFlipFlopDestination() {
            var nextPoint = points[_currentCircleIndex].position;
            
            _flipFlopIndexIncrement += _flipFlopIndexIncrement;
            if (_flipFlopIndexIncrement >= points.Length) {
                _flipFlopIndexIncrement = -1;
                _currentFlipFlopIndex -= 2;
                // when only one point
                if (_currentFlipFlopIndex < 0) _currentFlipFlopIndex = 0;
            }
            else if (_currentFlipFlopIndex < 0) {
                _flipFlopIndexIncrement = 1;
                _currentFlipFlopIndex += 2;
                // when only one point
                if (_currentFlipFlopIndex >= points.Length) _currentFlipFlopIndex = 0;
            }
            _currentCircleIndex = _currentFlipFlopIndex;

            return nextPoint;
        }

        private Vector3 NextDestination() {
            return patrolType switch {
                PatrolType.Random => NextRandomDestination(),
                PatrolType.FlipFlop => NextFlipFlopDestination(),
                PatrolType.Circle => NextCircleDestination(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private IEnumerator SetNextDestinationLater() {
            animator.CrossFade(idleAnimation.name, animationCrossFade);
            _isIdle = true;
            
            yield return new WaitForSeconds(Random.Range(idleTime.x, idleTime.y));
            agent.SetDestination(NextDestination());

            _isIdle = false;
            animator.CrossFade(walkAnimation.name, animationCrossFade);
        }

        private bool ReachedDestination() {
            return Vector3.Distance(agent.destination, transform.position) < offsetToDestination;
        }
    }
}
