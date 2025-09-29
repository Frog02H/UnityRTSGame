using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActorCost : MonoBehaviour
{
    public string unitName = default;
    [SerializeField] float height;
    // public float radius = 5;
    // float originalHeight;
    public Vector3 bornPos;
    [SerializeField] int totalWorkToComplete = 100;
    public int currentWork;
    public int[] resourceCost = default;
    Transform unitTransform;
    public bool isHover = false;
    private bool done;
    // [ColorUsage(true, true)]
    // [SerializeField] private Color[] stateColors;
    MeshRenderer unitRender;
    //Cinemachine.CinemachineImpulseSource impulse;
    public BoxCollider rayCastBoxCollider;
    // public GameObject TagPosCube;
    // public Vector3[] bottomCorners;
    public Vector3 UnitGoTo;
    private void Awake()
    {

    }

    void Start()
    {
        unitTransform = GetComponent<Transform>();
        //TagPosCube.transform.position = buildingTransform.position;

        unitRender = unitTransform.GetComponent<MeshRenderer>();
        //impulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        currentWork = 0;
        //bornPos = unitTransform.localPosition;
        //originalHeight = unitTransform.localPosition.y;
        //buildingTransform.localPosition = Vector3.down * height;

        //GameObject CenterPosCube = Instantiate(TagPosCube, buildingTransform.position, Quaternion.identity);
        rayCastBoxCollider = GetComponent<BoxCollider>();
        //GetBottomPointWorldPos();

        /*检测
        Debug.Log("height:"+height);
        Debug.Log("originalHeight:"+originalHeight);
        Debug.Log("currentWork:"+currentWork);
        */
    }
    
    private void Update()
    {
        
    }
    
    public void Start_PlayerGetOutAnimation()
    {
        //unitTransform.localPosition = Vector3.Lerp(UnitGoTo, bornPos, (float)currentWork / totalWorkToComplete);
        unitTransform.localPosition = Vector3.Lerp(UnitGoTo, bornPos, 0.2f);
        /*检测
        Debug.Log("Building的currentWork:"+currentWork);
        */
        //visual
        unitTransform.DOComplete();
        //unitTransform.DOShakeScale(.5f, .2f, 10, 90, true);
        //BuildingManager.instance.PlayParticle(transform.position);
    }
    public void Build(int work)
    {
        currentWork += work;
    }

    public bool IsFinished()
    {
        //if (currentWork >= totalWorkToComplete && !done && unitRender)
        if (currentWork >= totalWorkToComplete && !done)
        {
            done = true;
            //unitRender.material.DOColor(stateColors[1], "_EmissionColor", .1f).OnComplete(() => unitRender.material.DOColor(stateColors[0], "_EmissionColor", .5f));
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
    
    public void IsChangeLayer()
    {
        if(gameObject.layer != LayerMask.NameToLayer("default"))
        {
            return;
            //如果层级已经更改则跳过此函数
        }


        /*         
        if (IsFinished())
        {
            int layer = LayerMask.NameToLayer("FinBuilding");
            gameObject.layer = layer;
        } 
        */
        
    }

    public int[] Cost()
    {
        return resourceCost;
    }
/*
    private void OnMouseEnter()
    {
        isHover = true;
    }
    private void OnMouseExit()
    {
        isHover = false;
    }
*/
/*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
*/
    //
}
