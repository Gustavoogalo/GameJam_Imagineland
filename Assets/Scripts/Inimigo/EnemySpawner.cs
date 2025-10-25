using UnityEngine;
using System.Collections.Generic;

namespace Inimigo
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 15;
        [SerializeField] private Transform spawnPoint;

        [Header("Enemy Prefabs")]
        [SerializeField] private EnemyManager enemyTypeAPrefab;
        [SerializeField] private EnemyManager enemyTypeBPrefab;

        public enum EnemyTypeToSpawn { TypeA, TypeB }
        [Header("Spawn Control")]
        public EnemyTypeToSpawn currentSpawnType = EnemyTypeToSpawn.TypeA;

        [Header("Spawn Timer")]
        [SerializeField] private float timeBetweenSpawns = 5f;
        private float spawnCounter;

        public enum SpawnerState { Active, Paused }
        private SpawnerState currentState = SpawnerState.Active;

        private List<EnemyManager> availableEnemies = new List<EnemyManager>();
        private List<EnemyManager> activeEnemies = new List<EnemyManager>();
        
        [SerializeField] private List<Transform> targetStands = new List<Transform>();

        private void Awake()
        {
            if (spawnPoint == null)
            {
                spawnPoint = transform;
            }
            InitializePool();
            spawnCounter = timeBetweenSpawns;
        }

        private void InitializePool()
        {
            int countA = poolSize / 2;
            int countB = poolSize - countA; 

            for (int i = 0; i < countA; i++)
            {
                CreateNewEnemy(enemyTypeAPrefab, EnemyManager.EnemyType.TypeA);
            }

            for (int i = 0; i < countB; i++)
            {
                CreateNewEnemy(enemyTypeBPrefab, EnemyManager.EnemyType.TypeB);
            }
        }

        private void CreateNewEnemy(EnemyManager prefab, EnemyManager.EnemyType type)
        {
            if (prefab == null) return;

            EnemyManager newEnemy = Instantiate(prefab, transform);
            var randomStand = Random.Range(0, targetStands.Count);
            newEnemy.Setup(this, type, targetStands[randomStand]);
            newEnemy.OnReturnToPool += ReturnEnemyToPool;
            
            availableEnemies.Add(newEnemy);
        }

        private void Update()
        {
            if (currentState == SpawnerState.Paused)
            {
                return;
            }

            if (spawnCounter > 0)
            {
                spawnCounter -= Time.deltaTime;
            }
            else
            {
                AttemptToSpawnEnemy();
                spawnCounter = timeBetweenSpawns;
            }
        }

        private void AttemptToSpawnEnemy()
        {
            EnemyManager spawned = SpawnEnemy();
            if (spawned == null)
            {
                spawnCounter = 0;
            }
        }

        public EnemyManager SpawnEnemy()
        {
            EnemyManager.EnemyType targetType = currentSpawnType == EnemyTypeToSpawn.TypeA ? 
                                                    EnemyManager.EnemyType.TypeA : 
                                                    EnemyManager.EnemyType.TypeB;
            
            EnemyManager enemyToSpawn = availableEnemies.Find(e => e.Type == targetType);

            if (enemyToSpawn != null)
            {
                availableEnemies.Remove(enemyToSpawn);
                activeEnemies.Add(enemyToSpawn);
                
                enemyToSpawn.Activate(spawnPoint.position);
                
                return enemyToSpawn;
            }
            else
            {
                return null;
            }
        }
        
        private void ReturnEnemyToPool(EnemyManager enemy)
        {
            if (activeEnemies.Contains(enemy))
            {
                activeEnemies.Remove(enemy);
                availableEnemies.Add(enemy);
                
                if (spawnCounter <= 0)
                {
                    spawnCounter = 0;
                }
            }
        }

        public void PauseSpawner()
        {
            currentState = SpawnerState.Paused;
        }

        public void ResumeSpawner()
        {
            if (currentState == SpawnerState.Paused)
            {
                currentState = SpawnerState.Active;
            }
        }
    }
}