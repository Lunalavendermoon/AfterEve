using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 房间模板创建器 - 根据设计文档快速创建10种房间类型
/// </summary>
public class RoomTemplateCreator : MonoBehaviour
{
    [Header("创建设置")]
    [SerializeField] private Material wallMaterial;
    [SerializeField] private Material floorMaterial;
    [SerializeField] private bool createVisualMarkers = true;
    
    /// <summary>
    /// 创建所有房间模板
    /// </summary>
    [ContextMenu("创建所有房间模板")]
    public void CreateAllRoomTemplates()
    {
        Create_Cross_16();
        Create_URoom_16();
        Create_T_16();
        Create_Corner_L_16();
        Create_Corridor_16x8();
        Create_DeadEnd_16();
        Create_Arena_16();
        Create_Ring_16();
        Create_SplitRoom_DualDoor_16();
        Create_SpiritGate_16();
        
        Debug.Log("所有房间模板创建完成！");
    }
    
    /// <summary>
    /// 1. Cross_16 - 中心十字房
    /// </summary>
    [ContextMenu("创建 Cross_16")]
    public void Create_Cross_16()
    {
        GameObject room = CreateBaseRoom("Cross_16", new Vector2Int(16, 16), true);
        
        // 创建四个门洞
        CreateSocket(room, Dir.N, new Vector3(0, 8, 0));
        CreateSocket(room, Dir.E, new Vector3(8, 0, 0));
        CreateSocket(room, Dir.S, new Vector3(0, -8, 0));
        CreateSocket(room, Dir.W, new Vector3(-8, 0, 0));
        
        // 创建中心十字形障碍
        CreateCrossObstacle(room, 6f);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("Cross_16 创建完成！");
    }
    
    /// <summary>
    /// 2. URoom_16 - U形房
    /// </summary>
    [ContextMenu("创建 URoom_16")]
    public void Create_URoom_16()
    {
        GameObject room = CreateBaseRoom("URoom_16", new Vector2Int(16, 16), true);
        
        // 创建南侧双门（间距6格）
        CreateSocket(room, Dir.S, new Vector3(-3, -8, 0), "Socket_S_A");
        CreateSocket(room, Dir.S, new Vector3(3, -8, 0), "Socket_S_B");
        
        // 创建U形围挡
        CreateUShapeWalls(room, 16, 16, 1f);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("URoom_16 创建完成！");
    }
    
    /// <summary>
    /// 3. T_16 - T字路口
    /// </summary>
    [ContextMenu("创建 T_16")]
    public void Create_T_16()
    {
        GameObject room = CreateBaseRoom("T_16", new Vector2Int(16, 16), true);
        
        // 创建三个门洞（北、东、西）
        CreateSocket(room, Dir.N, new Vector3(0, 8, 0));
        CreateSocket(room, Dir.E, new Vector3(8, 0, 0));
        CreateSocket(room, Dir.W, new Vector3(-8, 0, 0));
        
        // 创建T形墙体
        CreateTShapeWalls(room, 16, 16);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("T_16 创建完成！");
    }
    
    /// <summary>
    /// 4. Corner_L_16 - 直角转弯房
    /// </summary>
    [ContextMenu("创建 Corner_L_16")]
    public void Create_Corner_L_16()
    {
        GameObject room = CreateBaseRoom("Corner_L_16", new Vector2Int(16, 16), true);
        
        // 创建两个门洞（东、北）
        CreateSocket(room, Dir.E, new Vector3(8, 0, 0));
        CreateSocket(room, Dir.N, new Vector3(0, 8, 0));
        
        // 创建L形墙体
        CreateLShapeWalls(room, 16, 16);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("Corner_L_16 创建完成！");
    }
    
    /// <summary>
    /// 5. Corridor_16x8 - 直走廊
    /// </summary>
    [ContextMenu("创建 Corridor_16x8")]
    public void Create_Corridor_16x8()
    {
        GameObject room = CreateBaseRoom("Corridor_16x8", new Vector2Int(16, 8), true);
        
        // 创建两个门洞（东、西）
        CreateSocket(room, Dir.E, new Vector3(8, 0, 0));
        CreateSocket(room, Dir.W, new Vector3(-8, 0, 0));
        
        // 创建走廊墙体
        CreateCorridorWalls(room, 16, 8);
        
        // 创建地面
        CreateFloor(room, 16, 8);
        
        Debug.Log("Corridor_16x8 创建完成！");
    }
    
