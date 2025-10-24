using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 房间块数据类，存储房间块的基本信息
/// </summary>
[System.Serializable]
public class RoomBlockData
{
    [Header("房间块基本信息")]
    public string roomName;
    public Vector2Int size; // 房间块尺寸（以网格单位计算）
    public RoomType roomType;
    
    [Header("门洞信息")]
    public List<DoorConnection> doorConnections = new List<DoorConnection>();
    
    [Header("生成权重")]
    [Range(0f, 1f)]
    public float spawnWeight = 1f;
    
    [Header("特殊标记")]
    public bool isStartingRoom = false;
    public bool isEndingRoom = false;
}

/// <summary>
/// 门洞连接信息
/// </summary>
[System.Serializable]
public class DoorConnection
{
    public Dir direction;
    public Vector2Int localPosition; // 相对于房间块原点的位置
    public bool isConnected = false;
    public DoorType doorType = DoorType.Normal;
}

/// <summary>
/// 门洞类型
/// </summary>
public enum DoorType
{
    Normal,     // 普通门洞
    Special,    // 特殊门洞
    Secret      // 隐藏门洞
}

/// <summary>
/// 方向枚举 (N=0, E=1, S=2, W=3)
/// </summary>
public enum Dir
{
    N = 0,  // North
    E = 1,  // East  
    S = 2,  // South
    W = 3   // West
}

/// <summary>
/// 门洞接口数据结构
/// </summary>
[System.Serializable]
public class Socket
{
    [Header("门洞方向")]
    public Dir dir;
    
    [Header("门洞位置")]
    public Transform t;
    
    [Header("连接状态")]
    public bool isConnected = false;
    
    [Header("相反方向")]
    public Dir oppositeDir;
    
    public Socket(Dir direction, Transform transform)
    {
        dir = direction;
        t = transform;
        oppositeDir = GetOppositeDirection(direction);
    }
    
    private Dir GetOppositeDirection(Dir direction)
    {
        return (Dir)(((int)direction + 2) % 4);
    }
}

/// <summary>
/// 房间块组件，附加到房间块Prefab上
/// </summary>
public class RoomBlock : MonoBehaviour
{
    [Header("房间块基本信息")]
    [SerializeField] private string roomName = "NewRoom";
    
    [Header("房间块尺寸")]
    [SerializeField] private Vector2Int size = new Vector2Int(10, 10);
    
    [Header("旋转设置")]
    [SerializeField] private bool allowRotate = true;
    
    [Header("门洞接口")]
    [SerializeField] private List<Socket> sockets = new List<Socket>();
    
    [Header("可选：Tilemap包围盒")]
    [SerializeField] private BoundsInt boundsLocal = new BoundsInt();
    
