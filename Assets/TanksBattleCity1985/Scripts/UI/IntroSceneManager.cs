using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        //videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
        StartCoroutine(nameof(LoadMenuScene));
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        SceneManager.LoadScene("MenuScene");
    }

    private IEnumerator LoadMenuScene()
    {
        yield return new WaitForSeconds(12f);
        SceneManager.LoadScene("MenuScene");
    }
}
