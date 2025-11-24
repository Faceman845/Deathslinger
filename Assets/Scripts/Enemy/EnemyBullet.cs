using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float velocidade = 8f;
    public Vector2 direcao; // A bala precisa saber pra onde ir
    public float tempoVida = 3f;

    void Start()
    {
        Destroy(gameObject, tempoVida); // Destroi depois de um tempo
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo - 90);
    }

    void Update()
    {
        // Move na direção definida quando ela nasceu
        transform.Translate(direcao * velocidade * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            // Tenta pegar o script de vida do Player
            PlayerHealth vidaPlayer = outro.GetComponent<PlayerHealth>();

            if (vidaPlayer != null)
            {
                vidaPlayer.ReceberDano(1);
            }

            // Destroi a bala
            Destroy(gameObject);
        }
    }
}