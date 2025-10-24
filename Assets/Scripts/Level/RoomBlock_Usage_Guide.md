image.png# RoomBlock 使用指南

## 概述

RoomBlock.cs 是2D关卡房间块拼接系统的核心组件，提供了完整的房间块数据结构和门洞接口系统。

## 核心功能

### 1. 数据结构

#### 基本字段
- `Vector2Int size` - 房间块尺寸（宽高，单位=格）
- `bool allowRotate` - 是否允许90°旋转
- `List<Socket> sockets` - 门洞接口列表
- `BoundsInt boundsLocal` - 可选：Tilemap局部包围盒

#### Socket 门洞接口
```csharp
public class Socket
{
    public Dir dir;        // 门洞方向 (N/E/S/W)
    public Transform t;    // 门洞位置
    public bool isConnected; // 连接状态
}
```

#### Dir 方向枚举
```csharp
public enum Dir
{
    N = 0,  // North
    E = 1,  // East  
    S = 2,  // South
    W = 3   // West
}
```

### 2. Inspector 配置

在Inspector中可以配置：
- **房间块基本信息**: 房间名称
- **房间块尺寸**: Vector2Int size (宽高)
- **旋转设置**: bool allowRotate
- **门洞接口**: List<Socket> sockets
- **可选：Tilemap包围盒**: BoundsInt boundsLocal
- **调试设置**: 显示Gizmos、颜色等

### 3. 门洞接口管理

#### 自动设置门洞接口
```csharp
[ContextMenu("自动设置门洞接口")]
public void AutoSetupSockets()
```
- 自动为四个方向创建门洞接口子物体
- 子物体命名格式：Socket_N, Socket_E, Socket_S, Socket_W
- 自动设置默认位置

#### 手动管理门洞接口
```csharp
[ContextMenu("添加门洞接口")]
public void AddSocket()

[ContextMenu("清除门洞接口")]
public void ClearSockets()
```

### 4. 可视化功能

#### OnDrawGizmosSelected (选中时显示)
- 绘制房间块占位框 (绿色线框)
- 绘制门洞接口位置 (红色球体)
- 绘制方向箭头和标记
- 显示房间名称标签

#### OnDrawGizmos (始终显示)
- 绘制简化的房间块边界 (半透明)

### 5. 核心API

#### 门洞接口查询
```csharp
public Socket GetSocket(Dir direction)           // 获取指定方向的门洞
public bool HasSocket(Dir direction)             // 检查是否有门洞
public List<Dir> GetAvailableSockets()           // 获取可用门洞
public void MarkSocketConnected(Dir direction)   // 标记门洞为已连接
```

#### 位置和边界
```csharp
public Bounds GetWorldBounds()                   // 获取世界边界
public Bounds GetLocalBounds()                   // 获取局部边界
public Vector3 GetSocketWorldPosition(Dir dir)   // 获取门洞世界位置
public Vector3 GetSocketLocalPosition(Dir dir)   // 获取门洞局部位置
```

#### 旋转和方向
```csharp
public void Rotate90()                           // 旋转90度
public Vector3 GetDirectionVector(Dir direction) // 获取方向向量
public Dir GetOppositeDirection(Dir direction)   // 获取相反方向
```

#### 碰撞检测
```csharp
public bool OverlapsWith(RoomBlock other, float margin = 0.1f)
```

## 使用步骤

### 步骤1：创建房间块GameObject
1. 在场景中创建空的GameObject
2. 添加RoomBlock组件
3. 设置房间名称和尺寸

### 步骤2：配置门洞接口
1. 点击"自动设置门洞接口"按钮
2. 或者手动添加门洞接口
3. 调整门洞位置（通过移动子物体）

### 步骤3：验证配置
1. 选中房间块GameObject
2. 在Scene视图中查看Gizmos
3. 使用RoomBlockTester进行功能测试

### 步骤4：保存为Prefab
1. 将配置好的房间块拖拽到Project窗口
2. 保存为Prefab供系统使用

## 示例用法

### 创建U形房间
```csharp
// 1. 创建GameObject并添加RoomBlock组件
GameObject uRoom = new GameObject("U_Shape_Room");
RoomBlock roomBlock = uRoom.AddComponent<RoomBlock>();

// 2. 设置基本属性
roomBlock.transform.position = Vector3.zero;

// 3. 自动设置门洞接口
roomBlock.AutoSetupSockets();

// 4. 移除不需要的门洞（U形房间只需要北门和南门）
// 通过Inspector或代码移除东门和西门
```

### 检查门洞连接
```csharp
// 检查北门是否存在
if (roomBlock.HasSocket(Dir.N))
{
    Vector3 northDoorPos = roomBlock.GetSocketWorldPosition(Dir.N);
    Debug.Log($"北门位置: {northDoorPos}");
}

// 获取所有可用门洞
var availableSockets = roomBlock.GetAvailableSockets();
foreach (Dir direction in availableSockets)
{
    Debug.Log($"可用门洞: {direction}");
}
```

### 房间块旋转
```csharp
// 旋转房间块90度
if (roomBlock.AllowRotate)
{
    roomBlock.Rotate90();
    Debug.Log("房间块已旋转90度");
}
```

## 调试和测试

### 使用RoomBlockTester
1. 添加RoomBlockTester组件到房间块
2. 点击"运行所有测试"按钮
3. 查看控制台输出的测试结果

### 可视化调试
- 选中房间块查看详细Gizmos
- 调整Gizmos颜色和显示设置
- 使用Scene视图的Gizmos开关

## 注意事项

1. **门洞接口子物体命名**: 必须按照Socket_N, Socket_E, Socket_S, Socket_W的格式命名
2. **尺寸单位**: size字段使用网格单位，确保与游戏网格对齐
3. **旋转限制**: 如果allowRotate为false，房间块不能旋转
4. **门洞位置**: 门洞接口的Transform位置决定了连接点
5. **性能考虑**: 大量房间块时注意Gizmos的性能影响

## 扩展功能

### 自定义门洞类型
可以扩展Socket类添加更多属性：
```csharp
public class Socket
{
    public Dir dir;
    public Transform t;
    public bool isConnected;
    public DoorType doorType;  // 新增：门洞类型
    public int priority;       // 新增：优先级
}
```

### 特殊房间标记
可以添加特殊房间的标记：
```csharp
public bool isStartingRoom = false;
public bool isEndingRoom = false;
public float spawnWeight = 1f;
```

这个RoomBlock系统为2D关卡房间块拼接提供了完整的基础设施，支持灵活的门洞接口管理和可视化调试。
