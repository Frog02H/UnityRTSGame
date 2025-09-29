using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController_UnitsProducting : MonoBehaviour
{
    public List<UnitData> producibleUnits;  // 可生产单位列表
    public Queue<ProductionTask> productionQueue = new Queue<ProductionTask>();
    public Transform spawnPoint;            // 单位生成位置
    public Vector3 spawnPos;            // 单位生成位置
    public Vector3 GoTo;            // 单位生成后的前往位置


    public void AddToQueue(UnitData unit)
    {
        //对应的Cost，其实可以后期修改成只拿取对应UnitID获取资源消耗
        int[] Cost = unit.prefab.GetComponent<ActorCost>().Cost();

        if (this.CompareTag(ActorManager.instance.PlayerStr_Get))
        {
            if (ResourceManager.Instance.CanAfford(Cost))
            {
                Debug.Log("资源足够，立即生成当前单位!");
                ResourceManager.Instance.ConsumeAll(Cost);

                var task = new ProductionTask { unitData = unit, remainingTime = unit.productionTime };
                //
                task.unitData.costs = Cost;
                //
                productionQueue.Enqueue(task);
                StartCoroutine(ProcessTask(task));
            }
            else
            {
                // Debug.Log("资源不够生成当前单位!");
            }
        }

        if (this.CompareTag(ComActorManager.instance.PlayerStr_Get))
        {
            if (ComResourceManager.Instance.CanAfford(Cost))
            {
                Debug.Log("资源足够，立即生成当前单位!");
                ComResourceManager.Instance.ConsumeAll(Cost);

                var task = new ProductionTask { unitData = unit, remainingTime = unit.productionTime };
                //
                task.unitData.costs = Cost;
                //
                productionQueue.Enqueue(task);
                StartCoroutine(ProcessTask(task));
            }
            else
            {
                Debug.Log("资源不够生成当前单位!");
            }
        }
    }

    private IEnumerator ProcessTask(ProductionTask task)
    {
        ///*
        while (task.remainingTime > 0 && !task.isPaused)
        {
            task.remainingTime -= Time.deltaTime;
            yield return null;
        }
        //*/

        if (this.CompareTag(ActorManager.instance.PlayerStr_Get))
        {
            UnitProduceManager.instance.SpawnUnit(task.unitData.prefab, spawnPoint.position, this);
            // UnitProduceManager.instance.UnitGetOut(task.unitData.prefab, spawnPoint.position);
        }

        if (this.CompareTag(ComActorManager.instance.PlayerStr_Get))
        {
            ComUnitProduceManager.instance.SpawnUnit(task.unitData.prefab, spawnPoint.position, this);
            // ComUnitProduceManager.instance.UnitGetOut(task.unitData.prefab, spawnPoint.position);
        }

        productionQueue.Dequeue();

        //暂时添加，后要删除
        yield return null;
    }

    public void SetBornPos(Transform pos)
    {
        spawnPoint = pos;
        spawnPos = spawnPoint.position;
    }

    public void SetGoToPos(Vector3 pos)
    {
        GoTo = pos;
    }

    public void IsChangeLayerAndTag(string tagStr, string layerStr, GameObject prefab)
    {
        if (gameObject.layer == LayerMask.NameToLayer(layerStr) && CompareTag(tagStr))
        {
            Debug.Log("Tag和Layer已改变！");
            return;
            //如果层级已经更改则跳过此函数
        }

        Debug.Log("改变兵种单位层级ing");
        int layer = LayerMask.NameToLayer(layerStr);

        prefab.gameObject.layer = layer;
        Debug.Log("改变兵种单位标签ing");

        //暂时
        prefab.gameObject.tag = tagStr;
    }
}
