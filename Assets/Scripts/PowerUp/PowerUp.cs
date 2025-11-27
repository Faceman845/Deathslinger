using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerupEffect powerUpEffect;
    [Header("Movimento")]
    public float velocidadeDescida = 1f;
    void Update()
    {
        // 1. Mover para baixo (no Mundo, independente da rotação da nave)
        transform.Translate(Vector2.down * velocidadeDescida * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            powerUpEffect.Apply(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
