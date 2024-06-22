using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, ITakeDamage
{
    public static event Action OnPlayerDeadEvent;
    
    [Header("Player")]
    [SerializeField] private PlayerConfig playerConfig;

    private Coroutine regenerateArmorCoroutine;

    public void RecoverHealth(float amount)
    {
        playerConfig.CurrentHealth += amount;
        if (playerConfig.CurrentHealth > playerConfig.MaxHealth)
        {
            playerConfig.CurrentHealth = playerConfig.MaxHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        DamageManager.Instance.ShowDamage(amount, transform);
        if (playerConfig.Armor > 0)
        {
            // Break armor
            // Damage health
            float remainingDamage = amount - playerConfig.Armor;
            playerConfig.Armor = Mathf.Max(playerConfig.Armor - amount, 0f);
            if (remainingDamage > 0)
            {
                playerConfig.CurrentHealth =
                    Mathf.Max(playerConfig.CurrentHealth - remainingDamage, 0f);
            }
        }
        else
        {
            // Damage health
            playerConfig.CurrentHealth =
                Mathf.Max(playerConfig.CurrentHealth - amount, 0f);
        }

        if (playerConfig.CurrentHealth <= 0)
        {
            PlayerDead();
        }
        // Restart the regeneration coroutine
        StartRegenerateArmorCoroutine();
    }

    private void PlayerDead()
    {
        OnPlayerDeadEvent?.Invoke();
        Destroy(gameObject);
    }

    private void StartRegenerateArmorCoroutine()
    {
        if (regenerateArmorCoroutine != null)
        {
            StopCoroutine(regenerateArmorCoroutine);
        }
        regenerateArmorCoroutine = StartCoroutine(RegenerateArmor());
        regenerateArmorCoroutine = StartCoroutine(RegenerateEnergy());
    }

    private IEnumerator RegenerateArmor()
    {
        while (true)
        {
            yield return new WaitForSeconds(3);

            playerConfig.Armor += 1;
            if (playerConfig.Armor > playerConfig.MaxArmor)
            {
                playerConfig.Armor = playerConfig.MaxArmor;
            }
        }
    }
    private IEnumerator RegenerateEnergy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            playerConfig.Energy += 1;
            if (playerConfig.Energy > playerConfig.MaxEnergy)
            {
                playerConfig.Energy = playerConfig.MaxEnergy;
            }
        }
    }
}
