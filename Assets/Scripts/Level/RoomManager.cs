using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 房间管理器，协调房间生成和元素填充的整个流程
/// </summary>
public class RoomManager : MonoBehaviour
{
    [Header("组件引用")]
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private ElementFiller elementFiller;
    
    [Header("生成设置")]
    [SerializeField] private bool autoGenerateOnStart = true;
    [SerializeField] private bool generateElementsAfterRooms = true;
    [SerializeField] private float generationDelay = 0.1f;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showRoomBounds = true;
    [SerializeField] private bool showElementCounts = true;
    
    private List<RoomBlock> currentRooms = new List<RoomBlock>();
    private List<PlacedElement> currentElements = new List<PlacedElement>();
    
    public System.Action OnGenerationComplete;
    public System.Action<List<RoomBlock>> OnRoomsGenerated;
    public System.Action<List<PlacedElement>> OnElementsGenerated;
    
    private void Start()
    {
        InitializeComponents();
        
        if (autoGenerateOnStart)
        {
            GenerateCompleteLevel();
        }
    }
    
    /// <summary>
    /// 初始化组件
    /// </summary>
    private void InitializeComponents()
    {
        if (roomGenerator == null)
        {
            roomGenerator = GetComponent<RoomGenerator>();
            if (roomGenerator == null)
            {
                roomGenerator = gameObject.AddComponent<RoomGenerator>();
            }
        }
        
        if (elementFiller == null)
        {
            elementFiller = GetComponent<ElementFiller>();
            if (elementFiller == null)
            {
                elementFiller = gameObject.AddComponent<ElementFiller>();
            }
        }
        
        // 订阅事件
        roomGenerator.OnRoomsGenerated += OnRoomsGeneratedHandler;
        elementFiller.OnElementsFilled += OnElementsFilledHandler;
    }
    
    /// <summary>
    /// 生成完整关卡
    /// </summary>
    public void GenerateCompleteLevel()
    {
        Debug.Log("RoomManager.GenerateCompleteLevel CALLED");
        if (showDebugInfo)
        {
            Debug.Log("开始生成完整关卡...");
        }
        
        StartCoroutine(GenerateLevelCoroutine());
    }
    
    /// <summary>
    /// 生成关卡协程
    /// </summary>
    private System.Collections.IEnumerator GenerateLevelCoroutine()
    {
        // 第一步：生成房间
        yield return StartCoroutine(GenerateRoomsCoroutine());
        
        // 等待一帧
        yield return null;
        
        // 第二步：填充元素
        if (generateElementsAfterRooms)
        {
            yield return StartCoroutine(GenerateElementsCoroutine());
        }
        
        // 完成生成
        OnGenerationComplete?.Invoke();
        
        if (showDebugInfo)
        {
            Debug.Log($"关卡生成完成！房间数：{currentRooms.Count}，元素数：{currentElements.Count}");
        }
    }
    
    /// <summary>
    /// 生成房间协程
    /// </summary>
    private System.Collections.IEnumerator GenerateRoomsCoroutine()
    {
        if (showDebugInfo)
        {
            Debug.Log("正在生成房间...");
        }
        
        currentRooms = roomGenerator.GenerateRoomLayout();
        OnRoomsGenerated?.Invoke(currentRooms);
        
        yield return new WaitForSeconds(generationDelay);
    }
    
    /// <summary>
    /// 生成元素协程
    /// </summary>
    private System.Collections.IEnumerator GenerateElementsCoroutine()
    {
        if (showDebugInfo)
        {
            Debug.Log("正在填充元素...");
        }
        
        currentElements = elementFiller.FillElements();
        OnElementsGenerated?.Invoke(currentElements);
        
        yield return new WaitForSeconds(generationDelay);
    }
    
    /// <summary>
    /// 房间生成完成处理
    /// </summary>
    private void OnRoomsGeneratedHandler(List<RoomBlock> rooms)
    {
        currentRooms = rooms;
        
        if (showDebugInfo)
        {
            Debug.Log($"房间生成完成，共 {rooms.Count} 个房间");
            foreach (var room in rooms)
            {
                Debug.Log($"- {room.RoomName} ({room.roomData?.roomType})");
            }
        }
    }
    
