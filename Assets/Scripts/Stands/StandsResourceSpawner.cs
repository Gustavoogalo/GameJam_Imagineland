using System;
using Stands.Resource;
using UnityEngine;

namespace Stands
{
    public enum States
    {
        Ready,
        Crafting,
        Waiting,
        Paused
    }

    public class StandsResourceSpawner : MonoBehaviour
    {
        [Header("Crafting Settings")] [SerializeField]
        private float timerToCraft = 5f;

        [SerializeField] private GameObject resourcePrefab;
        [SerializeField] private Transform spawnPoint;

        [SerializeField] private float counterTimer;
        [SerializeField] private States currentState;
        [SerializeField] private ResourceCrafted craftedResource = null;

        public float CurrentCraftProgress => (timerToCraft - counterTimer) / timerToCraft;
        public States CurrentState => currentState;

        private void Awake()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
        }

        private void Start()
        {
            StartCraft();
            //ResetTimerAndState();
        }

        private void Update()
        {
            switch (currentState)
            {
                case States.Crafting:
                    HandleCraftingState();
                    break;
                default:
                    break;
            }
        }

        private void HandleCraftingState()
        {
            if (counterTimer > 0)
            {
                counterTimer -= Time.deltaTime;
            }
            else
            {
                Craft();
            }
        }

        public void Craft()
        {
            if (currentState == States.Crafting && resourcePrefab != null && craftedResource == null)
            {
                GameObject newFoodGO = Instantiate(resourcePrefab, spawnPoint.position, Quaternion.identity);
                craftedResource = newFoodGO.GetComponent<ResourceCrafted>();

                if (craftedResource != null)
                {
                    craftedResource.ResourceCollected += OnResourceCollected;

                    currentState = States.Waiting;
                }
                else
                {
                    Debug.LogError("O Prefab da comida não tem o componente CraftedFood!");
                    ResetTimerAndState();
                }
            }
        }

        private void OnResourceCollected(ResourceCrafted resource)
        {
            if (craftedResource == resource)
            {
                craftedResource.ResourceCollected -= OnResourceCollected;
                craftedResource = null;
                ResetTimerAndState();
                StartCraft();
            }
        }

        public void StartCraft()
        {
            if (currentState == States.Ready)
            {
                currentState = States.Crafting;
            }
        }

        public void PauseCraft()
        {
            if (currentState != States.Waiting)
            {
                currentState = States.Paused;
            }
        }

        public void ResumeCraft()
        {
            if (currentState == States.Paused)
            {
                currentState = States.Crafting;
            }
        }

        private void ResetTimerAndState()
        {
            counterTimer = timerToCraft;
            currentState = (craftedResource == null) ? States.Ready : States.Waiting;
        }
    }
}