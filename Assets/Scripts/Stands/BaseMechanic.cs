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
        
        // NOVO: Referência à Ação Final
        [Header("Ação Final")]
        [Tooltip("O script que contém a lógica de 'vitória' a ser executada ao completar a base (100%).")]
        [SerializeField]
        private BaseFinalAction finalAction; 
        
        // **REMOVIDO:** Os campos de projétil, força, spawnpoint e target.
        // **REMOVIDO:** O evento 'OnGameWon'. Ele foi movido para BaseFinalAction.
        // public static event Action OnGameWon; // Removido

        // Variáveis de estado
        private BaseStage currentStage = BaseStage.Initial;
        private bool hasFinalActionBeenExecuted = false; // Renomeado para refletir o novo propósito

        private void Awake()
        {
            if (baseInventory == null)
            {
                Debug.LogError("BaseMechanic precisa de uma referência ao InventoryResource.");
                enabled = false;
                return;
            }
            // NOVO: Checagem da Ação Final
            if (finalAction == null)
            {
                Debug.LogWarning("BaseMechanic não tem uma BaseFinalAction configurada. A ação final não será executada.");
            }
        }

        private void OnEnable()
        {
            baseInventory.OnResourceChanged += OnBaseResourceChanged;
            // Garante que o estado inicial seja atualizado
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

            // Lógica Modular: Verifica 100% para executar a Ação Final
            if (progress >= 1.0f && !hasFinalActionBeenExecuted)
            {
                Debug.Log("Base Completa! Executando a Ação Final...");
                hasFinalActionBeenExecuted = true;
                
                if (finalAction != null)
                {
                    finalAction.ExecuteFinalAction(); // CHAMA O NOVO SCRIPT DE AÇÃO
                }
                
                // Opcional: Desabilitar o monitoramento da base após o disparo
                // baseInventory.OnResourceChanged -= OnBaseResourceChanged;
            }

            Debug.Log($"Progresso da Base: {Mathf.Round(progress * 100)}% - Estágio: {currentStage}");
        }

        // **REMOVIDO:** O método SpawnWinningBullet. Ele foi movido para BaseFinalAction.

        private void UpdateVisualModel()
        {
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

        // **REMOVIDO:** O método InvokeGameWonEvent estático (movido para BaseFinalAction).
        /*
        public static void InvokeGameWonEvent()
        {
            OnGameWon?.Invoke();
        }
        */
    }
    
    // A classe BaseAction pode ser removida se BaseFinalAction for usada
    // ou renomeada para BaseMechanic, já que não é usada.
    // public class BaseAction {} 
}