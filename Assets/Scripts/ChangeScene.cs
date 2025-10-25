using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private GameObject credits;
    public void Change (string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void CreditsScene()
    {
        credits.SetActive(true);
    }

    public void Back()
    {
        credits.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Saiu!");
        Application.Quit();
    }
}
