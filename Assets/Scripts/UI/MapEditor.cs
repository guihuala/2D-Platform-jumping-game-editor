using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MapEditorUI : MonoBehaviour
{
    [Header("Map")]
    public MyTileMap tileMap;

    #region 获取组件
    [Header("UI")]
    public TMP_InputField importText;
    public TextMeshProUGUI positionText;
    public Button importButton;
    public Button exportButton;
    public Button undoButton;
    public Button redoButton;
    public Button playButton;
    public Button stopButton;
    public Button clearButton;
    public Button rectangleToolButton;
    public Button singleTileToolButton;
    public GameObject tileSelectionPanel;
    #endregion

    #region 拖拽
    private Vector3Int startDragPosition;
    private Vector3Int endDragPosition;
    private bool isDragging = false;
    #endregion

    [Header("indicator")]
    [SerializeField] private GameObject indicator; // 用于放置的指示物体

    [Header("Prefab")]
    [SerializeField] private GameObject SelectedTilePrefab;
    private SpriteRenderer indicatorSpriteRenderer;

    // 定义工具类型
    private enum ToolType { SingleTile, Rectangle }
    private ToolType currentTool = ToolType.SingleTile; // 默认使用单块绘制工具

    void Start()
    {
        // 订阅游戏模式切换事件
        GameManager.OnGameStateChanged += UpdateUIVisibility;

        // 初始化指示物体
        indicator = Instantiate(SelectedTilePrefab);
        indicatorSpriteRenderer = indicator.GetComponent<SpriteRenderer>();
        indicator.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f); // 设置为半透明
        indicator.SetActive(false); // 默认为不可见

        // 绑定按钮事件
        importButton.onClick.AddListener(() => OnImportButtonClicked());
        exportButton.onClick.AddListener(() => OnExportButtonClicked());
        undoButton.onClick.AddListener(() => tileMap.Undo());
        redoButton.onClick.AddListener(() => tileMap.Redo());
        playButton.onClick.AddListener(() => GameManager.Instance.StartPlayMode());
        stopButton.onClick.AddListener(() => GameManager.Instance.EndPlayMode());
        clearButton.onClick.AddListener(() => tileMap.ClearMap());

        // 绑定工具选择按钮
        rectangleToolButton.onClick.AddListener(() => SelectTool(ToolType.Rectangle));
        singleTileToolButton.onClick.AddListener(() => SelectTool(ToolType.SingleTile));
    }

    void Update()
    {
        // 根据选择的工具来决定绘制逻辑
        if (currentTool == ToolType.SingleTile)
        {
            OnTileDrew(); // 单块绘制
        }
        else if (currentTool == ToolType.Rectangle)
        {
            OnDrawRectangleClicked(); // 矩形绘制
        }

        UpdateIndicatorPosition(); // 更新指示物体位置
    }

    // 工具选择逻辑
    private void SelectTool(ToolType tool)
    {
        currentTool = tool;
    }

    // 更新编辑模式UI的可见性
    private void UpdateUIVisibility(GameManager.GameState gameState)
    {
        bool isEditMode = gameState == GameManager.GameState.EditMode;

        // 控制按钮和方块选择框的可见性
        undoButton.gameObject.SetActive(isEditMode);
        redoButton.gameObject.SetActive(isEditMode);
        playButton.gameObject.SetActive(isEditMode);
        stopButton.gameObject.SetActive(!isEditMode);
        tileSelectionPanel.SetActive(isEditMode);

        // 控制指示物体的显隐
        indicator.SetActive(isEditMode);
    }

    public void SetCurrentTile(GameObject tilePrefab, TileSO tileSO)
    {
        SelectedTilePrefab = tilePrefab; // 设置当前选中的TileSO
        UpdateIndicator(tileSO.tileSprite);
    }

    public void UpdateIndicator(Sprite sprite)
    {
        indicatorSpriteRenderer.sprite = sprite;
    }

    private void OnTileDrew()
    {
        if (GameManager.Instance.IsInPlayMode()) return; // 检查是否在游戏模式

        // 检查鼠标是否在 UI 元素上
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3Int mousePosition = GetMouseGridPosition();
        positionText.SetText($"X: {mousePosition.x}, Y: {mousePosition.y}");

        if (Input.GetMouseButtonDown(0))
        {
            tileMap.SetTile(mousePosition.x, mousePosition.y, SelectedTilePrefab);
        }
    }

    private void UpdateIndicatorPosition()
    {
        if (GameManager.Instance.IsInPlayMode())
        {
            indicator.SetActive(false);
            return;
        }

        // 检查鼠标是否在 UI 元素上
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3Int mousePosition = GetMouseGridPosition();

        // 更新指示物体的位置
        Vector3 indicatorPosition = new Vector3(mousePosition.x * tileMap.tileSize, mousePosition.y * tileMap.tileSize, 0);
        indicator.transform.position = indicatorPosition;
        indicator.SetActive(true); // 显示指示物体
    }

    private void OnDrawRectangleClicked()
    {
        if (GameManager.Instance.IsInPlayMode()) return; // 检查是否在游戏模式

        // 检查鼠标是否在 UI 元素上
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector3Int mousePosition = GetMouseGridPosition();
        positionText.SetText($"X: {mousePosition.x}, Y: {mousePosition.y}");

        // 开始拖拽
        if (Input.GetMouseButtonDown(0))
        {
            startDragPosition = mousePosition;
            isDragging = true;
        }

        // 更新拖拽位置
        if (isDragging)
        {
            endDragPosition = mousePosition;
        }

        // 结束拖拽
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            // 记录拖拽开始与结束时的坐标
            int startX = Mathf.Min(startDragPosition.x, endDragPosition.x);
            int startY = Mathf.Min(startDragPosition.y, endDragPosition.y);
            int rectWidth = Mathf.Abs(endDragPosition.x - startDragPosition.x) + 1;
            int rectHeight = Mathf.Abs(endDragPosition.y - startDragPosition.y) + 1;

            // 绘制矩形
            tileMap.DrawRectangle(startX, startY, rectWidth, rectHeight, Tile.TileType.Ground, SelectedTilePrefab);
        }
    }

    private void OnImportButtonClicked()
    {
        // 从输入框中读取地图数据并导入
        string importInfo = importText.text;
        if (!string.IsNullOrEmpty(importInfo))
        {
            MapData.instance.LoadMap(importInfo);
        }
    }

    private void OnExportButtonClicked()
    {
        // 获取地图数据并复制到剪贴板
        string exportData = MapData.instance.SaveMap();
        GUIUtility.systemCopyBuffer = exportData;
    }

    private Vector3Int GetMouseGridPosition()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int mouseX = Mathf.RoundToInt(mouseWorldPosition.x / tileMap.tileSize);
        int mouseY = Mathf.RoundToInt(mouseWorldPosition.y / tileMap.tileSize);
        return new Vector3Int(mouseX, mouseY, 0);
    }
}
