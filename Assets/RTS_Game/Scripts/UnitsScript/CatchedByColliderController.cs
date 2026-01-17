using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

//[RequireComponent(typeof(Actor))]
public class CatchedByColliderController : MonoBehaviour
{
    NavMeshAgent agent;
    Building currentBuilding;
    Builder builder;
    BoxCollider targetBoxCollider; // 需要检测的BoxCollider

    
    // Start is called before the first frame update
    void Start()
    {
        builder = GetComponent<Builder>();
        agent = builder.GetComponent<NavMeshAgent>();
        currentBuilding = builder.GetComponent<Building>();
        //agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(currentBuilding && currentBuilding.IsFinished() || IsInsideBoxCollider(builder.gameObject.transform.position))
        //if(currentBuilding && currentBuilding.IsFinished() || IsInBoxCollider())

        if(currentBuilding == null)
        {
            currentBuilding = builder.GetComponent<Building>();
        }
        else
        {
            targetBoxCollider = currentBuilding.GetComponent<BoxCollider>();
            
            if(currentBuilding || IsInBoxCollider())
            {
                //老方法
                //agent.isStopped = true;
                //老方法
                //agent.ResetPath();
                //最不推荐agent.enabled = false;
                agent.SetDestination(agent.transform.position);
                //StopCoroutine(builder.currentTask);
                //yield break;
            }

            if(currentBuilding.IsFinished() && currentBuilding.gameObject.layer == LayerMask.NameToLayer("FinBuilding"))
            {
                agent.SetDestination(agent.transform.position);
                StopCoroutine(builder.currentTask);
            }
        }
    }
    
    bool IsInBoxCollider()
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

     private bool IsInsideBoxCollider(Vector3 point)
    {
        if (currentBuilding.rayCastBoxCollider == null)
        {
            return false;
        }

        // 将点转换到BoxCollider的局部空间
        Vector3 localPoint = currentBuilding.rayCastBoxCollider.transform.InverseTransformPoint(point);

        // 检查点是否在BoxCollider的边界内
        return localPoint.x >= -currentBuilding.rayCastBoxCollider.size.x / 2 && localPoint.x <= currentBuilding.rayCastBoxCollider.size.x / 2 &&
               localPoint.y >= -currentBuilding.rayCastBoxCollider.size.y / 2 && localPoint.y <= currentBuilding.rayCastBoxCollider.size.y / 2 &&
               localPoint.z >= -currentBuilding.rayCastBoxCollider.size.z / 2 && localPoint.z <= currentBuilding.rayCastBoxCollider.size.z / 2;
    }
}
