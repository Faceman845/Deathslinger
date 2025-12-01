using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vidaTotal = 3;
    public int valorEmPontos = 100;

    [Header("Efeitos Visuais")]
    public GameObject prefabExplosao;

    // Função para receber dano
    public void ReceberDano(int dano = 1)
    {
        vidaTotal -= dano;

        //Tenta avisar o BossController se houver
        BossController boss = GetComponent<BossController>();
        if (boss != null)
        {
            boss.PiscarDano();
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