using UnityEngine;
using Inventory;
using System;
using System.Linq;

namespace Stands
{
    // Define o estado atual da base para facilitar o controle visual
    public enum BaseStage
    {
        Initial, // 1/3 do progresso total ou menos
        Medium, // Acima de 1/3 até 2/3 do progresso total
        Final // Acima de 2/3 até o total (3/3)
    }

    public class BaseMechanic : MonoBehaviour
    {
        [Header("Inventário")] [Tooltip("O InventoryResource que a base irá monitorar.")] [SerializeField]
        private InventoryResource baseInventory;

        [Header("Configuração de Progresso")]
        [Tooltip("O tipo de recurso que determina o progresso da base.")]
        [SerializeField]
        private ResourceType requiredResourceType = ResourceType.tipo1;

        [Tooltip("A quantidade TOTAL deste recurso necessária para atingir 3/3 de vida (Disparo do Projétil).")]
        [SerializeField]
        private int requiredTotalAmount = 100;

        [Header("Modelos 3D por Estágio")] [Tooltip("Modelo 3D para o estágio Inicial (0 a 1/3).")] [SerializeField]
        private GameObject initialModel;

        [Tooltip("Modelo 3D para o estágio Médio (acima de 1/3 a 2/3).")] [SerializeField]
        private GameObject mediumModel;

        [Tooltip("Modelo 3D para o estágio Final (acima de 2/3 a 3/3).")] [SerializeField]
        private GameObject finalModel;

        // NOVO: Mecânica do Projétil de Vitória
        [Header("Mecânica do Projétil de Vitória")]
        [Tooltip("O Prefab do projétil que será disparado.")]
        [SerializeField]
        private GameObject winningBulletPrefab;
        public float bulletForce;

        [Tooltip("O Transform de onde o projétil será spawnado (o canhão da base).")] [SerializeField]
        private Transform bulletSpawnPoint;

        [SerializeField] private Transform targetToBullet;
        [SerializeField] private GameObject cameratoEnemy;


        // Evento que será disparado ao VENCER o jogo (agora, disparado pelo projétil)
        public static event Action OnGameWon;

        // Variáveis de estado
        private BaseStage currentStage = BaseStage.Initial;
        private bool hasBulletBeenSpawned = false; // Garante que o projétil só dispare uma vez

        // ... (métodos Awake, OnEnable, OnDisable, OnBaseResourceChanged permanecem os mesmos) ...

        private void Awake()
        {
            // Verifica se o inventário foi anexado
            if (baseInventory == null)
            {
                Debug.LogError("BaseMechanic precisa de uma referência ao InventoryResource.");
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            baseInventory.OnResourceChanged += OnBaseResourceChanged;
            UpdateBaseState(baseInventory.GetResourceAmount(requiredResourceType));
        }

        private void OnDisable()
        {
            baseInventory.OnResourceChanged -= OnBaseResourceChanged;
        }

        private void OnBaseResourceChanged(ResourceType type, int newAmount)
        {
            if (type == requiredResourceType)
            {
                UpdateBaseState(newAmount);
            }
        }

        private void UpdateBaseState(int currentAmount)
        {
            if (requiredTotalAmount <= 0)
            {
                Debug.LogWarning("requiredTotalAmount precisa ser maior que zero.");
                return;
            }

            int clampedAmount = Mathf.Min(currentAmount, requiredTotalAmount);
            float progress = (float)clampedAmount / requiredTotalAmount;

            const float oneThird = 1f / 3f;
            const float twoThirds = 2f / 3f;

            BaseStage newStage = currentStage;

            if (progress >= twoThirds)
            {
                newStage = BaseStage.Final;
            }
            else if (progress >= oneThird)
            {
                newStage = BaseStage.Medium;
            }
            else
            {
                newStage = BaseStage.Initial;
            }

            // Atualiza os modelos visuais apenas se o estágio mudou
            if (newStage != currentStage)
            {
                currentStage = newStage;
                UpdateVisualModel();
            }

            // NOVO: Verifica 100% para disparar o projétil
            if (progress >= 1.0f && !hasBulletBeenSpawned)
            {
                Debug.Log("Base Completa! Disparando o Projétil de Vitória...");
                hasBulletBeenSpawned = true;
                SpawnWinningBullet();

                // Opcional: Desabilitar o monitoramento da base após o disparo
                // baseInventory.OnResourceChanged -= OnBaseResourceChanged;
            }

            Debug.Log($"Progresso da Base: {Mathf.Round(progress * 100)}% - Estágio: {currentStage}");
        }

        // NOVO: Método para spawnar o projétil
        private void SpawnWinningBullet()
        {
            if (winningBulletPrefab == null || bulletSpawnPoint == null)
            {
                Debug.LogError(
                    "WinningBulletPrefab ou BulletSpawnPoint não configurados. Não foi possível disparar o projétil.");
                return;
            }

            bulletSpawnPoint.LookAt(targetToBullet);

            // Instancia o projétil na posição e rotação do ponto de spawn
            GameObject bullet = Instantiate(winningBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Debug.Log("Projétil de Vitória disparado!");

            cameratoEnemy.SetActive(true);
            // Opcional: Adicionar uma força para iniciar o movimento (pode ser movido para o script do projétil)
            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.linearVelocity = bulletSpawnPoint.forward * 5 * Time.deltaTime; 
            }
        }

        private void UpdateVisualModel()
        {
            // ... (Lógica de ativação/desativação de modelos permanece a mesma) ...
            if (initialModel != null) initialModel.SetActive(false);
            if (mediumModel != null) mediumModel.SetActive(false);
            if (finalModel != null) finalModel.SetActive(false);

            switch (currentStage)
            {
                case BaseStage.Initial:
                    if (initialModel != null) initialModel.SetActive(true);
                    break;
                case BaseStage.Medium:
                    if (mediumModel != null) mediumModel.SetActive(true);
                    break;
                case BaseStage.Final:
                    if (finalModel != null) finalModel.SetActive(true);
                    break;
            }

            Debug.Log($"Modelo da Base Atualizado para: {currentStage}");
        }

        public static void InvokeGameWonEvent()
        {
            // A invocação real do evento acontece aqui dentro, onde é permitido.
            OnGameWon?.Invoke();

            // NOTA: Se você quiser que o script BaseMechanic pare de reagir a tudo após a vitória,
            // considere adicionar mais lógica aqui, mas por enquanto vamos manter o foco no evento.
        }
    }
}