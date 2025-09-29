using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [HideInInspector] public UnityEvent onDestroy = new UnityEvent();
    [HideInInspector] public UnityEvent onHit = new UnityEvent();
    [HideInInspector] public NavMeshObstacle NavMeshObstacle;
    [SerializeField] int totalHealth = 100;
    public int currentHealth;
    public bool isRealPlayerOrCom = true;
    private void Start()
    {
        NavMeshObstacle = GetComponent<NavMeshObstacle>();
        //NavMeshObstacle.enabled = true;
        currentHealth = totalHealth;
    }

    public void Hit(int damage)
    {
        onHit.Invoke();
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Destroy();
        }
    }

    void Destroy()
    {
        onDestroy.Invoke();
        NavMeshObstacle.enabled = false;
        // 这里暂时是销毁
        
        Destroy(gameObject);
    }
}
