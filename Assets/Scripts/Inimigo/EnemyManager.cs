using System;
using Stands;
using UnityEngine;
using UnityEngine.AI;

namespace Inimigo
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyManager : MonoBehaviour
    {
        private NavMeshAgent agent;
        [SerializeField] private float velocity = 5f;
        [SerializeField] private Transform target;
        [SerializeField] private float offset;

        [SerializeField] private StandsResourceSpawner resourceSpawner;
        [SerializeField] private TriggerCheck colliderCheck;
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            colliderCheck.EnteredTrigger += EnteredInStand;
        }

        private void OnDisable()
        {
            colliderCheck.EnteredTrigger -= EnteredInStand;
        }

        private void Start()
        {
            if (target == null) return;
            var targetPos = new Vector3(target.position.x - offset, 0f, target.position.z - offset);
            agent.SetDestination(targetPos);
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

                //inicia animacao de magia...
                resourceSpawner.PauseCraft();
            }
        }
        

        public void Death()
        {
            agent.isStopped = true;
            if (resourceSpawner != null)
            {
                resourceSpawner.ResumeCraft();
            }
        }
    }
}