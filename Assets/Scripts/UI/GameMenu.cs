using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private Canvas _pauseScreen;
    private InputAction _escapeAction;
    
    private void Start()
    {
        GlobalEventHandler.Instance.OnGameOver += LoadLoseScreen;
        GlobalEventHandler.Instance.OnWin += LoadWinScreen;
        
        _escapeAction = InputSystem.actions.FindAction("OpenPauseMenu");
        _escapeAction.started += PauseUnpauseGame;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnGameOver -= LoadLoseScreen;
        GlobalEventHandler.Instance.OnWin -= LoadWinScreen;
        _escapeAction.started -= PauseUnpauseGame;
    }

    [ContextMenu("Win Game")]
    private void LoadWinScreen()
    {
        _sceneLoader.LoadScene(2);
    }

    private void LoadLoseScreen()
    {
        _sceneLoader.LoadScene(3);
    }

    private void PauseUnpauseGame(InputAction.CallbackContext obj)
    {
        _pauseScreen.gameObject.SetActive(!_pauseScreen.gameObject.activeSelf);
        Time.timeScale = _pauseScreen.gameObject.activeSelf ? 0 : 1;
    }
}
