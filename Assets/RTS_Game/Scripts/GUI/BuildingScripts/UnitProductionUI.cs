using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitProductionUI : MonoBehaviour
{
    CanvasGroup ParentcanvasGroup;
    public Transform resourceGroup;
    public CanvasGroup CostcanvasGroup;
    private void Awake()
    {
        ParentcanvasGroup = GetComponent<CanvasGroup>();
    }
    void Start()
    {
        CanvasGroup[] ChildrencanvasGroup = GetComponentsInChildren<CanvasGroup>();
        //Debug.Log("ChildrencanvasGroup[0]:"+ ChildrencanvasGroup[0]);

        Button[] Unitbuttons = ChildrencanvasGroup[0].GetComponentsInChildren<Button>();

        for (int i = 0; i < Unitbuttons.Length; i++)
        {
            int index = i;
            /*检测
            Debug.Log("index:"+ index);
            */
            Unitbuttons[index].onClick.AddListener(() => SelectUnit(index));

            //Building b = BuildingManager.instance.buildingPrefabs[index];
            //Buildingbuttons[index].GetComponentInChildren<TextMeshProUGUI>().text = GetButtonText(b);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ParentcanvasGroup.alpha = 1;
            BuildingManager.instance.showUI();
            UnitProduceManager.instance.InvisionableUI();
            BuildingManager.instance.UIRefresh();
            //isPlacing = false;
        }
    }

    void SelectUnit(int index)
    {
        Debug.Log("index:" + index);
        //CostPannelUI调用！显示消耗的资源

        //Actor u = UnitProduceManager.instance.unitDataList[index];
        UnitData UD = UnitProduceManager.instance.unitDataList[index];

        // CostPannel 输入
        Debug.Log("UD.prefab.GetComponent<Actor>():" + UD.prefab.GetComponent<Actor>());
        Debug.Log("CostcanvasGroup.GetComponent<CostPannelUI>():" + CostcanvasGroup.GetComponent<CostPannelUI>());

        CostcanvasGroup.GetComponent<CostPannelUI>().ShowCostPannelUI(UD.prefab.GetComponent<Actor>());

        UnitProduceManager.instance.ClickToProduct(UD);

        /*检测
        Debug.Log("在SelectBuilding中的currentIndex:"+ currentIndex);
        */
        //ActorManager.instance.DeselectActors();
        // 是这个让UI消失了?
        // ParentcanvasGroup.alpha = 0;
        //isPlacing = true;
        //buildingPreviewMesh = UnitProducingManager.instance.GetPrefab(index).GetComponentInChildren<MeshFilter>().sharedMesh;
    }

    string GetButtonText(Building b)
    {
        string buildingName = b.buildingName;
        int resourceAmount = b.resourceCost.Length;
        string[] resourceNames = new string[] { "Food", "Wood", "Steel", "Oil", "Gold", "Manpower" };
        string resourceString = string.Empty;
        for (int j = 0; j < resourceAmount; j++)
            resourceString += "\n " + resourceNames[j] + " (" + b.resourceCost[j] + ")";

        return "<size=23><b>" + buildingName + "</b></size>" + resourceString;
    }

    public void RefreshResources()
    {
        for (int i = 0; i < resourceGroup.childCount; i++)
        {
            resourceGroup.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ResourceManager.Instance.Resources[i].ToString();
        }
    }
}
