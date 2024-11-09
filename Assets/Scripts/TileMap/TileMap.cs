using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MyTileMap : MonoBehaviour
{
    public int width = 100;
    public int height = 50;
    public float tileSize = 1f;

    public GameObject[,] tiles;

    private Stack<TileAction> undoStack = new Stack<TileAction>();
    private Stack<TileAction> redoStack = new Stack<TileAction>();

    public int offset;

    public Transform tileRoot; // 用于存储所有方块的根节点
    public Transform decoRoot; // 用于存储所有装饰方块的根节点

    void Start()
    {
        tiles = new GameObject[width, height];
        offset = Mathf.FloorToInt(width / 2 * tileSize);
    }

    public void SetTile(int x, int y, GameObject tilePrefab)
    {
        if (x >= -offset && x < offset && y >= -offset && y < offset)
        {
            int arrayX = x + offset;
            int arrayY = y + offset;

            GameObject newTileObject = Instantiate(tilePrefab, new Vector3(x * tileSize, y * tileSize, 0), Quaternion.identity, tileRoot);


            if (tiles[arrayX, arrayY] == null || tiles[arrayX, arrayY].name != tilePrefab.name)
            {
                string previousTileType = tiles[arrayX, arrayY] != null ? tiles[arrayX, arrayY].GetComponent<Tile>().tileSO.tileName : null; // 记录之前的方块类型

                TileAction action = new TileAction(arrayX, arrayY, previousTileType, tilePrefab.GetComponent<Tile>().tileSO.tileName);
                undoStack.Push(action);

                if (undoStack.Count > 10)
                {
                    undoStack = new Stack<TileAction>(undoStack.Reverse().Take(10).Reverse());
                }

                redoStack.Clear();
            }

            if (tiles[arrayX, arrayY] != null)
            {
                Destroy(tiles[arrayX, arrayY]); // 销毁旧的方块实例
            }

            tiles[arrayX, arrayY] = newTileObject;

            UpdateSurroundingGroundTiles(x, y);
        }
    }



    public void DrawRectangle(int startX, int startY, int rectWidth, int rectHeight, Tile.TileType tileType, GameObject tilePrefab)
    {
        int startArrayX = startX + offset;
        int startArrayY = startY + offset;
        int endArrayX = startArrayX + rectWidth;
        int endArrayY = startArrayY + rectHeight;

        if (startArrayX >= 0 && startArrayX < width &&
            startArrayY >= 0 && startArrayY < height &&
            endArrayX >= 0 && endArrayX < width &&
            endArrayY >= 0 && endArrayY < height)
        {
            for (int x = startArrayX; x < endArrayX; x++)
            {
                for (int y = startArrayY; y < endArrayY; y++)
                {
                    SetTile(x - offset, y - offset, tilePrefab);
                }
            }
        }
    }

    private void UpdateTileInstanceList(int arrayX, int arrayY, GameObject newTileObject)
    {
        var currentTileList = new List<GameObject>();

        // 获取当前格子的所有活跃方块
        foreach (Transform child in tileRoot)
        {
            if (child.gameObject.activeSelf &&
                Mathf.RoundToInt(child.position.x / tileSize) == arrayX - offset &&
                Mathf.RoundToInt(child.position.y / tileSize) == arrayY - offset)
            {
                currentTileList.Add(child.gameObject);
            }
        }

        // 确保只保留最多两个实例
        if (currentTileList.Count > 2)
        {
            var extraTiles = currentTileList.GetRange(0, currentTileList.Count - 2); // 获取多余的方块实例
            foreach (var tile in extraTiles)
            {
                Destroy(tile); // 删除多余的方块实例
            }
        }
    }

    private void UpdateSurroundingGroundTiles(int worldX, int worldY)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if ((dx == 0 && dy != 0) || (dy == 0 && dx != 0) || (dx == 0 && dy == 0))
                {
                    int neighborWorldX = worldX + dx;
                    int neighborWorldY = worldY + dy;

                    int neighborArrayX = neighborWorldX + offset;
                    int neighborArrayY = neighborWorldY + offset;

                    if (neighborArrayX >= 0 && neighborArrayX < width && neighborArrayY >= 0 && neighborArrayY < height)
                    {
                        if (tiles[neighborArrayX, neighborArrayY] != null && tiles[neighborArrayX, neighborArrayY].TryGetComponent(out GroundTile groundTile))
                        {
                            groundTile.DeterminePosition(neighborWorldX, neighborWorldY, this);
                        }
                    }
                }
            }
        }
    }

    public bool HasTileOfType(int x, int y, Tile.TileType type)
    {
        int arrayX = x + offset;
        int arrayY = y + offset;

        if (x >= -offset && x < offset && y >= -offset && y < offset)
        {
            return tiles[arrayX, arrayY] != null && tiles[arrayX, arrayY].GetComponent<Tile>().Type == type;
        }
        return false;
    }

    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            TileAction action = undoStack.Pop();
            redoStack.Push(action);

            int worldX = action.x - offset;
            int worldY = action.y - offset;

            // 删除当前方块
            if (tiles[action.x, action.y] != null)
            {
                Destroy(tiles[action.x, action.y]);
                tiles[action.x, action.y] = null;
            }

            // 还原上一个方块
            if (!string.IsNullOrEmpty(action.previousTileType))
            {
                Debug.Log("Undo: " + action.previousTileType);
                GameObject previousTilePrefab = TileprefabManager.Instance.FindPrefab(action.previousTileType); // 加载预制体
                if (previousTilePrefab != null)
                {
                    GameObject previousTile = Instantiate(previousTilePrefab, new Vector3(worldX * tileSize, worldY * tileSize, 0), Quaternion.identity, tileRoot);
                    tiles[action.x, action.y] = previousTile;
                }
            }
            else
            {
                tiles[action.x, action.y] = null;
            }

            UpdateSurroundingGroundTiles(worldX, worldY);
        }
    }

    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            TileAction action = redoStack.Pop();
            undoStack.Push(action);

            int worldX = action.x - offset;
            int worldY = action.y - offset;

            if (tiles[action.x, action.y] != null)
            {
                Destroy(tiles[action.x, action.y]);
                tiles[action.x, action.y] = null;
            }

            if (!string.IsNullOrEmpty(action.newTileType))
            {
                GameObject newTilePrefab = TileprefabManager.Instance.FindPrefab(action.newTileType); // 加载预制体
                if (newTilePrefab != null)
                {
                    GameObject newTile = Instantiate(newTilePrefab, new Vector3(worldX * tileSize, worldY * tileSize, 0), Quaternion.identity, tileRoot);
                    tiles[action.x, action.y] = newTile;
                }
            }
            else
            {
                tiles[action.x, action.y] = null;
            }

            UpdateSurroundingGroundTiles(worldX, worldY);
        }
    }


    public bool HasTileAt(int x, int y)
    {
        // 某坐标上是否已经存在方块
        if (x >= -offset && x < offset && y >= -offset && y < offset)
        {
            int arrayX = x + offset;
            int arrayY = y + offset;
            return tiles[arrayX, arrayY] != null;
        }
        return false;
    }

    public void ClearMap()
    {
        // 清空二维数组
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = null;
            }
        }

        redoStack.Clear();
        undoStack.Clear();
        undoStack.TrimExcess();

        foreach (Transform child in tileRoot)
        {
            Destroy(child.gameObject);
        }
    }
}

public class TileAction
{
    public int x, y;
    public string previousTileType; // 上一个方块的类型
    public string newTileType;      // 新方块的类型

    public TileAction(int x, int y, string previousTileType, string newTileType)
    {
        this.x = x;
        this.y = y;
        this.previousTileType = previousTileType;
        this.newTileType = newTileType;
    }
}