    /// <summary>
    /// 元素填充完成处理
    /// </summary>
    private void OnElementsFilledHandler(List<PlacedElement> elements)
    {
        currentElements = elements;
        
        if (showDebugInfo && showElementCounts)
        {
            Debug.Log($"元素填充完成，共 {elements.Count} 个元素");
            
            // 统计各类型元素数量
            var elementCounts = elements.GroupBy(e => e.elementType)
                                      .ToDictionary(g => g.Key, g => g.Count());
            
            foreach (var kvp in elementCounts)
            {
                Debug.Log($"- {kvp.Key}: {kvp.Value} 个");
            }
        }
    }
    
    /// <summary>
    /// 重新生成关卡
    /// </summary>
    [ContextMenu("重新生成关卡")]
    public void RegenerateLevel()
    {
        ClearCurrentLevel();
        GenerateCompleteLevel();
    }
    
    /// <summary>
    /// 清除当前关卡
    /// </summary>
    [ContextMenu("清除关卡")]
    public void ClearCurrentLevel()
    {
        // 清除房间
        if (roomGenerator != null)
        {
            roomGenerator.ClearRooms();
        }
        
        // 清除元素
        if (elementFiller != null)
        {
            elementFiller.ClearElements();
        }
        
        currentRooms.Clear();
        currentElements.Clear();
        
        if (showDebugInfo)
        {
            Debug.Log("关卡已清除");
        }
    }
    
    /// <summary>
    /// 获取当前房间列表
    /// </summary>
    public List<RoomBlock> GetCurrentRooms()
    {
        return new List<RoomBlock>(currentRooms);
    }
    
    /// <summary>
    /// 获取当前元素列表
    /// </summary>
    public List<PlacedElement> GetCurrentElements()
    {
        return new List<PlacedElement>(currentElements);
    }
    
    /// <summary>
    /// 获取指定类型的元素
    /// </summary>
    public List<PlacedElement> GetElementsOfType(ElementType elementType)
    {
        return currentElements.Where(e => e.elementType == elementType).ToList();
    }
    
    /// <summary>
    /// 获取房间统计信息
    /// </summary>
    public RoomStatistics GetRoomStatistics()
    {
        var stats = new RoomStatistics
        {
            totalRooms = currentRooms.Count,
            totalElements = currentElements.Count,
            roomTypes = currentRooms.GroupBy(r => r.roomData?.roomType ?? RoomType.Square)
                                 .ToDictionary(g => g.Key, g => g.Count()),
            elementTypes = currentElements.GroupBy(e => e.elementType)
                                        .ToDictionary(g => g.Key, g => g.Count())
        };
        
        return stats;
    }
    
    private void OnDrawGizmos()
    {
        if (!showRoomBounds) return;
        
        // 绘制房间边界
        Gizmos.color = Color.green;
        foreach (RoomBlock room in currentRooms)
        {
            Bounds bounds = room.GetWorldBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
        
        // 绘制元素位置
        if (showElementCounts)
        {
            foreach (PlacedElement element in currentElements)
            {
                Gizmos.color = GetElementColor(element.elementType);
                Gizmos.DrawWireSphere(element.position, 0.3f);
            }
        }
    }
    
    private Color GetElementColor(ElementType elementType)
    {
        switch (elementType)
        {
            case ElementType.Bomb: return Color.red;
            case ElementType.Destructible: return Color.gray;
            case ElementType.BulletWall: return Color.black;
            case ElementType.Hole: return Color.blue;
            case ElementType.HotGround: return Color.yellow;
            case ElementType.Teleporter: return Color.magenta;
            case ElementType.SpiritualVision: return Color.cyan;
            case ElementType.PowerUp: return Color.green;
            case ElementType.EnemySpawner: return Color.red;
            case ElementType.Treasure: return Color.yellow;
            default: return Color.white;
        }
    }
}

/// <summary>
/// 房间统计信息
/// </summary>
[System.Serializable]
public class RoomStatistics
{
    public int totalRooms;
    public int totalElements;
    public Dictionary<RoomType, int> roomTypes;
    public Dictionary<ElementType, int> elementTypes;
}