    /// <summary>
    /// 6. DeadEnd_16 - 死路节点
    /// </summary>
    [ContextMenu("创建 DeadEnd_16")]
    public void Create_DeadEnd_16()
    {
        GameObject room = CreateBaseRoom("DeadEnd_16", new Vector2Int(16, 16), true);
        
        // 创建一个门洞（南）
        CreateSocket(room, Dir.S, new Vector3(0, -8, 0));
        
        // 创建死路墙体
        CreateDeadEndWalls(room, 16, 16);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("DeadEnd_16 创建完成！");
    }
    
    /// <summary>
    /// 7. Arena_16 - 小型战斗间
    /// </summary>
    [ContextMenu("创建 Arena_16")]
    public void Create_Arena_16()
    {
        GameObject room = CreateBaseRoom("Arena_16", new Vector2Int(16, 16), true);
        
        // 创建四个门洞
        CreateSocket(room, Dir.N, new Vector3(0, 8, 0));
        CreateSocket(room, Dir.E, new Vector3(8, 0, 0));
        CreateSocket(room, Dir.S, new Vector3(0, -8, 0));
        CreateSocket(room, Dir.W, new Vector3(-8, 0, 0));
        
        // 创建战斗间墙体
        CreateArenaWalls(room, 16, 16);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("Arena_16 创建完成！");
    }
    
    /// <summary>
    /// 8. Ring_16 - 环形走道
    /// </summary>
    [ContextMenu("创建 Ring_16")]
    public void Create_Ring_16()
    {
        GameObject room = CreateBaseRoom("Ring_16", new Vector2Int(16, 16), true);
        
        // 创建三个门洞（北、东、南）
        CreateSocket(room, Dir.N, new Vector3(0, 8, 0));
        CreateSocket(room, Dir.E, new Vector3(8, 0, 0));
        CreateSocket(room, Dir.S, new Vector3(0, -8, 0));
        
        // 创建环形墙体（中央8×8岛）
        CreateRingWalls(room, 16, 16, 8);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("Ring_16 创建完成！");
    }
    
    /// <summary>
    /// 9. SplitRoom_DualDoor_16 - 同侧双舱房
    /// </summary>
    [ContextMenu("创建 SplitRoom_DualDoor_16")]
    public void Create_SplitRoom_DualDoor_16()
    {
        GameObject room = CreateBaseRoom("SplitRoom_DualDoor_16", new Vector2Int(16, 16), true);
        
        // 创建南侧双门
        CreateSocket(room, Dir.S, new Vector3(-4, -8, 0), "Socket_S_Left");
        CreateSocket(room, Dir.S, new Vector3(4, -8, 0), "Socket_S_Right");
        
        // 创建分隔墙体
        CreateSplitRoomWalls(room, 16, 16);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("SplitRoom_DualDoor_16 创建完成！");
    }
    
    /// <summary>
    /// 10. SpiritGate_16 - 灵视门房
    /// </summary>
    [ContextMenu("创建 SpiritGate_16")]
    public void Create_SpiritGate_16()
    {
        GameObject room = CreateBaseRoom("SpiritGate_16", new Vector2Int(16, 16), true);
        
        // 创建南门（常规）
        CreateSocket(room, Dir.S, new Vector3(0, -8, 0));
        
        // 创建北门（灵视门，标记为特殊）
        GameObject northSocket = CreateSocket(room, Dir.N, new Vector3(0, 8, 0));
        northSocket.name = "Socket_N_Spirit";
        
        // 创建灵视门房墙体
        CreateSpiritGateWalls(room, 16, 16);
        
        // 创建地面
        CreateFloor(room, 16, 16);
        
        Debug.Log("SpiritGate_16 创建完成！");
    }
    
    // ==================== 辅助方法 ====================
    
