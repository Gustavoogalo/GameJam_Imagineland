using UnityEngine;
using System;

namespace Stands
{
    public class WinningBullet : MonoBehaviour
    {
        // NOVO: Adicione um campo para a velocidade. Se a força for aplicada, esta velocidade 
        // é o quão rápido o projétil se moverá após o lançamento.
        [Header("Configuração do Projétil")] 
        [Tooltip("A Tag que identifica o inimigo final na cena.")] 
        [SerializeField]
        private string finalEnemyTag = "FinalEnemy";

        [Tooltip("Tempo de vida do projétil caso não atinja nada.")] 
        [SerializeField]
        private float lifetime = 5f;

        private Rigidbody rb;

        // O método Awake agora apenas inicializa e configura o Rigidbody
        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning(
                    "O Projétil de Vitória não tem Rigidbody e foi adicionado automaticamente. Considere adicionar um manualmente.");
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.useGravity = false;

            // Destrói o projétil após um tempo para limpar a cena
            Destroy(gameObject, lifetime);
        }
        
        /// <summary>
        /// Método chamado por quem instanciou o projétil para lançá-lo.
        /// </summary>
        /// <param name="targetDirection">A direção normalizada para onde o projétil deve ir.</param>
        /// <param name="force">A força/velocidade a ser aplicada.</param>
        public void Launch(Vector3 targetDirection, float force)
        {
            if (rb != null)
            {
                transform.forward = targetDirection; 
                rb.AddForce(targetDirection * force, ForceMode.VelocityChange);
            }
        }
        
        /// <summary>
        /// Chamado quando o projétil colide com outro objeto.
        /// É NECESSÁRIO que o projétil e o inimigo final tenham Colliders e Rigidbody.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other.gameObject);
        }
        private void HandleHit(GameObject hitObject)
        {
            // 1. Verifica se atingiu o Inimigo Final
            if (hitObject.CompareTag(finalEnemyTag))
            {
                // O jogo é vencido!
                Debug.Log($"Inimigo Final '{finalEnemyTag}' atingido pelo projétil de vitória! JOGO VENCIDO!");

                // Dispara o evento de Ação - "Venceu o Jogo"
                BaseFinalAction.InvokeGameWonEvent();
                
                // Destrói o projétil após o impacto
                Destroy(gameObject);
            }
            else
            {
                // Opcional: Se atingiu algo, destrua a bala para evitar que ela continue.
                // Destroy(gameObject);
            }
        }
    }
}