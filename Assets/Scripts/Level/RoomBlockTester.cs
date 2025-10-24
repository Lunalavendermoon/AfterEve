using UnityEngine;

/// <summary>
/// RoomBlock测试脚本，用于验证功能
/// </summary>
public class RoomBlockTester : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private RoomBlock testRoomBlock;
    [SerializeField] private bool runTestsOnStart = false;
    
    private void Start()
    {
        if (runTestsOnStart)
        {
            RunAllTests();
        }
    }
    
    /// <summary>
    /// 运行所有测试
    /// </summary>
    [ContextMenu("运行所有测试")]
    public void RunAllTests()
    {
        if (testRoomBlock == null)
        {
            testRoomBlock = GetComponent<RoomBlock>();
        }
        
        if (testRoomBlock == null)
        {
            Debug.LogError("未找到RoomBlock组件！");
            return;
        }
        
        Debug.Log("=== RoomBlock 功能测试 ===");
        
        TestBasicProperties();
        TestSocketSystem();
        TestRotation();
        TestBounds();
        TestUtilityMethods();
        
        Debug.Log("=== 测试完成 ===");
    }
    
    /// <summary>
    /// 测试基本属性
    /// </summary>
    private void TestBasicProperties()
    {
        Debug.Log("--- 测试基本属性 ---");
        Debug.Log($"房间名称: {testRoomBlock.RoomName}");
        Debug.Log($"房间尺寸: {testRoomBlock.Size}");
        Debug.Log($"允许旋转: {testRoomBlock.AllowRotate}");
        Debug.Log($"门洞数量: {testRoomBlock.Sockets.Count}");
    }
    
    /// <summary>
    /// 测试门洞系统
    /// </summary>
    private void TestSocketSystem()
    {
        Debug.Log("--- 测试门洞系统 ---");
        
        // 测试每个方向的门洞
        for (int i = 0; i < 4; i++)
        {
            Dir direction = (Dir)i;
            bool hasSocket = testRoomBlock.HasSocket(direction);
            Debug.Log($"方向 {direction}: {(hasSocket ? "有门洞" : "无门洞")}");
            
            if (hasSocket)
            {
                Vector3 worldPos = testRoomBlock.GetSocketWorldPosition(direction);
                Vector3 localPos = testRoomBlock.GetSocketLocalPosition(direction);
                Debug.Log($"  世界位置: {worldPos}");
                Debug.Log($"  局部位置: {localPos}");
            }
        }
        
        // 测试可用门洞
        var availableSockets = testRoomBlock.GetAvailableSockets();
        Debug.Log($"可用门洞: {string.Join(", ", availableSockets)}");
    }
    
    /// <summary>
    /// 测试旋转功能
    /// </summary>
    private void TestRotation()
    {
        Debug.Log("--- 测试旋转功能 ---");
        
        if (testRoomBlock.AllowRotate)
        {
            Vector3 originalRotation = testRoomBlock.transform.eulerAngles;
            Debug.Log($"原始旋转: {originalRotation}");
            
            testRoomBlock.Rotate90();
            Vector3 newRotation = testRoomBlock.transform.eulerAngles;
            Debug.Log($"旋转后: {newRotation}");
            
            // 恢复原始旋转
            testRoomBlock.transform.eulerAngles = originalRotation;
            Debug.Log("已恢复原始旋转");
        }
        else
        {
            Debug.Log("房间块不允许旋转");
        }
    }
    
    /// <summary>
    /// 测试边界功能
    /// </summary>
    private void TestBounds()
    {
        Debug.Log("--- 测试边界功能 ---");
        
        Bounds worldBounds = testRoomBlock.GetWorldBounds();
        Bounds localBounds = testRoomBlock.GetLocalBounds();
        
        Debug.Log($"世界边界: 中心={worldBounds.center}, 大小={worldBounds.size}");
        Debug.Log($"局部边界: 中心={localBounds.center}, 大小={localBounds.size}");
    }
    
    /// <summary>
    /// 测试工具方法
    /// </summary>
    private void TestUtilityMethods()
    {
        Debug.Log("--- 测试工具方法 ---");
        
        // 测试方向向量
        for (int i = 0; i < 4; i++)
        {
            Dir direction = (Dir)i;
            Vector3 directionVector = testRoomBlock.GetDirectionVector(direction);
            Dir oppositeDirection = testRoomBlock.GetOppositeDirection(direction);
            Debug.Log($"方向 {direction}: 向量={directionVector}, 相反方向={oppositeDirection}");
        }
    }
    
    /// <summary>
    /// 创建测试房间块
    /// </summary>
    [ContextMenu("创建测试房间块")]
    public void CreateTestRoomBlock()
    {
        GameObject testRoom = new GameObject("TestRoomBlock");
        testRoom.transform.position = Vector3.zero;
        
        RoomBlock roomBlock = testRoom.AddComponent<RoomBlock>();
        
        // 设置基本属性
        testRoomBlock = roomBlock;
        
        // 自动设置门洞接口
        roomBlock.AutoSetupSockets();
        
        Debug.Log("测试房间块创建完成！");
    }
    
    /// <summary>
    /// 测试门洞连接
    /// </summary>
    [ContextMenu("测试门洞连接")]
    public void TestSocketConnection()
    {
        if (testRoomBlock == null) return;
        
        Debug.Log("--- 测试门洞连接 ---");
        
        var availableSockets = testRoomBlock.GetAvailableSockets();
        if (availableSockets.Count > 0)
        {
            Dir testDirection = availableSockets[0];
            Debug.Log($"测试连接门洞: {testDirection}");
            
            testRoomBlock.MarkSocketConnected(testDirection);
            Debug.Log($"门洞 {testDirection} 已标记为连接");
            
            // 检查连接状态
            var socket = testRoomBlock.GetSocket(testDirection);
            if (socket != null)
            {
                Debug.Log($"连接状态: {socket.isConnected}");
            }
        }
        else
        {
            Debug.Log("没有可用的门洞进行测试");
        }
    }
    
    /// <summary>
    /// 测试重叠检测
    /// </summary>
    [ContextMenu("测试重叠检测")]
    public void TestOverlapDetection()
    {
        if (testRoomBlock == null) return;
        
        Debug.Log("--- 测试重叠检测 ---");
        
        // 创建另一个房间块进行重叠测试
        GameObject otherRoom = new GameObject("OtherRoomBlock");
        otherRoom.transform.position = testRoomBlock.transform.position + Vector3.right * 5f;
        
        RoomBlock otherRoomBlock = otherRoom.AddComponent<RoomBlock>();
        otherRoomBlock.AutoSetupSockets();
        
        bool overlaps = testRoomBlock.OverlapsWith(otherRoomBlock);
        Debug.Log($"与另一个房间块重叠: {overlaps}");
        
        // 移动另一个房间块到重叠位置
        otherRoom.transform.position = testRoomBlock.transform.position + Vector3.right * 2f;
        overlaps = testRoomBlock.OverlapsWith(otherRoomBlock);
        Debug.Log($"移动后重叠: {overlaps}");
        
        // 清理
        DestroyImmediate(otherRoom);
    }
}
