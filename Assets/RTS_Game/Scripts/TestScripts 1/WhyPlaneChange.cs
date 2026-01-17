using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhyPlaneChange : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.LogError($"{gameObject.name} 层级：{gameObject.layer}"); // 观察运行时层级是否变为 UnFinBuilding 的层级值
        if (gameObject.layer == 9)
        { 
            Debug.LogError($"{gameObject.name} 层级：{gameObject.layer}, 并且" + "Plane的层级变成UnFinBuilding了!!!");
        }
        if (this.gameObject.layer == LayerMask.NameToLayer("UnFinBuilding"))
        {
            Debug.LogError($"{gameObject.name} 层级：{gameObject.layer}, 并且" + "Plane的层级变成UnFinBuilding了!!!");
        }
    }
}
