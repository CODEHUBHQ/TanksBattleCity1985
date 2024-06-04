using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCityMapObject : MonoBehaviour
{
    [SerializeField] private MapObjectType mapObjectType;

    public MapObjectType GetMapObjectType()
    {
        return mapObjectType;
    }
}
