using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComResourceManager : Singleton<ComResourceManager>
{
    private ComResourceManager() { }

    #region 资源变量声明 
    //资源声明(Food, Wood, Steel, Oil, Gold, Manpower)
    public int Food { get; private set; }
    public int Wood { get; private set; }
    public int Steel { get; private set; }
    public int Oil { get; private set; }
    public int Gold { get; private set; }
    public int Manpower { get; private set; }

    private int[] resources = default;

    public int[] Resources
    {
        get
        {
            return resources;
        }
    }

    #endregion

    #region 资源初始化和更新
    public void InitResources()
    {
        /*
        Food = 10;
        Wood = 10;
        Steel = 0;
        Oil = 0;
        Gold = 0;
        Manpower = 10;
        */
        //*
        Food = 0;
        Wood = 0;
        Steel = 0;
        Oil = 0;
        Gold = 0;
        Manpower = 0;
        //*/
        resources = new int[] { Food, Wood, Steel, Oil, Gold, Manpower };
    }

    public int[] GiveResources()
    {
        return resources;
    }
    #endregion

    #region 根据类型单个添加
    public void ConsumeAllResource(int[] resourceCost)
    {
        for (int i = 0; i < resourceCost.Length; i++)
        {
            if (resourceCost[i] > 0)
            {
                resources[i] -= resourceCost[i];
            }
        }
    }

    public void AddResource(ResourceType resourceType, int amount)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            // Debug.Log("(int)resourceType:"+ (int)resourceType);
            // Debug.Log("resources["+ i + "]:"+ resources[i]);
            if (i == (int)resourceType)
            {
                resources[i] += amount;
                // Debug.Log("New的resources["+ i + "]:"+ resources[i]);
            }
        }
    }
    #endregion

    #region 业务逻辑方法

    public bool CanAfford(int[] resourceCost)
    {
        /*检测
        Debug.Log("resourceCost.Length:"+resourceCost.Length);
        */
        for (int i = 0; i < resourceCost.Length; i++)
        {
            if (resources[i] < resourceCost[i])
            {
                return false;
            }
        }
        return true;
    }

    public void ConsumeAll(int[] resourceCost)
    {
        ConsumeAllResource(resourceCost);
    }
    #endregion
}
