using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 关卡系统使用示例
/// 展示如何在Dihao场景中使用房间块拼接和元素填充系统
/// </summary>
public class LevelSystemExample : MonoBehaviour
{
    [Header("系统组件")]
    [SerializeField] private RoomManager roomManager;
    [SerializeField] private LevelConfig levelConfig;
    
    [Header("示例设置")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool showExampleUsage = true;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupLevelSystem();
        }
        
        if (showExampleUsage)
        {
            ShowUsageExamples();
        }
    }
    
    /// <summary>
    /// 设置关卡系统
    /// </summary>
    private void SetupLevelSystem()
    {
        // 1. 创建或获取RoomManager
        if (roomManager == null)
        {
            roomManager = FindFirstObjectByType<RoomManager>();
            if (roomManager == null)
            {
                GameObject managerObj = new GameObject("RoomManager");
                roomManager = managerObj.AddComponent<RoomManager>();
            }
        }
        
        // 2. 应用配置
        if (levelConfig != null)
        {
            levelConfig.ApplyToRoomGenerator(roomManager.GetComponent<RoomGenerator>());
            levelConfig.ApplyToElementFiller(roomManager.GetComponent<ElementFiller>());
        }
        
        // 3. 订阅事件
        roomManager.OnGenerationComplete += OnLevelGenerated;
        roomManager.OnRoomsGenerated += OnRoomsGenerated;
        roomManager.OnElementsGenerated += OnElementsGenerated;
        
        Debug.Log("关卡系统设置完成！");
    }
    
    /// <summary>
    /// 显示使用示例
    /// </summary>
    private void ShowUsageExamples()
    {
        Debug.Log("=== 关卡系统使用示例 ===");
        Debug.Log("1. 创建房间块Prefab：");
        Debug.Log("   - 使用RoomBlockCreator组件");
        Debug.Log("   - 设置房间类型、尺寸、门洞");
        Debug.Log("   - 点击'创建房间块Prefab'");
        
        Debug.Log("2. 配置元素Prefab：");
        Debug.Log("   - 创建炸弹、障碍物、传送器等Prefab");
        Debug.Log("   - 在ElementFiller中配置元素规则");
        
        Debug.Log("3. 生成关卡：");
        Debug.Log("   - 调用roomManager.GenerateCompleteLevel()");
        Debug.Log("   - 或使用Context Menu '重新生成关卡'");
        
        Debug.Log("4. 自定义配置：");
        Debug.Log("   - 创建LevelConfig ScriptableObject");
        Debug.Log("   - 调整房间数量、元素规则等");
    }
    
    /// <summary>
    /// 关卡生成完成回调
    /// </summary>
    private void OnLevelGenerated()
    {
        Debug.Log("关卡生成完成！");
        
        // 获取统计信息
        var stats = roomManager.GetRoomStatistics();
        Debug.Log($"房间数：{stats.totalRooms}，元素数：{stats.totalElements}");
        
        // 显示房间类型统计
        foreach (var kvp in stats.roomTypes)
        {
            Debug.Log($"- {kvp.Key}: {kvp.Value} 个");
        }
        
        // 显示元素类型统计
        foreach (var kvp in stats.elementTypes)
        {
            Debug.Log($"- {kvp.Key}: {kvp.Value} 个");
        }
    }
    
    /// <summary>
    /// 房间生成完成回调
    /// </summary>
    private void OnRoomsGenerated(List<RoomBlock> rooms)
    {
        Debug.Log($"房间生成完成，共 {rooms.Count} 个房间");
        
        // 可以在这里添加房间特定的逻辑
        foreach (var room in rooms)
        {
            Debug.Log($"房间：{room.RoomName} ({room.roomData?.roomType})");
        }
    }
    
    /// <summary>
    /// 元素生成完成回调
    /// </summary>
    private void OnElementsGenerated(List<PlacedElement> elements)
    {
        Debug.Log($"元素填充完成，共 {elements.Count} 个元素");
        
        // 可以在这里添加元素特定的逻辑
        foreach (var element in elements)
        {
            Debug.Log($"元素：{element.elementType} 在位置 {element.position}");
        }
    }
    
    /// <summary>
    /// 手动生成关卡
    /// </summary>
    [ContextMenu("生成示例关卡")]
    public void GenerateExampleLevel()
    {
        if (roomManager == null)
        {
            Debug.LogError("RoomManager未找到！");
            return;
        }
        
        roomManager.GenerateCompleteLevel();
    }
    
    /// <summary>
    /// 创建示例房间块
    /// </summary>
    [ContextMenu("创建示例房间块")]
    public void CreateExampleRoomBlocks()
    {
        // 创建U形房间
        CreateUShapeRoom();
        
        // 创建十字房间
        CreateCrossRoom();
        
        // 创建L形房间
        CreateLShapeRoom();
        
        Debug.Log("示例房间块创建完成！");
    }
    
    private void CreateUShapeRoom()
    {
        GameObject uRoom = new GameObject("U_Shape_Room");
        RoomBlock roomBlock = uRoom.AddComponent<RoomBlock>();
        
        roomBlock.roomData = new RoomBlockData
        {
            roomName = "U_Shape_Room",
            size = new Vector2Int(8, 6),
            roomType = RoomType.U_Shape,
            spawnWeight = 1f,
            isStartingRoom = true
        };
        
        // 自动设置门洞接口
        roomBlock.AutoSetupSockets();
        
        Debug.Log("U形房间创建完成");
    }
    
    private void CreateCrossRoom()
    {
        GameObject crossRoom = new GameObject("Cross_Room");
        RoomBlock roomBlock = crossRoom.AddComponent<RoomBlock>();
        
        roomBlock.roomData = new RoomBlockData
        {
            roomName = "Cross_Room",
            size = new Vector2Int(6, 6),
            roomType = RoomType.Cross,
            spawnWeight = 1f
        };
        
        // 自动设置门洞接口
        roomBlock.AutoSetupSockets();
        
        Debug.Log("十字房间创建完成");
    }
    
    private void CreateLShapeRoom()
    {
        GameObject lRoom = new GameObject("L_Shape_Room");
        RoomBlock roomBlock = lRoom.AddComponent<RoomBlock>();
        
        roomBlock.roomData = new RoomBlockData
        {
            roomName = "L_Shape_Room",
            size = new Vector2Int(6, 8),
            roomType = RoomType.Corner_L,
            spawnWeight = 1f
        };
        
        // 自动设置门洞接口
        roomBlock.AutoSetupSockets();
        
        Debug.Log("L形房间创建完成");
    }
    
    // GetCrossRoomDoorPosition方法已移除，使用AutoSetupSockets替代
    
    /// <summary>
    /// 创建示例元素配置
    /// </summary>
    [ContextMenu("创建示例元素配置")]
    public void CreateExampleElementConfigs()
    {
        Debug.Log("=== 示例元素配置 ===");
        Debug.Log("1. 炸弹：生成概率50%，数量1-3个，避免中心区域");
        Debug.Log("2. 可破坏物：生成概率70%，数量2-5个，避免边缘");
        Debug.Log("3. 传送器：生成概率30%，数量1-2个，需要远离其他传送器");
        Debug.Log("4. 灵视物品：生成概率40%，数量1-2个，需要靠近墙壁");
        Debug.Log("5. 敌人生成点：生成概率60%，数量1-3个，避免重叠");
    }
}
