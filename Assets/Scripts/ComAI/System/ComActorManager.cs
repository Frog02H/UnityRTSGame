using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;


public class ComActorManager : MonoBehaviour
{
    //Box的选择框
    public static ComActorManager instance;
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
    public List<Actor> builders = new List<Actor>();
    public List<Actor> attackers = new List<Actor>();
    public List<Actor> RPActors = new List<Actor>();

    public bool isGActive = false;

    private string ComPlayerStr = "Enemy";
    private string RealPlayerStr = "Player";
    public string PlayerStr_Get
    {
        get { return ComPlayerStr; }
    }
    public string EnemyStr_Get
    {
        get { return RealPlayerStr; }
    }

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
        foreach (Actor actor in AllActorsGO.GetComponentsInChildren<Actor>())
        {
            if (actor.CompareTag(ComPlayerStr))
            {
                allActors.Add(actor);
            }
            //资源生产型单位列表
            if (actor.GetComponent<Farmer>() != null && actor.CompareTag(ComPlayerStr) && (actor.gameObject.layer == LayerMask.NameToLayer(ComPlayerStr)))
            {
                RPActors.Add(actor);
            }
            if (actor.GetComponent<Builder>() != null && actor.CompareTag(ComPlayerStr) && (actor.gameObject.layer == LayerMask.NameToLayer(ComPlayerStr)))
            {
                builders.Add(actor);
            }
            if (actor.GetComponent<ActorAttack>() != null && actor.CompareTag(ComPlayerStr) && (actor.gameObject.layer == LayerMask.NameToLayer(ComPlayerStr)))
            {
                attackers.Add(actor);
            }
        }

    }

    void Update()
    {
        SelectUpdate();

        //更新ActorManager的allActors
        RomveUpdateAllActors();
        RomveUpdateSelectedActors();
        RomveUpdateRPActors();
        RomveUpdateBuilders();
        RomveUpdateAllAttackers();

        GameOverOrNot();
    }

    #region 
    private void GameOverOrNot()
    {
        if (BuildingManager.instance.allBuildings.Count <= 0)
        {
            Debug.Log("电脑玩家输了！！！");

            // KillThemAll();
            // ActorManager.instance.KillThemAll();
        }
    }
    #endregion

    #region SelectUpdate()方法和其内涵方法
    void SelectUpdate()
    {
        SelectActors();
        //自添加
        ControlSoldierMove();
    }

    void SelectActors()
    {
        if (selectedActors.Count <= 0)
        {
            DeselectActors();
            if (allActors.Count > 0)
            {
                foreach (Actor actor in allActors)
                {
                    // if (actor.gameObject.tag == PlayerStr && )
                    if (actor.CompareTag(ComPlayerStr) && (actor.gameObject.layer == LayerMask.NameToLayer(ComPlayerStr)))
                    {
                        selectedActors.Add(actor);
                        // 暂时添加，后面需要包装进方法！
                        actor.IsIn = false;
                    }
                }
            }
        }
    }

    private void ControlSoldierMove()
    {
        SetTask();
    }

    void SetTask()
    {
        /*
        if (selectedActors.Count == 0)
        {
            return;
        }
        */

        RomveUpdateSelectedActors();

        if (ComBuildingManager.instance.allComBuildings.Count <= 0)
        {
            Debug.Log("电脑玩家已被打败,真人玩家胜利!!!");
            KillThemAll();
        }
        else
        {
            if (ComResourceManager.Instance.CanAfford(ComBuildingManager.instance.buildingPrefabs[0].Cost()))
            {
                Debug.Log("理应在建造，但方法还没写！咱先减资源。");
                int[] cost = ComBuildingManager.instance.buildingPrefabs[0].Cost();
                ComResourceManager.Instance.ConsumeAll(cost);

                //此处代码是,在随机位置生产建筑
                /*
                foreach (Actor actor in selectedActors)
                {
                    if (actor is Builder)
                    {
                        Builder builder = actor as Builder;
                        if (!builder.HasTask())
                        {
                            // builder.GiveJob(ComBuildingManager.instance.buildingPrefabs[0]);
                        }
                    }
                }
                */
            }
            else
            {
                //资源不够满足建造消费,就去挖!
                foreach (Actor actor in selectedActors)
                {
                    if (actor is Builder)
                    {
                        Builder builder = actor as Builder;
                        if (!builder.HasTask())
                        {
                            actor.AttackTarget(ComBuildingManager.instance.allResources[0].Damageable_Get);
                        }
                    }
                }
            }
        }

        if (selectedActors.Count > 0)
        {
            // 小于小队人数则继续生产Soldier
            if (attackers.Count <= 5)
            {
                foreach (Building building in ComBuildingManager.instance.allComBasicAttackerBuildings)
                {
                    ComUnitProduceManager.instance.ProductorBuilding = building;
                    ComUnitProduceManager.instance.AiAttackerTestProduct();
                }
            }
            else    // 大于小队人数则攻击玩家建筑
            {
                foreach (Actor actor in attackers)
                {
                    if (actor is Soldier)
                    {
                        Soldier attacker = actor as Soldier;
                        // Debug.LogError("执行到AI攻击了!");
                        // Debug.LogError($"攻击对象的名字是：{ComBuildingManager.instance.allPlayerBuildings[0].name}！");
                        // Debug.LogError("ComBuildingManager.instance.allPlayerBuildings[0].attackable的存在：" + ComBuildingManager.instance.allPlayerBuildings[0].attackable);

                        if (ComBuildingManager.instance.allPlayerBuildings.Count > 0)
                        {
                            attacker.AIAttack(ComBuildingManager.instance.allPlayerBuildings[0].attackable);
                        }
                        // attacker.AIAttack(ComBuildingManager.instance.allPlayerBuildings[0].attackable);
                    }
                }
            }
        }
        else
        {
            // 选择队列没人就生产Builder
            if (builders.Count <= 5)
            {
                foreach (Building building in ComBuildingManager.instance.allComBasicAttackerBuildings)
                {
                    ComUnitProduceManager.instance.ProductorBuilding = building;
                    ComUnitProduceManager.instance.AiBuilderTestProduct();
                }
            }
        }

    }
    #endregion

    #region 清理队列用的常规方法
    public void DeselectActors()
    {
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

    public void RomveUpdateBuilders()
    {
        for (int i = 0; i < builders.Count; i++)
        {
            if (builders[i] == null)
            {
                builders.RemoveAt(i);
            }
        }
    }

    public void RomveUpdateAllAttackers()
    {
        for (int i = 0; i < attackers.Count; i++)
        {
            if (attackers[i] == null)
            {
                attackers.RemoveAt(i);
            }
        }
    }

    public void KillThemAll()
    {
        foreach (Actor actor in selectedActors)
        {
            actor.damageable.Hit(actor.damageable.currentHealth);
        }
        RomveUpdateAllActors();
        DeselectActors();
        RPActors.Clear();
        builders.Clear();
        attackers.Clear();
    }
    #endregion
}
