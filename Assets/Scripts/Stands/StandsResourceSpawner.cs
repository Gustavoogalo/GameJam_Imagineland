using System;
using Stands.Resource;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        [Header("Crafting Settings")]
        [SerializeField]
        private float timerToCraft = 5f;

        [Header("Sliders")]
        [SerializeField] private float counterTimer;
        [SerializeField] private float maxHealth = 10f;
        [SerializeField] private float health;
        [SerializeField] private float healthDecayRate = 0.5f;
        [SerializeField] private Slider sliderHealth;
        [SerializeField] private Slider sliderResource;

        [SerializeField] private GameObject gameOver;
        [SerializeField] private GameObject resourcePrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private States currentState;
        [SerializeField] private ResourceCrafted craftedResource = null;

        public int enemiesinRange = 0;

        public float CurrentCraftProgress => (timerToCraft - counterTimer) / timerToCraft;
        public States CurrentState => currentState;

        private void Awake()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }

            Time.timeScale = 1f;
        }

        private void Start()
        {
            health = maxHealth;
            StartCraft();
            sliderHealth.maxValue = maxHealth;
            sliderHealth.value = health;
            sliderResource.maxValue = timerToCraft;
            sliderResource.value = counterTimer;
        }

        private void Update()
        {
            if (enemiesinRange > 0)
            {
                PauseCraft();
            }

            switch (currentState)
            {
                case States.Crafting:
                    HandleCraftingState();
                    break;
                case States.Paused:
                    HandlePausedState();
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
                sliderResource.value = counterTimer;
            }
            else
            {
                Craft();
            }
        }
        private void HandlePausedState()
        {
            // reduz vida no pause
            health -= healthDecayRate * Time.deltaTime;
            sliderHealth.value = health;

            if (enemiesinRange <= 0)
            {
                ResumeCraft();
            }

            if (health <= 0)
            {
                Debug.Log("Stand destruído por falta de vida!");
                gameOver.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1f;
        }

        public void BackMenu(string scene)
        {
            SceneManager.LoadScene(scene);
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
            if (currentState == States.Paused) return;
            //if (currentState != States.Waiting)
            //{
                currentState = States.Paused;
           // }
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
            sliderResource.value = counterTimer;
        }
    }
}