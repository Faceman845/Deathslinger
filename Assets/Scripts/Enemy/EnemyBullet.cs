using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float velocidade = 8f;
    public Vector2 direcao; // A bala precisa saber pra onde ir
    public float tempoVida = 3f;

    void Start()
    {
        Destroy(gameObject, tempoVida); // Destroi depois de um tempo
    }

    void Update()
    {
        // Move na direção definida quando ela nasceu
        transform.Translate(direcao * velocidade * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Player"))
        {
            // Aqui vamos causar dano no Player futuramente
            // Por enquanto, apenas destrói a bala e loga
            Debug.Log("Player Atingido!");
            Destroy(gameObject);
        }
    }
}