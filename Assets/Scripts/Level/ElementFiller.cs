using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 元素类型枚举
/// </summary>
public enum ElementType
{
    Bomb,           // 炸弹
    Destructible,   // 可破坏物
    BulletWall,     // 子弹可穿障碍
    Hole,           // 洞
    HotGround,      // 热地
    Teleporter,     // 传送器
    SpiritualVision, // 灵视相关物
    PowerUp,        // 强化道具
    EnemySpawner,   // 敌人生成点
    Treasure        // 宝藏
}

/// <summary>
/// 元素配置数据
/// </summary>
[System.Serializable]
public class ElementConfig
{
    [Header("元素信息")]
    public ElementType elementType;
    public GameObject elementPrefab;
    
    [Header("生成规则")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;
    public int minCount = 0;
    public int maxCount = 3;
    
    [Header("位置限制")]
    public bool avoidCenter = false;
    public bool avoidEdges = false;
    public float minDistanceFromWalls = 1f;
    public float minDistanceBetweenElements = 2f;
    
    [Header("特殊规则")]
    public List<ElementType> incompatibleElements = new List<ElementType>();
    public List<ElementType> requiredElements = new List<ElementType>();
}

/// <summary>
/// 元素填充器，负责在房间中放置各种游戏元素
/// </summary>
public class ElementFiller : MonoBehaviour
{
    [Header("元素配置")]
    [SerializeField] private List<ElementConfig> elementConfigs = new List<ElementConfig>();
    
