using UnityEngine;
using UnityEditor;

/// <summary>
/// 房间类型枚举
/// </summary>
public enum RoomType
{
    Cross,              // 十字房
    U_Shape,            // U形房
    Arena,              // 小型战斗间
    SplitRoom_DualDoor, // 同侧双舱房
    Grid,               // 网格房
    Window,             // 窗户房
    Irregular,          // 不规则房
    SpiritGate,         // 灵视门房
    Corner_L,           // 直角转弯房
    T_Shape,            // T形房
    Square,             // 方形房
    Special             // 特殊房
}

/// <summary>
/// 房间块创建工具，用于快速创建房间块Prefab
/// </summary>
public class RoomBlockCreator : MonoBehaviour
{
    [Header("房间块设置")]
    [SerializeField] private string roomName = "NewRoom";
    [SerializeField] private RoomType roomType = RoomType.Square;
    [SerializeField] private Vector2Int roomSize = new Vector2Int(10, 10);
    
    [Header("门洞设置")]
    [SerializeField] private bool hasNorthDoor = true;
    [SerializeField] private bool hasEastDoor = true;
    [SerializeField] private bool hasSouthDoor = true;
    [SerializeField] private bool hasWestDoor = true;
    
    [Header("特殊设置")]
    [SerializeField] private bool isStartingRoom = false;
    [SerializeField] private bool isEndingRoom = false;
    [SerializeField] private float spawnWeight = 1f;
    
    [Header("几何设置")]
    [SerializeField] private GameObject tilemapPrefab;
    [SerializeField] private GameObject collisionPrefab;
    
    /// <summary>
    /// 创建房间块Prefab
    /// </summary>
    [ContextMenu("创建房间块Prefab")]
    public void CreateRoomBlockPrefab()
    {
        // 创建根对象
        GameObject roomBlock = new GameObject(roomName);
        RoomBlock roomBlockComponent = roomBlock.AddComponent<RoomBlock>();
        
        // 设置房间数据
        roomBlockComponent.roomData = new RoomBlockData
        {
            roomName = roomName,
            size = roomSize,
            roomType = roomType,
            spawnWeight = spawnWeight,
            isStartingRoom = isStartingRoom,
            isEndingRoom = isEndingRoom
        };
        
        // 自动设置门洞接口
        roomBlockComponent.AutoSetupSockets();
        
        // 创建几何结构
        CreateGeometry(roomBlock);
        
        // 门洞连接器已通过AutoSetupSockets自动创建
        
        Debug.Log($"房间块 '{roomName}' 创建完成！");
    }
    
    // AddDoorConnections方法已移除，使用AutoSetupSockets替代
    
    /// <summary>
    /// 创建几何结构
    /// </summary>
    private void CreateGeometry(GameObject roomBlock)
    {
        // 创建Tilemap父对象
        GameObject tilemapParent = new GameObject("Tilemap");
        tilemapParent.transform.SetParent(roomBlock.transform);
        
        // 创建碰撞体父对象
        GameObject collisionParent = new GameObject("Collision");
        collisionParent.transform.SetParent(roomBlock.transform);
        
        // 设置RoomBlock组件引用
        RoomBlock roomBlockComponent = roomBlock.GetComponent<RoomBlock>();
        roomBlockComponent.tilemapParent = tilemapParent.transform;
        roomBlockComponent.collisionParent = collisionParent.transform;
        
        // 创建基础几何
        CreateBasicGeometry(tilemapParent, collisionParent);
    }
    
    /// <summary>
    /// 创建基础几何
    /// </summary>
    private void CreateBasicGeometry(GameObject tilemapParent, GameObject collisionParent)
    {
        // 创建地面
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Quad);
        floor.name = "Floor";
        floor.transform.SetParent(tilemapParent.transform);
        floor.transform.localScale = new Vector3(roomSize.x, roomSize.y, 1);
        floor.transform.localPosition = new Vector3(roomSize.x / 2f - 0.5f, roomSize.y / 2f - 0.5f, 0);
        
        // 创建墙壁
        CreateWalls(collisionParent);
    }
    
    /// <summary>
    /// 创建墙壁
    /// </summary>
    private void CreateWalls(GameObject collisionParent)
    {
        float wallThickness = 0.5f;
        
        // 北墙
        if (!hasNorthDoor)
        {
            CreateWall(collisionParent, "NorthWall", 
                new Vector3(roomSize.x / 2f - 0.5f, roomSize.y - wallThickness / 2f, 0),
                new Vector3(roomSize.x, wallThickness, 1));
        }
        
        // 南墙
        if (!hasSouthDoor)
        {
            CreateWall(collisionParent, "SouthWall",
                new Vector3(roomSize.x / 2f - 0.5f, -wallThickness / 2f, 0),
                new Vector3(roomSize.x, wallThickness, 1));
        }
        
        // 东墙
        if (!hasEastDoor)
        {
            CreateWall(collisionParent, "EastWall",
                new Vector3(roomSize.x - wallThickness / 2f, roomSize.y / 2f - 0.5f, 0),
                new Vector3(wallThickness, roomSize.y, 1));
        }
        
        // 西墙
        if (!hasWestDoor)
        {
            CreateWall(collisionParent, "WestWall",
                new Vector3(-wallThickness / 2f, roomSize.y / 2f - 0.5f, 0),
                new Vector3(wallThickness, roomSize.y, 1));
        }
    }
    
    /// <summary>
    /// 创建单个墙壁
    /// </summary>
    private void CreateWall(GameObject parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent.transform);
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
        
        // 添加碰撞体标签
        wall.tag = "Wall";
    }
    
    // SetupDoorConnectors方法已移除，使用AutoSetupSockets替代
    
    // GetDoorConnectorPosition方法已移除，使用AutoSetupSockets替代
    
    /// <summary>
    /// 预设房间块模板
    /// </summary>
    [ContextMenu("创建U形房间")]
    public void CreateUShapeRoom()
    {
        roomName = "U_Shape_Room";
        roomType = RoomType.U_Shape;
        roomSize = new Vector2Int(8, 6);
        hasNorthDoor = true;
        hasEastDoor = false;
        hasSouthDoor = true;
        hasWestDoor = false;
        CreateRoomBlockPrefab();
    }
    
    [ContextMenu("创建十字房间")]
    public void CreateCrossRoom()
    {
        roomName = "Cross_Room";
        roomType = RoomType.Cross;
        roomSize = new Vector2Int(6, 6);
        hasNorthDoor = true;
        hasEastDoor = true;
        hasSouthDoor = true;
        hasWestDoor = true;
        CreateRoomBlockPrefab();
    }
    
    [ContextMenu("创建L形房间")]
    public void CreateLShapeRoom()
    {
        roomName = "L_Shape_Room";
        roomType = RoomType.Corner_L;
        roomSize = new Vector2Int(6, 8);
        hasNorthDoor = true;
        hasEastDoor = true;
        hasSouthDoor = false;
        hasWestDoor = false;
        CreateRoomBlockPrefab();
    }
}
