using UnityEngine;

// 1. 定义一个最简单的接口和类进行测试
public interface ITestState { }
public class TestSimpleState : ITestState
{
    public TestSimpleState()
    {
        Debug.Log("TestSimpleState 构造函数被调用！");
    }
}

public class IsolationTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 开始隔离测试 ===");
        
        // 测试1：直接实例化一个最简单的类
        var simpleObj = new TestSimpleState();
        Debug.Log($"简单类实例是否为 null: {simpleObj == null}");
        
        // 测试2：实例化您项目中的 State_Idle
        RTS_Game.Units.State_Idle yourState = null;
        try
        {
            yourState = new RTS_Game.Units.State_Idle();
            Debug.Log($"State_Idle 实例是否为 null: {yourState == null}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"创建 State_Idle 失败: {e.Message}");
        }
        
        Debug.Log("=== 隔离测试结束 ===");
    }
}