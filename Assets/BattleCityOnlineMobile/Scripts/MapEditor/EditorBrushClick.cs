using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBrushClick : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        var r = spriteRenderer.color.r;
        var g = spriteRenderer.color.g;
        var b = spriteRenderer.color.b;

        spriteRenderer.color = new Color(r, g, b, 0.4f);
    }

    private void OnMouseExit()
    {
        var r = spriteRenderer.color.r;
        var g = spriteRenderer.color.g;
        var b = spriteRenderer.color.b;

        spriteRenderer.color = new Color(r, g, b, 1f);
    }

    private void OnMouseUpAsButton()
    {
        if (MapEditorHandler.Instance.IsPaused())
        {
            return;
        }

        if (gameObject.name.Contains("Erazer"))
        {
            MapEditorHandler.Instance.SetEditorBrush(null);
            MapEditorHandler.Instance.SetIsErazing(true);

            SetRenderer();
        }

        if (gameObject.name.Contains("Wall"))
        {
            MapEditorHandler.Instance.SetEditorBrush(MapEditorHandler.Instance.GetWallEditorPrefab());
            MapEditorHandler.Instance.SetIsErazing(false);

            SetRenderer();
        }

        if (gameObject.name.Contains("Iron"))
        {
            MapEditorHandler.Instance.SetEditorBrush(MapEditorHandler.Instance.GetIronEditorPrefab());
            MapEditorHandler.Instance.SetIsErazing(false);

            SetRenderer();
        }

        if (gameObject.name.Contains("Bush"))
        {
            MapEditorHandler.Instance.SetEditorBrush(MapEditorHandler.Instance.GetBushEditorPrefab());
            MapEditorHandler.Instance.SetIsErazing(false);

            SetRenderer();
        }

        if (gameObject.name.Contains("Ice"))
        {
            MapEditorHandler.Instance.SetEditorBrush(MapEditorHandler.Instance.GetIceEditorPrefab());
            MapEditorHandler.Instance.SetIsErazing(false);

            SetRenderer();
        }

        if (gameObject.name.Contains("Water"))
        {
            MapEditorHandler.Instance.SetEditorBrush(MapEditorHandler.Instance.GetWaterEditorPrefab());
            MapEditorHandler.Instance.SetIsErazing(false);

            SetRenderer();
        }
    }

    private void SetRenderer()
    {
        foreach (var go in MapEditorHandler.Instance.GetAllTools())
        {
            if (go == gameObject) continue;

            go.transform.Find("Selected").gameObject.SetActive(false);
        }

        gameObject.transform.Find("Selected").gameObject.SetActive(true);
    }
}
