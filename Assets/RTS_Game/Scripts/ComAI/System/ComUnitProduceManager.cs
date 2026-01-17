using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ComUnitProduceManager : MonoBehaviour
// : Singleton<UnitProduceManager>
{
    /*
        // Update is called once per frame
        void Update()
        {

        } 
    */
    public static ComUnitProduceManager instance;

    public List<UnitData> unitDataList = default;
    //List<Building> ProductorBuildings = new List<Building>();
    public Building ProductorBuilding;
    public List<UnitData> producibleUnits;  // 可生产单位列表

    public int[] currentResources = default;

    private ComUnitProduceManager() { }

    private void Awake()
    {
        // 单例冲突处理
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            // 如果需要跨场景保留，取消注释下一行
            // DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentResources = ComResourceManager.Instance.Resources;
    }

    void Update()
    {
        //暂时
        currentResources = ComResourceManager.Instance.Resources;
    }

    public void SpawnUnit(GameObject prefab, Vector3 position)
    {
        // Create Building,重新new一个（此处做法）或从对象池里拿取一个（此处没写）
        // prefab = Instantiate(prefab, position, Quaternion.identity);
        prefab = Instantiate(prefab, position, Quaternion.identity, ComActorManager.instance.AllActors_Get.transform);
        ComActorManager.instance.allActors.Add(prefab.GetComponent<Actor>());
        //暂时使用,之后删除!
        prefab.GetComponent<Builder>().ChangeUnitNavMeshSettingsFF();
    }

    public void SpawnUnit(GameObject prefab, Vector3 position, BuildingController_UnitsProducting BC_UP)
    {
        // Create Building,重新new一个（此处做法）或从对象池里拿取一个（此处没写）
        // prefab = Instantiate(prefab, position, Quaternion.identity);
        prefab = Instantiate(prefab, position, Quaternion.identity, ComActorManager.instance.AllActors_Get.transform);
        ComActorManager.instance.allActors.Add(prefab.GetComponent<Actor>());
        
        BC_UP.IsChangeLayerAndTag(BC_UP.gameObject.tag, BC_UP.gameObject.tag, prefab);

        //暂时使用,之后删除!
        // prefab.GetComponent<Builder>().ChangeUnitNavMeshSettingsFF();

        UnitGetOut(prefab, position);
    }

    public void UnitGetOut(GameObject prefab, Vector3 position)
    {
        //暂时使用,之后删除!
        prefab.transform.DOMove(position, 3f);
    }

    public void CatchUnitsProductingBuilding(Transform currentBuilding)
    {
        Building building = currentBuilding.GetComponent<Building>();
        //ProductorBuildings.Add(building);
    }

    public void GetCollider_UnitsProducting(Transform currentBuilding)
    {
        //
    }

    // 点击地图上的建筑拿取
    public void SetCurrentBuilding(Transform currentBuilding)
    {
        ProductorBuilding = currentBuilding.GetComponent<Building>();
        ProductorBuilding.isSelected = true;
    }

    public void SetCurrentBuilding(Collider collider)
    {
        ProductorBuilding = collider.GetComponent<Building>();
    }

    // 点击UId对应的按钮开启生产任务
    public void AiProduct(UnitData unitData)
    {
        Debug.Log("AiProduct方法执行，生产单位尝试添加至队列！");
        ProductorBuilding.GetComponent<BuildingController_UnitsProducting>().AddToQueue(unitData);
    }
    
    public void AiBuilderTestProduct()
    {
        if (ComResourceManager.Instance.CanAfford(unitDataList[0].costs))
        {
            Debug.Log("AiBuilderTestProduct方法执行，生产单位尝试添加至队列！");
            ProductorBuilding.GetComponent<BuildingController_UnitsProducting>().AddToQueue(unitDataList[0]);
        }
    }

    public void AiAttackerTestProduct()
    {
        if (ComResourceManager.Instance.CanAfford(unitDataList[1].costs))
        {
            Debug.Log("AiAttackerTestProduct方法执行，生产单位尝试添加至队列！");
            ProductorBuilding.GetComponent<BuildingController_UnitsProducting>().AddToQueue(unitDataList[1]);
        }
    }
}
