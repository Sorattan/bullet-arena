using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 10;

    public int CurrentHealth { get; private set; }
    public int MaxHealth => maxHealth;
    public bool IsAlive => CurrentHealth > 0;

    public event Action<int> Damaged;
    public event Action Died;

    private void Awake() { CurrentHealth = maxHealth; }

    public void TakeDamage(int amount)
    {
        if (!IsAlive || amount <= 0) return;
        int actual = Mathf.Min(CurrentHealth, amount);
        CurrentHealth -= actual;
        Damaged?.Invoke(actual);
        if (!IsAlive)
        {
            if (TryGetComponent(out PlayerShooter shooter)) shooter.enabled = false;
            if (TryGetComponent(out PlayerController movement)) movement.enabled = false;
            Died?.Invoke();
        }
    }
}
