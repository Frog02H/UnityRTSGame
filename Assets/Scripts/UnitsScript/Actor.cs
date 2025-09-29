using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Damageable))]

/* 
RequireComponent的使用：
当你添加的一个用了RequireComponent组件的脚本，需要的组件将会自动被添加到game object（游戏物体）。
可以有效的避免组装错误。
举个例子一个脚本可能需要刚体总是被添加在相同的game object（游戏物体）上。
用RequireComponent属性的话，这个过程将被自动完成，因此你可以永远不会犯组装错误。
*/

public class Actor : MonoBehaviour
{
    protected Rigidbody actorRigidbody;
    protected NavMeshAgent agent;
    protected NavMeshObstacle navMeshObstacle;
    [HideInInspector] public Damageable damageable;
    [HideInInspector] public Damageable damageableTarget;
    [HideInInspector] public Animator animator;
    [HideInInspector] public AnimationEventListener animationEvent;
    [HideInInspector] public Coroutine currentTask;
    [HideInInspector] public ActorVisualHandler visualHandler;
    public ActorCost ActorCost;
    public ActorAttack ActorAttack;

    public bool isHover = false;
    bool isResource;
    private bool isIn = true;
    public bool IsIn
    {
        get { return isIn; }
        set { isIn = value; }
    }

    //不依靠这个bool值调整select队列,这个bool值只用于减少对select队列的遍历,不遍历就可以知道该单位已经被选择
    private bool isSelected = false;
    public bool IsSelected
    {
        get { return isSelected; }
        set { isSelected = value; }
    }

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
        animator = GetComponentInChildren<Animator>();
        animationEvent = GetComponentInChildren<AnimationEventListener>();
        visualHandler = GetComponent<ActorVisualHandler>();
        animationEvent.attackEvent.AddListener(Attack);
        isResource = GetComponent<Resource>() ? true : false;

        //
        actorRigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = false;

