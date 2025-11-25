using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] public float life = 100f;
    private Rigidbody rb;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
    }

    public void GetHit(float value, Vector3 hitDirection)
    {
        life -= value;
        Debug.Log("disparo acertado, nueva vida: " + life);

        if (rb != null)
        {
            //rb.AddForce(hitDirection * 5f, ForceMode.Impulse);
            animator.SetTrigger("Hurt");
            //StartCoroutine(DoHitAnimation());
        }

        if (life <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator DoHitAnimation()
    {
        animator.SetTrigger("Hurt");
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("Hit", false);
    }
}