using System;
using System.Collections;
using Camera;
using Helpers;
using Inventory;
using Player;
using Stands;
using UnityEngine;
using UnityEngine.AI;

namespace Inimigo
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyManager : MonoBehaviour
    {
        public enum EnemyType
        {
            TypeA,
            TypeB
        }

        public EnemyType Type { get; private set; }

        public event Action<EnemyManager> OnReturnToPool;
        private EnemySpawner spawner;

        private NavMeshAgent agent;
        [SerializeField] private float velocity = 5f;
        [SerializeField] private Transform target;
        [SerializeField] private float offset;

        private Animator _animator;
        private HealthSystem _healthSystem;
        [SerializeField] private GameObject _vfxPrefab;
        private Camera_ShakeController cameraShake;

        private StandsResourceSpawner resourceSpawner;
        [SerializeField] private TriggerCheck colliderCheck;

        [SerializeField] public Transform guardedPosToReturnResource;
 
        private void Awake()
        {
        }

        public void Setup(EnemySpawner owner, EnemyType type, Transform targetStand)
        {
            spawner = owner;
            Type = type;
            agent = GetComponent<NavMeshAgent>();
            target = targetStand;
           
            if (colliderCheck == null)
            {
                colliderCheck = GetComponentInChildren<TriggerCheck>();
            }

            if (_healthSystem == null)
            {
                _healthSystem = GetComponent<HealthSystem>();
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }


            colliderCheck.EnteredTrigger += EnteredInStand;
            _healthSystem.OnDeath += OnDeathHandler;

            gameObject.SetActive(false);
        }

        public void Activate(Vector3 spawnPosition)
        {
            ResetState();
            transform.position = spawnPosition;
            agent.enabled = true;
            agent.isStopped = false;
            if (cameraShake == null)
            {
                cameraShake = ServiceLocator.Get<Camera_ShakeController>();
            }

            gameObject.SetActive(true);

            if (target == null) return;
            var targetPos = new Vector3(target.position.x - offset, 0f, target.position.z - offset);
            agent.SetDestination(targetPos);
            agent.speed = velocity;
        }

        private void OnDisable()
        {
        }

        private void EnteredInStand(Collider other)
        {
            if (other.CompareTag("Stand"))
            {
                agent.isStopped = true;
                if (resourceSpawner == null)
                {
                    resourceSpawner = other.GetComponent<StandsResourceSpawner>();
                }

                resourceSpawner.PauseCraft();
                resourceSpawner.enemiesinRange++;

            }
            else if (other.CompareTag("Base"))
            {
               var baseInventory = other.GetComponent<InventoryResource>();
                baseInventory.GetStolenResource(ResourceType.tipo1, 1);
                agent.SetDestination(guardedPosToReturnResource.position);
            } 
        }

        private void OnDeathHandler(HealthSystem health)
        {
            _healthSystem.OnDeath -= OnDeathHandler;
            StartCoroutine(Dying());
        }

        IEnumerator Dying()
        {
            agent.isStopped = true;
            agent.enabled = false;

            _animator.Play("EnemyDeath");

            yield return new WaitForSeconds(0.2f);

            cameraShake.ShakeCamera(5f, 0.5f, 0.1f);

            yield return new WaitForSeconds(0.1f);

            ReturnToPoolLogic();
        }

        public void ReturnToPoolLogic()
        {
            if (resourceSpawner != null)
            {
                if(resourceSpawner.enemiesinRange > 0)
                {
                    resourceSpawner.enemiesinRange--;
                }
                else if(resourceSpawner.enemiesinRange <= 0)
                {
                    resourceSpawner.ResumeCraft();
                }
                resourceSpawner = null;
            }

            _healthSystem.Revive();
            _healthSystem.OnDeath += OnDeathHandler;
            gameObject.SetActive(false);
            OnReturnToPool?.Invoke(this);
        }

        private void ResetState()
        {
            _healthSystem.Heal(_healthSystem.MaxHealth);

            if (_animator != null)
            {
                _animator.Play("Idle");
            }

            agent.enabled = true;
            resourceSpawner = null;
        }
    }
}