        //
        ActorCost = GetComponent<ActorCost>();
    }
    public virtual void Update()
    {
        animator.SetFloat("Speed", Mathf.Clamp(agent.velocity.magnitude, 0, 1));
    }

    public void SetDestination(Vector3 destination)
    {
        agent.enabled = true;
        agent.destination = destination;
        //agent.enabled = false;
    }

    public WaitUntil WaitForNavMesh()
    {
        return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        //agent.pathPending: Is a path  in the process of being computed but not yet ready? (Read Only)
        //remainingDistance也就是(距离终点)剩余移动距离，让它跟stopingDistance(结束距离)进行比较,remainingDistance不是立刻更新的,故需要使用前一个条件解决该问题
    }
    void Attack()
    {
        if (damageableTarget)
        {
            damageableTarget.Hit(10);
        }
    }

    /*
        public void AttackTarget(Damageable target)
        {
            StopTask();
            damageableTarget = target;

            currentTask = StartCoroutine(StartAttack());

            IEnumerator StartAttack()
            {
                while (damageableTarget)
                {
                    SetDestination(damageableTarget.transform.position);

                    yield return WaitForNavMesh();

                    while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < 4f)
                    // while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < ActorAttack.radius)
                    {
                        yield return new WaitForSeconds(1);
                        if (damageableTarget)
                        {
                            animator.SetTrigger("Attack");
                        }
                    }

                }

                currentTask = null;
            }
        } 
    */
    #region 攻击目标
    public virtual void AttackTarget(Damageable target)
    {
        StopTask();
        damageableTarget = target;

        currentTask = StartCoroutine(StartAttack());

        IEnumerator StartAttack()
        {
            while (damageableTarget)
            {
                SetDestination(damageableTarget.transform.position);

                yield return WaitForNavMesh();

                // while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < 4f)
                while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < ActorAttack.radius)
                {
                    yield return new WaitForSeconds(1);
                    if (damageableTarget)
                    {
                        animator.SetTrigger("Attack");

                        if (this.CompareTag(ActorManager.instance.PlayerStr_Get))
                        {
                            Debug.Log("resource 归真人玩家了!");
                            ComBuildingManager.instance.allResources[0].Damageable_Get.isRealPlayerOrCom = true;
                        }
                        else if (this.CompareTag(ActorManager.instance.EnemyStr_Get) && this.gameObject.layer == LayerMask.NameToLayer(ActorManager.instance.PlayerStr_Get))
                        {
                            Debug.Log("多人游戏暂时没计划!");
                        }
                        else
                        {
                            // resource 归谁的解决方法
                            Debug.Log("resource 归电脑玩家了!");
                            ComBuildingManager.instance.allResources[0].Damageable_Get.isRealPlayerOrCom = false;
                        }
                    }
                }

            }

            currentTask = null;
        }
    }
    #endregion

    #region 攻击目标（警备状态）
    public virtual void Guard()
    {
        Debug.Log("Virtual下的Actor.Guard()");
        StopTask();

        currentTask = StartCoroutine(StartGuardAttack());

        IEnumerator StartGuardAttack()
        {
            //打开警戒索敌
            ActorAttack.isGuarding = true;

            while (ActorAttack.isGuarding)
            {
                Debug.Log("We Got In!");
                if (ActorAttack.isLock)
                {
                    Debug.Log("We Are In The If Setence!");
                    damageableTarget = null;
                    // 拿取对应对象
                    // Transform attackTargetTransform = ActorAttack.GiveTarget();
                    damageableTarget = ActorAttack.GiveTarget();
                    Debug.Log("damageableTarget:" + damageableTarget);
                    // agent.SetDestination(attackTargetTransform.position);

                    // yield return WaitForNavMesh();

                    // while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < 4f)
                    while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < ActorAttack.radius)
                    {
                        Debug.Log("attackTargetTransform Still Alive !");
                        yield return new WaitForSeconds(1);
                        if (damageableTarget)
                        {
                            Debug.Log("We Are In 'if (attackTargetTransform)' !");
                            animator.SetTrigger("Attack");
                        }
                        Debug.Log("attackTargetTransform Gets Attacked!");
                    }
                    Debug.Log("attackTargetTransform Is Dead!");
                }
            }

            //暂时设置
            ActorAttack.isAttack = false;
            ActorAttack.isLock = false;
            ActorAttack.isGuarding = false;

            currentTask = null;
        }
    }

    #endregion

    #region 电脑AI单位,攻击目标（警备状态）
    public virtual void AIAttack(Damageable target)
    {
        SetDestination(target.transform.position);
        Guard();
    }
    #endregion

    public virtual void StopTask()
    {
        damageableTarget = null;
        if (currentTask != null)
        {
            StopCoroutine(currentTask);
            currentTask = null;
        }
    }

    private void OnMouseEnter()
    {
        isHover = true;
    }
    private void OnMouseExit()
    {
        isHover = false;
    }

    #region ActorCost 包裹函数
    public bool IsFinished()
    {
        return ActorCost.IsFinished();
    }

    public bool CanBuild(int[] resources)
    {
        return ActorCost.CanBuild(resources);
    }

    public void IsChangeLayer()
    {
        ActorCost.IsChangeLayer();
    }

    public void Build(int work)
    {
        ActorCost.Build(work);
    }

    #endregion

    #region Agent和Obstacle的设置方法
    public void ChangeUnitNavMeshSettingsFT()
    {
        agent.enabled = false;
        navMeshObstacle.enabled = true;
    }

    public void ChangeUnitNavMeshSettingsTF()
    {
        navMeshObstacle.enabled = false;
        agent.enabled = true;
    }

    public void ChangeUnitNavMeshSettingsFF()
    {
        navMeshObstacle.enabled = false;
        agent.enabled = false;
    }

    public bool IsUnitNavMeshSettingsTF()
    {
        return !agent.enabled;
    }

    public bool IsUnitAgent()
    {
        return agent.enabled;
    }

    public bool IsUnitObstacle()
    {
        return navMeshObstacle.enabled;
    }

    public void StayHere()
    {
        // Debug.Log("执行StayHere()了!");
        agent.SetDestination(transform.position);
    }
    #endregion
}
