using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public float damage = 10f;
    public float pushForce = 3f;
    public float cooldown = 0.08f;
    public bool canHit = true;
    
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            Vector3 hitDirection = (other.transform.position - transform.position).normalized;
            enemy.GetHit(damage, hitDirection);
            
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse(1f);
            }
            
            StartCoroutine(CoolDown());
        }
    }

    private System.Collections.IEnumerator CoolDown()
    {
        canHit = false;
        yield return new WaitForSeconds(cooldown);
        canHit = true;
    }
}
