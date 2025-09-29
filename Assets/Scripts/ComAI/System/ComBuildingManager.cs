using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComBuildingManager : MonoBehaviour
{
    public static ComBuildingManager instance;
    //List<Building> allBuildings = new List<Building>();
    public List<Building> allBuildings = new List<Building>();
    public List<Building> allBuildings_BeforeUpdate = new List<Building>();
    public List<Building> allComBuildings = new List<Building>();
    public List<Building> allComBasicAttackerBuildings = new List<Building>();
    public List<Resource> allResources = new List<Resource>();
    public List<Building> allPlayerBuildings = new List<Building>();
    public Building[] buildingPrefabs = default;
    public int[] currentResources = default;
    public int Index_Field = 1;

    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;
    public BuildingUI ui;

    [SerializeField] public GameObject AllBuildingsGO = default;
    // [SerializeField] public GameObject PlayerBuildingGO = default;
    [SerializeField] public GameObject MapSetting = default;
    private void Awake()
    {
        instance = this;
    }

    #region 资源设置（暂时）
    private void Start()
    {
        // 资源开始的设置！
        // currentResources = new int[] { 999, 900, 999, 999, 999, 999};
        ComResourceManager.Instance.InitResources();
        currentResources = ComResourceManager.Instance.Resources;

        foreach (Resource resource in MapSetting.GetComponentsInChildren<Resource>())
        {
            allResources.Add(resource);
        }

        foreach (Building building in AllBuildingsGO.GetComponentsInChildren<Building>())
        {
            allBuildings.Add(building);
            //资源生产型单位列表
            if (building.CompareTag(ComActorManager.instance.PlayerStr_Get))
            {
                allComBuildings.Add(building);
            }
            if (building.GetComponent<BuildingController_UnitsProducting>() != null && building.CompareTag(ComActorManager.instance.PlayerStr_Get))
            {
                allComBasicAttackerBuildings.Add(building);
            }
            if (building.CompareTag(ComActorManager.instance.EnemyStr_Get))
            {
                allPlayerBuildings.Add(building);
            }
        }
    }
    #endregion
    private void Update()
    {
        //暂时
        currentResources = ComResourceManager.Instance.Resources;

        foreach (Actor actor in ComActorManager.instance.selectedActors)
        {
            if (actor is Builder)
            {
                Builder builder = actor as Builder;
                Debug.DrawLine(builder.transform.position, builder.hitInfoPoint, Color.green);
                Debug.DrawLine(builder.hitInfoPoint, builder.hitInfoPoint + Vector3.up, Color.red);
                //Debug.DrawRay(builder.transform.position, builder.hitInfoPoint * 1000, Color.blue);
                //OnDrawGizmos();
            }
        }

        //  更新列表啊！
        RomveUpdateAll();
    }

    public void SpawnBuilding(int index, Vector3 position)
    {
        // 预先拿取一个对象自带的预制体，使用预制体内的方法检查能不能建设
        Building building = buildingPrefabs[index];
        if (!ResourceManager.Instance.CanAfford(building.resourceCost))
        {
            return;
        }
        /*         
                if (!building.CanBuild(currentResources))
                {
                    return;
                } 
        */

        // Create Building,重新new一个（此处做法）或从对象池里拿取一个（此处没写）
        // building = Instantiate(buildingPrefabs[index], position, Quaternion.identity);
        building = Instantiate(buildingPrefabs[index], position, Quaternion.identity, AllBuildingsGO.transform);
        building.IsChangeLayerAndTag(ComActorManager.instance.PlayerStr_Get);
        allBuildings.Add(building);
        allComBuildings.Add(building);

        Debug.Log("在fieldID赋值的if语句外!");
        //资源生产建筑的初始化,资源生产脚本的初始化
        if (building.RPFunc)
        {
            Debug.Log("在fieldID赋值的if语句里的执行前!");
            building.RPFunc.fieldID = Index_Field;
            Index_Field++;
            Debug.Log("在fieldID赋值的if语句结束了!");
        }
        /*
        StartCoroutine(BuildFor5Seconds());

        IEnumerator BuildFor5Seconds()
        {
            while (!building.IsFinished())
            {
                yield return new WaitForSeconds(1);
                if (!building.IsFinished())
                {
                    animator.SetTrigger("Attack");
                    building.Build(10);
                }
            }
        }
        */

        //设置一个监听器，监听是否RemoveBuilding(building)
        //!!!!!!!!!!!!!!!
        building.attackable.onDestroy.AddListener(() => RemoveBuilding(building));


        // Give builders build task
        foreach (Actor actor in ComActorManager.instance.selectedActors)
        {
            if (actor is Builder)
            {
                Builder builder = actor as Builder;
                if (!builder.HasTask())
                {
                    //builder.CheckJobPosition(building);
                    //builder.CheckJobPositionWithBoxCollider(building);
                    //builder.CheckJobPositionBeforeMoveToBoxCollider(building);
                    builder.GiveJob(building);
                }
            }
        }

        // Subtract resources
        int[] cost = building.Cost();
        /*检测
        Debug.Log("cost.Length:"+cost.Length);
        */

        ResourceManager.Instance.ConsumeAll(cost);
        UIRefresh();

        /*
        for (int i = 0; i < cost.Length; i++)
        {
            currentResources[i] -= cost[i];
            if (ui)
            {
                ui.RefreshResources();
            }
        }
        */
        //
    }

    public List<Building> GetBuildings()
    {
        return allBuildings;
    }
    public Building GetPrefab(int index)
    {
        return buildingPrefabs[index];
    }

    public Building GetRandomBuilding()
    {
        if (allBuildings.Count > 0)
        {
            return allBuildings[Random.Range(0, allBuildings.Count)];
        }
        else
        {
            return null;
        }
    }
    public void RemoveBuilding(Building building)
    {
        allBuildings.Remove(building);
    }
    public void AddResource(ResourceType resourceType, int amount)
    {
        currentResources[(int)resourceType] += amount;

        UIRefresh();
    }
    public void PlayParticle(Vector3 position)
    {
        if (buildParticle)
        {
            buildParticle.transform.position = position;
            buildParticle.Play();
        }
    }

    public void UIRefresh()
    {
        if (ui)
        {
            ui.RefreshResources();
        }
    }

    #region 清理队列用的常规方法
    public void RomveUpdateAllBuildings()
    {
        for (int i = 0; i < allBuildings.Count; i++)
        {
            if (allBuildings[i] == null)
            {
                allBuildings.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateAllComBuildings()
    {
        for (int i = 0; i < allComBuildings.Count; i++)
        {
            if (allComBuildings[i] == null)
            {
                allComBuildings.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateAllComBasicAttackerBuildings()
    {
        for (int i = 0; i < allComBasicAttackerBuildings.Count; i++)
        {
            if (allComBasicAttackerBuildings[i] == null)
            {
                allComBasicAttackerBuildings.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateAllResources()
    {
        for (int i = 0; i < allResources.Count; i++)
        {
            if (allResources[i] == null)
            {
                allResources.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateAllPlayerBuildings()
    {
        for (int i = 0; i < allPlayerBuildings.Count; i++)
        {
            if (allPlayerBuildings[i] == null)
            {
                allPlayerBuildings.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateAll()
    {
        RomveUpdateAllBuildings();
        RomveUpdateAllComBuildings();
        RomveUpdateAllComBasicAttackerBuildings();
        RomveUpdateAllResources();
        RomveUpdateAllPlayerBuildings();
    }
    #endregion

    /*    
    public void OnDrawGizmos()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.localPosition, hitInfoPoint);
        Gizmos.DrawCube(hitInfoPoint, new Vector3(0.1f, 0.1f, 0.1f));
    } 
    */

}
