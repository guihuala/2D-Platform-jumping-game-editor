using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject playerPrefab;
    public Transform spawnPoint;
    public MyTileMap tileMap;
    public CameraController cameraController;

    private GameObject playerInstance;

    // 游戏模式状态
    public enum GameState
    {
        EditMode,
        PlayMode
    }

    public GameState CurrentState { get; private set; } = GameState.EditMode; // 默认为编辑模式

    // 定义模式切换事件
    public static event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void StartPlayMode()
    {
        if (CurrentState == GameState.EditMode)
        {
            CurrentState = GameState.PlayMode; // 切换为游戏模式

            if (playerInstance == null)
            {
                playerInstance = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
                cameraController.SetTarget(playerInstance.transform); // 设置相机跟随玩家
            }

            // 触发模式切换事件
            OnGameStateChanged?.Invoke(CurrentState);
        }
    }

    public void EndPlayMode()
    {
        if (CurrentState == GameState.PlayMode)
        {
            CurrentState = GameState.EditMode; // 切换回编辑模式
            if (playerInstance != null)
            {
                Destroy(playerInstance);
            }

            // 恢复相机设置
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.ResetCameraToDefault();

            // 触发模式切换事件
            OnGameStateChanged?.Invoke(CurrentState);
        }
    }


    // 检查游戏状态
    public bool IsInEditMode()
    {
        return CurrentState == GameState.EditMode;
    }

    public bool IsInPlayMode()
    {
        return CurrentState == GameState.PlayMode;
    }
}
