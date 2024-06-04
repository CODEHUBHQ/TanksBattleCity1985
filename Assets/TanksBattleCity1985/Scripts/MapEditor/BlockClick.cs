using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockClick : MonoBehaviour
{
    private Vector2Int coords;

    private void OnMouseEnter()
    {
        if (Input.GetMouseButton(0))
        {
            TryAddCoords();
        }
    }

    private void OnMouseDrag()
    {
        TryAddCoords();
    }

    private void OnMouseUpAsButton()
    {
        TryAddCoords();
    }

    private void TryAddCoords()
    {
        if (MapEditorHandler.Instance.IsPaused())
        {
            return;
        }

        coords = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        var filteredEnemiesSpawnPosList = MapEditorHandler.Instance.GetFilteredEnemiesSpawnPosList();
        var filteredPlayersSpawnPosList = MapEditorHandler.Instance.GetFilteredPlayersSpawnPosList();
        var filteredEagleBasePosList = MapEditorHandler.Instance.GetFilteredEagleBasePosList();

        if (filteredEnemiesSpawnPosList.Contains(new Vector2Int(coords.x, coords.y)) ||
                filteredPlayersSpawnPosList.Contains(new Vector2Int(coords.x, coords.y)) ||
                filteredEagleBasePosList.Contains(new Vector2Int(coords.x, coords.y)))
        {
            return;
        }

        if (!MapEditorHandler.Instance.IsErazing() && MapEditorHandler.Instance.GetEditorBrush() != null)
        {
            var editorBrush = MapEditorHandler.Instance.GetEditorBrush();

            if (editorBrush != null)
            {
                if (editorBrush.name.Contains("Wall"))
                {
                    MapEditorHandler.Instance.TryAddCoordsToChange(coords, "o");
                }
                else if (editorBrush.name.Contains("Iron"))
                {
                    MapEditorHandler.Instance.TryAddCoordsToChange(coords, "Q");
                }
                else if (editorBrush.name.Contains("Bush"))
                {
                    MapEditorHandler.Instance.TryAddCoordsToChange(coords, "b");
                }
                else if (editorBrush.name.Contains("Ice"))
                {
                    MapEditorHandler.Instance.TryAddCoordsToChange(coords, "i");
                }
                else if (editorBrush.name.Contains("Water"))
                {
                    MapEditorHandler.Instance.TryAddCoordsToChange(coords, "w");
                }
            }
        }
        else if (MapEditorHandler.Instance.IsErazing() && MapEditorHandler.Instance.GetEditorBrush() == null)
        {
            if (!gameObject.name.Contains("EmptySpace"))
            {
                MapEditorHandler.Instance.TryAddCoordsToRemove(coords, ".");
            }
        }
    }
}