    [Header("填充设置")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private int maxRetryAttempts = 10;
    [SerializeField] private bool fillOnStart = false;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool showElementGizmos = true;
    
    private List<PlacedElement> placedElements = new List<PlacedElement>();
    private List<Vector2> occupiedPositions = new List<Vector2>();
    
    public System.Action<List<PlacedElement>> OnElementsFilled;
    
    private void Start()
    {
        if (fillOnStart)
        {
            FillElements();
        }
    }
    
    /// <summary>
    /// 填充元素到房间中
    /// </summary>
    public List<PlacedElement> FillElements()
    {
        ClearExistingElements();
        
        // 获取房间边界
        Bounds roomBounds = GetRoomBounds();
        if (roomBounds.size == Vector3.zero)
        {
            Debug.LogWarning("无法获取房间边界，跳过元素填充");
            return placedElements;
        }
        
        // 为每种元素类型进行填充
        foreach (ElementConfig config in elementConfigs)
        {
            FillElementType(config, roomBounds);
        }
        
        OnElementsFilled?.Invoke(placedElements);
        
        if (showDebugInfo)
        {
            Debug.Log($"成功填充 {placedElements.Count} 个元素");
        }
        
        return placedElements;
    }
    
    /// <summary>
    /// 填充特定类型的元素
    /// </summary>
    private void FillElementType(ElementConfig config, Bounds roomBounds)
    {
        if (config.elementPrefab == null) return;
        
        // 检查前置条件
        if (!CheckRequiredElements(config)) return;
        
        // 计算要生成的数量
        int elementCount = CalculateElementCount(config);
        
        for (int i = 0; i < elementCount; i++)
        {
            Vector2 position = FindValidPosition(config, roomBounds);
            if (position != Vector2.zero)
            {
                PlaceElement(config, position);
            }
        }
    }
    
    /// <summary>
    /// 计算元素数量
    /// </summary>
    private int CalculateElementCount(ElementConfig config)
    {
        if (Random.Range(0f, 1f) > config.spawnChance) return 0;
        
        return Random.Range(config.minCount, config.maxCount + 1);
    }
    
    /// <summary>
    /// 查找有效位置
    /// </summary>
    private Vector2 FindValidPosition(ElementConfig config, Bounds roomBounds)
    {
        for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
        {
            Vector2 position = GenerateRandomPosition(roomBounds);
            
            if (IsValidPosition(position, config, roomBounds))
            {
                return position;
            }
        }
        
        return Vector2.zero; // 未找到有效位置
    }
    
    /// <summary>
    /// 生成随机位置
    /// </summary>
    private Vector2 GenerateRandomPosition(Bounds roomBounds)
    {
        float x = Random.Range(roomBounds.min.x, roomBounds.max.x);
        float y = Random.Range(roomBounds.min.y, roomBounds.max.y);
        return new Vector2(x, y);
    }
    
    /// <summary>
    /// 检查位置是否有效
    /// </summary>
    private bool IsValidPosition(Vector2 position, ElementConfig config, Bounds roomBounds)
    {
        // 检查是否在房间内
        if (!roomBounds.Contains(position)) return false;
        
        // 检查中心区域限制
        if (config.avoidCenter && IsInCenterArea(position, roomBounds)) return false;
        
        // 检查边缘限制
        if (config.avoidEdges && IsNearEdge(position, roomBounds)) return false;
        
        // 检查与墙壁的距离
        if (!IsFarFromWalls(position, config.minDistanceFromWalls, roomBounds)) return false;
        
        // 检查与其他元素的距离
        if (!IsFarFromOtherElements(position, config.minDistanceBetweenElements)) return false;
        
        // 检查不兼容元素
        if (HasIncompatibleElements(position, config)) return false;
        
        return true;
    }
    
    /// <summary>
    /// 检查是否在中心区域
    /// </summary>
    private bool IsInCenterArea(Vector2 position, Bounds roomBounds)
    {
        Vector2 center = roomBounds.center;
        float centerRadius = Mathf.Min(roomBounds.size.x, roomBounds.size.y) * 0.3f;
        return Vector2.Distance(position, center) < centerRadius;
    }
    
    /// <summary>
    /// 检查是否靠近边缘
    /// </summary>
    private bool IsNearEdge(Vector2 position, Bounds roomBounds)
    {
        float edgeThreshold = 2f;
        return position.x < roomBounds.min.x + edgeThreshold ||
               position.x > roomBounds.max.x - edgeThreshold ||
               position.y < roomBounds.min.y + edgeThreshold ||
               position.y > roomBounds.max.y - edgeThreshold;
    }
    
    /// <summary>
    /// 检查与墙壁的距离
    /// </summary>
    private bool IsFarFromWalls(Vector2 position, float minDistance, Bounds roomBounds)
    {
        return position.x - roomBounds.min.x >= minDistance &&
               roomBounds.max.x - position.x >= minDistance &&
               position.y - roomBounds.min.y >= minDistance &&
               roomBounds.max.y - position.y >= minDistance;
    }
    
    /// <summary>
    /// 检查与其他元素的距离
    /// </summary>
    private bool IsFarFromOtherElements(Vector2 position, float minDistance)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(position, occupiedPos) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 检查是否有不兼容元素
    /// </summary>
    private bool HasIncompatibleElements(Vector2 position, ElementConfig config)
    {
        foreach (PlacedElement element in placedElements)
        {
            if (config.incompatibleElements.Contains(element.elementType))
            {
                if (Vector2.Distance(position, element.position) < config.minDistanceBetweenElements)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    /// <summary>
    /// 检查前置元素
    /// </summary>
    private bool CheckRequiredElements(ElementConfig config)
    {
        foreach (ElementType requiredType in config.requiredElements)
        {
            bool hasRequired = placedElements.Any(element => element.elementType == requiredType);
            if (!hasRequired) return false;
        }
        return true;
    }
    
    /// <summary>
    /// 放置元素
    /// </summary>
    private void PlaceElement(ElementConfig config, Vector2 position)
    {
        GameObject elementInstance = Instantiate(config.elementPrefab, position, Quaternion.identity, transform);
        
        PlacedElement placedElement = new PlacedElement
        {
            elementType = config.elementType,
            position = position,
            gameObject = elementInstance
        };
        
        placedElements.Add(placedElement);
        occupiedPositions.Add(position);
    }
    
    /// <summary>
    /// 获取房间边界
    /// </summary>
    private Bounds GetRoomBounds()
    {
        // 尝试从房间生成器获取房间边界
        RoomGenerator roomGenerator = FindObjectOfType<RoomGenerator>();
        if (roomGenerator != null)
        {
            return CalculateCombinedRoomBounds(roomGenerator);
        }
        
        // 如果没有房间生成器，使用当前对象的边界
        return GetComponent<Collider2D>()?.bounds ?? new Bounds(transform.position, Vector3.one * 10);
    }
    
    /// <summary>
    /// 计算组合房间边界
    /// </summary>
    private Bounds CalculateCombinedRoomBounds(RoomGenerator roomGenerator)
    {
        Bounds combinedBounds = new Bounds();
        bool hasBounds = false;
        
        // 这里需要访问房间生成器的房间列表
        // 由于RoomGenerator的generatedRooms是私有的，我们需要添加一个公共方法
        // 暂时使用一个简化的实现
        
        return new Bounds(transform.position, Vector3.one * 20); // 临时实现
    }
    
    /// <summary>
    /// 清除现有元素
    /// </summary>
    private void ClearExistingElements()
    {
        foreach (PlacedElement element in placedElements)
        {
            if (element.gameObject != null)
            {
                DestroyImmediate(element.gameObject);
            }
        }
        placedElements.Clear();
        occupiedPositions.Clear();
    }
    
    /// <summary>
    /// 手动触发填充
    /// </summary>
    [ContextMenu("填充元素")]
    public void FillElementsManually()
    {
        FillElements();
    }
    
    /// <summary>
    /// 清除所有元素
    /// </summary>
    [ContextMenu("清除元素")]
    public void ClearElements()
    {
        ClearExistingElements();
    }
    
    private void OnDrawGizmos()
    {
        if (!showElementGizmos) return;
        
        // 绘制已放置的元素
        foreach (PlacedElement element in placedElements)
        {
            Gizmos.color = GetElementColor(element.elementType);
            Gizmos.DrawWireSphere(element.position, 0.5f);
        }
    }
    
    /// <summary>
    /// 获取元素颜色（用于调试）
    /// </summary>
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
/// 已放置的元素数据
/// </summary>
[System.Serializable]
public class PlacedElement
{
    public ElementType elementType;
    public Vector2 position;
    public GameObject gameObject;
}