    [Header("调试设置")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private Color socketColor = Color.red;
    
    [Header("几何组件")]
    [SerializeField] public Transform tilemapParent;
    [SerializeField] public Transform collisionParent;
    
    [Header("房间数据")]
    [SerializeField] public RoomBlockData roomData;
    
    // 公共属性
    public Vector2Int Size => size;
    public bool AllowRotate => allowRotate;
    public List<Socket> Sockets => sockets;
    public BoundsInt BoundsLocal => boundsLocal;
    public string RoomName => roomName;
    public Transform TilemapParent => tilemapParent;
    public Transform CollisionParent => collisionParent;
    public RoomBlockData RoomData => roomData;
    
    private void Awake()
    {
        InitializeSockets();
    }
    
    /// <summary>
    /// 初始化门洞接口
    /// </summary>
    private void InitializeSockets()
    {
        // 自动查找子物体中的门洞接口
        if (sockets.Count == 0)
        {
            FindSocketTransforms();
        }
    }
    
    /// <summary>
    /// 查找子物体中的门洞接口
    /// </summary>
    private void FindSocketTransforms()
    {
        // 查找标准的门洞接口子物体
        Transform socketN = transform.Find("Socket_N");
        Transform socketE = transform.Find("Socket_E");
        Transform socketS = transform.Find("Socket_S");
        Transform socketW = transform.Find("Socket_W");
        
        if (socketN != null) sockets.Add(new Socket(Dir.N, socketN));
        if (socketE != null) sockets.Add(new Socket(Dir.E, socketE));
        if (socketS != null) sockets.Add(new Socket(Dir.S, socketS));
        if (socketW != null) sockets.Add(new Socket(Dir.W, socketW));
    }
    
    /// <summary>
    /// 获取指定方向的门洞接口
    /// </summary>
    public Socket GetSocket(Dir direction)
    {
        foreach (Socket socket in sockets)
        {
            if (socket.dir == direction)
            {
                return socket;
            }
        }
        return null;
    }
    
    /// <summary>
    /// 检查指定方向是否有门洞
    /// </summary>
    public bool HasSocket(Dir direction)
    {
        return GetSocket(direction) != null;
    }
    
    /// <summary>
    /// 获取所有可用的门洞方向
    /// </summary>
    public List<Dir> GetAvailableSockets()
    {
        List<Dir> availableSockets = new List<Dir>();
        foreach (Socket socket in sockets)
        {
            if (!socket.isConnected)
            {
                availableSockets.Add(socket.dir);
            }
        }
        return availableSockets;
    }
    
    /// <summary>
    /// 标记门洞为已连接
    /// </summary>
    public void MarkSocketConnected(Dir direction)
    {
        Socket socket = GetSocket(direction);
        if (socket != null)
        {
            socket.isConnected = true;
        }
    }
    
    /// <summary>
    /// 标记门洞为已连接（兼容旧版本）
    /// </summary>
    public void MarkDoorConnected(Dir direction)
    {
        MarkSocketConnected(direction);
    }
    
    /// <summary>
    /// 获取房间块的世界边界
    /// </summary>
    public Bounds GetWorldBounds()
    {
        Vector3 center = transform.position;
        Vector3 size3D = new Vector3(size.x, size.y, 0);
        return new Bounds(center, size3D);
    }
    
    /// <summary>
    /// 获取房间块的局部边界
    /// </summary>
    public Bounds GetLocalBounds()
    {
        Vector3 size3D = new Vector3(size.x, size.y, 0);
        return new Bounds(Vector3.zero, size3D);
    }
    
    /// <summary>
    /// 旋转房间块90度
    /// </summary>
    public void Rotate90()
    {
        if (!allowRotate) return;
        
        transform.Rotate(0, 0, 90);
        
        // 更新门洞方向
        UpdateSocketDirections();
    }
    
    /// <summary>
    /// 更新门洞方向（旋转后）
    /// </summary>
    private void UpdateSocketDirections()
    {
        foreach (Socket socket in sockets)
        {
            int newDirection = ((int)socket.dir + 1) % 4;
            socket.dir = (Dir)newDirection;
        }
    }
    
    /// <summary>
    /// 检查是否与另一个房间块重叠
    /// </summary>
    public bool OverlapsWith(RoomBlock other, float margin = 0.1f)
    {
        Bounds thisBounds = GetWorldBounds();
        Bounds otherBounds = other.GetWorldBounds();
        
        thisBounds.Expand(margin);
        otherBounds.Expand(margin);
        
        return thisBounds.Intersects(otherBounds);
    }
    
    /// <summary>
    /// 获取门洞的世界位置
    /// </summary>
    public Vector3 GetSocketWorldPosition(Dir direction)
    {
        Socket socket = GetSocket(direction);
        return socket?.t?.position ?? transform.position;
    }
    
    /// <summary>
    /// 获取门洞的局部位置
    /// </summary>
    public Vector3 GetSocketLocalPosition(Dir direction)
    {
        Socket socket = GetSocket(direction);
        return socket?.t?.localPosition ?? Vector3.zero;
    }
    
    /// <summary>
    /// 获取方向向量
    /// </summary>
    public Vector3 GetDirectionVector(Dir direction)
    {
        switch (direction)
        {
            case Dir.N: return Vector3.up;
            case Dir.E: return Vector3.right;
            case Dir.S: return Vector3.down;
            case Dir.W: return Vector3.left;
            default: return Vector3.zero;
        }
    }
    
    /// <summary>
    /// 获取相反方向
    /// </summary>
    public Dir GetOppositeDirection(Dir direction)
    {
        return (Dir)(((int)direction + 2) % 4);
    }
    
    /// <summary>
    /// 手动添加门洞接口
    /// </summary>
    [ContextMenu("添加门洞接口")]
    public void AddSocket()
    {
        // 找到下一个可用的方向
        Dir nextDir = Dir.N;
        foreach (Dir dir in System.Enum.GetValues(typeof(Dir)))
        {
            if (!HasSocket(dir))
            {
                nextDir = dir;
                break;
            }
        }
        
        // 创建门洞接口子物体
        GameObject socketObj = new GameObject($"Socket_{nextDir}");
        socketObj.transform.SetParent(transform);
        socketObj.transform.localPosition = GetDefaultSocketPosition(nextDir);
        
        // 添加到列表
        sockets.Add(new Socket(nextDir, socketObj.transform));
    }
    
    /// <summary>
    /// 获取默认门洞位置
    /// </summary>
    private Vector3 GetDefaultSocketPosition(Dir direction)
    {
        switch (direction)
        {
            case Dir.N: return new Vector3(0, size.y / 2f, 0);
            case Dir.E: return new Vector3(size.x / 2f, 0, 0);
            case Dir.S: return new Vector3(0, -size.y / 2f, 0);
            case Dir.W: return new Vector3(-size.x / 2f, 0, 0);
            default: return Vector3.zero;
        }
    }
    
    /// <summary>
    /// 清除所有门洞接口
    /// </summary>
    [ContextMenu("清除门洞接口")]
    public void ClearSockets()
    {
        foreach (Socket socket in sockets)
        {
            if (socket.t != null)
            {
                DestroyImmediate(socket.t.gameObject);
            }
        }
        sockets.Clear();
    }
    
    /// <summary>
    /// 自动设置门洞接口
    /// </summary>
    [ContextMenu("自动设置门洞接口")]
    public void AutoSetupSockets()
    {
        ClearSockets();
        
        // 为每个方向创建门洞接口
        for (int i = 0; i < 4; i++)
        {
            Dir dir = (Dir)i;
            GameObject socketObj = new GameObject($"Socket_{dir}");
            socketObj.transform.SetParent(transform);
            socketObj.transform.localPosition = GetDefaultSocketPosition(dir);
            
            sockets.Add(new Socket(dir, socketObj.transform));
        }
    }
    
    /// <summary>
    /// 可视化绘制（仅在选中时显示）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        // 绘制房间块占位框
        Gizmos.color = gizmoColor;
        Bounds bounds = GetWorldBounds();
        Gizmos.DrawWireCube(bounds.center, bounds.size);
        
        // 绘制门洞接口
        Gizmos.color = socketColor;
        foreach (Socket socket in sockets)
        {
            if (socket.t != null)
            {
                // 绘制门洞位置
                Gizmos.DrawWireSphere(socket.t.position, 0.3f);
                
                // 绘制方向箭头
                Vector3 direction = GetDirectionVector(socket.dir);
                Gizmos.DrawRay(socket.t.position, direction * 1f);
                
                // 绘制方向标记
                Vector3 arrowEnd = socket.t.position + direction * 1f;
                Vector3 arrowLeft = arrowEnd + Quaternion.Euler(0, 0, 135) * (-direction * 0.3f);
                Vector3 arrowRight = arrowEnd + Quaternion.Euler(0, 0, -135) * (-direction * 0.3f);
                
                Gizmos.DrawLine(arrowEnd, arrowLeft);
                Gizmos.DrawLine(arrowEnd, arrowRight);
            }
        }
        
        // 绘制房间名称
        Vector3 labelPos = bounds.center + Vector3.up * (bounds.size.y / 2f + 0.5f);
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(labelPos, roomName);
        #endif
    }
    
    /// <summary>
    /// 普通Gizmos绘制（始终显示）
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // 绘制简化的房间块边界
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Bounds bounds = GetWorldBounds();
        Gizmos.DrawCube(bounds.center, bounds.size);
    }
}
