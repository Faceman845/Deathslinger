using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerupEffect powerUpEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            powerUpEffect.Apply(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
