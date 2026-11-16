using System;
using UnityEngine;

public class EnemyAgent : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField, Min(0f)] private float moveSpeed = 2.5f;
    [SerializeField, Min(0.5f)] private float attackRange = 1.2f;
    [SerializeField, Min(0.2f)] private float attackCooldown = 1f;
    private Transform target;
    private PlayerHealth playerHealth;
    private int currentHealth;
    private float nextAttackTime;
    private bool defeated;
    public event Action<EnemyAgent> Defeated;

    private void Awake() { currentHealth = maxHealth; }

    public void Initialize(Transform player)
    {
        target = player;
        playerHealth = player != null ? player.GetComponent<PlayerHealth>() : null;
    }

    private void Update()
    {
        if (defeated || target == null || playerHealth == null || !playerHealth.IsAlive) return;
        Vector3 difference = target.position - transform.position;
        difference.y = 0f;
        float distance = difference.magnitude;
        if (distance < 0.001f) return;

        Vector3 facing = difference / distance;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(facing), 10f * Time.deltaTime);

        if (distance > attackRange)
        {
            float step = Mathf.Min(moveSpeed * Time.deltaTime, distance - attackRange);
            transform.position += facing * step;
        }
        else if (Time.time >= nextAttackTime)
        {
            playerHealth.TakeDamage(1);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        if (defeated || amount <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth > 0) return;
        defeated = true;
        Defeated?.Invoke(this);
        Destroy(gameObject);
    }
}
