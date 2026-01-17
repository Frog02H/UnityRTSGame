using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

public class ActorAttack : MonoBehaviour
{
    [SerializeField] float height;
    [SerializeField] private LayerMask targetLayers; // 在Inspector中勾选多个层
    public float radius = default;
    private float realRadius;
    public float RealRadius_Get
    {
        get { return realRadius; }
    }
    public bool isHover = false;
    //
    Actor actor;
    // public Vector3 selfProjectionPos;
    // public Vector3 targetProjectionPos;
    // private List<Collider> detect;
    // public List<Collider> detect;
    public List<Damageable> detect;
    public List<Damageable> attack;
    // private Transform targetTransform;
    /* 
    public List<Collider> Detect
    {
        get { return Detect; }
        set { Detect = detect; }
    } 
    */
    private Damageable damageableTarget;

    public Damageable DamageableTarget
    {
        get { return damageableTarget; }
        set { damageableTarget = value; }
    }
    // public LayerMask layerMask;
    //

    public bool isAttack = false;
    public bool isGuarding = false;
    private bool isChange = false;
    public bool isLock = false;
    public bool isGuardAttack = false;
    public bool isGather = false;

    private void Awake()
    {
        actor = GetComponent<Actor>();
        //detect = new List<Collider>();
        detect = new List<Damageable>();
        attack = new List<Damageable>();
    }

    void Start()
    {
        if (actor == null)
        {
            actor = GetComponent<Actor>();
        }

        if (detect == null)
        {
            //detect = new List<Collider>();
            detect = new List<Damageable>();
        }

        if (attack == null)
        {
            attack = new List<Damageable>();
        }
    }

    private void Update()
    {
        AAUpdate();
    }

    public void AAUpdate()
    {
        if (isAttack)
        {
            ClickAttack();
        }
        else if (isGuarding)
        {
            GuardAttack();
        }

    }

    public void ClickAttack()
    {
        // if (!isLock && targetTransform)
        if (!isLock && damageableTarget)
        {
            // AttackRangeCheck(transform, damageableTarget.transform, radius);

            if (damageableTarget.transform.GetComponent<Building>())
            {
                // Debug.Log("ClickAttack()正在执行对建筑的攻击LOCK判定！");
                realRadius = radius + damageableTarget.transform.GetComponent<Building>().radius;
                //AttackRangeCheck(transform, damageableTarget.transform, radius + damageableTarget.transform.GetComponent<Building>().radius);
            }

            if (damageableTarget.transform.GetComponent<Actor>())
            {
                // Debug.Log("ClickAttack()正在执行对单位的攻击LOCK判定！");
                realRadius = radius;
                //AttackRangeCheck(transform, damageableTarget.transform, radius);
            }

            // AttackRangeCheck(transform, detect[0].transform, realRadius);
            AttackRangeCheck(transform, damageableTarget.transform, realRadius);
        }

        if (detect.Count > 0 && detect[0] == null)
        {
            Debug.Log("detect[0] == null: " + detect[0] == null);
        }

        //移除已经消灭的单位
        if (detect.Count > 0 && (detect[0].currentHealth <= 0 || detect[0] == null))
        {
            detect.RemoveAt(0);
        }

        /*
        if (!isLock && detect.Count > 0)
        {
            //在攻击范围内就Lock
            AttackRangeCheck(transform, detect[0].transform, radius);
        } 
        */
    }

    public void GuardAttack()
    {
        // Debug.Log("SearchNearUnits()执行前");
        SearchNearUnits();
        // Debug.Log("SearchNearUnits()执行后");

        if (detect.Count > 0 && detect[0] == null)
        {
            Debug.Log("detect[0] == null: " + detect[0] == null);
        }

        //移除已经消灭的单位
        if (detect.Count > 0 && (detect[0].currentHealth <= 0 || detect[0] == null))
        {
            detect.RemoveAt(0);
        }

        if (!isLock && detect.Count > 0)
        {
            Debug.Log("GuardAttack()准备LOCK对象！");
            //在攻击范围内就Lock
            // AttackRangeCheck(transform, detect[0].transform, radius);
            if (detect[0].transform.GetComponent<Building>())
            {
                Debug.Log("GuardAttack()正在执行对建筑的攻击LOCK判定！");
                realRadius = radius + detect[0].transform.GetComponent<Building>().radius;
                //AttackRangeCheck(transform, detect[0].transform, radius + detect[0].transform.GetComponent<Building>().radius);
            }

            if (detect[0].transform.GetComponent<Actor>())
            {
                Debug.Log("GuardAttack()正在执行对单位的攻击LOCK判定！");
                realRadius = radius;
                //AttackRangeCheck(transform, detect[0].transform, radius);
            }

            AttackRangeCheck(transform, detect[0].transform, realRadius);
        }
    }



    /*
        public void OldAttackInUpdate()
        {
            if (isGuarding)
            {
                SearchNearUnits();
            }

            if (!isLock && detect.Count > 0)
            {
                //移除已经消灭的单位
                if (detect[0].transform == null)
                {
                    detect.RemoveAt(0);
                }

                AttackRangeCheck(transform, detect[0].transform, radius);
            }
        }
    */

