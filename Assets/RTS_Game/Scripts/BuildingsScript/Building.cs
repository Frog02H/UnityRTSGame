using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.ProBuilder.Shapes;
using System;

//[RequireComponent(typeof(Damageable))]
public class Building : MonoBehaviour
{
    public string buildingName;

    [SerializeField] float height;
    public float radius = 5;
    float originalHeight;
    [SerializeField] int totalWorkToComplete = 100;
    public int currentWork;
    public int[] resourceCost = default;
    // [HideInInspector] public Building_UnitsProducting building_UnitsProducting;
    public BuildingController_UnitsProducting Controller_UnitsProducting;
    Transform buildingTransform;

    [HideInInspector] public Damageable attackable;

    public bool isHover = false;
    private bool done;
    [ColorUsage(true, true)]
    [SerializeField] private Color[] stateColors;
    MeshRenderer buildingRender;
    //Cinemachine.CinemachineImpulseSource impulse;

    public BoxCollider rayCastBoxCollider;
    public GameObject TagPosCube;
    public Vector3[] bottomCorners;

    public bool isSelected = false;

    public ResourceProductionFunc RPFunc;

    public bool IsBuilt = false;
    
    private void Awake()
    {
        attackable = GetComponent<Damageable>();
        // building_UnitsProducting = GetComponent<Building_UnitsProducting>();
        Controller_UnitsProducting = GetComponent<BuildingController_UnitsProducting>();

        //拿取,为null也没关系
        RPFunc = gameObject.GetComponent<ResourceProductionFunc>();
    }

    void Start()
    {
        buildingTransform = transform.GetChild(0);
        //TagPosCube.transform.position = buildingTransform.position;

        buildingRender = buildingTransform.GetComponent<MeshRenderer>();
        //impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        currentWork = 0;
        originalHeight = buildingTransform.localPosition.y;
        buildingTransform.localPosition = Vector3.down * height;

        // 中心方块生成!!!测试用!!!
        // GameObject CenterPosCube = Instantiate(TagPosCube, buildingTransform.position, Quaternion.identity);
        // GameObject CenterPosCube = Instantiate(TagPosCube, buildingTransform.position, Quaternion.identity, BuildingManager.instance.AllBuildingsGO.transform);
        rayCastBoxCollider = GetComponent<BoxCollider>();
        GetBottomPointWorldPos();

        //单位出生的位置
        if (Controller_UnitsProducting != null)
        {
            Controller_UnitsProducting.SetBornPos(buildingTransform);
        }

        if (IsBuilt)
        {
            BuildNow();
            /*
            if (IsFinished())
            {
                IsChangeLayerAndTag();
            }
            */
        }
        /*检测
        Debug.Log("height:"+height);
        Debug.Log("originalHeight:"+originalHeight);
        Debug.Log("currentWork:"+currentWork);
        */
    }

    private void Update()
    {

    }

    public void Build(int work)
    {
        currentWork += work;
        buildingTransform.localPosition = Vector3.Lerp(Vector3.down * height, new Vector3(0, originalHeight, 0), (float)currentWork / totalWorkToComplete);
        /*检测
        Debug.Log("Building的currentWork:"+currentWork);
        */
        //visual
        buildingTransform.DOComplete();
        buildingTransform.DOShakeScale(.5f, .2f, 10, 90, true);
        //BuildingManager.instance.PlayParticle(transform.position);
    }

    public bool IsFinished()
    {
        if (currentWork >= totalWorkToComplete && !done && buildingRender)
        {
            done = true;
            buildingRender.material.DOColor(stateColors[1], "_EmissionColor", .1f).OnComplete(() => buildingRender.material.DOColor(stateColors[0], "_EmissionColor", .5f));
            /*
            if (impulse)
            {
                impulse.GenerateImpulse();
            }
            */
        }
        return currentWork >= totalWorkToComplete;
    }

