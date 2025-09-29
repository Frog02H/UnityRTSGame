using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class ResourceProductionFunc : MonoBehaviour
{
    private List<Farmer> farmersList = new List<Farmer>();
    public List<Farmer> Look_farmersList;
    public int containCount = default;
    private int ID;
    public int fieldID
    {
        get { return ID; }
        set { ID = value; }
    }
    public float resourceProductionFuncCount = default;
    public float timeForHarvest = default;
    private float timeForCount;

    private List<Transform> surface_Steps = new List<Transform>();
    public List<Transform> Look_surface_Steps;
    // public int surfaceStepNum;

    [HideInInspector] public Coroutine ResourceTask;

    // Start is called before the first frame update
    void Start()
    {
        // farmersList = new List<Farmer>();
        // surface_steps = new List<Transform>();
        FindChild();
        // surfaceStepNum = surface_Steps.Count;
    }

    // Update is called once per frame
    void Update()
    {
        //检查列表里的Builder的耕种目标是否为当前区块
        ChangeFarmers();

        if (ResourceTask == null && farmersList.Count > 0)
        {
            StartProduction();
        }
        //
        Look_farmersList = farmersList;
        Look_surface_Steps = surface_Steps;
    }

    private void ChangeFarmers()
    {
        if (farmersList.Count <= 0)
        {
            return;
        }

        // Debug.Log("farmersList.Count: "+ farmersList.Count);
        for (int i = 0; i < farmersList.Count; i++)
        {
            if (farmersList[i].fieldID != fieldID)
            {
                farmersList.RemoveAt(i);
            }
        }
    }

    private void StartProduction()
    {
        // StopTask();
        ResourceTask = StartCoroutine(Production());

        IEnumerator Production()
        {
            while (farmersList.Count > 0)
            {
                //timeForCount += * resourceProductionFuncCount * farmersList.Count;
                //
                if (timeForCount < (timeForHarvest / (surface_Steps.Count - 1)) && timeForCount > 0)
                {
                    if (!surface_Steps[0].gameObject.activeSelf)
                    {
                        surface_Steps[0].gameObject.SetActive(true);
                        surface_Steps[0].localPosition = Vector3.Lerp(Vector3.down * 1, new Vector3(0,  surface_Steps[0].localPosition.y, 0), 1);
                        Shake(0);
                    }

                }

                if (timeForCount < (timeForHarvest / ((surface_Steps.Count - 1) * 2)) && timeForCount >= (timeForHarvest / (surface_Steps.Count - 1)))
                {
                    if (!surface_Steps[1].gameObject.activeSelf)
                    {
                        surface_Steps[0].gameObject.SetActive(false);
                        surface_Steps[1].gameObject.SetActive(true);
                        Shake(1);
                    }

                    // surface_Steps[0].gameObject.SetActive(false);
                    // surface_Steps[1].gameObject.SetActive(true);
                }

                if (timeForCount >= timeForHarvest)
                {
                    // ShiftToNextStep(surface_Steps.Count - 2, surface_Steps.Count - 1);
                    // surface_Steps[surface_Steps.Count - 2].gameObject.SetActive(false); 
                    // surface_Steps[surface_Steps.Count - 1].gameObject.SetActive(true);
                    if (!surface_Steps[1].gameObject.activeSelf)
                    {
                        surface_Steps[surface_Steps.Count - 1].gameObject.SetActive(false);
                        Shake(surface_Steps.Count - 1);
                    }

                    timeForCount = 0;
                }
            }
            yield return null;
        }
    }

    public void Grow(float work)
    {
        timeForCount += work;
    }

    public void Shake(int shakeIndex)
    {
        // surface_Steps[current].localPosition = Vector3.Lerp(Vector3.down * height, new Vector3(0, originalHeight, 0), (float)currentWork / totalWorkToComplete);
        //visual
        surface_Steps[shakeIndex].DOComplete();
        surface_Steps[shakeIndex].DOShakeScale(.5f, .2f, 10, 90, true);
        // surface_Steps[current].gameObject.SetActive(false);

        //BuildingManager.instance.PlayParticle(transform.position);

        // surface_Steps[next].gameObject.SetActive(true);
    }

    public void StopTask()
    {
        farmersList.Clear();
        if (ResourceTask != null)
        {
            StopCoroutine(ResourceTask);
        }
    }

    public void AddToFarmers(Farmer farmer)
    {
        farmersList.Add(farmer);
        farmer.fieldID = fieldID;
    }

    public bool IsStopAdd()
    {
        if (farmersList.Count < containCount)
        {
            return false;
        }
        return true;
    }

    public void FindChild()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            surface_Steps.Add(transform.GetChild(i));
        }

    }
}
