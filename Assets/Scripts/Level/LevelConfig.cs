using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 关卡配置，存储所有关卡生成相关的设置
/// </summary>
[CreateAssetMenu(fileName = "LevelConfig", menuName = "Level/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("房间生成设置")]
    [SerializeField] private int minRooms = 2;
    [SerializeField] private int maxRooms = 3;
    [SerializeField] private float roomSpacing = 1f;
    [SerializeField] private int maxRetryAttempts = 10;
    [SerializeField] private float connectionMargin = 0.5f;
    
    [Header("房间块设置")]
    [SerializeField] private List<RoomBlock> availableRoomBlocks = new List<RoomBlock>();
    [SerializeField] private bool useWeightedSelection = true;
    
    [Header("元素填充设置")]
    [SerializeField] private List<ElementConfig> elementConfigs = new List<ElementConfig>();
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private int elementMaxRetryAttempts = 10;
    
    [Header("特殊房间设置")]
    [SerializeField] private List<RoomBlock> startingRooms = new List<RoomBlock>();
    [SerializeField] private List<RoomBlock> endingRooms = new List<RoomBlock>();
    [SerializeField] private bool forceStartingRoom = true;
    [SerializeField] private bool forceEndingRoom = false;
    
    [Header("调试设置")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showRoomBounds = true;
    [SerializeField] private bool showElementGizmos = true;
    [SerializeField] private bool generateOnStart = false;
    
    // 公共属性
    public int MinRooms => minRooms;
    public int MaxRooms => maxRooms;
    public float RoomSpacing => roomSpacing;
    public int MaxRetryAttempts => maxRetryAttempts;
    public float ConnectionMargin => connectionMargin;
    public List<RoomBlock> AvailableRoomBlocks => availableRoomBlocks;
    public bool UseWeightedSelection => useWeightedSelection;
    public List<ElementConfig> ElementConfigs => elementConfigs;
    public float GridSize => gridSize;
    public int ElementMaxRetryAttempts => elementMaxRetryAttempts;
    public List<RoomBlock> StartingRooms => startingRooms;
    public List<RoomBlock> EndingRooms => endingRooms;
    public bool ForceStartingRoom => forceStartingRoom;
    public bool ForceEndingRoom => forceEndingRoom;
    public bool ShowDebugInfo => showDebugInfo;
    public bool ShowRoomBounds => showRoomBounds;
    public bool ShowElementGizmos => showElementGizmos;
    public bool GenerateOnStart => generateOnStart;
    
    /// <summary>
    /// 应用配置到房间生成器
    /// </summary>
    public void ApplyToRoomGenerator(RoomGenerator generator)
    {
        if (generator == null) return;
        
        // 使用反射设置私有字段（或者添加公共属性）
        var generatorType = typeof(RoomGenerator);
        
        // 设置房间块列表
        var availableRoomBlocksField = generatorType.GetField("availableRoomBlocks", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        availableRoomBlocksField?.SetValue(generator, availableRoomBlocks);
        
        // 设置其他参数
        var minRoomsField = generatorType.GetField("minRooms", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        minRoomsField?.SetValue(generator, minRooms);
        
        var maxRoomsField = generatorType.GetField("maxRooms", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        maxRoomsField?.SetValue(generator, maxRooms);
        
        var roomSpacingField = generatorType.GetField("roomSpacing", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        roomSpacingField?.SetValue(generator, roomSpacing);
        
        var maxRetryAttemptsField = generatorType.GetField("maxRetryAttempts", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        maxRetryAttemptsField?.SetValue(generator, maxRetryAttempts);
        
        var connectionMarginField = generatorType.GetField("connectionMargin", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        connectionMarginField?.SetValue(generator, connectionMargin);
    }
    
    /// <summary>
    /// 应用配置到元素填充器
    /// </summary>
    public void ApplyToElementFiller(ElementFiller filler)
    {
        if (filler == null) return;
        
        var fillerType = typeof(ElementFiller);
        
        // 设置元素配置
        var elementConfigsField = fillerType.GetField("elementConfigs", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        elementConfigsField?.SetValue(filler, elementConfigs);
        
        // 设置其他参数
        var gridSizeField = fillerType.GetField("gridSize", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gridSizeField?.SetValue(filler, gridSize);
        
        var maxRetryAttemptsField = fillerType.GetField("maxRetryAttempts", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        maxRetryAttemptsField?.SetValue(filler, elementMaxRetryAttempts);
    }
    
    /// <summary>
    /// 验证配置
    /// </summary>
    public bool ValidateConfig()
    {
        bool isValid = true;
        
        if (minRooms < 1)
        {
            Debug.LogError("最小房间数不能小于1");
            isValid = false;
        }
        
        if (maxRooms < minRooms)
        {
            Debug.LogError("最大房间数不能小于最小房间数");
            isValid = false;
        }
        
        if (availableRoomBlocks.Count == 0)
        {
            Debug.LogError("没有可用的房间块");
            isValid = false;
        }
        
        if (forceStartingRoom && startingRooms.Count == 0)
        {
            Debug.LogWarning("强制使用起始房间但没有配置起始房间");
        }
        
        if (forceEndingRoom && endingRooms.Count == 0)
        {
            Debug.LogWarning("强制使用结束房间但没有配置结束房间");
        }
        
        return isValid;
    }
    
    /// <summary>
    /// 重置为默认值
    /// </summary>
    [ContextMenu("重置为默认值")]
    public void ResetToDefaults()
    {
        minRooms = 2;
        maxRooms = 3;
        roomSpacing = 1f;
        maxRetryAttempts = 10;
        connectionMargin = 0.5f;
        gridSize = 1f;
        elementMaxRetryAttempts = 10;
        useWeightedSelection = true;
        forceStartingRoom = true;
        forceEndingRoom = false;
        showDebugInfo = true;
        showRoomBounds = true;
        showElementGizmos = true;
        generateOnStart = false;
    }
}
