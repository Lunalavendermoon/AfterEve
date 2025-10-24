using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 房间生成器，负责房间块的拼接和融合
/// </summary>
public class RoomGenerator : MonoBehaviour
{
    [Header("房间块配置")]
    [SerializeField] private List<RoomBlock> availableRoomBlocks = new List<RoomBlock>();
    [SerializeField] private int minRooms = 2;
    [SerializeField] private int maxRooms = 3;
    
    [Header("生成设置")]
    [SerializeField] private float roomSpacing = 1f;
    [SerializeField] private int maxRetryAttempts = 10;
    [SerializeField] private float connectionMargin = 0.5f;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private bool generateOnStart = false;
    
    private List<RoomBlock> generatedRooms = new List<RoomBlock>();
    private List<Vector2Int> occupiedPositions = new List<Vector2Int>();
    
    public System.Action<List<RoomBlock>> OnRoomsGenerated;
    
    private void Start()
    {
        if (generateOnStart)
        {
            GenerateRoomLayout();
        }
    }
    
    /// <summary>
    /// 生成房间布局
    /// </summary>
    public List<RoomBlock> GenerateRoomLayout()
    {
        ClearExistingRooms();
        
        int roomCount = Random.Range(minRooms, maxRooms + 1);
        List<RoomBlock> layout = new List<RoomBlock>();
        
        // 第一步：放置起始房间
        RoomBlock startingRoom = PlaceStartingRoom();
        if (startingRoom == null)
        {
            Debug.LogError("无法放置起始房间！");
            return layout;
        }
        
        layout.Add(startingRoom);
        generatedRooms.Add(startingRoom);
        
        // 第二步：连接其他房间
        for (int i = 1; i < roomCount; i++)
        {
            RoomBlock newRoom = TryPlaceNextRoom(layout);
            if (newRoom != null)
            {
                layout.Add(newRoom);
                generatedRooms.Add(newRoom);
            }
            else
            {
                Debug.LogWarning($"无法放置第 {i + 1} 个房间，停止生成");
                break;
            }
        }
        
        OnRoomsGenerated?.Invoke(layout);
        
        if (showDebugInfo)
        {
            Debug.Log($"成功生成 {layout.Count} 个房间");
        }
        
        return layout;
    }
    
    /// <summary>
    /// 放置起始房间
    /// </summary>
    private RoomBlock PlaceStartingRoom()
    {
        // 选择起始房间（优先选择标记为起始房间的）
        RoomBlock startingRoomPrefab = GetStartingRoomPrefab();
        if (startingRoomPrefab == null)
        {
            startingRoomPrefab = GetRandomRoomBlock();
        }
        
        if (startingRoomPrefab == null)
        {
            Debug.LogError("没有可用的房间块！");
            return null;
        }
        
        RoomBlock instance = InstantiateRoomBlock(startingRoomPrefab, Vector3.zero);
        occupiedPositions.Add(Vector2Int.zero);
        
        return instance;
    }
    
