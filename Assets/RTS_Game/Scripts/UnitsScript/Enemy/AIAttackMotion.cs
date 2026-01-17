using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

public class AIAttackMotion : MonoBehaviour
{
    Actor actor;
    string tagStr;
    int layer;

    private void Awake()
    {
        // actorAttack = GetComponent<ActorAttack>();
        actor = GetComponent<Actor>();
        tagStr = actor.gameObject.tag;
        layer = gameObject.layer;
    }

    void Start()
    {
        if (!actor)
        {
            actor = GetComponent<Actor>();
            tagStr = actor.gameObject.tag;
            layer = gameObject.layer;
        }

        /*
        if (actorAttack == null)
        {
            actorAttack = GetComponent<ActorAttack>();
        }
        */

        if (actor.CompareTag("Enemy"))
        {
            if (actor.ActorAttack)
            {
                if (!actor.IsUnitAgent())
                {
                    actor.ChangeUnitNavMeshSettingsTF();
                }
                actor.ActorAttack.isGuarding = true;
            }
            else
            {

            }
        }
    }

    private void Update()
    {
        AAUpdate();
    }

    public void AAUpdate()
    {
        if (this.CompareTag("Enemy"))
        {
            if (actor.ActorAttack)
            {
                if (!actor.IsUnitAgent())
                {
                    actor.ChangeUnitNavMeshSettingsTF();
                }
                actor.ActorAttack.isGuarding = true;

                /*
                if (actor.ActorAttack.isLock && !actor.ActorAttack.isGuardAttack)
                {
                    // Debug.Log("actor.Guard();");
                    actor.Guard();
                }
                */
            }
            else
            {

            }
        }

        /*
        if (actor.ActorAttack.isLock && !actor.ActorAttack.isGuardAttack)
        {
            // Debug.Log("actor.Guard();");
            actor.Guard();
        } 
        */
    }

    /*
    public void ClickAttack()
    {
        // if (!isLock && targetTransform)
        if (!isLock && damageableTarget)
        {
            AttackRangeCheck(transform, damageableTarget.transform, radius);
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
        ///
    }

    public void GuardAttack()
    {
        SearchNearUnits();

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
            //在攻击范围内就Lock
            AttackRangeCheck(transform, detect[0].transform, radius);
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
    ///

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
        Collider collider = damageableTarget.transform.GetComponent<Collider>();
        // if (detect.Count > 0 && detect[detect.Count - 1] == collider)
        if (detect.Count > 0 && detect[detect.Count - 1] == damageableTarget)
        {
            return;
        }
        // detect.Add(collider);
        detect.Add(damageableTarget);
    }
    
    public void SearchNearUnits()
    {
        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));

        if (colliders.Length <= 0)
        {
            return;
        }

        /*
                for (int i = 0; i < colliders.Length; i++)
                {
                    print(colliders[i].gameObject.name);
                }
        ///

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

        foreach (Collider target in colliders)
        {
            // detect.Add(target);
            detect.Add(target.GetComponent<Damageable>());

            Debug.Log("colliders:" + target.gameObject.name);
        }

        Debug.Log("//////////////////////////////////////");

        //detect.AddRange(colliders);
        // foreach (Collider target in detect)
        foreach (Damageable target in detect)
        {
            Debug.Log("detect:" + target.gameObject.name);
        }
    }

    /*     public void GetTarget(Damageable damageableTarget)
        {
            targetTransform = damageableTarget.transform;
        } 
    ///

    public void GetTarget(Damageable Target)
    {
        damageableTarget = Target;
    }

    /*
    public Collider GiveTargetInCollider()
    {
        return detect[0];
    }
    ///
    public Damageable GiveTargetInDamageable()
    {
        return detect[0];
    }

    /*     
    public Transform GiveTarget()
        {
            damageableTarget = detect[0].transform.GetComponent<Damageable>();
            actor.damageableTarget = damageableTarget;
            return detect[0].transform;
        } 
    ///

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
    ///

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    */
}
