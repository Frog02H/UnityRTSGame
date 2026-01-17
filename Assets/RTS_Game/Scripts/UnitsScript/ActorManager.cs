using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;


public class ActorManager : MonoBehaviour
{
    public bool BoxOrLine = default;
    //Box的选择框
    public static ActorManager instance;
    [SerializeField] LayerMask actorLayer = default;
    [SerializeField] Transform selectionArea = default;
    [SerializeField] GameObject AllActorsGO = default;
    public GameObject AllActors_Get
    {
        get { return AllActorsGO; }
    }

    // [SerializeField] GameObject AllOwnPlayers = default;
    public List<Actor> allActors = new List<Actor>();
    //[SerializeField] List<Actor> selectedActors = new List<Actor>();
    public List<Actor> selectedActors = new List<Actor>();
    //资源生产单位列表
    public List<Actor> RPActors = new List<Actor>();
    Camera mainCamera;
    Vector3 startDrag;
    Vector3 endDrag;
    Vector3 dragCenter;
    Vector3 dragSize;
    bool dragging;

    //Line的选择框
    public bool isMouseDown;

    public LineRenderer line;
    public Vector3 beginDownInputPos;
    public Vector3 endDownInputPos;
    public Vector3 rightUpPos;
    public Vector3 leftDownPos;

    // public RaycastHit hitInfo;
    public Vector3 TempPos;
    public Vector3 beginWorldPos;
    //

    public bool isGActive = false;

    private string RealPlayerStr = "Player";

    public string PlayerStr_Get
    {
        get { return RealPlayerStr; }
    }

    private string ComPlayerStr = "Enemy";

    public string EnemyStr_Get
    {
        get { return ComPlayerStr; }
    }
    /*     
        private void Awake()
        {
            instance = this;
        } 
    */
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

    void Start()
    {
        mainCamera = Camera.main;
        foreach (Actor actor in AllActorsGO.GetComponentsInChildren<Actor>())
        {
            if (actor.CompareTag(RealPlayerStr))
            {
                allActors.Add(actor);
            }
            //资源生产型单位列表
            if (actor.GetComponent<Farmer>() != null && actor.CompareTag("Player"))
            {
                RPActors.Add(actor);
            }
        }

        BoxOrLine = false;
        selectionArea.gameObject.SetActive(false);
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            dragging = false;
            return;
        }

        if (BoxOrLine)//0为box，1为line
        {
            SelectUpdateWithLine();
        }
        else
        {
            SelectUpdate();
        }

        /*         if (Input.anyKeyDown && selectedActors.Count > 0)
                {
                    Debug.Log("按下按键了！");
                } */

        if (Input.anyKeyDown && selectedActors.Count > 0)
        {
            PressActionKey();
        }

        GuardUpdate();

        //更新ActorManager的allActors
        RomveUpdateAllActors();
        RomveUpdateSelectedActors();
        RomveUpdateRPActors();

