# 2D关卡房间块拼接系统

这是一个为AfterEve项目设计的2D关卡房间块拼接和元素填充系统，支持快速生成可玩的大房间关卡。

## 系统架构

### 核心组件

1. **RoomBlock.cs** - 房间块基础类
   - 存储房间块数据（尺寸、门洞、类型等）
   - 管理门洞连接器
   - 处理房间块旋转和对齐

2. **RoomGenerator.cs** - 房间生成器
   - 随机选择房间块进行拼接
   - 处理门洞连接和旋转对齐
   - 占位检测和重试机制

3. **ElementFiller.cs** - 元素填充器
   - 根据规则在房间中放置游戏元素
   - 支持多种元素类型（炸弹、障碍物、传送器等）
   - 智能位置检测和冲突避免

4. **RoomManager.cs** - 房间管理器
   - 协调整个生成流程
   - 管理房间和元素的生成顺序
   - 提供统计信息和调试功能

5. **LevelConfig.cs** - 关卡配置
   - 存储所有生成参数
   - 支持ScriptableObject配置
   - 便于设计师调整参数

## 使用方法

### 1. 创建房间块Prefab

使用`RoomBlockCreator`组件快速创建房间块：

```csharp
// 在Inspector中设置房间参数
roomName = "U_Shape_Room";
roomType = RoomType.U_Shape;
roomSize = new Vector2Int(8, 6);
hasNorthDoor = true;
hasSouthDoor = true;

// 点击"创建房间块Prefab"按钮
```

### 2. 配置元素规则

在`ElementFiller`中配置元素生成规则：

```csharp
// 炸弹配置示例
ElementConfig bombConfig = new ElementConfig
{
    elementType = ElementType.Bomb,
    spawnChance = 0.5f,
    minCount = 1,
    maxCount = 3,
    avoidCenter = true,
    minDistanceFromWalls = 2f
};
```

### 3. 生成关卡

```csharp
// 获取房间管理器
RoomManager roomManager = FindObjectOfType<RoomManager>();

// 生成完整关卡
roomManager.GenerateCompleteLevel();

// 或者分步生成
List<RoomBlock> rooms = roomManager.GetComponent<RoomGenerator>().GenerateRoomLayout();
List<PlacedElement> elements = roomManager.GetComponent<ElementFiller>().FillElements();
```

## 房间块类型

### 预定义房间类型

- **U_Shape** - U形房间，适合作为起始房间
- **Cross** - 十字房间，连接性好
- **L_Shape** - L形房间，增加复杂度
- **Straight** - 直线房间，简单连接
- **Corner** - 角落房间，特殊布局
- **T_Shape** - T形房间，分支结构
- **Square** - 方形房间，标准布局
- **Special** - 特殊房间，自定义用途

### 门洞系统

每个房间块支持四个方向的门洞：
- **North** - 北门
- **East** - 东门  
- **South** - 南门
- **West** - 西门

门洞连接器自动处理旋转和对齐。

## 元素类型

### 支持的元素类型

- **Bomb** - 炸弹
- **Destructible** - 可破坏物
- **BulletWall** - 子弹可穿障碍
- **Hole** - 洞
- **HotGround** - 热地
- **Teleporter** - 传送器
- **SpiritualVision** - 灵视相关物
- **PowerUp** - 强化道具
- **EnemySpawner** - 敌人生成点
- **Treasure** - 宝藏

### 元素规则配置

```csharp
public class ElementConfig
{
    public ElementType elementType;           // 元素类型
    public float spawnChance;                // 生成概率
    public int minCount, maxCount;           // 数量范围
    public bool avoidCenter;                // 避免中心
    public bool avoidEdges;                // 避免边缘
    public float minDistanceFromWalls;      // 与墙壁最小距离
    public float minDistanceBetweenElements; // 元素间最小距离
    public List<ElementType> incompatibleElements; // 不兼容元素
    public List<ElementType> requiredElements;     // 前置元素
}
```

## 高级功能

### 权重系统

房间块支持权重选择，权重越高的房间块被选中的概率越大：

```csharp
roomData.spawnWeight = 1.5f; // 1.5倍权重
```

### 特殊房间标记

```csharp
roomData.isStartingRoom = true;  // 起始房间
roomData.isEndingRoom = true;    // 结束房间
```

### 调试功能

- **Gizmos显示** - 在Scene视图中显示房间边界和门洞
- **统计信息** - 显示房间和元素的数量统计
- **日志输出** - 详细的生成过程日志

## 扩展指南

### 添加新的房间类型

1. 在`RoomType`枚举中添加新类型
2. 在`RoomBlockCreator`中添加创建方法
3. 更新房间生成逻辑

### 添加新的元素类型

1. 在`ElementType`枚举中添加新类型
2. 创建对应的Prefab
3. 在`ElementFiller`中配置生成规则

### 自定义生成规则

继承`RoomGenerator`或`ElementFiller`类，重写相关方法来实现自定义逻辑。

## 性能优化

- 使用对象池管理频繁创建销毁的对象
- 限制最大重试次数避免无限循环
- 使用空间分割优化碰撞检测
- 缓存常用的计算结果

## 注意事项

1. 确保房间块Prefab正确设置了门洞连接器
2. 元素Prefab需要包含必要的组件（Collider2D等）
3. 合理设置生成参数避免性能问题
4. 使用调试功能验证生成结果

## 示例场景

在Dihao场景中，系统会自动：
1. 生成2-3个随机房间块
2. 将它们拼接成一个大房间
3. 在房间中填充各种游戏元素
4. 提供完整的可玩关卡

这个系统让设计师只需要创建几个房间块Prefab，就能快速生成大量不同的关卡变体。
