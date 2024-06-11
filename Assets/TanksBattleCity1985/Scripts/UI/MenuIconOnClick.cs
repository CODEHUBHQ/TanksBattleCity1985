using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuIconOnClick : MonoBehaviour
{
    //private void OnMouseUpAsButton()
    //{
    //    if (!MapEditorLoadMapHandler.Instance.IsOpen() && !MapEditorPlayMapHandler.Instance.IsOpen())
    //    {
    //        MapEditorGameMenuUI.Instance.OpenMapEditorMenu();
    //    }
    //}

    public void OnMapEditorMenuClicked()
    {
        if (!MapEditorLoadMapHandler.Instance.IsOpen() && !MapEditorPlayMapHandler.Instance.IsOpen())
        {
            MapEditorGameMenuUI.Instance.OpenMapEditorMenu();
        }
    }
}
