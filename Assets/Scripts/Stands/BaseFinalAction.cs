using UnityEngine;
using System;

namespace Stands
{
    // Esta classe contém a lógica da ação final da base, permitindo a modularidade.
    public class BaseFinalAction : MonoBehaviour
    {
        // Evento que será disparado ao VENCER o jogo (agora, disparado pelo projétil)
        public static event Action OnGameWon;

        [Header("Mecânica do Projétil de Vitória")]
        [Tooltip("O Prefab do projétil que será disparado.")]
        [SerializeField]
        private GameObject winningBulletPrefab;
        public float bulletForce = 10f; // A força a ser usada no lançamento

        [Tooltip("O Transform de onde o projétil será spawnado (o canhão da base).")]
        [SerializeField]
        private Transform bulletSpawnPoint;

        [Tooltip("O alvo para onde o projétil deve ser direcionado.")]
        [SerializeField]
        private Transform targetToBullet;

        [Tooltip("A câmera a ser ativada/controlada na ação final.")]
        [SerializeField]
        private GameObject cameraToEnemy;

        // O método principal que o BaseMechanic irá chamar
        public void ExecuteFinalAction()
        {
            SpawnWinningBullet();
        }

        // Método para spawnar e lançar o projétil
        private void SpawnWinningBullet()
        {
            if (winningBulletPrefab == null || bulletSpawnPoint == null || targetToBullet == null)
            {
                Debug.LogError(
                    "WinningBulletPrefab, BulletSpawnPoint ou TargetToBullet não configurados. Não foi possível disparar o projétil.");
                return;
            }

            // 1. Calcule a direção
            Vector3 direction = (targetToBullet.position - bulletSpawnPoint.position).normalized;

            // 2. Instancia o projétil na posição de spawn.
            // A rotação inicial não é crítica, pois o método Launch a corrigirá.
            GameObject bulletGO = Instantiate(winningBulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
            
            // 3. Obtém o script do projétil
            if (bulletGO.TryGetComponent<WinningBullet>(out WinningBullet bulletScript))
            {
                Debug.Log("Projétil de Vitória disparado e lançado pela Ação Final!");
                
                // 4. Lança o projétil usando a direção e a força
                bulletScript.Launch(direction, bulletForce);
                
                // Opcional: faz o ponto de spawn olhar para o alvo (apenas visual)
                bulletSpawnPoint.LookAt(targetToBullet);
            }
            else
            {
                Debug.LogError("O Prefab do Projétil de Vitória não contém o script WinningBullet.");
                Destroy(bulletGO); // Limpa o objeto recém-criado
                return;
            }

            // Ativa a câmera de visualização
            if (cameraToEnemy != null)
            {
                cameraToEnemy.SetActive(true);
            }
        }

        // Método estático para invocar o evento de vitória (permanece o mesmo)
        public static void InvokeGameWonEvent()
        {
            OnGameWon?.Invoke();
            Debug.Log("Evento de Vitória Invocado!");
        }
    }
}