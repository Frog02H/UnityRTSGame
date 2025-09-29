using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempUIManager : MonoBehaviour
{
    public Transform BuildingGroup;
    public Transform UnitGroup;
    public Transform ActionkeyGroup;

    void Start()
    {
        BuildingGroup = transform.GetChild(0);
        UnitGroup = transform.GetChild(1);
        ActionkeyGroup = transform.GetChild(2);
    }
}
