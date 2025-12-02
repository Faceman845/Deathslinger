using UnityEngine;

public class LaserHazard : MonoBehaviour
{
    // OnTriggerStay roda todo frame que o player continua dentro do laser
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.ReceberDano(1);
            }
        }
    }
}