using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameMenuUISelectionButtonHandler : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        var gameMenuOrderedButtons = GameMenuUI.Instance.GameMenuOrderedButtons;

        for (int i = 0; i < gameMenuOrderedButtons.Count; i++)
        {
            var button = gameMenuOrderedButtons[i];

            if (button.gameObject.name == gameObject.name)
            {
                if (gameObject.transform.Find("Selected").TryGetComponent(out Image selectedButtonImage))
                {
                    selectedButtonImage.enabled = true;
                }

                continue;
            }

            if (button.gameObject.transform.Find("Selected").TryGetComponent(out Image notSelectedButtonImage))
            {
                notSelectedButtonImage.enabled = false;
            }
        }
    }
}