    /// <summary>
    /// 创建基础房间
    /// </summary>
    private GameObject CreateBaseRoom(string name, Vector2Int size, bool allowRotate)
    {
        GameObject room = new GameObject(name);
        room.transform.SetParent(transform);
        room.transform.position = Vector3.zero;
        
        RoomBlock roomBlock = room.AddComponent<RoomBlock>();
        
        // 使用反射设置私有字段
        var sizeField = typeof(RoomBlock).GetField("size", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        sizeField?.SetValue(roomBlock, size);
        
        var allowRotateField = typeof(RoomBlock).GetField("allowRotate", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        allowRotateField?.SetValue(roomBlock, allowRotate);
        
        var roomNameField = typeof(RoomBlock).GetField("roomName", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        roomNameField?.SetValue(roomBlock, name);
        
        return room;
    }
    
    /// <summary>
    /// 创建门洞接口
    /// </summary>
    private GameObject CreateSocket(GameObject parent, Dir direction, Vector3 localPos, string customName = null)
    {
        string socketName = customName ?? $"Socket_{direction}";
        GameObject socket = new GameObject(socketName);
        socket.transform.SetParent(parent.transform);
        socket.transform.localPosition = localPos;
        
        if (createVisualMarkers)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Marker";
            marker.transform.SetParent(socket.transform);
            marker.transform.localScale = Vector3.one * 0.5f;
            marker.transform.localPosition = Vector3.zero;
            
            Renderer renderer = marker.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.red;
                renderer.material = mat;
            }
            
            DestroyImmediate(marker.GetComponent<Collider>());
        }
        
        return socket;
    }
    
    /// <summary>
    /// 创建地面
    /// </summary>
    private void CreateFloor(GameObject parent, int width, int height)
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(parent.transform);
        floor.transform.localScale = new Vector3(width, height, 0.1f);
        floor.transform.localPosition = Vector3.zero;
        
        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer != null && floorMaterial != null)
        {
            renderer.material = floorMaterial;
        }
        else if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            renderer.material = mat;
        }
        
        DestroyImmediate(floor.GetComponent<Collider>());
    }
    
    /// <summary>
    /// 创建十字形障碍
    /// </summary>
    private void CreateCrossObstacle(GameObject parent, float size)
    {
        GameObject obstacle = new GameObject("CrossObstacle");
        obstacle.transform.SetParent(parent.transform);
        obstacle.transform.localPosition = Vector3.zero;
        
        // 横向障碍
        CreateWallSegment(obstacle, "Horizontal", new Vector3(0, 0, 0.1f), new Vector3(size, 1f, 0.2f));
        
        // 纵向障碍
        CreateWallSegment(obstacle, "Vertical", new Vector3(0, 0, 0.1f), new Vector3(1f, size, 0.2f));
    }
    
    /// <summary>
    /// 创建U形墙体
    /// </summary>
    private void CreateUShapeWalls(GameObject parent, int width, int height, float thickness)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        
        // 北墙
        CreateWallSegment(walls, "NorthWall", new Vector3(0, halfHeight - thickness / 2f, 0.1f), 
            new Vector3(width - 2, thickness, 0.2f));
        
        // 东墙
        CreateWallSegment(walls, "EastWall", new Vector3(halfWidth - thickness / 2f, 0, 0.1f), 
            new Vector3(thickness, height - 2, 0.2f));
        
        // 西墙
        CreateWallSegment(walls, "WestWall", new Vector3(-halfWidth + thickness / 2f, 0, 0.1f), 
            new Vector3(thickness, height - 2, 0.2f));
    }
    
    /// <summary>
    /// 创建T形墙体
    /// </summary>
    private void CreateTShapeWalls(GameObject parent, int width, int height)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        float halfHeight = height / 2f;
        
        // 南墙（封闭）
        CreateWallSegment(walls, "SouthWall", new Vector3(0, -halfHeight + 0.5f, 0.1f), 
            new Vector3(width - 2, 1f, 0.2f));
    }
    
    /// <summary>
    /// 创建L形墙体
    /// </summary>
    private void CreateLShapeWalls(GameObject parent, int width, int height)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        
        // 南墙
        CreateWallSegment(walls, "SouthWall", new Vector3(0, -halfHeight + 0.5f, 0.1f), 
            new Vector3(width - 2, 1f, 0.2f));
        
        // 西墙
        CreateWallSegment(walls, "WestWall", new Vector3(-halfWidth + 0.5f, 0, 0.1f), 
            new Vector3(1f, height - 2, 0.2f));
    }
    
    /// <summary>
    /// 创建走廊墙体
    /// </summary>
    private void CreateCorridorWalls(GameObject parent, int width, int height)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        float halfHeight = height / 2f;
        
        // 北墙
        CreateWallSegment(walls, "NorthWall", new Vector3(0, halfHeight - 0.5f, 0.1f), 
            new Vector3(width - 2, 1f, 0.2f));
        
        // 南墙
        CreateWallSegment(walls, "SouthWall", new Vector3(0, -halfHeight + 0.5f, 0.1f), 
            new Vector3(width - 2, 1f, 0.2f));
    }
    
    /// <summary>
    /// 创建死路墙体
    /// </summary>
    private void CreateDeadEndWalls(GameObject parent, int width, int height)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        
        // 北墙
        CreateWallSegment(walls, "NorthWall", new Vector3(0, halfHeight - 0.5f, 0.1f), 
            new Vector3(width - 2, 1f, 0.2f));
        
        // 东墙
        CreateWallSegment(walls, "EastWall", new Vector3(halfWidth - 0.5f, 0, 0.1f), 
            new Vector3(1f, height - 2, 0.2f));
        
        // 西墙
        CreateWallSegment(walls, "WestWall", new Vector3(-halfWidth + 0.5f, 0, 0.1f), 
            new Vector3(1f, height - 2, 0.2f));
    }
    
    /// <summary>
    /// 创建战斗间墙体
    /// </summary>
    private void CreateArenaWalls(GameObject parent, int width, int height)
    {
        // 战斗间基本为开放空间，只需要外围墙体
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
    }
    
    /// <summary>
    /// 创建环形墙体
    /// </summary>
    private void CreateRingWalls(GameObject parent, int width, int height, int centerSize)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        // 中央岛
        CreateWallSegment(walls, "CenterIsland", new Vector3(0, 0, 0.1f), 
            new Vector3(centerSize, centerSize, 0.2f));
        
        // 西墙（封闭）
        float halfWidth = width / 2f;
        CreateWallSegment(walls, "WestWall", new Vector3(-halfWidth + 0.5f, 0, 0.1f), 
            new Vector3(1f, height - 2, 0.2f));
    }
    
    /// <summary>
    /// 创建分隔房间墙体
    /// </summary>
    private void CreateSplitRoomWalls(GameObject parent, int width, int height)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        // 中间分隔墙
        CreateWallSegment(walls, "DividerWall", new Vector3(0, 0, 0.1f), 
            new Vector3(1f, height - 4, 0.2f));
    }
    
    /// <summary>
    /// 创建灵视门房墙体
    /// </summary>
    private void CreateSpiritGateWalls(GameObject parent, int width, int height)
    {
        GameObject walls = new GameObject("Walls");
        walls.transform.SetParent(parent.transform);
        
        // 北墙（灵视门位置）- 用特殊颜色标记
        GameObject northWall = CreateWallSegment(walls, "NorthWall_Spirit", 
            new Vector3(0, height / 2f - 0.5f, 0.1f), 
            new Vector3(width - 6, 1f, 0.2f));
        
        Renderer renderer = northWall.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.5f, 0.5f, 1f, 0.5f); // 蓝色半透明
            renderer.material = mat;
        }
    }
    
    /// <summary>
    /// 创建墙体段
    /// </summary>
    private GameObject CreateWallSegment(GameObject parent, string name, Vector3 localPos, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent.transform);
        wall.transform.localPosition = localPos;
        wall.transform.localScale = scale;
        
        Renderer renderer = wall.GetComponent<Renderer>();
        if (renderer != null && wallMaterial != null)
        {
            renderer.material = wallMaterial;
        }
        else if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.3f, 0.3f, 0.3f);
            renderer.material = mat;
        }
        
        DestroyImmediate(wall.GetComponent<Collider>());
        
        return wall;
    }
}
