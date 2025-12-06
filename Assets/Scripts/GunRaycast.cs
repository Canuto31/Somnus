using UnityEngine;

public class GunRaycast : MonoBehaviour
{
    public float rayDistance = 50f;
    public LayerMask layerMask;
    public EnemyAI enemyTarget;
    public bool hasHitEnemy = false;
    public Transform muzzlePoint; 
    public GameObject muzzleFlashPrefab; 

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out hit, rayDistance, layerMask))
        {
            enemyTarget = hit.collider.gameObject.GetComponent<EnemyAI>();
            Debug.DrawLine(origin, hit.point, Color.red);
            hasHitEnemy = true;
        }
        else
        {
            Debug.DrawRay(origin, direction * rayDistance, Color.green);
            enemyTarget = null;
            hasHitEnemy = false;
        }
    }

    public void showMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Destroy(flash, 0.15f);
        }
    }
}