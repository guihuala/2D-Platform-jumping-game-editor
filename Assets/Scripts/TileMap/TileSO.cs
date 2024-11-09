using UnityEngine;

[CreateAssetMenu(fileName = "NewTile", menuName = "SO/Tile")]
public class TileSO : ScriptableObject
{
    public GameObject tilePrefab; // 该 Tile 对应的预制件
    public string tileName; // Tile 名称
    public Sprite tileSprite; // Tile 的精灵
    public TileType tileType; // Tile 的类型
}

public enum TileType
{
    Ground,
    Decoration,
    Water,
    Platform
}

