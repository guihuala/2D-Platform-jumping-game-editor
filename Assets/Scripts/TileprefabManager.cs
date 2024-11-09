using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileprefabManager : MonoBehaviour
{
    public static TileprefabManager Instance;

    public GameObject[] tilePrefabs;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tilePrefabs = Resources.LoadAll<GameObject>("Tile");
    }

    public GameObject FindPrefab(string name)
    {
        foreach (GameObject prefab in tilePrefabs)
        {
            Tile tileScript = prefab.GetComponent<Tile>();
            if (tileScript != null && tileScript.name == name)
            {
                return prefab;
            }
        }
        return null;
    }
}
