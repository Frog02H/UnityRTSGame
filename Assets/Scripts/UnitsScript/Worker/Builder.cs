using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class Builder : Actor
{
    Building currentBuilding;
    public Vector3 hitInfoPoint;
    public CapsuleCollider BuilderCollider;
    public Vector3 GoTo;
    public int workNum = default;

    private void Start()
    {
        BuilderCollider = GetComponent<CapsuleCollider>();
        BuilderCollider.enabled = true;
        animationEvent.attackEvent.AddListener(DoWork);

        if(!IsIn)
        {
            ChangeUnitNavMeshSettingsTF();    
        }
    }

    public override void Update()
    {
        //Actor修改成了抽象类，可以去除virtual
        base.Update();

        if(!IsIn && IsUnitNavMeshSettingsTF())
        {
            ChangeUnitNavMeshSettingsTF();    
        }

        //如果没有Builder要建造的建筑，啥也不做
        if (currentBuilding == null)
        {

        }
        else
        {
            //如果Builder有要建造的建筑，随时检测是否贴近建筑（即重新设置路径目标位置为当前位置，实现抵达目标位置），如果完成建筑则开启agent并关闭navMeshObstacle
            BoxCollider targetBoxCollider = currentBuilding.GetComponent<BoxCollider>();

            //if(IsInBoxCollider(targetBoxCollider) && agent.enabled)
            if (IsInBoxCollider(targetBoxCollider) && !navMeshObstacle.enabled && agent.enabled && !currentBuilding.IsFinished())
            {

                // agent.SetDestination(agent.transform.position);
                SetDestination(agent.transform.position);
            }

            //当建筑完成建造,但单位仍未抵达时,则需立即停下
            Debug.Log("agent.remainingDistance > agent.stoppingDistance: " + (agent.remainingDistance > agent.stoppingDistance));
            if (agent.remainingDistance > agent.stoppingDistance && currentBuilding.IsFinished())
            {
                // StopCoroutine(currentTask);
                StopJob();
                ChangeUnitNavMeshSettingsTF();
                /* 
                if (currentBuilding.gameObject.layer == LayerMask.NameToLayer("UnFinBuilding"))
                {
                    //
                    currentBuilding.IsChangeLayerAndTag();
                    //
                } 
                */
                // agent.ResetPath(); // 清除当前路径，停止移动
                // agent.SetDestination(agent.transform.position);
                SetDestination(agent.transform.position);
                // StayHere();
            }
        }
    }

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

    #region 执行建造任务方法（包括寻路和建造）
    //任务开始前的位置检查，拿取对应目标位置坐标
    public void CheckJobPositionWithBoxCollider(Building job)
    {
        currentBuilding = job;

        //
        Vector3 jobPosition = GiveMeJobPoint();
        //

        transform.LookAt(jobPosition);
        SetDestination(jobPosition);
    }

    //失败测试，尝试先进行寻路再建造，之后可以再尝试
    public void CheckJobPositionBeforeMoveToBoxCollider(Building job)
    {
        Vector3 jobPosition;

        currentBuilding = job;

        if (currentTask != null)
        {
            StopCoroutine(currentTask);
        }

        currentTask = StartCoroutine(Go2JobPos());

        IEnumerator Go2JobPos()
        {
            yield return null;

            yield return jobPosition = GiveMeJobPoint();

            transform.LookAt(jobPosition);
            SetDestination(jobPosition);

        }
    }

    //给予建造任务（携程包括寻路和建造）
    public void GiveJob(Building job)
    {
        //currentBuilding = job;

        if (currentTask != null)
        {
            StopCoroutine(currentTask);
        }

        currentTask = StartCoroutine(StartJob());

        IEnumerator StartJob()
        {
            yield return null;
            CheckJobPositionWithBoxCollider(job);

            yield return WaitForNavMesh();

            ChangeUnitNavMeshSettingsFT();

            while (!currentBuilding.IsFinished())
            {
                yield return new WaitForSeconds(1);
                if (!currentBuilding.IsFinished())
                {
                    animator.SetTrigger("Attack");
                    //我们自己添加的transform.LookAt(currentBuilding.transform);
                }
            }

            ChangeUnitNavMeshSettingsTF();

            Debug.Log("在job.IsChangeLayer();之前！！！");
            // currentBuilding.IsChangeLayer();
            // job.IsChangeLayerAndTag();
            job.IsChangeLayerAndTag(this.gameObject.tag);
            Debug.Log("在job.IsChangeLayer();之后！！！");
            // HasBuilding = false;
            //
            currentBuilding = null;
            currentTask = null;
        }
    }
    
    //停止建造任务
    public void StopJob()
    {
        if (currentTask != null)
        {
            StopCoroutine(currentTask);
        }

        currentBuilding = null;
        currentTask = null;
    }

    #endregion

    #region 
    public override void AttackTarget(Damageable target)
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

                        if (this.CompareTag(ActorManager.instance.PlayerStr_Get))
                        {
                            // Debug.Log("resource 归真人玩家了!");
                            ComBuildingManager.instance.allResources[0].Damageable_Get.isRealPlayerOrCom = true;
                        }
                        else if (this.CompareTag(ActorManager.instance.EnemyStr_Get) && this.gameObject.layer == LayerMask.NameToLayer(ActorManager.instance.PlayerStr_Get))
                        {
                            Debug.Log("多人游戏暂时没计划!");
                        }
                        else
                        {
                            // resource 归谁的解决方法
                            // Debug.Log("resource 归电脑玩家了!");
                            ComBuildingManager.instance.allResources[0].Damageable_Get.isRealPlayerOrCom = false;
                        }
                    }
                }

            }

            currentTask = null;
        }
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
        currentBuilding = null;
    }

    //currentBuilding建造值增加
    void DoWork()
    {
        /*检测
        Debug.Log("currentBuilding:"+ currentBuilding.name);
        */
        if (currentBuilding)
        {
            currentBuilding.Build(workNum);
            /*检测
            Debug.Log("After" + BuilderCollider.gameObject.name + "'s currentBuilding.Build(10)!!!!!!");
            */
        }
    }

    public WaitUntil Builder_WaitForNavMesh()
    {

        return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);
        //agent.pathPending: Is a path  in the process of being computed but not yet ready? (Read Only)
        //remainingDistance也就是(距离终点)剩余移动距离，让它跟stopingDistance(结束距离)进行比较,remainingDistance不是立刻更新的,故需要使用前一个条件解决该问题
    }
    #endregion

/*     #region Agent和Obstacle的设置方法
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
    #endregion */

    #region 拿取目标位置
    //
    public Vector3 GiveMeJobPoint()
    {
        //Vector3 JobCornerPoint = Vector3.zero;
        // 检查currentBuilding.bottomCorners是否为空
        if (currentBuilding.bottomCorners == null || currentBuilding.bottomCorners.Length == 0)
        {
            Debug.LogError("我的提醒：No corners available in currentBuilding.bottomCorners");
            return Vector3.zero; // 或者返回一个默认值，或者抛出异常
        }

        // 初始化JobCornerPoint为第一个角点
        Vector3 JobCornerPoint = currentBuilding.bottomCorners[0];

        foreach (var corner in currentBuilding.bottomCorners)
        {
            if (JobCornerPoint == Vector3.zero)
            {
                JobCornerPoint = corner;
            }

            if (Vector3.Distance(JobCornerPoint, BuilderCollider.transform.position) < Vector3.Distance(corner, BuilderCollider.transform.position))
            {
                JobCornerPoint = corner;
            }

            Debug.Log("JobCornerPoint: " + JobCornerPoint);
        }
        return JobCornerPoint;
    }
    //
    #endregion

}
