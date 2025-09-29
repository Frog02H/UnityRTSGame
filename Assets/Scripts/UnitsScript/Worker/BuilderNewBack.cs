using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;

public class BuilderNewBack : Actor
{
    Building currentBuilding;
    public Vector3 hitInfoPoint;
    public CapsuleCollider BuilderCollider;
    public Vector3 GoTo;
    public bool IsHit = false;
    public bool HasBuilding = false;
    public bool IsInPos = false;
    //public GameObject PlayerPlane;
    //public CatchedByColliderController catchedByColliderController;
    private void Start()
    {
        BuilderCollider = GetComponent<CapsuleCollider>();
        BuilderCollider.enabled = true;
        animationEvent.attackEvent.AddListener(DoWork);
        //PlayerPlane = transform.GetChild(3).gameObject;
        /*检测
        Debug.Log("DoWork已经向attackEvent插入监听器。");
        */
        //catchedByColliderController.currentBuilding = currentBuilding;
    }

    public override void Update()
    {
        //Actor修改成了抽象类，可以去除virtual
        base.Update();

        //如果没有Builder要建造的建筑，啥也不做
        if(currentBuilding == null)
        {

        }
        else 
        {
            Debug.Log("");
            //如果Builder有要建造的建筑，随时检测是否贴近建筑（即重新设置路径目标位置为当前位置，实现抵达目标位置），如果完成建筑则开启agent并关闭navMeshObstacle
            BoxCollider targetBoxCollider = currentBuilding.GetComponent<BoxCollider>();
            
            //if(IsInBoxCollider(targetBoxCollider) && agent.enabled)
            if(IsInBoxCollider(targetBoxCollider) && !navMeshObstacle.enabled && agent.enabled && !currentBuilding.IsFinished())
            {
                //老方法
                //agent.isStopped = true;
                //老方法
                //agent.ResetPath();
                //最不推荐agent.enabled = false;
                agent.SetDestination(agent.transform.position);
                //Plane.GetComponent<NavMeshModifier>().enabled = true;
                //PlayerPlane.GetComponent<NavMeshModifierVolume>().enabled = true;
                //ChangeUnitNavMeshSettingsFT();
                //StopCoroutine(builder.currentTask);
                //yield break;
            }

            if(currentBuilding.IsFinished())
            {
                StopCoroutine(currentTask);
                ChangeUnitNavMeshSettingsTF();
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
/*
    //射线检测Builder要抵达并建造的位置，检测过于单一死板不全面，不好用
    public void CheckJobPosition(Building job)
    {
        Vector3 jobPosition = job.transform.position;
        
        Ray ray = new(transform.position, jobPosition);
        RaycastHit hitInfo;

        LayerMask layerMask = 1 << LayerMask.NameToLayer("UnFinBuilding");
            
            Debug.Log("↓↓↓↓↓↓-" + BuilderCollider.gameObject.name + "的Raycast的if前-↓↓↓↓↓↓");
            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity ,layerMask.value))
            {
                jobPosition.x = hitInfo.point.x;
                jobPosition.z = hitInfo.point.z;
                Debug.Log("碰撞点的建筑为：" + hitInfo.collider.name);
                Debug.Log(BuilderCollider.gameObject.name + "的hitInfo已经更新。");
                hitInfoPoint = jobPosition;
                transform.LookAt(job.transform);
                SetDestination(jobPosition);
                IsHit = true;
            }
            else
            {
                Debug.Log(BuilderCollider.gameObject.name + "的Raycast没有碰撞啊！");
                IsHit = false;
            }
            Debug.Log("↑↑↑↑↑↑-" + BuilderCollider.gameObject.name + "的Raycast的if后-↑↑↑↑↑↑");
    
    } 
*/
#region 执行建造任务方法（包括寻路和建造）
    //任务开始前的位置检查，拿取对应目标位置坐标
    public void CheckJobPositionWithBoxCollider(Building job)
    {
        //Vector3 jobPosition = job.transform.position;

        //
        //HasBuilding = true;
        //catchedByColliderController.currentBuilding = job;
        //
        currentBuilding = job;

        //transform.LookAt(jobPosition);

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
            
            //StopCoroutine(Go2JobPos());
            //StopCoroutine(currentTask);
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

        //CheckJobPositionBeforeMoveToBoxCollider(job);
        //CheckJobPositionWithBoxCollider(job);

        currentTask = StartCoroutine(StartJob());

        IEnumerator StartJob()
        {
            //catchedByColliderController.currentBuilding = currentBuilding;
            
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
            //currentBuilding.IsChangeLayer();
            job.IsChangeLayerAndTag();
            Debug.Log("在job.IsChangeLayer();之后！！！");
            //HasBuilding = false;
            //
            currentBuilding = null;
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
            currentBuilding.Build(10);
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

#region Agent和Obstacle的设置方法
    public void ChangeUnitNavMeshSettingsFT()
    {
        agent.enabled = false;
        navMeshObstacle.enabled = true;
        //navMeshObstacle.carving = true;
        //agent.isStopped = true;
        //agent.ResetPath();
    }

    public void ChangeUnitNavMeshSettingsTF()
    {
        navMeshObstacle.enabled = false;
        //navMeshObstacle.carving = false;
        agent.enabled = true;
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
            if(JobCornerPoint == Vector3.zero)
            {
                JobCornerPoint = corner;
            }

            if( Vector3.Distance(JobCornerPoint, BuilderCollider.transform.position) < Vector3.Distance(corner, BuilderCollider.transform.position) )
            {
                JobCornerPoint = corner;
            }

            Debug.Log("JobCornerPoint: " + JobCornerPoint);
        }
        return JobCornerPoint;
    }
    //
    
    public bool BoxCircleIntersect(Vector2 c, Vector2 h, Vector2 p, float r)
    {
        Vector2 v = p - c;
        // 对向量v的每个分量取绝对值
        v.x = Mathf.Abs(v.x);
        v.y = Mathf.Abs(v.y);

        Vector2 u = Vector2.Max(v - h, Vector2.zero);

        // 计算u的平方长度,Vector2.Dot(u,u)同理，都是点积
        float uSquaredLength = u.x * u.x + u.y * u.y;

        // 判断u的平方长度是否小于等于半径r的平方
        return uSquaredLength <= r * r;
    }

    public List<Vector2> GetIntersectionPoints(Vector2 c, Vector2 h, Vector2 p, float r)
    {
        List<Vector2> intersectionPoints = new List<Vector2>();

        // 计算圆心到矩形中心的向量
        Vector2 v = p - c;

        // 将矩形和圆的相对位置转换为标准位置
        Vector2 stdV = new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
        Vector2 stdH = new Vector2(h.x, h.y);

        // 检查圆心是否在矩形内部
        bool inside = stdV.x <= stdH.x && stdV.y <= stdH.y;

        // 计算交点
        if (inside)
        {
            // 圆心在矩形内部，计算圆与矩形边的交点
            if (stdV.x + r >= stdH.x)
            {
                float y = Mathf.Sqrt(r * r - (stdH.x - stdV.x) * (stdH.x - stdV.x));
                intersectionPoints.Add(new Vector2(c.x + h.x * Mathf.Sign(v.x), c.y + y * Mathf.Sign(v.y)));
                intersectionPoints.Add(new Vector2(c.x + h.x * Mathf.Sign(v.x), c.y - y * Mathf.Sign(v.y)));
            }
            if (stdV.y + r >= stdH.y)
            {
                float x = Mathf.Sqrt(r * r - (stdH.y - stdV.y) * (stdH.y - stdV.y));
                intersectionPoints.Add(new Vector2(c.x + x * Mathf.Sign(v.x), c.y + h.y * Mathf.Sign(v.y)));
                intersectionPoints.Add(new Vector2(c.x - x * Mathf.Sign(v.x), c.y + h.y * Mathf.Sign(v.y)));
            }
        }
        else
        {
            // 圆心在矩形外部，计算圆与矩形边或角的交点
            if (stdV.x - r <= stdH.x)
            {
                float y = Mathf.Sqrt(r * r - (stdV.x - stdH.x) * (stdV.x - stdH.x));
                intersectionPoints.Add(new Vector2(c.x + h.x * Mathf.Sign(v.x), c.y + y * Mathf.Sign(v.y)));
                intersectionPoints.Add(new Vector2(c.x + h.x * Mathf.Sign(v.x), c.y - y * Mathf.Sign(v.y)));
            }
            if (stdV.y - r <= stdH.y)
            {
                float x = Mathf.Sqrt(r * r - (stdV.y - stdH.y) * (stdV.y - stdH.y));
                intersectionPoints.Add(new Vector2(c.x + x * Mathf.Sign(v.x), c.y + h.y * Mathf.Sign(v.y)));
                intersectionPoints.Add(new Vector2(c.x - x * Mathf.Sign(v.x), c.y + h.y * Mathf.Sign(v.y)));
            }

            // 检查圆是否与矩形的角相交
            Vector2[] corners = new Vector2[]
            {
                new Vector2(c.x + h.x, c.y + h.y),
                new Vector2(c.x + h.x, c.y - h.y),
                new Vector2(c.x - h.x, c.y + h.y),
                new Vector2(c.x - h.x, c.y - h.y)
            };

            foreach (Vector2 corner in corners)
            {
                if (Vector2.Distance(corner, p) <= r)
                {
                    intersectionPoints.Add(corner);
                }
            }
        }

        // 去除重复的交点
        intersectionPoints = RemoveDuplicates(intersectionPoints);

        return intersectionPoints;
    }

    private List<Vector2> RemoveDuplicates(List<Vector2> points)
    {
        List<Vector2> uniquePoints = new List<Vector2>();
        foreach (Vector2 point in points)
        {
            if (!uniquePoints.Contains(point))
            {
                uniquePoints.Add(point);
            }
        }
        return uniquePoints;
    }

#endregion

}
