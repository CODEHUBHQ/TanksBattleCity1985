using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class MapEditorHandler : MonoBehaviour
{
    public static MapEditorHandler Instance { get; private set; }

    [SerializeField] private GameObject emptySpaceEditorPrefab;
    [SerializeField] private GameObject wallEditorPrefab;
    [SerializeField] private GameObject ironEditorPrefab;
    [SerializeField] private GameObject waterEditorPrefab;
    [SerializeField] private GameObject iceEditorPrefab;
    [SerializeField] private GameObject bushEditorPrefab;
    [SerializeField] private List<GameObject> allTools;
    [SerializeField] private List<Sprite> mapObjects;

    // debug only
    //[SerializeField] private SpriteRenderer referenceMap;

    private GameObject editorBrush;

    private List<Vector2Int> filteredEnemiesSpawnPosList;
    private List<Vector2Int> filteredPlayersSpawnPosList;
    private List<Vector2Int> filteredEagleBasePosList;
    private List<Vector2Int> filteredEagleBaseWallPosList;

    private Dictionary<Vector2Int, string> coordsToChange;
    private Dictionary<Vector2Int, string> coordsToUpdate;
    private Dictionary<Vector2Int, string> coordsToRemove;
    private Dictionary<Vector2Int, GameObject> instantiatedEditorPrefabs;

    private List<string> customMaps = new List<string>();

    private int currentMapIndex;

    private bool isErazing;
    private bool isPaused;

    private void Awake()
    {
        Instance = this;

        currentMapIndex = -1;

        LoadMaps();

        filteredEnemiesSpawnPosList = new List<Vector2Int>()
        {
            new Vector2Int(-13, 12),
            new Vector2Int(-12, 12),
            new Vector2Int(-13, 11),
            new Vector2Int(-12, 11),

            new Vector2Int(-1, 12),
            new Vector2Int(0, 12),
            new Vector2Int(-1, 11),
            new Vector2Int(0, 11),

            new Vector2Int(12, 12),
            new Vector2Int(11, 12),
            new Vector2Int(12, 11),
            new Vector2Int(11, 11),
        };

        filteredPlayersSpawnPosList = new List<Vector2Int>()
        {
            new Vector2Int(-5, -12),
            new Vector2Int(-4, -12),
            new Vector2Int(-4, -13),
            new Vector2Int(-5, -13),
            new Vector2Int(4, -12),
            new Vector2Int(3, -12),
            new Vector2Int(3, -13),
            new Vector2Int(4, -13),
        };

        filteredEagleBasePosList = new List<Vector2Int>()
        {
            new Vector2Int(-2, -13),
            new Vector2Int(-2, -12),
            new Vector2Int(-2, -11),
            new Vector2Int(-1, -11),
            new Vector2Int(0, -11),
            new Vector2Int(1, -11),
            new Vector2Int(1, -12),
            new Vector2Int(1, -13),
            new Vector2Int(0, -13),
            new Vector2Int(-1, -13),
            new Vector2Int(-1, -12),
            new Vector2Int(0, -12),
        };

        filteredEagleBaseWallPosList = new List<Vector2Int>()
        {
            new Vector2Int(-2, -13),
            new Vector2Int(-2, -12),
            new Vector2Int(-2, -11),
            new Vector2Int(-1, -11),
            new Vector2Int(0, -11),
            new Vector2Int(1, -11),
            new Vector2Int(1, -12),
            new Vector2Int(1, -13),
        };

        coordsToChange = new Dictionary<Vector2Int, string>();
        coordsToUpdate = new Dictionary<Vector2Int, string>();
        coordsToRemove = new Dictionary<Vector2Int, string>();
        instantiatedEditorPrefabs = new Dictionary<Vector2Int, GameObject>();
    }

    private void Start()
    {
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var go = Instantiate(emptySpaceEditorPrefab, new Vector3(j - 13, 13 - (i + 1), 0), emptySpaceEditorPrefab.transform.rotation);
                var x = Mathf.RoundToInt(go.transform.position.x);
                var y = Mathf.RoundToInt(go.transform.position.y);

                instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), go);
            }
        }
    }

    private void Update()
    {
        // debug only
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    if (coordsToUpdate.Count == 0)
        //    {
        //        return;
        //    }

        //    string map = default;

        //    for (int i = 0; i < 26; i++)
        //    {
        //        for (int j = 0; j < 26; j++)
        //        {
        //            var x = j - 13;
        //            var y = 13 - (i + 1);

        //            if (coordsToUpdate.TryGetValue(new Vector2Int(x, y), out string blockType))
        //            {
        //                if (blockType == "o")
        //                {
        //                    map += "o";
        //                }
        //                else if (blockType == "Q")
        //                {
        //                    map += "Q";
        //                }
        //                else if (blockType == "b")
        //                {
        //                    map += "b";
        //                }
        //                else if (blockType == "i")
        //                {
        //                    map += "i";
        //                }
        //                else if (blockType == "w")
        //                {
        //                    map += "w";
        //                }
        //            }
        //            else if (filteredEagleBaseWallPosList.Contains(new Vector2Int(x, y)))
        //            {
        //                map += "o";
        //            }
        //            else
        //            {
        //                map += ".";
        //            }

        //            if (j + 1 == 26)
        //            {
        //                map += "\n";
        //            }
        //        }
        //    }

        //    if (currentMapIndex == -1)
        //    {
        //        customMaps.Add(map);
        //    }
        //    else
        //    {
        //        customMaps[currentMapIndex] = map;
        //    }

        //    var mapNumber = referenceMap.sprite.name.Contains("bc0") ? referenceMap.sprite.name.Split("bc0")[1] : referenceMap.sprite.name.Split("bc")[1];

        //    Debug.Log($"saving map to map{mapNumber}.txt");

        //    File.WriteAllText($"{Application.streamingAssetsPath}/Maps/map{mapNumber}.txt", map);
        //}
    }

    private void DrawCoordsToMap()
    {
        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var x = j - 13;
                var y = 13 - (i + 1);

                if (isErazing)
                {
                    if (coordsToRemove.TryGetValue(new Vector2Int(x, y), out string blockType))
                    {
                        var go = Instantiate(emptySpaceEditorPrefab, new Vector3(x, y, 0), emptySpaceEditorPrefab.transform.rotation);

                        if (instantiatedEditorPrefabs.TryGetValue(new Vector2Int(x, y), out GameObject blockgo))
                        {
                            Destroy(blockgo);
                            instantiatedEditorPrefabs.Remove(new Vector2Int(x, y));
                        }

                        instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), go);

                        TryRemoveCoordsToUpdate(new Vector2Int(x, y));
                        TryRemoveCoordsToRemove(new Vector2Int(x, y));
                    }
                }
                else if (coordsToChange.TryGetValue(new Vector2Int(x, y), out string blockType))
                {
                    if (blockType == "o")
                    {
                        GameObject wall = Instantiate(wallEditorPrefab, new Vector3(x, y, 0), wallEditorPrefab.transform.rotation);

                        if (instantiatedEditorPrefabs.TryGetValue(new Vector2Int(x, y), out GameObject go))
                        {
                            Destroy(go);
                            instantiatedEditorPrefabs.Remove(new Vector2Int(x, y));
                        }

                        instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), wall);
                    }
                    else if (blockType == "Q")
                    {
                        GameObject iron = Instantiate(ironEditorPrefab, new Vector3(x, y, 0), ironEditorPrefab.transform.rotation);

                        if (instantiatedEditorPrefabs.TryGetValue(new Vector2Int(x, y), out GameObject go))
                        {
                            Destroy(go);
                            instantiatedEditorPrefabs.Remove(new Vector2Int(x, y));
                        }

                        instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), iron);
                    }
                    else if (blockType == "b")
                    {
                        GameObject bush = Instantiate(bushEditorPrefab, new Vector3(x, y, 0), bushEditorPrefab.transform.rotation);

                        if (instantiatedEditorPrefabs.TryGetValue(new Vector2Int(x, y), out GameObject go))
                        {
                            Destroy(go);
                            instantiatedEditorPrefabs.Remove(new Vector2Int(x, y));
                        }

                        instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), bush);
                    }
                    else if (blockType == "i")
                    {
                        GameObject ice = Instantiate(iceEditorPrefab, new Vector3(x, y, 0), iceEditorPrefab.transform.rotation);

                        if (instantiatedEditorPrefabs.TryGetValue(new Vector2Int(x, y), out GameObject go))
                        {
                            Destroy(go);
                            instantiatedEditorPrefabs.Remove(new Vector2Int(x, y));
                        }

                        instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), ice);
                    }
                    else if (blockType == "w")
                    {
                        GameObject water = Instantiate(waterEditorPrefab, new Vector3(x, y, 0), waterEditorPrefab.transform.rotation);

                        if (instantiatedEditorPrefabs.TryGetValue(new Vector2Int(x, y), out GameObject go))
                        {
                            Destroy(go);
                            instantiatedEditorPrefabs.Remove(new Vector2Int(x, y));
                        }

                        instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), water);
                    }

                    TryRemoveCoordsToChange(new Vector2Int(x, y));
                    TryRemoveCoordsToUpdate(new Vector2Int(x, y));
                    TryAddCoordsToUpdate(new Vector2Int(x, y), blockType);
                }
            }
        }
    }

    public void SaveMapText()
    {
        if (coordsToUpdate.Count == 0)
        {
            PopUpWindowUI.Instance.ShowPopUpWindow($"You must edit map first!");

            return;
        }

        string map = default;

        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var x = j - 13;
                var y = 13 - (i + 1);

                if (coordsToUpdate.TryGetValue(new Vector2Int(x, y), out string blockType))
                {
                    if (blockType == "o")
                    {
                        map += "o";
                    }
                    else if (blockType == "Q")
                    {
                        map += "Q";
                    }
                    else if (blockType == "b")
                    {
                        map += "b";
                    }
                    else if (blockType == "i")
                    {
                        map += "i";
                    }
                    else if (blockType == "w")
                    {
                        map += "w";
                    }
                }
                else if (filteredEagleBaseWallPosList.Contains(new Vector2Int(x, y)))
                {
                    map += "o";
                }
                else
                {
                    map += ".";
                }

                if (j + 1 == 26)
                {
                    map += "\n";
                }
            }
        }

        if (currentMapIndex == -1)
        {
            customMaps.Add(map);
        }
        else
        {
            customMaps[currentMapIndex] = map;
        }

        SaveMaps();

        MapEditorGameMenuUI.Instance.Hide();

        PopUpWindowUI.Instance.ShowPopUpWindow($"Map was saved successfully!");
    }

    public void NewMap()
    {
        currentMapIndex = -1;

        foreach (var pair in instantiatedEditorPrefabs)
        {
            Destroy(pair.Value);
        }

        coordsToChange = new Dictionary<Vector2Int, string>();
        coordsToUpdate = new Dictionary<Vector2Int, string>();
        coordsToRemove = new Dictionary<Vector2Int, string>();
        instantiatedEditorPrefabs = new Dictionary<Vector2Int, GameObject>();

        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var go = Instantiate(emptySpaceEditorPrefab, new Vector3(j - 13, 13 - (i + 1), 0), emptySpaceEditorPrefab.transform.rotation);
                var x = Mathf.RoundToInt(go.transform.position.x);
                var y = Mathf.RoundToInt(go.transform.position.y);

                instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), go);
            }
        }
    }

    public Sprite GetCustomMapSpriteByIndex(int mapIndex)
    {
        var textures = new List<Texture2D>();

        string[] map = customMaps[mapIndex].Split("\n");

        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                if (map[i][j] == 'o')
                {
                    var wallTexture = mapObjects[0].texture;

                    textures.Add(wallTexture);
                }
                else if (map[i][j] == 'Q')
                {
                    var wallTexture = mapObjects[1].texture;

                    textures.Add(wallTexture);
                }
                else if (map[i][j] == 'b')
                {
                    var wallTexture = mapObjects[2].texture;

                    textures.Add(wallTexture);
                }
                else if (map[i][j] == 'i')
                {
                    var wallTexture = mapObjects[3].texture;

                    textures.Add(wallTexture);
                }
                else if (map[i][j] == 'w')
                {
                    var wallTexture = mapObjects[4].texture;

                    textures.Add(wallTexture);
                }
                else if (map[i][j] == '.')
                {
                    var wallTexture = mapObjects[^1].texture;

                    textures.Add(wallTexture);
                }
            }
        }

        var texture = BattleCityUtils.MergeTextures(textures);
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));

        return sprite;
    }

    public void LoadMapFromString(string customMap, int currentMapIndex)
    {
        this.currentMapIndex = currentMapIndex;

        string[] map = customMap.Split("\n");

        foreach (var pair in instantiatedEditorPrefabs)
        {
            Destroy(pair.Value);
        }

        coordsToChange = new Dictionary<Vector2Int, string>();
        coordsToUpdate = new Dictionary<Vector2Int, string>();
        coordsToRemove = new Dictionary<Vector2Int, string>();
        instantiatedEditorPrefabs = new Dictionary<Vector2Int, GameObject>();

        for (int i = 0; i < 26; i++)
        {
            for (int j = 0; j < 26; j++)
            {
                var x = j - 13;
                var y = 13 - (i + 1);

                if (map[i][j] == 'o')
                {
                    coordsToUpdate.TryAdd(new Vector2Int(x, y), "o");

                    var wallgo = Instantiate(wallEditorPrefab, new Vector3(x, y, 0), wallEditorPrefab.transform.rotation);

                    instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), wallgo);
                }
                else if (map[i][j] == 'Q')
                {
                    coordsToUpdate.TryAdd(new Vector2Int(x, y), "Q");

                    var irongo = Instantiate(ironEditorPrefab, new Vector3(x, y, 0), ironEditorPrefab.transform.rotation);

                    instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), irongo);
                }
                else if (map[i][j] == 'b')
                {
                    coordsToUpdate.TryAdd(new Vector2Int(x, y), "b");

                    var bushgo = Instantiate(bushEditorPrefab, new Vector3(x, y, 0), bushEditorPrefab.transform.rotation);

                    instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), bushgo);
                }
                else if (map[i][j] == 'i')
                {
                    coordsToUpdate.TryAdd(new Vector2Int(x, y), "i");

                    var icego = Instantiate(iceEditorPrefab, new Vector3(x, y, 0), iceEditorPrefab.transform.rotation);

                    instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), icego);
                }
                else if (map[i][j] == 'w')
                {
                    coordsToUpdate.TryAdd(new Vector2Int(x, y), "w");

                    var watergo = Instantiate(waterEditorPrefab, new Vector3(x, y, 0), waterEditorPrefab.transform.rotation);

                    instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), watergo);
                }
                else if (map[i][j] == '.')
                {
                    var emptySpacego = Instantiate(emptySpaceEditorPrefab, new Vector3(x, y, 0), emptySpaceEditorPrefab.transform.rotation);

                    instantiatedEditorPrefabs.TryAdd(new Vector2Int(x, y), emptySpacego);
                }
            }
        }
    }

    public void SaveMaps()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedMaps.gd");

        bf.Serialize(file, customMaps);
        file.Close();
    }

    public void LoadMaps()
    {
        if (File.Exists(Application.persistentDataPath + "/savedMaps.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedMaps.gd", FileMode.Open);

            customMaps = (List<string>)bf.Deserialize(file);

            file.Close();
        }
    }

    public bool TryAddCoordsToUpdate(Vector2Int coords, string blockType)
    {
        if (coordsToUpdate.ContainsKey(coords))
        {
            return false;
        }

        var isAdded = coordsToUpdate.TryAdd(coords, blockType);

        if (isAdded)
        {
            DrawCoordsToMap();
        }

        return isAdded;
    }

    public bool TryRemoveCoordsToUpdate(Vector2Int coords)
    {
        return coordsToUpdate.Remove(coords);
    }

    public bool TryAddCoordsToChange(Vector2Int coords, string blockType)
    {
        if (coordsToChange.ContainsKey(coords))
        {
            return false;
        }

        var isAdded = coordsToChange.TryAdd(coords, blockType);

        if (isAdded)
        {
            DrawCoordsToMap();
        }

        return isAdded;
    }

    public bool TryRemoveCoordsToChange(Vector2Int coords)
    {
        return coordsToChange.Remove(coords);
    }

    public bool TryAddCoordsToRemove(Vector2Int coords, string blockType)
    {
        if (coordsToRemove.ContainsKey(coords))
        {
            return false;
        }

        var isAdded = coordsToRemove.TryAdd(coords, blockType);

        if (isAdded)
        {
            DrawCoordsToMap();
        }

        return isAdded;
    }

    public bool TryRemoveCoordsToRemove(Vector2Int coords)
    {
        return coordsToRemove.Remove(coords);
    }

    public bool HasBrush()
    {
        return editorBrush != null;
    }

    public void SetEditorBrush(GameObject editorBrush)
    {
        this.editorBrush = editorBrush;
    }

    public bool IsErazing()
    {
        return isErazing;
    }

    public void SetIsErazing(bool isErazing)
    {
        this.isErazing = isErazing;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void SetIsPaused(bool isPaused)
    {
        this.isPaused = isPaused;
    }

    public GameObject GetEditorBrush()
    {
        return editorBrush;
    }

    public GameObject GetWallEditorPrefab()
    {
        return wallEditorPrefab;
    }

    public GameObject GetIronEditorPrefab()
    {
        return ironEditorPrefab;
    }

    public GameObject GetBushEditorPrefab()
    {
        return bushEditorPrefab;
    }

    public GameObject GetIceEditorPrefab()
    {
        return iceEditorPrefab;
    }

    public GameObject GetWaterEditorPrefab()
    {
        return waterEditorPrefab;
    }

    public List<Vector2Int> GetFilteredEnemiesSpawnPosList()
    {
        return filteredEnemiesSpawnPosList;
    }

    public List<Vector2Int> GetFilteredPlayersSpawnPosList()
    {
        return filteredPlayersSpawnPosList;
    }

    public List<Vector2Int> GetFilteredEagleBasePosList()
    {
        return filteredEagleBasePosList;
    }

    public List<GameObject> GetAllTools()
    {
        return allTools;
    }

    public List<string> GetCustomMaps()
    {
        return customMaps;
    }
}
