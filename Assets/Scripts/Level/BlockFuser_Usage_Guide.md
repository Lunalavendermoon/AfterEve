# BlockFuser 使用指南

## 概述

BlockFuser.cs 是房间块融合生成器的核心组件，负责随机拼接2-3个房间块，实现"门洞-门洞"对齐和占位检测。

## 核心功能

### 1. 基本配置

#### Inspector字段
- **blockCatalog**: 房间块Prefab列表
- **minBlocks**: 最小房间块数量 (默认2)
- **maxBlocks**: 最大房间块数量 (默认3)
- **cellSize**: 网格大小 (默认1f，1单位=1格)
- **occupyMask**: 2D占位检测的LayerMask
- **generateOnStart**: 是否在Start()时自动生成
- **maxRetryAttempts**: 最大重试次数 (默认10)

#### 调试设置
- **showDebugInfo**: 显示调试信息
- **showGizmos**: 显示可视化Gizmos
- **placedBlockColor**: 已放置房间块颜色
- **failedBlockColor**: 失败房间块颜色

### 2. 生成流程

#### 第一步：清空旧内容
```csharp
ClearExistingBlocks();
```

#### 第二步：随机确定块数量
```csharp
int targetBlockCount = Random.Range(minBlocks, maxBlocks + 1);
```

#### 第三步：放置第一个房间块
- 从blockCatalog随机选择
- 放置在原点(0,0)
- 添加到已放置列表

#### 第四步：逐个放置其他房间块
- 随机选择宿主块和门洞
- 随机选择候选块和门洞
- 调整候选块旋转使门洞朝向相反
- 对齐门洞位置
- 进行占位检测
- 成功则加入已放置列表

### 3. 核心算法

#### 门洞对齐算法
```csharp
// 1. 获取宿主门洞位置
Vector3 hostSocketPos = hostBlock.GetSocketWorldPosition(hostSocket.dir);

// 2. 获取候选门洞位置
Vector3 candidateSocketPos = candidateBlock.GetSocketWorldPosition(targetDirection);

// 3. 计算偏移量
Vector3 offset = hostSocketPos - candidateSocketPos;

// 4. 网格对齐
Vector3 alignedPosition = AlignToGrid(offset, cellSize);
candidateBlock.transform.position = alignedPosition;
```

#### 旋转调整算法
```csharp
// 计算需要的旋转次数
int rotationSteps = ((int)targetDirection - (int)currentSocketDir + 4) % 4;

// 执行旋转
for (int i = 0; i < rotationSteps; i++)
{
    block.Rotate90();
}
```

#### 占位检测算法
```csharp
// 使用2D物理检测
Vector2 center = block.transform.position;
Vector2 size = new Vector2(block.Size.x * cellSize, block.Size.y * cellSize);
Collider2D[] overlaps = Physics2D.OverlapBoxAll(center, size, 0f, occupyMask);

// 检查与已放置房间块的重叠
foreach (var placedBlock in placedBlocks)
{
    if (block.OverlapsWith(placedBlock, overlapMargin))
    {
        return true; // 重叠
    }
}
```

### 4. 事件系统

#### 生成完成事件
```csharp
public System.Action<List<RoomBlock>> OnGenerationComplete;
```

#### 房间块放置事件
```csharp
public System.Action<RoomBlock> OnBlockPlaced;
```

#### 生成失败事件
```csharp
public System.Action<string> OnGenerationFailed;
```

### 5. 公共API

#### 生成控制
```csharp
[ContextMenu("生成房间块")]
public void Generate()

[ContextMenu("清空房间块")]
public void ClearBlocks()

[ContextMenu("重新生成")]
public void Regenerate()
```

#### 数据查询
```csharp
public List<RoomBlock> GetPlacedBlocks()
public GenerationStats GetGenerationStats()
```

## 使用步骤

### 步骤1：设置BlockFuser
1. 在场景中创建空GameObject
2. 添加BlockFuser组件
3. 配置基本参数

### 步骤2：准备房间块Prefab
1. 创建房间块GameObject
2. 添加RoomBlock组件
3. 设置size和门洞接口
4. 保存为Prefab

### 步骤3：配置房间块目录
1. 将房间块Prefab拖拽到blockCatalog列表
2. 设置minBlocks和maxBlocks
3. 配置cellSize和occupyMask

### 步骤4：生成测试
1. 点击"生成房间块"按钮
2. 查看Scene视图中的结果
3. 使用BlockFuserTester进行功能测试

## 示例用法

### 基本生成
```csharp
// 获取BlockFuser组件
BlockFuser fuser = FindObjectOfType<BlockFuser>();

// 生成房间块
fuser.Generate();

// 获取结果
List<RoomBlock> placedBlocks = fuser.GetPlacedBlocks();
Debug.Log($"生成了 {placedBlocks.Count} 个房间块");
```

### 事件监听
```csharp
// 监听生成完成事件
fuser.OnGenerationComplete += (blocks) => {
    Debug.Log($"生成完成，共 {blocks.Count} 个房间块");
};

// 监听房间块放置事件
fuser.OnBlockPlaced += (block) => {
    Debug.Log($"放置房间块: {block.RoomName}");
};

// 监听生成失败事件
fuser.OnGenerationFailed += (message) => {
    Debug.LogError($"生成失败: {message}");
};
```

### 统计信息
```csharp
GenerationStats stats = fuser.GetGenerationStats();
Debug.Log($"总房间块数: {stats.totalBlocks}");
Debug.Log($"目标房间块数: {stats.targetBlocks}");
Debug.Log($"成功率: {stats.successRate:P2}");
```

## 调试功能

### 可视化调试
- **已放置房间块**: 绿色线框显示
- **占用网格单元**: 灰色半透明方块
- **门洞接口**: 红色球体标记

### 调试信息
- 生成过程的详细日志
- 房间块放置状态
- 重叠检测结果
- 重试次数统计

### 测试工具
使用BlockFuserTester进行功能测试：
```csharp
[ContextMenu("运行所有测试")]
public void RunAllTests()

[ContextMenu("设置测试环境")]
public void SetupTestEnvironment()
```

## 注意事项

### 1. 房间块要求
- 必须包含RoomBlock组件
- 必须正确设置size和门洞接口
- 门洞接口子物体命名格式：Socket_N, Socket_E, Socket_S, Socket_W

### 2. 占位检测
- 使用2D物理检测，确保房间块有Collider2D
- 设置合适的occupyMask避免误检测
- 调整overlapMargin参数控制重叠检测精度

### 3. 性能优化
- 限制maxRetryAttempts避免无限重试
- 合理设置cellSize平衡精度和性能
- 使用对象池管理频繁创建销毁的对象

### 4. 网格对齐
- cellSize必须与游戏网格大小一致
- 房间块尺寸必须是cellSize的整数倍
- 门洞位置必须按网格对齐

## 扩展功能

### 自定义生成规则
可以继承BlockFuser类重写生成逻辑：
```csharp
public class CustomBlockFuser : BlockFuser
{
    protected override bool PlaceNextBlock()
    {
        // 自定义放置逻辑
        return base.PlaceNextBlock();
    }
}
```

### 权重系统
可以为房间块添加权重：
```csharp
[System.Serializable]
public class WeightedRoomBlock
{
    public RoomBlock block;
    public float weight;
}
```

### 特殊房间标记
支持起始房间和结束房间：
```csharp
public bool isStartingRoom = false;
public bool isEndingRoom = false;
```

这个BlockFuser系统提供了完整的房间块融合生成功能，支持灵活的参数配置和强大的调试工具，是2D关卡生成系统的核心组件。
