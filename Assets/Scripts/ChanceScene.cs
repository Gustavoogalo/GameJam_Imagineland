using UnityEngine;
using UnityEngine.SceneManagement;

public class ChanceScene : MonoBehaviour
{
    public class ChangeScene : MonoBehaviour
    {
        public void Change(int scene)
        {
            SceneManager.LoadScene(scene);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