    public bool CanBuild(int[] resources)
    {
        bool canBuild = true;
        /*检测
        Debug.Log("resourceCost.Length:"+resourceCost.Length);
        */
        for (int i = 0; i < resourceCost.Length; i++)
        {
            if (resources[i] < resourceCost[i])
            {
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    public void IsChangeLayerAndTag()
    {
        if (gameObject.layer == LayerMask.NameToLayer("FinBuilding") && CompareTag("Player"))
        {
            Debug.Log("Tag和Layer已改变！");
            return;
            //如果层级已经更改则跳过此函数
        }

        if (IsFinished())
        {
            if (gameObject.layer == LayerMask.NameToLayer("FinBuilding") && CompareTag("Player"))
            {
                Debug.Log("Tag和Layer已改变！");
                return;
                //如果层级已经更改则跳过此函数
            }

            Debug.Log("改变层级ing");
            int layer = LayerMask.NameToLayer("FinBuilding");
            
            if (RPFunc)
            {
                layer = LayerMask.NameToLayer("RPBuilding");
            }

            this.gameObject.layer = layer;
            Debug.Log("改变标签ing");
        }
        //暂时
        this.gameObject.tag = "Player";
    }
    
    public void IsChangeLayerAndTag(string tagStr)
    {
        if (gameObject.layer == LayerMask.NameToLayer("FinBuilding") && CompareTag(tagStr))
        {
            Debug.Log("Tag和Layer已改变！");
            return;
            //如果层级已经更改则跳过此函数
        }

        if (IsFinished())
        {
            if (gameObject.layer == LayerMask.NameToLayer("FinBuilding") && CompareTag(tagStr))
            {
                Debug.Log("Tag和Layer已改变！");
                return;
                //如果层级已经更改则跳过此函数
            }

            Debug.Log("改变层级ing");
            int layer = LayerMask.NameToLayer("FinBuilding");

            if (RPFunc)
            {
                layer = LayerMask.NameToLayer("RPBuilding");
            }

            this.gameObject.layer = layer;
            Debug.Log("改变标签ing");
        }
        //暂时
        this.gameObject.tag = tagStr;
    }

    public int[] Cost()
    {
        return resourceCost;
    }

    private void OnMouseEnter()
    {
        isHover = true;
    }
    private void OnMouseExit()
    {
        isHover = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    //
    private void GetBottomPointWorldPos()
    {
        // 获取BoxCollider底部四个顶点的世界坐标
        bottomCorners = GetBottomCorners(rayCastBoxCollider);

        // 打印底部四个顶点的世界坐标
        foreach (var corner in bottomCorners)
        {
            Debug.Log("Bottom Corner: " + corner);

            //// 四角方块生成!!!测试用!!!
            // GameObject CornerPosCube = Instantiate(TagPosCube, corner, Quaternion.identity);
            // GameObject CornerPosCube = Instantiate(TagPosCube, corner, Quaternion.identity, BuildingManager.instance.AllBuildingsGO.transform);
        }

        //return bottomCorners;
    }

    private Vector3[] GetBottomCorners(BoxCollider boxCollider)
    {
        // 获取BoxCollider的局部中心点和大小
        //Vector3 localCenter = boxCollider.center;
        Vector3 size = boxCollider.size;

        // 计算底部四个顶点的局部坐标
        Vector3 bottomLeftLocal = new Vector3(-size.x / 2, -size.y / 2, -size.z / 2);
        Vector3 bottomRightLocal = new Vector3(size.x / 2, -size.y / 2, -size.z / 2);
        Vector3 bottomBackLocal = new Vector3(-size.x / 2, -size.y / 2, size.z / 2);
        Vector3 bottomFrontLocal = new Vector3(size.x / 2, -size.y / 2, size.z / 2);

        // 将局部坐标转换为世界坐标
        Vector3 worldBottomLeft = boxCollider.transform.TransformPoint(bottomLeftLocal);
        worldBottomLeft.y = boxCollider.gameObject.transform.position.y;
        Vector3 worldBottomRight = boxCollider.transform.TransformPoint(bottomRightLocal);
        worldBottomRight.y = boxCollider.gameObject.transform.position.y;
        Vector3 worldBottomBack = boxCollider.transform.TransformPoint(bottomBackLocal);
        worldBottomBack.y = boxCollider.gameObject.transform.position.y;
        Vector3 worldBottomFront = boxCollider.transform.TransformPoint(bottomFrontLocal);
        worldBottomFront.y = boxCollider.gameObject.transform.position.y;

        // 返回底部四个顶点的世界坐标
        return new Vector3[] { worldBottomLeft, worldBottomRight, worldBottomBack, worldBottomFront };
    }

    public void BuildNow()
    {
        Build(totalWorkToComplete + 10);
    }

    public void GiveJob(Actor unit)
    {

    }
    //
}
