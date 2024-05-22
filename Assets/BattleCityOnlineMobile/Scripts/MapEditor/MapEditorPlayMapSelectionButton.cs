using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapEditorPlayMapSelectionButton : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        var mainMenuOrderedButotns = MapEditorPlayMapHandler.Instance.PlayMapMenuOrderedButtons;

        for (int i = 0; i < mainMenuOrderedButotns.Count; i++)
        {
            var button = mainMenuOrderedButotns[i];

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
