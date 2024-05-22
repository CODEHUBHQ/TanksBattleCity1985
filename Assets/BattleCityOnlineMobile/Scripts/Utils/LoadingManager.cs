using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LoadingManager
{
    public enum Scene
    {
        MenuScene,
        GameScene,
        LobbyScene,
        LoadingScene,
        MapEditorScene,
        SettingsScene,
    }

    public static Scene targetScene;

    private class LoadingMonoBehaviour : MonoBehaviour { }

    private static Action onLoadingManagerCallback;

    private static AsyncOperation asyncOperation;

    public static void LoadScene(Scene scene)
    {
        targetScene = scene;

        onLoadingManagerCallback = () =>
        {
            var loadingGameObject = new GameObject("LoadingGameObject");

            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };

        SceneManager.LoadScene($"{Scene.LoadingScene}");
    }

    private static IEnumerator LoadSceneAsync(Scene scene)
    {
        yield return null;

        asyncOperation = SceneManager.LoadSceneAsync($"{scene}");

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static void LoadSceneNetwork(Scene scene)
    {
        onLoadingManagerCallback = () =>
        {
            PhotonNetwork.LoadLevel($"{scene}");
        };

        PhotonNetwork.LoadLevel($"{Scene.LoadingScene}");
    }

    public static float GetLoadingProgress()
    {
        if (asyncOperation != null)
        {
            return asyncOperation.progress;
        }

        return 0.3f;
    }

    public static void LoadingManagerCallback()
    {
        if (onLoadingManagerCallback != null)
        {
            onLoadingManagerCallback?.Invoke();

            onLoadingManagerCallback = null;
        }
    }
}
