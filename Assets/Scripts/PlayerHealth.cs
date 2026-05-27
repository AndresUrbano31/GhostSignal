using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Invencibilidad")]
    public float invincibleDuration = 1.5f;  // segundos de invencibilidad tras recibir daño

    private bool isInvincible = false;
    private float invincibleTimer = 0f;

    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0f)
                isInvincible = false;
        }
    }

    public bool TryTakeDamage()
    {
        if (isInvincible) return false;

        isInvincible = true;
        invincibleTimer = invincibleDuration;
        return true;
    }
}