using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocidade = 20f;
    public float tempoDeVida = 2f;

    // OnEnable é chamado toda vez que o objeto é ativado
    void OnEnable()
    {
        // Agenda a desativação da bala após X segundos
        Invoke("Desativar", tempoDeVida);
    }

    void Update()
    {
        transform.Translate(Vector2.up * velocidade * Time.deltaTime);
    }

    // Detecta colisão com inimigos
    void OnTriggerEnter2D(Collider2D outro)
    {
        if (outro.CompareTag("Enemy"))
        {
            // Tenta pegar o script do Inimigo
            EnemyHealth vidaInimigo = outro.GetComponent<EnemyHealth>();

            if (vidaInimigo != null)
            {
                vidaInimigo.ReceberDano(1);
            }

            Desativar();
        }
    }

    void Desativar()
    {
        gameObject.SetActive(false); // Devolve para a pool
    }

    void OnDisable()
    {
        // Cancela qualquer agendamento de desativação pendente para não bugar quando reusar
        CancelInvoke();
    }
}