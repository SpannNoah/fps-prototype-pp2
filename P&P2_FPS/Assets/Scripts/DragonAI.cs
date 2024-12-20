using System.Collections;
using UnityEngine;

public class DragonAI : MonoBehaviour, IDamage
{
    [Header("Health Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private Renderer m_model = null; // Dragon model renderer for visual feedback
    [SerializeField] private Color damageColor = Color.red;
    private Color originalColor;

    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint; // Position from which the dragon attacks
    [SerializeField] private GameObject fireballPrefab; // Fireball prefab for dragon's ranged attack
    [SerializeField] private float attackCooldown = 2.0f; // Cooldown between attacks
    [SerializeField] private float fireballSpeed = 10.0f; // Speed of the fireball
    private bool canAttack = true;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;

    private Animator animator;

    private void Start()
    {
        if (m_model != null)
        {
            originalColor = m_model.material.color;
        }

        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        // Visual feedback for damage
        if (m_model != null)
        {
            StartCoroutine(DamageFlash());
        }

        // Play hit sound effect
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        Debug.Log($"Dragon took {amount} damage. Remaining health: {health}");

        // Check if the dragon should die
        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        if (m_model != null)
        {
            m_model.material.color = damageColor;
            yield return new WaitForSeconds(0.1f);
            m_model.material.color = originalColor;
        }
    }

    private void Die()
    {
        Debug.Log("Dragon has been defeated!");

        // Play death animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Play death sound effect
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Optionally destroy the dragon after animation finishes
        Destroy(gameObject, 2.0f); // Delay to allow death animation to play
    }

    private void Update()
    {
        // Check if the dragon can attack
        if (canAttack)
        {
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        // Play attack animation
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // Launch a fireball toward the player
        if (attackPoint != null && fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, attackPoint.position, Quaternion.identity);
            Rigidbody rb = fireball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = (GameManager.Instance.m_player.transform.position - attackPoint.position).normalized;
                rb.velocity = direction * fireballSpeed;
            }
        }

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}


