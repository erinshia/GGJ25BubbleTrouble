using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void OnClick(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
