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
        public float bulletForce = 10f; // Valor padrão para força

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

        // Método para spawnar o projétil (lógica movida do BaseMechanic)
        private void SpawnWinningBullet()
        {
            if (winningBulletPrefab == null || bulletSpawnPoint == null)
            {
                Debug.LogError(
                    "WinningBulletPrefab ou BulletSpawnPoint não configurados. Não foi possível disparar o projétil.");
                return;
            }

            // Garante que o ponto de spawn esteja olhando para o alvo
            if (targetToBullet != null)
            {
                bulletSpawnPoint.LookAt(targetToBullet);
            }

            // Instancia o projétil na posição e rotação do ponto de spawn
            GameObject bullet = Instantiate(winningBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Debug.Log("Projétil de Vitória disparado pela Ação Final!");

            // Ativa a câmera de visualização
            if (cameraToEnemy != null)
            {
                cameraToEnemy.SetActive(true);
            }

            // Adiciona a força para iniciar o movimento
            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                // Aplica a força na direção forward do ponto de spawn
                rb.AddForce(bulletSpawnPoint.forward * bulletForce, ForceMode.VelocityChange);
                // Ou, se preferir setar a velocidade diretamente:
                // rb.linearVelocity = bulletSpawnPoint.forward * bulletForce;
            }
            else
            {
                Debug.LogWarning("O Projétil de Vitória não tem um Rigidbody para aplicar a força.");
            }
        }

        // Método estático para invocar o evento de vitória (pode ser chamado pelo projétil ao acertar o alvo)
        public static void InvokeGameWonEvent()
        {
            OnGameWon?.Invoke();
            Debug.Log("Evento de Vitória Invocado!");
        }
    }
}