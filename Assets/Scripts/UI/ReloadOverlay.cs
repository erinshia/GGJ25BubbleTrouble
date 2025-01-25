using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class ReloadOverlay : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    private bool _firstTime = true;
        
    private void OnEnable()
    {
        Time.timeScale = 0;
        StartCoroutine(ReloadingCoroutine());
    }

    private IEnumerator ReloadingCoroutine()
    {
        if (!_firstTime)
        {
            _videoPlayer.playbackSpeed = 10;
        }
        _videoPlayer.Play();
        _firstTime = false;
        yield return new WaitForSecondsRealtime(((float) _videoPlayer.length) / _videoPlayer.playbackSpeed);
        GlobalEventHandler.Instance.ReloadFinished();
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