        GameOverOrNot();
    }

    private void GameOverOrNot()
    {
        if (BuildingManager.instance.allBuildings.Count <= 0)
        {
            Debug.Log("真人玩家的建筑全没了啊！！！真人玩家输了！！！");

            // KillThemAll();
            // ComActorManager.instance.KillThemAll();
        }
    }

    void SelectUpdate()
    {
        selActorObj();
        //自添加
        ControlSoldierMove();
    }

    void selActorObj()
    {
        if (Input.GetMouseButtonDown(1))
        {
            startDrag = Utilities.MouseToTerrainPosition();
            endDrag = startDrag;
        }
        else if (Input.GetMouseButton(1))
        {
            endDrag = Utilities.MouseToTerrainPosition();

            if (Vector3.Distance(startDrag, endDrag) > 1)
            {
                selectionArea.gameObject.SetActive(true);
                dragging = true;
                dragCenter = (startDrag + endDrag) / 2;
                dragSize = (endDrag - startDrag);
                selectionArea.transform.position = dragCenter;
                selectionArea.transform.localScale = dragSize + Vector3.up;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            if (dragging)
            {
                SelectActors();
                //dragging = false;
                selectionArea.gameObject.SetActive(false);
            }
            dragging = false;
            /*
            else
            {
                SetTask();
            }
            */
        }
    }

    void SetTask()
    {
        /*         if (selectedActors.Count == 0)
                {
                    return;
                } */

        RomveUpdateSelectedActors();

        Collider collider = Utilities.CameraRay().collider;
        //if (collider.CompareTag("Terrain"))
        if (collider.CompareTag("Ground"))
        {
            if (selectedActors.Count <= 0)
            {
                return;
            }

            foreach (Actor actor in selectedActors)
            {
                //停止单位的当前工作
                // if (actor.ActorAttack)
                 if (actor.ActorAttack)
                {
                    actor.ActorAttack.CancelAllAttackBool();
                    if (isGActive)
                    {
                        actor.ActorAttack.isGuarding = true;
                    }
                }
                else
                {
                    actor.StopTask();
                }

                actor.SetDestination(Utilities.MouseToTerrainPosition());
            }
        }
        else
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("BRO点到Player单位了!");

                if (collider.gameObject.layer == LayerMask.NameToLayer("UnFinBuilding"))
                {
                    if (selectedActors.Count <= 0)
                    {
                        return;
                    }

                    foreach (Actor actor in ActorManager.instance.selectedActors)
                    {
                        if (actor is Builder)
                        {
                            Builder builder = actor as Builder;
                            if (!builder.HasTask())
                            {
                                //builder.CheckJobPosition(building);
                                //builder.CheckJobPositionWithBoxCollider(building);
                                //builder.CheckJobPositionBeforeMoveToBoxCollider(building);
                                builder.GiveJob(collider.GetComponent<Building>());
                            }
                        }
                    }
                    Debug.Log("BRO点到UnFinBuilding单位了!");
                }

                //普通建筑,如单位生产类建筑
                if (collider.gameObject.layer == LayerMask.NameToLayer("FinBuilding"))
                {
                    Debug.Log("collider.gameObject:" + collider.gameObject);
                    Debug.Log("collider.gameObject.transform:" + collider.gameObject.transform);
                    Debug.Log("collider.transform:" + collider.transform);

                    // UnitProduceManager.instance.SetCurrentBuilding(collider.transform);
                    // UnitProduceManager.instance.SetCurrentBuilding(collider);

                    UnitProduceManager.instance.SetCurrentBuilding(collider.transform);

                    BuildingManager.instance.InvisionableUI();
                    UnitProduceManager.instance.showUI();
                    // UnitProduceManager.instance.InitUI();
                    Debug.Log("BRO点到FinBuilding单位了!");
                }

                //资源生产类建筑
                if (collider.gameObject.layer == LayerMask.NameToLayer("RPBuilding"))
                {
                    if (selectedActors.Count <= 0)
                    {
                        return;
                    }

                    Debug.Log("collider.gameObject:" + collider.gameObject);
                    Debug.Log("collider.gameObject.transform:" + collider.gameObject.transform);
                    Debug.Log("collider.transform:" + collider.transform);

                    Building building = collider.gameObject.GetComponent<Building>();
                    ResourceProductionFunc RPFunc = building.RPFunc;
                    foreach (Actor actor in RPActors)
                    {
                        if (RPFunc.IsStopAdd())
                        {
                            Debug.Log("RPFunc.IsStopAdd()发动了！");
                            break;
                        }

                        Debug.Log("actor.IsSelected:" + actor.IsSelected);
                        Debug.Log("RPFunc.fieldID:" + RPFunc.fieldID);
                        if (actor.IsSelected && RPFunc.fieldID != actor.GetComponent<Farmer>().fieldID)
                        {
                            Debug.Log("往Farmers队列加Actor了！");
                            // collider.gameObject.GetComponent<ResourceProductionFunc>().AddToFarmers(actor.GetComponent<Farmer>());
                            Farmer farmer = actor.GetComponent<Farmer>();
                            RPFunc.AddToFarmers(farmer);
                            farmer.GiveJob(building);
                            // actor.SetDestination(RPFunc.transform.position);
                        }

                    }

                    Debug.Log("BRO点到RPBuilding单位了!");
                }

            }
            else if (collider.CompareTag("Enemy"))
            {
                if (selectedActors.Count <= 0)
                {
                    return;
                }

                if (collider.TryGetComponent(out Damageable damageable))
                {
                    foreach (Actor actor in selectedActors)
                    {
                        actor.AttackTarget(damageable);
                    }
                }
                // BuildingManager.instance.UIRefresh();
            }
            // BuildingManager.instance.UIRefresh();
        }

    }

    void SelectActors()
    {
        DeselectActors();
        dragSize.Set(Mathf.Abs(dragSize.x / 2), 1, Mathf.Abs(dragSize.z / 2));
        RaycastHit[] hits = Physics.BoxCastAll(dragCenter, dragSize, Vector3.up, Quaternion.identity, 0, actorLayer.value);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out Actor actor))
            {
                if (actor.gameObject.tag == PlayerStr_Get)
                {
                    selectedActors.Add(actor);
                    actor.visualHandler.Select();
                    // 暂时添加，后面需要包装进方法！
                    actor.IsIn = false;
                }
            }
        }
    }
    public void DeselectActors()
    {
        foreach (Actor actor in selectedActors)
        {
            if (actor)
            {
                actor.visualHandler.Deselect();
            }
            // actor.visualHandler.Deselect();
        }
        selectedActors.Clear();
    }

    public void RomveUpdateAllActors()
    {
        for (int i = 0; i < allActors.Count; i++)
        {
            if (allActors[i] == null)
            {
                allActors.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateSelectedActors()
    {
        for (int i = 0; i < selectedActors.Count; i++)
        {
            if (selectedActors[i] == null)
            {
                selectedActors.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateRPActors()
    {
        for (int i = 0; i < RPActors.Count; i++)
        {
            if (RPActors[i] == null)
            {
                RPActors.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 center = (startDrag + endDrag) / 2;
        Vector3 size = (endDrag - startDrag);
        size.y = 1;
        Gizmos.DrawWireCube(center, size);
    }

    void SelectUpdateWithLine()
    {
        selSoldierObj();
        ControlSoldierMove();
    }

    private void selSoldierObj()
    {
        if (Input.GetMouseButtonDown(1))
        {
            beginDownInputPos = Input.mousePosition;
            isMouseDown = true;

            beginWorldPos = Utilities.MouseToTerrainPosition();
            /* 
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000 , 1 << LayerMask.NameToLayer("Ground")))
            {
                beginWorldPos = hitInfo.point;
            } 
            */

        }
        else if (Input.GetMouseButtonUp(1))
        {
            isMouseDown = false;
            line.positionCount = 0;

            //frontPos = Vector3.zero;

            DeselectActors();
            //执行actor.visualHandler.Deselect();
            //执行selectedActors.Clear();

            TempPos = Utilities.MouseToTerrainPosition();
            //在这调整高度
            Vector3 center = new Vector3((TempPos.x + beginWorldPos.x) / 2, 1, (TempPos.z + beginWorldPos.z) / 2);
            Vector3 half = new Vector3(Mathf.Abs(TempPos.x - beginWorldPos.x) / 2, 1, Mathf.Abs(TempPos.z - beginWorldPos.z) / 2);

            SelectActorsWithLine(center, half);
        }

        if (isMouseDown)
        {
            endDownInputPos = Input.mousePosition;

            rightUpPos.x = endDownInputPos.x;
            rightUpPos.y = beginDownInputPos.y;
            rightUpPos.z = 5;

            leftDownPos.x = beginDownInputPos.x;
            leftDownPos.y = endDownInputPos.y;
            leftDownPos.z = 5;

            beginDownInputPos.z = 5;
            endDownInputPos.z = 5;

            line.positionCount = 4;
            line.SetPosition(0, Camera.main.ScreenToWorldPoint(beginDownInputPos));
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(rightUpPos));
            line.SetPosition(2, Camera.main.ScreenToWorldPoint(endDownInputPos));
            line.SetPosition(3, Camera.main.ScreenToWorldPoint(leftDownPos));

        }
    }

    private void ControlSoldierMove()
    {
        if (Input.GetMouseButtonDown(0))
        {

            /*             
            if(selectedActors.Count == 0)
                {
                    return;
                } 
            */

            // 做单位出生集结点的，但暂时不知道怎么写，先放在这里
            if (false)
            {

            }

            SetTask();
        }

    }

    private void SelectActorsWithLine(Vector3 center, Vector3 half)
    {
        Collider[] colliders = Physics.OverlapBox(center, half);

        for (int i = 0; i < colliders.Length; i++)
        {
            Actor obj = colliders[i].GetComponent<Actor>();

            if (obj != null)
            {
                selectedActors.Add(obj);
                obj.visualHandler.Select();
            }
        }
    }

    private void GuardUpdate()
    {
        // Debug.Log("GuardUpdate!");
        if (isGActive)
        {
            // Debug.Log("isGActive!");
            foreach (Actor actor in selectedActors)
            {
                if (actor.ActorAttack != null)
                {
                    //暂时方案
                    /* 
                    if(actor.damageableTarget != actor.ActorAttack.DamageableTarget)
                    {
                        actor.damageableTarget = null;
                    } 
                    */

                    // Debug.Log("actor.ActorAttack.isLock: "+ actor.ActorAttack.isLock);
                    // Debug.Log("|T是有，F是无,我们只要第一次没攻击的时候去攻击，正在攻击就不要再攻击了|");
                    // Debug.Log("!actor.ActorAttack.isGuardAttack: "+ !actor.ActorAttack.isGuardAttack);

                    // if (actor.ActorAttack.isLock && !actor.ActorAttack.IsGuardAttack())
                    if (actor.ActorAttack.isLock && !actor.ActorAttack.isGuardAttack)
                    {
                        // Debug.Log("actor.Guard();");
                        actor.Guard();
                    }
                }
            }
        }
    }

    private void PressActionKey()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("按下G键了!");

            isGActive = !isGActive;

            if (isGActive)
            {
                Debug.Log("actor进入“警戒”状态");
                // Debug.Log("selectedActors:" + selectedActors);
                //*
                foreach (Actor actor in selectedActors)
                {
                    if (actor.currentTask != null)
                    {
                        actor.StopTask();
                    }
                    // actor.StopTask();
                    // Debug.Log("actor:" + actor);
                    if (actor.ActorAttack != null)
                    {
                        Debug.Log("actor.ActorAttack.isGuarding = true");
                        actor.ActorAttack.isGuarding = true;
                    }
                }
                //*/

            }
            else
            {
                Debug.Log("actor解除“警戒”状态");
                // Debug.Log("selectedActors:" + selectedActors);
                //*
                foreach (Actor actor in selectedActors)
                {
                    if (actor.currentTask != null)
                    {
                        actor.StopTask();
                    }
                    // actor.StopTask();
                    // Debug.Log("actor:" + actor);
                    if (actor.ActorAttack != null)
                    {
                        Debug.Log("actor.ActorAttack.isGuarding = false");
                        actor.ActorAttack.isGuarding = false;
                        //
                        actor.ActorAttack.isLock = false;
                        actor.ActorAttack.isGuardAttack = false;
                        actor.ActorAttack.isAttack = false;
                        // actor.StopTask();
                    }
                }
                //*/
            }

            Debug.Log("isGActive:" + isGActive);

        }
    }


    #region 玩家单位全杀！
    public void KillThemAll()
    {
        foreach (Actor actor in allActors)
        {
            actor.damageable.Hit(actor.damageable.currentHealth);
        }

        DeselectActors();
        RPActors.Clear();
        RomveUpdateAllActors();
    }
    #endregion
}
