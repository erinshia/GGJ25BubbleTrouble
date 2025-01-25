using System;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpauseGame();
        }
    }

    public void PauseUnpauseGame()
    {
        pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        Time.timeScale = pauseMenu.gameObject.activeSelf ? 0 : 1;
    }
}
