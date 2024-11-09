using UnityEngine;
using System.Text;
using System.Linq;

public class MapData : MonoBehaviour
{
    public static MapData instance; // 静态实例，用于访问单例

    private MyTileMap tileMap;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        tileMap = GameObject.Find("TileMap").GetComponent<MyTileMap>();
    }

    // 将地图数据导出为加密字符串格式
    public string SaveMap()
    {
        StringBuilder sb = new StringBuilder();
        for (int x = 0; x < tileMap.width; x++)
        {
            for (int y = 0; y < tileMap.height; y++)
            {
                if (tileMap.tiles[x, y] != null)
                {
                    Tile tile = tileMap.tiles[x, y].GetComponent<Tile>();
                    string prefabName = tile.tileSO.tileName;

                    int originalX = x - tileMap.offset;
                    int originalY = y - tileMap.offset;
                    sb.Append($"{originalX},{originalY},{prefabName};");
                }
            }
        }

        // 将拼接的字符串转为 Base64 编码
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(sb.ToString());
        return System.Convert.ToBase64String(bytesToEncode);
    }

    // 从加密字符串加载地图数据
    public void LoadMap(string encryptedMapData)
    {
        tileMap.ClearMap();

        // 将 Base64 字符串解码回原始字符串
        byte[] decodedBytes = System.Convert.FromBase64String(encryptedMapData);
        string mapData = Encoding.UTF8.GetString(decodedBytes);

        string[] tileDataArray = mapData.Split(';');
        foreach (string tileData in tileDataArray)
        {
            if (string.IsNullOrEmpty(tileData)) continue;
            string[] data = tileData.Split(',');

            int originalX = int.Parse(data[0]);
            int originalY = int.Parse(data[1]);
            int x = originalX + tileMap.offset;
            int y = originalY + tileMap.offset;
            string prefabName = data[2];

            GameObject prefab = TileprefabManager.Instance.FindPrefab(prefabName);

            if (prefab != null)
            {
                tileMap.SetTile(originalX, originalY, prefab);
            }
        }
    }
}
