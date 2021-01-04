using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeController
{
    float _maxHealth;
    public float _currentHealth;
    public delegate void DeathDelegate();
    DeathDelegate _deathDelegate;

    public LifeController(float initialhealth, DeathDelegate deathEvent)
    {
        _maxHealth = initialhealth;
        _currentHealth = _maxHealth;
        _deathDelegate = deathEvent;
    }

    public void GetDamage(float damage)
    {
        _currentHealth -= damage;      

        if (_currentHealth <= 0) _deathDelegate.Invoke();
    }

    public void GetHeal(float heal)
    {
        _currentHealth += heal;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        if (_currentHealth > 100) _currentHealth = 100;
    }

    public float GetCurrentLife()
    {
        return _currentHealth;
    }
}
