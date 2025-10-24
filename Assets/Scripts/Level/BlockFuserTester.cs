using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// BlockFuser测试脚本，用于验证房间块融合功能
/// </summary>
public class BlockFuserTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private BlockFuser blockFuser;
    [SerializeField] private bool runTestsOnStart = false;
    [SerializeField] private bool createTestBlocks = true;
    
    [Header("测试房间块")]
    [SerializeField] private GameObject testBlockPrefab1;
    [SerializeField] private GameObject testBlockPrefab2;
    
    private void Start()
    {
        if (runTestsOnStart)
        {
            SetupTestEnvironment();
            RunAllTests();
        }
    }
    
    /// <summary>
    /// 设置测试环境
    /// </summary>
    [ContextMenu("设置测试环境")]
    public void SetupTestEnvironment()
    {
        // 获取或创建BlockFuser
        if (blockFuser == null)
        {
            blockFuser = GetComponent<BlockFuser>();
            if (blockFuser == null)
            {
                blockFuser = gameObject.AddComponent<BlockFuser>();
            }
        }
        
        // 创建测试房间块
        if (createTestBlocks)
        {
            CreateTestRoomBlocks();
        }
        
        Debug.Log("测试环境设置完成！");
    }
    
    /// <summary>
    /// 创建测试房间块
    /// </summary>
    private void CreateTestRoomBlocks()
    {
        // 创建第一个测试房间块（方形房间）
        testBlockPrefab1 = CreateTestRoomBlock("TestRoom1", new Vector2Int(4, 4), new Dir[] { Dir.N, Dir.E, Dir.S, Dir.W });
        
        // 创建第二个测试房间块（L形房间）
        testBlockPrefab2 = CreateTestRoomBlock("TestRoom2", new Vector2Int(3, 5), new Dir[] { Dir.N, Dir.E });
        
        // 添加到BlockFuser的目录
        var blockCatalog = new List<RoomBlock>();
        blockCatalog.Add(testBlockPrefab1.GetComponent<RoomBlock>());
        blockCatalog.Add(testBlockPrefab2.GetComponent<RoomBlock>());
        
        // 使用反射设置blockCatalog
        var field = typeof(BlockFuser).GetField("blockCatalog", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(blockFuser, blockCatalog);
        
        Debug.Log("测试房间块创建完成！");
    }
    
    /// <summary>
    /// 创建测试房间块
    /// </summary>
    private GameObject CreateTestRoomBlock(string name, Vector2Int size, Dir[] socketDirs)
    {
        // 创建房间块GameObject
        GameObject roomBlock = new GameObject(name);
        roomBlock.transform.SetParent(transform);
        
        // 添加RoomBlock组件
        RoomBlock roomBlockComponent = roomBlock.AddComponent<RoomBlock>();
        
        // 设置基本属性（使用反射设置私有字段）
        var sizeField = typeof(RoomBlock).GetField("size", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        sizeField?.SetValue(roomBlockComponent, size);
        
        // 创建可视化占位符
        CreateVisualPlaceholder(roomBlock, size);
        
        // 创建门洞接口
        CreateSocketConnectors(roomBlock, socketDirs);
        
        return roomBlock;
    }
    
    /// <summary>
    /// 创建可视化占位符
    /// </summary>
    private void CreateVisualPlaceholder(GameObject roomBlock, Vector2Int size)
    {
        // 创建地面
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(roomBlock.transform);
        floor.transform.localScale = new Vector3(size.x, size.y, 0.1f);
        floor.transform.localPosition = Vector3.zero;
        
        // 设置材质颜色
        Renderer renderer = floor.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            renderer.material = material;
        }
        
        // 移除碰撞体（我们使用2D检测）
        Collider collider = floor.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
    }
    
    /// <summary>
    /// 创建门洞接口
    /// </summary>
    private void CreateSocketConnectors(GameObject roomBlock, Dir[] socketDirs)
    {
        RoomBlock roomBlockComponent = roomBlock.GetComponent<RoomBlock>();
        
        foreach (Dir dir in socketDirs)
        {
            // 创建门洞接口子物体
            GameObject socketObj = new GameObject($"Socket_{dir}");
            socketObj.transform.SetParent(roomBlock.transform);
            
            // 设置门洞位置
            Vector3 socketPos = GetDefaultSocketPosition(dir, roomBlockComponent.Size);
            socketObj.transform.localPosition = socketPos;
            
            // 创建可视化标记
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = "Marker";
            marker.transform.SetParent(socketObj.transform);
            marker.transform.localScale = Vector3.one * 0.3f;
            marker.transform.localPosition = Vector3.zero;
            
            // 设置标记颜色
            Renderer markerRenderer = marker.GetComponent<Renderer>();
            if (markerRenderer != null)
            {
                Material markerMaterial = new Material(Shader.Find("Standard"));
                markerMaterial.color = Color.red;
                markerRenderer.material = markerMaterial;
            }
            
            // 移除碰撞体
            Collider markerCollider = marker.GetComponent<Collider>();
            if (markerCollider != null)
            {
                DestroyImmediate(markerCollider);
            }
        }
        
        // 自动设置门洞接口
        roomBlockComponent.AutoSetupSockets();
    }
    
    /// <summary>
    /// 获取默认门洞位置
    /// </summary>
    private Vector3 GetDefaultSocketPosition(Dir direction, Vector2Int size)
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
    /// 运行所有测试
    /// </summary>
    [ContextMenu("运行所有测试")]
    public void RunAllTests()
    {
        if (blockFuser == null)
        {
            Debug.LogError("BlockFuser未找到！");
            return;
        }
        
        Debug.Log("=== BlockFuser 功能测试 ===");
        
        TestBasicGeneration();
        TestMultipleGenerations();
        TestBlockPlacement();
        TestStatistics();
        
        Debug.Log("=== 测试完成 ===");
    }
    
    /// <summary>
    /// 测试基本生成功能
    /// </summary>
    private void TestBasicGeneration()
    {
        Debug.Log("--- 测试基本生成功能 ---");
        
        // 清空现有房间块
        blockFuser.ClearBlocks();
        
        // 生成房间块
        blockFuser.Generate();
        
        // 检查结果
        var placedBlocks = blockFuser.GetPlacedBlocks();
        Debug.Log($"生成房间块数量: {placedBlocks.Count}");
        
        foreach (var block in placedBlocks)
        {
            Debug.Log($"- {block.RoomName} 在位置 {block.transform.position}");
        }
    }
    
    /// <summary>
    /// 测试多次生成
    /// </summary>
    private void TestMultipleGenerations()
    {
        Debug.Log("--- 测试多次生成 ---");
        
        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"第 {i + 1} 次生成:");
            blockFuser.Generate();
            
            var placedBlocks = blockFuser.GetPlacedBlocks();
            Debug.Log($"  生成房间块数量: {placedBlocks.Count}");
        }
    }
    
    /// <summary>
    /// 测试房间块放置
    /// </summary>
    private void TestBlockPlacement()
    {
        Debug.Log("--- 测试房间块放置 ---");
        
        blockFuser.Generate();
        var placedBlocks = blockFuser.GetPlacedBlocks();
        
        // 检查房间块是否重叠
        bool hasOverlap = false;
        for (int i = 0; i < placedBlocks.Count; i++)
        {
            for (int j = i + 1; j < placedBlocks.Count; j++)
            {
                if (placedBlocks[i].OverlapsWith(placedBlocks[j]))
                {
                    hasOverlap = true;
                    Debug.LogError($"房间块 {placedBlocks[i].RoomName} 和 {placedBlocks[j].RoomName} 重叠！");
                }
            }
        }
        
        if (!hasOverlap)
        {
            Debug.Log("✓ 所有房间块都没有重叠");
        }
        
        // 检查房间块是否连接
        bool allConnected = true;
        for (int i = 1; i < placedBlocks.Count; i++)
        {
            bool isConnected = false;
            for (int j = 0; j < i; j++)
            {
                if (AreBlocksConnected(placedBlocks[i], placedBlocks[j]))
                {
                    isConnected = true;
                    break;
                }
            }
            
            if (!isConnected)
            {
                allConnected = false;
                Debug.LogWarning($"房间块 {placedBlocks[i].RoomName} 可能没有连接到其他房间块");
            }
        }
        
        if (allConnected)
        {
            Debug.Log("✓ 所有房间块都已连接");
        }
    }
    
    /// <summary>
    /// 检查两个房间块是否连接
    /// </summary>
    private bool AreBlocksConnected(RoomBlock block1, RoomBlock block2)
    {
        float connectionDistance = 2f; // 连接距离阈值
        return Vector3.Distance(block1.transform.position, block2.transform.position) <= connectionDistance;
    }
    
    /// <summary>
    /// 测试统计信息
    /// </summary>
    private void TestStatistics()
    {
        Debug.Log("--- 测试统计信息 ---");
        
        var stats = blockFuser.GetGenerationStats();
        Debug.Log($"总房间块数: {stats.totalBlocks}");
        Debug.Log($"目标房间块数: {stats.targetBlocks}");
        Debug.Log($"生成尝试次数: {stats.generationAttempts}");
        Debug.Log($"成功率: {stats.successRate:P2}");
    }
    
    /// <summary>
    /// 创建简单测试房间块
    /// </summary>
    [ContextMenu("创建简单测试房间块")]
    public void CreateSimpleTestBlocks()
    {
        // 创建简单的方形房间块
        GameObject simpleBlock1 = CreateSimpleRoomBlock("SimpleBlock1", new Vector2Int(3, 3));
        GameObject simpleBlock2 = CreateSimpleRoomBlock("SimpleBlock2", new Vector2Int(4, 2));
        
        Debug.Log("简单测试房间块创建完成！");
    }
    
    /// <summary>
    /// 创建简单房间块
    /// </summary>
    private GameObject CreateSimpleRoomBlock(string name, Vector2Int size)
    {
        GameObject roomBlock = new GameObject(name);
        roomBlock.transform.SetParent(transform);
        
        // 添加RoomBlock组件
        RoomBlock roomBlockComponent = roomBlock.AddComponent<RoomBlock>();
        
        // 设置尺寸
        var sizeField = typeof(RoomBlock).GetField("size", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        sizeField?.SetValue(roomBlockComponent, size);
        
        // 自动设置门洞接口
        roomBlockComponent.AutoSetupSockets();
        
        // 创建可视化
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        visual.transform.SetParent(roomBlock.transform);
        visual.transform.localScale = new Vector3(size.x, size.y, 0.1f);
        visual.transform.localPosition = Vector3.zero;
        
        // 设置颜色
        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            renderer.material = material;
        }
        
        // 移除碰撞体
        Collider collider = visual.GetComponent<Collider>();
        if (collider != null)
        {
            DestroyImmediate(collider);
        }
        
        return roomBlock;
    }
}
