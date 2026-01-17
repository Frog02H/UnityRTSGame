using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
这玩意不使用,别看了哥们!
这玩意不使用,别看了哥们!
这玩意不使用,别看了哥们!
这玩意不使用,别看了哥们!
这玩意不使用,别看了哥们!
*/
public class UnitProducingManager : MonoBehaviour
{
    public static UnitProducingManager instance;
    List<Building> allBuildings = new List<Building>();
    public List<Actor> allNewUnits = new List<Actor>();
    List<Building> ProductorBuildings = new List<Building>();
    
    Building currentBuilding;
    public Actor[] newUnitPrefabs = default;
    public int[] currentResources = default;

    [SerializeField] private ParticleSystem buildParticle;
    [SerializeField] private ParticleSystem finishParticle;
    BuildingOfUnitProductionUI ui;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentResources = new int[] { 999, 900, 999, 999, 999, 999};
        ui = FindObjectOfType<BuildingOfUnitProductionUI>();
        if (ui)
        {
            ui.RefreshResources();
        }
        allBuildings = BuildingManager.instance.GetBuildings();
    }

    private void Update()
    {
        
    }

    public void AddProducingBuilding(Building b)
    {
        ProductorBuildings.Add(b);
    }
    public void RemoveProducingBuilding(Building b)
    {
        ProductorBuildings.Remove(b);
    }

    public void SetNewUnitPrefabs(Actor[] BuildingUnitPrefabs)
    {
        newUnitPrefabs = BuildingUnitPrefabs;
    }
    
    public void SpawnBuilding(int index, Vector3 position)
    {
        // 预先拿取一个对象自带的预制体，使用预制体内的方法检查能不能建设
        Actor unit = newUnitPrefabs[index];
        if (!unit.CanBuild(currentResources))
        {
            return;
        }

        // Create Building,重新new一个（此处做法）或从对象池里拿取一个（此处没写）
        unit = Instantiate(newUnitPrefabs[index], position, Quaternion.identity);
        allNewUnits.Add(unit);

        /*
        StartCoroutine(BuildFor5Seconds());

        IEnumerator BuildFor5Seconds()
        {
            while (!building.IsFinished())
            {
                yield return new WaitForSeconds(1);
                if (!building.IsFinished())
                {
                    animator.SetTrigger("Attack");
                    building.Build(10);
                }
            }
        }
        */

        //设置一个监听器，监听是否RemoveBuilding(building)
        //!building.attackable.onDestroy.AddListener(() => RemoveBuilding(building));!
        
        
        // Give builders build task
        /* foreach (Actor actor in ActorManager.instance.selectedActors)
        {
            if (actor is Builder)
            {
                Builder builder = actor as Builder;
                if (!builder.HasTask())
                {
                    //builder.CheckJobPosition(building);
                    //builder.CheckJobPositionWithBoxCollider(building);
                    //builder.CheckJobPositionBeforeMoveToBoxCollider(building);
                    // builder.GiveJob(unit);
                }
            }
        } */
        currentBuilding.GiveJob(unit);
        
        // Subtract resources
        int[] cost = unit.ActorCost.Cost();
        /*检测
        Debug.Log("cost.Length:"+cost.Length);
        */
        for (int i = 0; i < cost.Length; i++)
        {
            currentResources[i] -= cost[i];
            if (ui)
            {
                ui.RefreshResources();
            }
        }

        //
    }

    public List<Actor> GetUnits()
    {
        return allNewUnits;
    }
    public Actor GetPrefab(int index)
    {
        return newUnitPrefabs[index];
    }

    public Actor GetRandomNewUnits()
    {
        if (allNewUnits.Count > 0)
        {
            return allNewUnits[Random.Range(0, allNewUnits.Count)];
        }
        else
        {
            return null;
        }
    }
    public void RemoveNewUnits(Actor unit)
    {
        allNewUnits.Remove(unit);
    }
    public void AddResource(ResourceType resourceType, int amount)
    {
        currentResources[(int)resourceType] += amount;

        if(ui)
        {
            ui.RefreshResources();
        }
    }
    public void PlayParticle(Vector3 position)
    {
        if (buildParticle)
        {
            buildParticle.transform.position = position;
            buildParticle.Play();
        }
    }
    
}
