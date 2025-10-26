using UnityEngine;
using System.Collections;
using Stands; 

public class VictoryPanel : MonoBehaviour
{
    [Header("Configuração da Animação")]
    [Tooltip("A duração total da animação de crescimento (em segundos).")]
    [SerializeField]
    private float animationDuration = 0.5f;

    [Tooltip("A curva de animação para controlar a velocidade do crescimento (facilita o 'juice').")]
    [SerializeField]
    private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private RectTransform rectTransform;
    private Vector3 initialScale;

    void Awake()
    {
        // rectTransform = GetComponent<RectTransform>();
        // if (rectTransform == null)
        // {
        //     Debug.LogError("O objeto VictoryPanel deve ter um componente RectTransform.");
        //     enabled = false;
        //     return;
        // }

        // 1. Armazena a escala final desejada
       // initialScale = rectTransform.localScale;

        // 2. Inicialmente, o painel deve estar na escala zero
        //rectTransform.localScale = Vector3.zero;
        
        // Desativa o GameObject pai
        gameObject.SetActive(false); 
    }

    // REMOVIDO: Os métodos OnEnable e OnDisable (não precisamos mais da inscrição aqui)
    /*
    void OnEnable() { BaseFinalAction.OnGameWon += OnGameWonHandler; }
    void OnDisable() { BaseFinalAction.OnGameWon -= OnGameWonHandler; }
    private void OnGameWonHandler() { StartScaleAnimation(); } // REMOVIDO: Este método não é mais necessário
    */


    /// <summary>
    /// MÉTODO PÚBLICO: Chamado pelo FinalEnemyHandler para iniciar a animação.
    /// </summary>
    public void StartScaleAnimation()
    {
        var animator = GetComponent<Animator>();
        animator.Play("victoryAnim");
        // Se já estiver ativo, garante que a coroutine antiga pare antes de começar uma nova
        //StopAllCoroutines(); 
        //StartCoroutine(ScaleUpAnimation());
    }

    /// <summary>
    /// Coroutine que faz o painel crescer de 0 até a escala normal (initialScale).
    /// </summary>
    private IEnumerator ScaleUpAnimation()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime = Time.time - startTime;
            float progress = elapsedTime / animationDuration;
            float curveValue = scaleCurve.Evaluate(progress);

            // Calcula a nova escala interpolando
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, initialScale, curveValue);

            yield return null; 
        }

        // Garante a escala final
        rectTransform.localScale = initialScale;
        Debug.Log("Painel de Vitória Animado e Ativo.");
    }
}