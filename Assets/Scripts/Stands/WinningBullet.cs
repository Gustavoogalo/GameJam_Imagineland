using UnityEngine;
namespace Stands
{
    public class WinningBullet : MonoBehaviour
    {
        [Header("Configuração do Projétil")] [Tooltip("A velocidade com que o projétil se move.")] [SerializeField]
        private float speed = 50f;

        [Tooltip("A Tag que identifica o inimigo final na cena.")] [SerializeField]
        private string finalEnemyTag = "FinalEnemy";

        [Tooltip("Tempo de vida do projétil caso não atinja nada.")] [SerializeField]
        private float lifetime = 5f;

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogWarning(
                    "O Projétil de Vitória não tem Rigidbody e foi adicionado automaticamente. Considere adicionar um manualmente.");
                rb = gameObject.AddComponent<Rigidbody>();
            }

            // Configura o Rigidbody para ter movimento cinemático ou não.
            // É recomendado desmarcar "Use Gravity" e marcar "Is Kinematic" se for um projétil simples.
            rb.useGravity = false;

            // Define a velocidade inicial no sentido forward (para a frente) do ponto de spawn
            rb.linearVelocity = transform.forward * speed;

            // Destrói o projétil após um tempo para limpar a cena
            Destroy(gameObject, lifetime);
        }

        /// <summary>
        /// Chamado quando o projétil colide com outro objeto.
        /// É NECESSÁRIO que o projétil e o inimigo final tenham Colliders e Rigidbody.
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            HandleHit(collision.gameObject);
        }

        /*
        // Se você preferir usar Triggers (Colliders com 'Is Trigger' marcado):
        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other.gameObject);
        }
        */

        private void HandleHit(GameObject hitObject)
        {
            // 1. Verifica se atingiu o Inimigo Final
            if (hitObject.CompareTag(finalEnemyTag))
            {
                // O jogo é vencido!
                Debug.Log($"Inimigo Final '{finalEnemyTag}' atingido pelo projétil de vitória! JOGO VENCIDO!");

                // Dispara o evento de Ação - "Venceu o Jogo"
                //BaseMechanic.OnGameWon?.Invoke();
                BaseFinalAction.InvokeGameWonEvent();
                // Destrói o projétil após o impacto
                Destroy(gameObject);
            }
            else
            {
                // Opcional: Destruir o projétil ao atingir qualquer coisa que não seja o inimigo final 
                // para evitar que ele voe pela cena indefinidamente.
                Destroy(gameObject);
            }
        }
    }
}