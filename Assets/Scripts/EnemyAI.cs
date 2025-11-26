using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] public float health = 100f;
    private Rigidbody rb;

    public Animator animator;

    public Slider healthBar;
    public TMP_Text healthText;
    public float maxHealth = 100;

    public TMP_Text textDamage;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        maxHealth = health;
        textDamage.enabled = false;
    }

    void Update()
    {
        healthText.text = health + " / " + maxHealth;
        healthBar.value = (float)health / (float)maxHealth;
    }

    public void GetHit(float value, Vector3 hitDirection)
    {
        health -= value;
        Debug.Log("disparo acertado, nueva vida: " + health);

        if (rb != null)
        {
            //rb.AddForce(hitDirection * 5f, ForceMode.Impulse);
            animator.SetTrigger("Hurt");
            //StartCoroutine(DoHitAnimation());
            StartCoroutine(ShowDamageCoroutine(value));
        }

        if (health <= 0f)
        {
            Destroy(gameObject);
        }
    }
    
    private IEnumerator ShowDamageCoroutine(float value)
    {
        textDamage.enabled = true;
        textDamage.text = value.ToString();

        yield return new WaitForSeconds(0.6f); // el tiempo que quieras

        textDamage.enabled = false;
    }

}