using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public void OnGameOverEnded()
    {
        StartCoroutine(ResetGameDelayed());
    }

    private IEnumerator ResetGameDelayed()
    {
        yield return new WaitForSeconds(2f);

        var isCustomMap = bool.Parse(PlayerPrefs.GetString(StaticStrings.IS_CUSTOM_MAP, "false"));

        if (isCustomMap)
        {
            LoadingManager.LoadScene(LoadingManager.Scene.MapEditorScene);
        }
        else
        {
            BattleCityMapLoad.Instance.LoadMap(1);

            var gameOverAnimation = GetComponent<Animation>();

            GameManager.Instance.SetIsGameOver(false);

            gameObject.SetActive(false);

            gameOverAnimation.Rewind();
            gameOverAnimation.Play();
            gameOverAnimation.Sample();
            gameOverAnimation.Stop();
        }
    }
}
