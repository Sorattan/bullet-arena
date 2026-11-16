using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private BulletProjectile projectilePrefab;
    [SerializeField, Min(0.2f)] private float shotsPerSecond = 5f;
    [SerializeField] private float muzzleDistance = 0.85f;
    [SerializeField] private float muzzleHeight = 0.25f;
    private float nextShotTime;
    private PlayerHealth health;

    private void Awake() { health = GetComponent<PlayerHealth>(); }
    public void Configure(BulletProjectile prefab) { projectilePrefab = prefab; }

    private void Update()
    {
        if (projectilePrefab == null || Mouse.current == null) return;
        if (health != null && !health.IsAlive) return;
        if (!Mouse.current.leftButton.isPressed || Time.time < nextShotTime) return;

        Vector3 direction = transform.forward;
        Vector3 origin = transform.position + direction * muzzleDistance + Vector3.up * muzzleHeight;
        BulletProjectile bullet = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(direction));
        bullet.Initialize(direction);
        nextShotTime = Time.time + 1f / shotsPerSecond;
    }
}
