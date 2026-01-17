using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Actor
{
    // public CapsuleCollider BuilderCollider;

    private void Start()
    {
        if (!IsIn)
        {
            ChangeUnitNavMeshSettingsTF();
        }

        //ActorAttack暂时设置
        ActorAttack = GetComponent<ActorAttack>();
    }

    public override void Update()
    {
        //Actor修改成了抽象类，可以去除virtual
        base.Update();

        // ActorAttack.AAUpdate();

        if (!IsIn && IsUnitNavMeshSettingsTF())
        {
            ChangeUnitNavMeshSettingsTF();
        }

        if (ActorAttack.isLock)
        {
            StayHere();
        }

    }

    #region 攻击目标（远距离攻击）
    public override void AttackTarget(Damageable target)
    {
        //Actor基类的damageableTarget
        damageableTarget = null;
        StopTask();
        damageableTarget = target;
        //暂时设置
        ActorAttack.isAttack = true;

        //给ActorAttack的damageableTarget赋值,其实不太好
        // ActorAttack.GetTarget(damageableTarget);
        ActorAttack.DamageableTarget = damageableTarget;

        currentTask = StartCoroutine(StartAttack());

        IEnumerator StartAttack()
        {
            //打开警戒索敌
            ActorAttack.isGuarding = true;

            while (damageableTarget)
            {
                if (!ActorAttack.isGather && damageableTarget.transform.GetComponent<Resource>())
                {
                    ActorAttack.isAttack = false;
                    ActorAttack.isLock = false;
                    ActorAttack.isGuardAttack = false;
                    break;
                }
                //将该单位添加进侦察队列
                ActorAttack.AddTarget();
                //
                transform.LookAt(damageableTarget.transform.position);
                // agent.SetDestination(damageableTarget.transform.position);
                // 暂时
                if (!ActorAttack.isLock)
                {
                    SetDestination(damageableTarget.transform.position);
                }
                ///
                // SetDestination(damageableTarget.transform.position);
                
                // Debug.Log("正在WaitForNavMesh()！");

                yield return WaitForNavMesh();

                // Debug.Log("WaitForNavMesh()完成了！");

                // while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < 4f)
                while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < ActorAttack.RealRadius_Get)
                {
                    yield return new WaitForSeconds(0.5f);
                    if (damageableTarget)
                    {
                        animator.SetTrigger("Attack");
                    }
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

    #region 攻击目标（警备状态）
    public override void Guard()
    {
        // Debug.Log("Soldier.Guard()!");

        StopTask();

        currentTask = StartCoroutine(StartGuardAttack());

        IEnumerator StartGuardAttack()
        {
            //打开警戒索敌
            ActorAttack.isGuarding = true;

            while (ActorAttack.isGuarding)
            {
                // Debug.Log("We Got In!");
                if (ActorAttack.isLock)
                {
                    // Debug.Log("We Are In The If Setence!");
                    // damageableTarget = null;
                    // 拿取对应对象
                    // Transform attackTargetTransform = ActorAttack.GiveTarget();

                    // 这里是Actor的damagebleTarget赋值的地方！！！
                    damageableTarget = ActorAttack.GiveTarget();

                    ///* 
                    if (damageableTarget == null || damageableTarget.currentHealth <= 0)
                    {
                        // damageableTarget = null;
                        // ActorAttack.isGuarding = false;
                        ActorAttack.isAttack = false;
                        ActorAttack.isLock = false;
                        ActorAttack.isGuardAttack = false;
                        break;
                    }

                    if (!ActorAttack.isGather && damageableTarget.transform.GetComponent<Resource>())
                    {
                        ActorAttack.isAttack = false;
                        ActorAttack.isLock = false;
                        ActorAttack.isGuardAttack = false;
                        break;
                    }
                    //*/

                    // Debug.Log("damageableTarget:" + damageableTarget);
                    // agent.SetDestination(attackTargetTransform.position);

                    //因为已经Lock了，所以可以直接true
                    ActorAttack.isGuardAttack = true;
                    // 这里是单位面朝攻击对象
                    transform.LookAt(damageableTarget.transform.position);

                    // yield return WaitForNavMesh();

                    // while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < 4f)

                    while (damageableTarget && Vector3.Distance(damageableTarget.transform.position, transform.position) < ActorAttack.RealRadius_Get)
                    {
                        // Debug.Log("attackTargetTransform Still Alive !");
                        yield return new WaitForSeconds(0.5f);
                        if (damageableTarget)
                        {
                            // Debug.Log("We Are In 'if (attackTargetTransform)' !");
                            animator.SetTrigger("Attack");
                        }
                        // Debug.Log("attackTargetTransform Gets Attacked!");
                    }
                    // Debug.Log("attackTargetTransform Is Dead!");
                }
            }

            //暂时设置
            ActorAttack.isAttack = false;
            ActorAttack.isLock = false;
            // ActorAttack.isGuarding = false;
            ActorAttack.isGuardAttack = false;

            currentTask = null;
        }
    }

    #endregion

    #region 电脑AI单位,攻击目标（警备状态）
    public override void AIAttack(Damageable target)
    {
        ActorAttack.isAttack = true;

        // if (ActorAttack.detect.Count <= 0)
        if (ActorAttack.detect.Count > 0 && ActorAttack.DamageableTarget != target)
        {
            ActorAttack.detect.Clear();
        }
        // ActorAttack.detect.Clear();
        // if (ActorAttack.DamageableTarget != target && !ActorAttack.isLock)
        if (ActorAttack.DamageableTarget != target && !ActorAttack.isLock)
        {
            AttackTarget(target);
        }
        // AttackTarget(target);

        /* 
        if (ActorAttack.detect.Count <= 0)
        {
            AttackTarget(target);
        }

        if (ActorAttack.detect.Count > 0)
        {
            AttackTarget(target);
        } 
        */

        // AttackTarget(target);
        ///////
        // SetDestination(target.transform.position);
        // ActorAttack.detect.Add(target);
        // Guard();
    }
    #endregion

    #region 检测是否在包裹体内
    //检测是否在建筑的包裹体内，在内则返回True，反之返回False
    bool IsInBoxCollider(BoxCollider targetBoxCollider)
    {
        if (targetBoxCollider == null)
        {
            Debug.LogWarning("Target BoxCollider is not assigned!");
            return false;
        }

        // 获取BoxCollider的边界
        Bounds bounds = targetBoxCollider.bounds;

        // 检测当前物体的位置是否在BoxCollider范围内
        return bounds.Contains(agent.transform.position);
    }
    #endregion

    #region 常用方法
    public bool HasTask()
    {
        return currentTask != null;
    }
    override public void StopTask()
    {
        base.StopTask();
    }

    #endregion

    #region Agent和Obstacle的设置方法

    /*
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

    public void StayHere()
    {
        agent.SetDestination(transform.position);
    }
    
    */
    #endregion
}