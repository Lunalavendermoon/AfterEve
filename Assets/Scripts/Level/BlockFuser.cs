using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 房间块融合生成器，负责随机拼接2-3个房间块
/// </summary>
public class BlockFuser : MonoBehaviour
{
    [Header("房间块配置")]
    [SerializeField] private List<RoomBlock> blockCatalog = new List<RoomBlock>();
    
    [Header("生成设置")]
    [SerializeField] private int minBlocks = 2;
    [SerializeField] private int maxBlocks = 3;
    [SerializeField] private float cellSize = 1f;
    
    [Header("占位检测")]
    [SerializeField] private LayerMask occupyMask = -1;
    [SerializeField] private float overlapMargin = 0.1f;
    
    [Header("生成控制")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private int maxRetryAttempts = 10;
    [SerializeField] private bool showDebugInfo = true;
    
    [Header("调试设置")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color placedBlockColor = Color.green;
    [SerializeField] private Color failedBlockColor = Color.red;
    
    // 生成状态
    private List<RoomBlock> placedBlocks = new List<RoomBlock>();
    private List<Vector2Int> occupiedCells = new List<Vector2Int>();
    private int generationAttempts = 0;
    
    // 事件
    public System.Action<List<RoomBlock>> OnGenerationComplete;
    public System.Action<RoomBlock> OnBlockPlaced;
    public System.Action<string> OnGenerationFailed;
    
    private void Start()
    {
        if (generateOnStart)
        {
            Generate();
        }
    }
    
    /// <summary>
    /// 生成房间块布局
    /// </summary>
    [ContextMenu("生成房间块")]
    public void Generate()
    {
        if (showDebugInfo)
        {
            Debug.Log("=== 开始生成房间块布局 ===");
        }
        
        // 清空旧内容
        ClearExistingBlocks();
        
        // 随机确定块数量
        int targetBlockCount = Random.Range(minBlocks, maxBlocks + 1);
        
        if (showDebugInfo)
        {
            Debug.Log($"目标生成 {targetBlockCount} 个房间块");
        }
        
        // 生成房间块
        bool success = GenerateBlockLayout(targetBlockCount);
        
        if (success)
        {
            OnGenerationComplete?.Invoke(placedBlocks);
            
            if (showDebugInfo)
            {
                Debug.Log($"房间块生成完成！共生成 {placedBlocks.Count} 个房间块");
                foreach (var block in placedBlocks)
                {
                    Debug.Log($"- {block.RoomName} 在位置 {block.transform.position}");
                }
            }
        }
        else
        {
            OnGenerationFailed?.Invoke("生成失败：无法放置足够的房间块");
            Debug.LogError("房间块生成失败！");
        }
    }
    
    /// <summary>
    /// 生成房间块布局
    /// </summary>
    private bool GenerateBlockLayout(int targetCount)
    {
        // 第一步：放置第一个房间块
        if (!PlaceFirstBlock())
        {
            return false;
        }
        
        // 第二步：逐个放置其他房间块
        for (int i = 1; i < targetCount; i++)
        {
            if (!PlaceNextBlock())
            {
                if (showDebugInfo)
                {
                    Debug.LogWarning($"无法放置第 {i + 1} 个房间块，停止生成");
                }
                break;
            }
        }
        
        return placedBlocks.Count >= minBlocks;
    }
    
    /// <summary>
    /// 放置第一个房间块
    /// </summary>
    private bool PlaceFirstBlock()
    {
        if (blockCatalog.Count == 0)
        {
            Debug.LogError("房间块目录为空！");
            return false;
        }
        
        // 选择第一个房间块
        RoomBlock firstBlockPrefab = GetRandomBlockPrefab();
        if (firstBlockPrefab == null)
        {
            Debug.LogError("无法获取房间块Prefab！");
            return false;
        }
        
        // 实例化第一个房间块
        RoomBlock firstBlock = InstantiateRoomBlock(firstBlockPrefab, Vector3.zero);
        if (firstBlock == null)
        {
            Debug.LogError("无法实例化第一个房间块！");
            return false;
        }
        
        // 添加到已放置列表
        placedBlocks.Add(firstBlock);
        UpdateOccupiedCells(firstBlock);
        
        OnBlockPlaced?.Invoke(firstBlock);
        
        if (showDebugInfo)
        {
            Debug.Log($"放置第一个房间块: {firstBlock.RoomName}");
        }
        
        return true;
    }
    
    /// <summary>
    /// 放置下一个房间块
    /// </summary>
    private bool PlaceNextBlock()
    {
        for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
        {
            // 随机选择宿主块和门洞
            var hostInfo = GetRandomHostBlockAndSocket();
            if (hostInfo.block == null)
            {
                continue;
            }
            
            // 随机选择候选块
            RoomBlock candidatePrefab = GetRandomBlockPrefab();
            if (candidatePrefab == null)
            {
                continue;
            }
            
            // 尝试放置候选块
            RoomBlock placedBlock = TryPlaceCandidateBlock(hostInfo.block, hostInfo.socket, candidatePrefab);
            if (placedBlock != null)
            {
                // 成功放置
                placedBlocks.Add(placedBlock);
                UpdateOccupiedCells(placedBlock);
                
                // 标记门洞为已连接
                hostInfo.block.MarkSocketConnected(hostInfo.socket.dir);
                placedBlock.MarkSocketConnected(hostInfo.socket.oppositeDir);
                
                OnBlockPlaced?.Invoke(placedBlock);
                
                if (showDebugInfo)
                {
                    Debug.Log($"成功放置房间块: {placedBlock.RoomName} 连接到 {hostInfo.block.RoomName}");
                }
                
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 尝试放置候选房间块
    /// </summary>
    private RoomBlock TryPlaceCandidateBlock(RoomBlock hostBlock, Socket hostSocket, RoomBlock candidatePrefab)
    {
        // 实例化候选块
        RoomBlock candidateBlock = InstantiateRoomBlock(candidatePrefab, Vector3.zero);
        if (candidateBlock == null)
        {
            return null;
        }
        
        // 随机选择候选块的门洞
        var candidateSockets = candidateBlock.GetAvailableSockets();
        if (candidateSockets.Count == 0)
        {
            DestroyImmediate(candidateBlock.gameObject);
            return null;
        }
        
        Dir candidateSocketDir = candidateSockets[Random.Range(0, candidateSockets.Count)];
        Socket candidateSocket = candidateBlock.GetSocket(candidateSocketDir);
        
        // 计算目标方向（与宿主门洞相反）
        Dir targetDirection = hostBlock.GetOppositeDirection(hostSocket.dir);
        
        // 调整候选块旋转
        if (!RotateBlockToDirection(candidateBlock, candidateSocketDir, targetDirection))
        {
            DestroyImmediate(candidateBlock.gameObject);
            return null;
        }
        
        // 计算对齐位置
        Vector3 hostSocketPos = hostBlock.GetSocketWorldPosition(hostSocket.dir);
        Vector3 candidateSocketPos = candidateBlock.GetSocketWorldPosition(targetDirection);
        Vector3 offset = hostSocketPos - candidateSocketPos;
        
        // 按cellSize进行网格对齐
        Vector3 alignedPosition = AlignToGrid(offset, cellSize);
        candidateBlock.transform.position = alignedPosition;
        
        // 占位检测
        if (CheckBlockOverlap(candidateBlock))
        {
            DestroyImmediate(candidateBlock.gameObject);
            return null;
        }
        
        return candidateBlock;
    }
    
    /// <summary>
    /// 旋转房间块到指定方向
    /// </summary>
    private bool RotateBlockToDirection(RoomBlock block, Dir currentSocketDir, Dir targetDirection)
    {
        if (!block.AllowRotate)
        {
            return currentSocketDir == targetDirection;
        }
        
        // 计算需要的旋转次数
        int rotationSteps = ((int)targetDirection - (int)currentSocketDir + 4) % 4;
        
        // 执行旋转
        for (int i = 0; i < rotationSteps; i++)
        {
            block.Rotate90();
        }
        
        return true;
    }
    
    /// <summary>
    /// 网格对齐
    /// </summary>
    private Vector3 AlignToGrid(Vector3 position, float cellSize)
    {
        return new Vector3(
            Mathf.Round(position.x / cellSize) * cellSize,
            Mathf.Round(position.y / cellSize) * cellSize,
            position.z
        );
    }
    
    /// <summary>
    /// 检查房间块重叠
    /// </summary>
    private bool CheckBlockOverlap(RoomBlock block)
    {
        // 使用2D物理检测
        Vector2 center = block.transform.position;
        Vector2 size = new Vector2(block.Size.x * cellSize, block.Size.y * cellSize);
        
        Collider2D[] overlaps = Physics2D.OverlapBoxAll(center, size, 0f, occupyMask);
        
        // 检查是否与已放置的房间块重叠
        foreach (var placedBlock in placedBlocks)
        {
            if (block.OverlapsWith(placedBlock, overlapMargin))
            {
                return true;
            }
        }
        
        // 检查是否与场景中的其他碰撞体重叠
        foreach (var overlap in overlaps)
        {
            if (overlap.gameObject != block.gameObject)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 获取随机宿主块和门洞
    /// </summary>
    private (RoomBlock block, Socket socket) GetRandomHostBlockAndSocket()
    {
        // 收集所有可用的门洞
        List<(RoomBlock block, Socket socket)> availableSockets = new List<(RoomBlock, Socket)>();
        
        foreach (var block in placedBlocks)
        {
            var availableDirs = block.GetAvailableSockets();
            foreach (var dir in availableDirs)
            {
                var socket = block.GetSocket(dir);
                if (socket != null)
                {
                    availableSockets.Add((block, socket));
                }
            }
        }
        
        if (availableSockets.Count == 0)
        {
            return (null, null);
        }
        
        // 随机选择一个
        var selected = availableSockets[Random.Range(0, availableSockets.Count)];
        return selected;
    }
    
    /// <summary>
    /// 获取随机房间块Prefab
    /// </summary>
    private RoomBlock GetRandomBlockPrefab()
    {
        if (blockCatalog.Count == 0)
        {
            return null;
        }
        
        return blockCatalog[Random.Range(0, blockCatalog.Count)];
    }
    
    /// <summary>
    /// 实例化房间块
    /// </summary>
    private RoomBlock InstantiateRoomBlock(RoomBlock prefab, Vector3 position)
    {
        if (prefab == null)
        {
            return null;
        }
        
        RoomBlock instance = Instantiate(prefab, position, Quaternion.identity, transform);
        return instance;
    }
    
    /// <summary>
    /// 更新占用的网格单元
    /// </summary>
    private void UpdateOccupiedCells(RoomBlock block)
    {
        Vector2Int blockPos = new Vector2Int(
            Mathf.RoundToInt(block.transform.position.x / cellSize),
            Mathf.RoundToInt(block.transform.position.y / cellSize)
        );
        
        for (int x = 0; x < block.Size.x; x++)
        {
            for (int y = 0; y < block.Size.y; y++)
            {
                Vector2Int cellPos = blockPos + new Vector2Int(x, y);
                if (!occupiedCells.Contains(cellPos))
                {
                    occupiedCells.Add(cellPos);
                }
            }
        }
    }
    
    /// <summary>
    /// 清空现有房间块
    /// </summary>
    private void ClearExistingBlocks()
    {
        foreach (var block in placedBlocks)
        {
            if (block != null)
            {
                DestroyImmediate(block.gameObject);
            }
        }
        
        placedBlocks.Clear();
        occupiedCells.Clear();
        generationAttempts = 0;
        
        if (showDebugInfo)
        {
            Debug.Log("已清空现有房间块");
        }
    }
    
    /// <summary>
    /// 获取已放置的房间块列表
    /// </summary>
    public List<RoomBlock> GetPlacedBlocks()
    {
        return new List<RoomBlock>(placedBlocks);
    }
    
    /// <summary>
    /// 获取生成统计信息
    /// </summary>
    public GenerationStats GetGenerationStats()
    {
        return new GenerationStats
        {
            totalBlocks = placedBlocks.Count,
            targetBlocks = maxBlocks,
            generationAttempts = generationAttempts,
            successRate = placedBlocks.Count / (float)maxBlocks
        };
    }
    
    /// <summary>
    /// 手动清空房间块
    /// </summary>
    [ContextMenu("清空房间块")]
    public void ClearBlocks()
    {
        ClearExistingBlocks();
    }
    
    /// <summary>
    /// 重新生成房间块
    /// </summary>
    [ContextMenu("重新生成")]
    public void Regenerate()
    {
        Generate();
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // 绘制已放置的房间块
        Gizmos.color = placedBlockColor;
        foreach (var block in placedBlocks)
        {
            if (block != null)
            {
                Bounds bounds = block.GetWorldBounds();
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }
        }
        
        // 绘制占用的网格单元
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        foreach (var cell in occupiedCells)
        {
            Vector3 cellPos = new Vector3(cell.x * cellSize, cell.y * cellSize, 0);
            Gizmos.DrawCube(cellPos, Vector3.one * cellSize);
        }
    }
}

/// <summary>
/// 生成统计信息
/// </summary>
[System.Serializable]
public class GenerationStats
{
    public int totalBlocks;
    public int targetBlocks;
    public int generationAttempts;
    public float successRate;
}
