using UnityEngine;
using Stands; // Para acessar BaseFinalAction

public class FinalEnemyHandler : MonoBehaviour
{
    [Header("Componente de Vitória")]
    [Tooltip("O GameObject do painel de vitória que será ativado e animado.")]
    [SerializeField]
    private VictoryPanel victoryPanel;

    [Tooltip("Opcional: O transform para onde o painel de vitória deve se mover/alinhar (se diferente do inimigo).")]
    [SerializeField]
    private Transform victoryPanelSpawnPoint;

    private void Awake()
    {
        if (victoryPanel == null)
        {
            Debug.LogError("O FinalEnemyHandler precisa de uma referência ao VictoryPanel.");
            enabled = false;
        }
        else
        {
            // O painel já deve estar desativado no script VictoryPanel.Awake, mas garantimos.
            victoryPanel.gameObject.SetActive(false); 
        }
    }

    private void OnEnable()
    {
        // Se inscreve no evento de vitória
        BaseFinalAction.OnGameWon += HandleVictory;
    }

    private void OnDisable()
    {
        // Desinscreve-se ao ser desativado ou destruído
        BaseFinalAction.OnGameWon -= HandleVictory;
    }

    /// <summary>
    /// Chamado quando o evento OnGameWon é disparado pela bala.
    /// </summary>
    private void HandleVictory()
    {
        Debug.Log("Inimigo Final recebeu o evento de Vitória. Ativando Painel.");

        if (victoryPanel != null)
        {
            // 1. Posiciona o painel (opcional, se você quiser que ele fique em um local específico)
            // if (victoryPanelSpawnPoint != null)
            // {
            //     // Se o painel for UI, ele deve ser filho do Canvas, então movemos o Canvas/Painel
            //     // para perto do inimigo (ajuste conforme seu setup de UI/Canvas World Space)
            //     victoryPanel.transform.position = victoryPanelSpawnPoint.position;
            //     victoryPanel.transform.rotation = victoryPanelSpawnPoint.rotation;
            // } 
            // else
            // {
            //     // Se não houver ponto de spawn, use a posição do inimigo
            //     // Isso só funciona se o painel estiver em World Space ou se for um elemento 3D
            //     // Se for um Canvas Screen Space, esta linha não terá efeito visual.
            //     victoryPanel.transform.position = transform.position;
            // }

            // 2. O VictoryPanel já contém a lógica de ativação e animação no seu OnGameWonHandler,
            // mas como movemos a inscrição para cá, precisamos reescrever a ativação.
            
            // O VictoryPanel precisa ter sua lógica modificada para não se inscrever mais no evento!
            
            // Se o VictoryPanel já for o objeto que queremos ativar (com o script VictoryPanel)
            victoryPanel.gameObject.SetActive(true);
            
            // O VictoryPanel deve conter um método público para iniciar a animação.
            // Para isso, faremos uma pequena modificação nele (veja o próximo passo).
            victoryPanel.StartScaleAnimation(); // Chamando o método público (ver passo 2)
            
            // Opcional: Destruir/desativar o inimigo.
            // Destroy(gameObject);
        }
    }
}