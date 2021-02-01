using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController
{
    public delegate void DeathDelegate();
    DeathDelegate _deathDelegate;
    public float currentHealth;
    private float _maxHealth;
    private bool _lowHealth;

    public LifeController(float initialhealth, DeathDelegate deathEvent)
    {
        _maxHealth = initialhealth;
        currentHealth = _maxHealth;
        _deathDelegate = deathEvent;
    }

    public void GetDamage(float damage)
    {
        currentHealth -= damage;      

        if (currentHealth <= 0) _deathDelegate.Invoke();
    }

    public void GetHeal(float heal, bool isBoss)
    {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);
        if (isBoss) {
            if (currentHealth > 500) currentHealth = 500;
        }
        if (!isBoss) {
            if (currentHealth > 100) currentHealth = 100; }
        
    }

    public bool LowHealth()
    {
        return _lowHealth;
    }

    public float GetCurrentLife()
    {
        return currentHealth;
    }
}
