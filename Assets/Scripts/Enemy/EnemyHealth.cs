using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vidaTotal = 3;
    public int valorEmPontos = 100;

    [Header("Efeitos Visuais")]
    public GameObject prefabExplosao;

    void Start()
    {
        // Se o GameManager existir, aplicamos o multiplicador
        if (GameManager.Instance != null)
        {
            float multi = GameManager.Instance.multiplicadorDificuldade;

            // Aplica o multiplicador de dificuldade
            vidaTotal = Mathf.RoundToInt(vidaTotal * multi);
            // Aplica o multiplicador de pontos
            valorEmPontos = Mathf.RoundToInt(valorEmPontos * multi);
        }
    }

    // Função para receber dano
    public void ReceberDano(int dano = 1)
    {
        vidaTotal -= dano;

        //Tenta avisar o BossController se houver
        BossController boss1 = GetComponent<BossController>();
        if (boss1 != null)
        {
            boss1.PiscarDano();
        }

        Boss2Controller boss2 = GetComponent<Boss2Controller>();
        if (boss2 != null)
        {
            boss2.PiscarDano();
        }

        if (vidaTotal <= 0) Morrer();
    }

    // Função para matar o inimigo
    void Morrer()
    {
        if (prefabExplosao != null)
        {
            Instantiate(prefabExplosao, transform.position, Quaternion.identity);
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AdicionarPontos(valorEmPontos);
        }
        Destroy(gameObject);
    }
}