    public void AttackRangeCheck(Transform self, Transform target, float radius)
    {
        // selfProjectionPos = self.position;
        // targetProjectionPos = target.position;

        if (Vector3.Distance(self.position, target.position) <= radius)
        {
            isLock = true;
        }
        else
        {
            isLock = false;
        }
    }

    public void AddTarget()
    {
        // Collider collider = damageableTarget.transform.GetComponent<Collider>();
        // if (detect.Count > 0 && detect[detect.Count - 1] == collider)
        if (detect.Count > 0 && detect[detect.Count - 1] == damageableTarget)
        {
            return;
        }
        // detect.Add(collider);
        // Debug.Log("正确的添加！");
        detect.Add(damageableTarget);
    }

    public void SearchNearUnits()
    {
        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
        // int layers = 1 << LayerMask.NameToLayer("Enemy") || 1 << LayerMask.NameToLayer("Player") || 1 << LayerMask.NameToLayer("UnFinBuilding") || 1 << LayerMask.NameToLayer("FinBuilding") || 1 << LayerMask.NameToLayer("UnFinBuilding") || ;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, targetLayers);

        /* !!!!!!!!!!!!!调试：输出检测到的物体及其层
        foreach (Collider col in colliders)
        {
            Debug.Log($"检测者: {this.gameObject.name}");
            Debug.Log($"检测到物体: {col.gameObject.name}, 层: {LayerMask.LayerToName(col.gameObject.layer)}");
        }
        */

        if (colliders.Length <= 0)
        {
            return;
        }

        /*
                for (int i = 0; i < colliders.Length; i++)
                {
                    print(colliders[i].gameObject.name);
                }
        */

        if (colliders.Length == detect.Count)
        {
            isChange = false;
            return;
        }
        else
        {
            isChange = true;
        }

        detect.Clear();
        //每次清空了过后，如果当前攻击对象存在，则一定要往攻击检测队列里添加当前攻击对象
        /*
        if (damageableTarget)
        {
            detect.Add(damageableTarget);
        }
        */


        foreach (Collider target in colliders)
        {
            // 排除自身
            if (target.gameObject == gameObject)
            {
                continue;
            }

            /*
            if (target.CompareTag("Ground"))
            {
                continue;
            }
            */

            // detect.Add(target);
            // if (target.CompareTag("Enemy"))
            if (!target.CompareTag(this.gameObject.tag))
            // if (!target.CompareTag(this.gameObject.tag) && ((targetLayers.value & (1 << target.gameObject.layer)) != 0))
            {
                detect.Add(target.GetComponent<Damageable>());
                Debug.Log("target:" + target.gameObject.name);
            }
            // detect.Add(target.GetComponent<Damageable>());

            // Debug.Log("colliders:" + target.gameObject.name);
        }

        if (detect.Count <= 0)
        {
            Debug.Log("detect.Count <= 0");
            return;
        }

        if (detect.Count > 0)
        {
            Debug.Log("//////////////////////////////////////");
            RomveUpdateDetect();
        }

        //detect.AddRange(colliders);
        // foreach (Collider target in detect)
        Debug.Log("detect.Count: " + detect.Count);
        foreach (Damageable target in detect)
        {
            Debug.Log("detect:" + target.gameObject.name);
        }
    }

    /*     public void GetTarget(Damageable damageableTarget)
        {
            targetTransform = damageableTarget.transform;
        } */

    public void GetTarget(Damageable Target)
    {
        damageableTarget = Target;
    }

    /*
    public Collider GiveTargetInCollider()
    {
        return detect[0];
    }
    */
    public Damageable GiveTargetInDamageable()
    {
        return detect[0];
    }

    /*     public Transform GiveTarget()
        {
            damageableTarget = detect[0].transform.GetComponent<Damageable>();
            actor.damageableTarget = damageableTarget;
            return detect[0].transform;
        } */

    public Damageable GiveTarget()
    {

        // if(detect[0] == null || detect.Count <= 0)
        if (detect.Count <= 0)
        {
            Debug.Log("return null了");
            return null;
        }
        Debug.Log("如果前面有null,就不对了啊!这里是damageableTarget = detect[0].transform.GetComponent<Damageable>();");
        // damageableTarget = detect[0].transform.GetComponent<Damageable>();
        damageableTarget = detect[0];
        return damageableTarget;
    }

    public bool IsDetect()
    {
        // if(detect.Count > 0 || detect[0] != null)
        if (detect.Count > 0)
        {
            return true;
        }
        return false;
    }

    /* 
    public bool IsGuardAttack()
    {
        if(damageableTarget)
        {
            return true;
        }
        return false;
    } 
    */
    public void CancelAllAttackBool()
    {
        isAttack = false;
        isLock = false;
        isGuardAttack = false;
        isGuarding = false;
        actor.StopTask();
        // actor.currentTask = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


    #region 清理队列用的常规方法
    public void RomveUpdateDetect()
    {
        for (int i = 0; i < detect.Count; i++)
        {
            if (detect[i] == null)
            {
                detect.RemoveAt(i);
            }
        }
    }
    #endregion
    //
}
