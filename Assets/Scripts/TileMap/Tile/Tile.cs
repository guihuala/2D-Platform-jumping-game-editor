using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum TileType { Ground, Platform, Decoration, Water, }

    public TileSO tileSO;

    public TileType Type;

    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
