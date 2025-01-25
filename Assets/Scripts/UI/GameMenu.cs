using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private Canvas _pauseScreen;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpauseGame();
        }
    }

    [ContextMenu("Win Game")]
    private void CompleteGame()
    {
        _sceneLoader.LoadScene(2);
    }

    public void PauseUnpauseGame()
    {
        _pauseScreen.gameObject.SetActive(!_pauseScreen.gameObject.activeSelf);
        Time.timeScale = _pauseScreen.gameObject.activeSelf ? 0 : 1;
    }
}
