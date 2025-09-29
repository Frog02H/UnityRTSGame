using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private ResourceManager() { }

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

        Food = 199;
        Wood = 199;
        Steel = 199;
        Oil = 199;
        Gold = 199;
        Manpower = 199;

        resources = new int[] { Food, Wood, Steel, Oil, Gold, Manpower };
    }

    public int[] GiveResources()
    {
        return resources;
    }

    /*
        public void RefrashResources(int[] currentResource)
        {
            for (int i = 0; i < resources.Length; i++)
            {
                if(resources[i] != currentResource[i])
                {
                    resources[i] = currentResource[i];
                }
            }
        }

        public void RefrashResources(string resourceType, int amount)
        {
            for (int i = 0; i < resources.Length; i++)
            {
                switch (resourceType)
                {
                    case "Food":
                        if (Food != amount)
                        {
                            Food -= amount;
                        }
                        break;
                    case "Wood":
                        if (Wood >= amount)
                        {
                            Wood -= amount;
                        }
                        break;
                    case "Steel":
                        if (Steel >= amount)
                        {
                            Steel -= amount;
                        }
                        break;
                    case "Oil":
                        if (Oil >= amount)
                        {
                            Oil -= amount;
                        }
                        break;
                    case "Gold":
                        if (Gold >= amount)
                        {
                            Gold -= amount;
                        }
                        break;
                    case "Manpower":
                        if (Manpower >= amount)
                        {
                            Manpower -= amount;
                        }
                        break;
                    default:
                        Debug.Log("你在调用一个不存在的资源！");
                        break;
                }
            }
        }
    */
    #endregion

    #region 根据类型单个添加
    /*
    public void AddResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                Food += amount;
                break;
            case ResourceType.Wood:
                Wood += amount;
                break;
            case ResourceType.Steel:
                Steel += amount;
                break;
            case ResourceType.Oil:
                Oil += amount;
                break;
            case ResourceType.Gold:
                Gold += amount;
                break;
            case ResourceType.Manpower:
                Manpower += amount;
                break;
            default:
                Debug.Log("你在添加一个不存在的资源！");
                break;
        }
    }

    public bool ConsumeResource(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                if (Food >= amount)
                {
                    Food -= amount;
                    return true;
                }
                break;
            case ResourceType.Wood:
                if (Wood >= amount)
                {
                    Wood -= amount;
                    return true;
                }
                break;
            case ResourceType.Steel:
                if (Steel >= amount)
                {
                    Steel -= amount;
                    return true;
                }
                break;
            case ResourceType.Oil:
                if (Oil >= amount)
                {
                    Oil -= amount;
                    return true;
                }
                break;
            case ResourceType.Gold:
                if (Gold >= amount)
                {
                    Gold -= amount;
                    return true;
                }
                break;
            case ResourceType.Manpower:
                if (Manpower >= amount)
                {
                    Manpower -= amount;
                    return true;
                }
                break;
            default:
                Debug.Log("你在调用一个不存在的资源！");
                break;
        }
        return false;
    }
    */
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
            Debug.Log("(int)resourceType:"+ (int)resourceType);
            Debug.Log("resources["+ i + "]:"+ resources[i]);
            if (i == (int)resourceType)
            {
                resources[i] += amount;
                Debug.Log("New的resources["+ i + "]:"+ resources[i]);
            }
        }
    }

    public void ReturnResource(int[] resourceCost)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            resources[i] += resourceCost[i];
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

    /*
    public void Consume(int[] resourceCost)
    {
        for (int i = 0; i < resourceCost.Length; i++)
        {
            ResourceManager.Instance.ConsumeResource((ResourceType)i, resourceCost[i]);
        }

        for (int i = 0; i < resources.Length; i++)
        {
            switch ((ResourceType)i)
            {
                case ResourceType.Food:
                    resources[i] = Food;
                    break;
                case ResourceType.Wood:
                    resources[i] = Wood;
                    break;
                case ResourceType.Steel:
                    resources[i] = Steel;
                    break;
                case ResourceType.Oil:
                    resources[i] = Oil;
                    break;
                case ResourceType.Gold:
                    resources[i] = Gold;
                    break;
                case ResourceType.Manpower:
                    resources[i] = Manpower;
                    break;
                default:
                    Debug.Log("你在去除一个不存在的资源！");
                    break;
            }
        }
    }
    */
    #endregion
}
