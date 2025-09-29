using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farmer : MonoBehaviour
{
    public int fieldID;
    public float farmerWorkNum = default;

    Actor actor;

    Building currentBuilding;
    ResourceProductionFunc RPFunc;
    public CapsuleCollider BuilderCollider;

    // Start is called before the first frame update
    void Start()
    {
        actor = transform.GetComponent<Actor>();
        actor.animationEvent.attackEvent.AddListener(DoFarm);
    }

    // Update is called once per frame
    public void Update()
    {
        if (!actor.IsIn && actor.IsUnitNavMeshSettingsTF())
        {
            actor.ChangeUnitNavMeshSettingsTF();
        }

        if (currentBuilding)
        {
            // if (IsInBoxCollider(targetBoxCollider) && !actor.navMeshObstacle.enabled && agent.enabled && !currentBuilding.IsFinished())
            if (fieldID > 0 && !actor.IsUnitObstacle() && actor.IsUnitAgent() && IsInBoxCollider(currentBuilding.GetComponent<BoxCollider>()))
            {
                actor.ChangeUnitNavMeshSettingsTF();
                actor.StayHere();
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
        return bounds.Contains(actor.transform.position);
    }
    #endregion

    #region 执行建造任务方法（包括寻路和建造）

    //任务开始前的位置检查，拿取对应目标位置坐标
    public void CheckJobPositionWithBoxCollider(Building job)
    {
        currentBuilding = job;

        RPFunc = currentBuilding.GetComponent<ResourceProductionFunc>();

        //
        Vector3 jobPosition = GiveMeJobPoint();
        //

        transform.LookAt(jobPosition);
        actor.SetDestination(jobPosition);
    }

    //给予建造任务（携程包括寻路和建造）
    public void GiveJob(Building job)
    {
        //currentBuilding = job;

        if (actor.currentTask != null)
        {
            StopCoroutine(actor.currentTask);
        }

        actor.currentTask = StartCoroutine(StartJob());

        IEnumerator StartJob()
        {
            yield return null;
            CheckJobPositionWithBoxCollider(job);

            yield return actor.WaitForNavMesh();

            actor.ChangeUnitNavMeshSettingsFT();

            while (fieldID == RPFunc.fieldID)
            {
                yield return new WaitForSeconds(1);
                actor.animator.SetTrigger("Attack");
            }

            actor.ChangeUnitNavMeshSettingsTF();

            RPFunc = null;
            actor.currentTask = null;
        }
    }
    #endregion

    #region 监听器内的方法
    //currentBuilding建造值增加
    void DoFarm()
    {
        /*检测
        Debug.Log("currentBuilding:"+ currentBuilding.name);
        */
        if (RPFunc)
        {
            RPFunc.Grow(farmerWorkNum);
            /*检测
            Debug.Log("After" + BuilderCollider.gameObject.name + "'s currentBuilding.Build(10)!!!!!!");
            */
        }
    }

    #endregion

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
