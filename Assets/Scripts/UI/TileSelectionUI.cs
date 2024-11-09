using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileSelectionUI : MonoBehaviour
{
    public MapEditorUI editorUI;
    [SerializeField] private Button tileButtonPrefab; // Tile按钮预制体
    [SerializeField] private Button tabButtonPrefab;  // 页签按钮预制体
    [SerializeField] private Transform tabContainer;  // 页签按钮的父容器
    [SerializeField] private Transform ScrollContent; // 滚动内容父容器
    [SerializeField] private TileSOGroup[] tileSOGroups; // TileSO 分组

    private void Start()
    {
        foreach (var group in tileSOGroups)
        {
            Button tabButton = Instantiate(tabButtonPrefab, tabContainer);
            tabButton.GetComponentInChildren<TextMeshProUGUI>().text = group.groupName;
            tabButton.onClick.AddListener(() => ShowTileGroup(group.tileSOs));
        }

        // 默认展示第一个分组
        if (tileSOGroups.Length > 0)
        {
            ShowTileGroup(tileSOGroups[0].tileSOs);
        }
    }

    private void ShowTileGroup(TileSO[] tileSOs)
    {
        // 清空当前 ScrollContent 的子对象
        foreach (Transform child in ScrollContent)
        {
            Destroy(child.gameObject);
        }

        // 为新组的 TileSOs 创建按钮
        foreach (var tile in tileSOs)
        {
            Button button = Instantiate(tileButtonPrefab, ScrollContent);
            button.GetComponent<Image>().sprite = tile.tileSprite;
            button.onClick.AddListener(() => editorUI.SetCurrentTile(tile.tilePrefab, tile));
        }
    }
}


[System.Serializable]
public class TileSOGroup
{
    public string groupName; // 页签名称
    public TileSO[] tileSOs; // 该组内的 TileSO
}
