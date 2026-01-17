using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不继承MonoBehaviour的泛型单例基类
/// </summary>
public class Singleton<T> where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            // 保证对象的唯一性
            if (instance == null)
            {
                instance = Activator.CreateInstance(typeof(T), true) as T; // 使用反射创建实例
            }
            return instance;
        }
    }

    // 私有构造函数，防止外部实例化
    protected Singleton() { }
}

