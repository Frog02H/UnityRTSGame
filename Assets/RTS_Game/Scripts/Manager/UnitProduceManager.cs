using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UnitProduceManager : MonoBehaviour
// : Singleton<UnitProduceManager>
{
    /*
        // Update is called once per frame
        void Update()
        {

        } 
    */
    public static UnitProduceManager instance;

    public List<UnitData> unitDataList = default;
    //List<Building> ProductorBuildings = new List<Building>();
    public Building ProductorBuilding;

    public GameObject go;

    public UnitProductionUI ui;
    public tempUIManager tempUIManager;

    private UnitProduceManager() { }

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
        InitUI();

        if (ui && ProductorBuilding)
        {
            ui.RefreshResources();
        }
    }

    public void SpawnUnit(GameObject prefab, Vector3 position)
    {
        // Create Building,重新new一个（此处做法）或从对象池里拿取一个（此处没写）
        // prefab = Instantiate(prefab, position, Quaternion.identity);
        prefab = Instantiate(prefab, position, Quaternion.identity, ActorManager.instance.AllActors_Get.transform);
        ActorManager.instance.allActors.Add(prefab.GetComponent<Actor>());

        //暂时使用,之后删除!
        prefab.GetComponent<Builder>().ChangeUnitNavMeshSettingsFF();

        UnitGetOut(prefab, position);
    }

    public void SpawnUnit(GameObject prefab, Vector3 position, BuildingController_UnitsProducting BC_UP)
    {
        // Create Building,重新new一个（此处做法）或从对象池里拿取一个（此处没写）
        // prefab = Instantiate(prefab, position, Quaternion.identity);
        prefab = Instantiate(prefab, position, Quaternion.identity, ActorManager.instance.AllActors_Get.transform);
        ActorManager.instance.allActors.Add(prefab.GetComponent<Actor>());

        BC_UP.IsChangeLayerAndTag(BC_UP.gameObject.tag, BC_UP.gameObject.tag, prefab);

        //暂时使用,之后删除!
        prefab.GetComponent<Builder>().ChangeUnitNavMeshSettingsFF();

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

    public void SetCurrentBuilding(GameObject GO)
    {
        go = GO;
        ProductorBuilding = go.GetComponent<Building>();
    }

    // 点击UId对应的按钮开启生产任务
    public void ClickToProduct(UnitData unitData)
    {
        Debug.Log("ClickToProduct方法执行，生产单位已经添加至队列！");
        ProductorBuilding.GetComponent<BuildingController_UnitsProducting>().AddToQueue(unitData);

        if (ui)
        {
            ui.RefreshResources();
        }
    }

    public void InitUI()
    {
        //InvisionableUI();
        Debug.Log("ui" + ui);
        if (!ui)
        {
            //GameObject的初始Active是false啊！
            // ui = FindObjectOfType<UnitProductionUI>();
            tempUIManager = FindObjectOfType<tempUIManager>();
            ui = tempUIManager.UnitGroup.GetComponent<UnitProductionUI>();
            Debug.Log("ui 为 null, 故而取值 !");
        }
        else
        {
            Debug.Log("执行了else里的showUI()");
            showUI();
            Debug.Log("ui 不为 null !");
        }
    }

    public void InvisionableUI()
    {
        ui.gameObject.SetActive(false);
        ui.enabled = false;
    }
    public void showUI()
    {
        if (ui == null)
        {
            Debug.Log("ui等于null啊！");
        }
        else
        {
            Debug.Log("ui有值！");
        }

        if (ui.gameObject == null)
        {
            Debug.Log("ui.gameObject等于null啊！");
        }
        else
        {
            Debug.Log("ui.gameObject有值！");
        }

        Debug.Log("ui.gameObject:" + ui.gameObject);
        Debug.Log("ui.gameObject.activeSelf:" + ui.gameObject.activeSelf);
        ui.gameObject.SetActive(true);
        ui.enabled = true;
    }
    
}
