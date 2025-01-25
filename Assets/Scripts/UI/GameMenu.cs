using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private Canvas _pauseScreen;

    private void Start()
    {
        GlobalEventHandler.Instance.OnGameOver += LoadLoseScreen;
        GlobalEventHandler.Instance.OnWin += LoadWinScreen;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnGameOver -= LoadLoseScreen;
        GlobalEventHandler.Instance.OnWin -= LoadWinScreen;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpauseGame();
        }
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

    public void PauseUnpauseGame()
    {
        _pauseScreen.gameObject.SetActive(!_pauseScreen.gameObject.activeSelf);
        Time.timeScale = _pauseScreen.gameObject.activeSelf ? 0 : 1;
    }
}
