using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "ScriptableObject/单位数据", order = 0)]
public class UnitData : ScriptableObject
{
    public GameObject prefab;
    public float productionTime;
    public int[] costs;
    public UnitData upgradedVersion; // 升级后的单位配置
}
