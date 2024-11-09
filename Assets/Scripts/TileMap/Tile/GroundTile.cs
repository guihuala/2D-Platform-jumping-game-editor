using UnityEngine;

public class GroundTile : Tile
{
    [Header("Default Sprites")] // 默认，即上下为空
    public Sprite defaultSprite;
    public Sprite defaultCenterSprite;
    public Sprite defaultRightSprite;
    public Sprite defaultLeftSprite;

    [Header("Top Sprites")] // 顶
    public Sprite topSprite;
    public Sprite topCenterSprite;
    public Sprite topLeftSprite;
    public Sprite topRightSprite;

    [Header("Bottom Sprites")] // 底
    public Sprite bottomSprite;
    public Sprite bottomCenterSprite;
    public Sprite bottomLeftSprite;
    public Sprite bottomRightSprite;

    [Header("Center Sprites")] // 中间
    public Sprite centerSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    public Sprite barCenterSprite;

    protected override void Awake()
    {
        base.Awake();
        Type = TileType.Ground;
    }

    public void DeterminePosition(int x, int y, MyTileMap tileMap)
    {
        // 用位标志来表示四个方向
        int flags = 0;
        flags |= (tileMap.HasTileOfType(x, y + 1, TileType.Ground) ? 1 : 0) << 0; // 顶
        flags |= (tileMap.HasTileOfType(x, y - 1, TileType.Ground) ? 1 : 0) << 1; // 底
        flags |= (tileMap.HasTileOfType(x - 1, y, TileType.Ground) ? 1 : 0) << 2; // 左
        flags |= (tileMap.HasTileOfType(x + 1, y, TileType.Ground) ? 1 : 0) << 3; // 右

        switch (flags)
        {
            case 0b0000: // 所有方向都为空，使用默认图像
                spriteRenderer.sprite = defaultSprite;
                break;

            case 0b0001: // 仅底部有方块
                spriteRenderer.sprite = bottomSprite;
                break;

            case 0b0010: // 仅顶部有方块
                spriteRenderer.sprite = topSprite;
                break;

            case 0b0011: // 顶部和底部
                spriteRenderer.sprite = barCenterSprite; // 中间底部
                break;

            case 0b0100: // 仅右侧有方块
                spriteRenderer.sprite = defaultRightSprite;
                break;

            case 0b0101: // 右侧和底部
                spriteRenderer.sprite = bottomRightSprite;
                break;

            case 0b0110: // 右侧和顶部
                spriteRenderer.sprite = topRightSprite;
                break;

            case 0b0111: // 顶部、底部和右侧
                spriteRenderer.sprite = rightSprite;
                break;

            case 0b1000: // 仅左侧有方块
                spriteRenderer.sprite = defaultLeftSprite;
                break;

            case 0b1001: // 左侧和底部
                spriteRenderer.sprite = bottomLeftSprite;
                break;

            case 0b1010: // 左侧和顶部
                spriteRenderer.sprite = topLeftSprite;
                break;

            case 0b1011: // 顶部、底部和左侧
                spriteRenderer.sprite = leftSprite;
                break;

            case 0b1100: // 左右
                spriteRenderer.sprite = defaultCenterSprite;
                break;

            case 0b1101: // 左侧、底部和右侧
                spriteRenderer.sprite = bottomCenterSprite;
                break;

            case 0b1110: // 顶部、左侧和右侧
                spriteRenderer.sprite = topCenterSprite;
                break;

            case 0b1111: // 所有方向都有方块
                spriteRenderer.sprite = centerSprite; // 中间
                break;

            default:
                spriteRenderer.sprite = centerSprite; // 默认处理
                break;
        }
    }

    public void SetTileSprite()
    {
        spriteRenderer.sprite = topSprite;
    }
}
