using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    [SerializeField, Min(1f)] private float speed = 18f;
    [SerializeField, Min(0.1f)] private float lifetime = 2.5f;
    [SerializeField, Min(1)] private int damage = 1;
    private Vector3 direction;
    private bool initialized;
    private bool hasHit;

    public void Initialize(Vector3 shotDirection)
    {
        direction = shotDirection.normalized;
        initialized = direction.sqrMagnitude > 0.01f;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (!initialized || hasHit) return;
        transform.position += direction * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasHit) return;
        if (other.TryGetComponent(out EnemyAgent enemy))
        {
            hasHit = true;
            enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
