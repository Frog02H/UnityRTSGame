using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ResourceType
{
    Food = 0,
    Wood = 1,
    Steel = 2,
    Oil = 3,
    Gold = 4,
    Manpower = 5
}

public class Resource : MonoBehaviour
{
    [SerializeField] ResourceType resourceType;
    [SerializeField] int amount;
    Damageable damageable;
    public bool isHover;

    public Damageable Damageable_Get
    {
        get { return damageable; }
    }

    //HoverVisual
    private new Renderer renderer;
    private Color emissionColor;

    void Awake()
    {
        damageable = GetComponent<Damageable>();
        damageable.onDestroy.AddListener(GiveResource);
        damageable.onHit.AddListener(HitResource);

        renderer = GetComponent<Renderer>();
        if (renderer)
        {
            emissionColor = renderer.material.GetColor("_EmissionColor");
        }
    }

    void GiveResource()
    {
        // BuildingManager.instance.AddResource(resourceType, amount);
        if (damageable.isRealPlayerOrCom)
        {
            ResourceManager.Instance.AddResource(resourceType, amount);
            BuildingManager.instance.UIRefresh();
        }
        else
        {
            ComResourceManager.Instance.AddResource(resourceType, amount);
        }
    }

    void HitResource()
    {
        //visual
        transform.DOComplete();
        transform.DOShakeScale(.5f, .2f, 10, 90, true);
    }

    private void OnMouseEnter()
    {
        isHover = true;
        if (renderer)
            renderer.material.SetColor("_EmissionColor", Color.grey);
    }
    private void OnMouseExit()
    {
        isHover = false;
        if (renderer)
            renderer.material.SetColor("_EmissionColor", emissionColor);
    }
}
