using System;
using UnityEngine;

public interface IDamagable
{
    void TakeDamage(int amount);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;

    public Condition Health { get { return uiCondition.health; } }
    public Condition Stamina { get { return uiCondition.stamina; } }

    public event Action OnTakeDamage;

    private void Update()
    {
        Stamina.Add(Stamina.regenRate * Time.deltaTime);

        if (Health.curValue <= 0.0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void IncreaseStamina(float amount)
    {
        Stamina.Add(amount);
    }

    public void TakeDamage(int damageAmount)
    {
        Health.Subtract(damageAmount);
        OnTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (Stamina.curValue - amount < 0)
        {
            return false;
        }

        Stamina.Subtract(amount);
        return true;
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었습니다.");
    }
}