    /// <summary>
    /// 尝试放置下一个房间
    /// </summary>
    private RoomBlock TryPlaceNextRoom(List<RoomBlock> existingRooms)
    {
        for (int attempt = 0; attempt < maxRetryAttempts; attempt++)
        {
            // 随机选择一个已存在的房间
            RoomBlock targetRoom = existingRooms[Random.Range(0, existingRooms.Count)];
            
            // 获取可用的门洞
            List<Dir> availableDoors = targetRoom.GetAvailableSockets();
            if (availableDoors.Count == 0)
            {
                continue;
            }
            
            // 随机选择一个门洞
            Dir selectedDoor = availableDoors[Random.Range(0, availableDoors.Count)];
            
            // 选择要连接的新房间
            RoomBlock newRoomPrefab = GetRandomRoomBlock();
            if (newRoomPrefab == null) continue;
            
            // 尝试放置新房间
            RoomBlock newRoom = TryConnectRoom(targetRoom, selectedDoor, newRoomPrefab);
            if (newRoom != null)
            {
                // 标记门洞为已连接
                targetRoom.MarkDoorConnected(selectedDoor);
                newRoom.MarkSocketConnected(targetRoom.GetOppositeDirection(selectedDoor));
                
                return newRoom;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 尝试连接房间
    /// </summary>
    private RoomBlock TryConnectRoom(RoomBlock targetRoom, Dir targetDoor, RoomBlock newRoomPrefab)
    {
        // 计算目标门洞的世界位置
        Transform targetDoorConnector = targetRoom.GetSocket(targetDoor)?.t;
        if (targetDoorConnector == null) return null;
        
        Vector3 targetDoorPosition = targetDoorConnector.position;
        
        // 创建新房间实例
        RoomBlock newRoom = InstantiateRoomBlock(newRoomPrefab, targetDoorPosition);
        
        // 找到新房间中与目标门洞相对的门洞
        Dir oppositeDirection = targetRoom.GetOppositeDirection(targetDoor);
        Transform newRoomDoorConnector = newRoom.GetSocket(oppositeDirection)?.t;
        
        if (newRoomDoorConnector == null)
        {
            // 如果没有合适的门洞，尝试旋转房间
            if (!TryRotateRoomToMatchDoor(newRoom, oppositeDirection))
            {
                DestroyImmediate(newRoom.gameObject);
                return null;
            }
            newRoomDoorConnector = newRoom.GetSocket(oppositeDirection)?.t;
        }
        
        // 计算新房间的最终位置
        Vector3 offset = targetDoorPosition - newRoomDoorConnector.position;
        newRoom.transform.position += offset;
        
        // 检查是否与现有房间重叠
        if (CheckRoomOverlap(newRoom))
        {
            DestroyImmediate(newRoom.gameObject);
            return null;
        }
        
        return newRoom;
    }
    
    /// <summary>
    /// 尝试旋转房间以匹配门洞
    /// </summary>
    private bool TryRotateRoomToMatchDoor(RoomBlock room, Dir requiredDirection)
    {
        for (int rotation = 0; rotation < 4; rotation++)
        {
            if (room.HasSocket(requiredDirection))
            {
                return true;
            }
            room.transform.Rotate(0, 0, 90);
            // 更新门洞方向（旋转后）
            foreach (var socket in room.Sockets)
            {
                int newDirection = ((int)socket.dir + 1) % 4;
                socket.dir = (Dir)newDirection;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 检查房间重叠
    /// </summary>
    private bool CheckRoomOverlap(RoomBlock newRoom)
    {
        foreach (RoomBlock existingRoom in generatedRooms)
        {
            if (newRoom.OverlapsWith(existingRoom, connectionMargin))
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取起始房间Prefab
    /// </summary>
    private RoomBlock GetStartingRoomPrefab()
    {
        var startingRooms = availableRoomBlocks.Where(room => room.roomData != null && room.roomData.isStartingRoom).ToList();
        if (startingRooms.Count > 0)
        {
            return startingRooms[Random.Range(0, startingRooms.Count)];
        }
        return null;
    }
    
    /// <summary>
    /// 获取随机房间块
    /// </summary>
    private RoomBlock GetRandomRoomBlock()
    {
        if (availableRoomBlocks.Count == 0) return null;
        
        // 基于权重选择房间
        float totalWeight = availableRoomBlocks.Sum(room => room.roomData != null ? room.roomData.spawnWeight : 1f);
        float randomValue = Random.Range(0f, totalWeight);
        
        float currentWeight = 0f;
        foreach (var room in availableRoomBlocks)
        {
            currentWeight += room.roomData != null ? room.roomData.spawnWeight : 1f;
            if (randomValue <= currentWeight)
            {
                return room;
            }
        }
        
        return availableRoomBlocks[Random.Range(0, availableRoomBlocks.Count)];
    }
    
    /// <summary>
    /// 实例化房间块
    /// </summary>
    private RoomBlock InstantiateRoomBlock(RoomBlock prefab, Vector3 position)
    {
        RoomBlock instance = Instantiate(prefab, position, Quaternion.identity, transform);
        return instance;
    }
    
    /// <summary>
    /// 获取相反方向
    /// </summary>
    // 这个方法已经移到RoomBlock类中，不再需要
    
    /// <summary>
    /// 清除现有房间
    /// </summary>
    private void ClearExistingRooms()
    {
        foreach (RoomBlock room in generatedRooms)
        {
            if (room != null)
            {
                DestroyImmediate(room.gameObject);
            }
        }
        generatedRooms.Clear();
        occupiedPositions.Clear();
    }
    
    /// <summary>
    /// 手动触发生成
    /// </summary>
    [ContextMenu("生成房间布局")]
    public void GenerateRooms()
    {
        GenerateRoomLayout();
    }
    
    /// <summary>
    /// 清除所有房间
    /// </summary>
    [ContextMenu("清除房间")]
    public void ClearRooms()
    {
        ClearExistingRooms();
    }
    
    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;
        
        // 绘制已占用的位置
        Gizmos.color = Color.yellow;
        foreach (Vector2Int pos in occupiedPositions)
        {
            Gizmos.DrawWireCube(new Vector3(pos.x, pos.y, 0), Vector3.one);
        }
    }
}
