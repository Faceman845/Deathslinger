using UnityEngine;

public class ClusterBomb : MonoBehaviour
{
    [Header("Configuração")]
    public float velocidade = 5f;
    public GameObject projetilFilhoPrefab; 
    public int quantidadeEstilhacos = 8;

    private Vector2 destino;
    private bool chegou = false;

    public void ConfigurarDestino(Vector2 alvo)
    {
        destino = alvo;
    }

    void Update()
    {
        if (chegou) return;

        // Move a bomba em direção ao destino
        transform.position = Vector2.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);

        // Verifica se chegou muito perto do destino (0.1f de distância)
        if (Vector2.Distance(transform.position, destino) < 0.1f)
        {
            chegou = true;
            Explodir();
        }
    }

    void Explodir()
    {
        // Cria os estilhaços em círculo (igual fizemos no EnemySpread)
        float passoAngular = 360f / quantidadeEstilhacos;

        for (int i = 0; i < quantidadeEstilhacos; i++)
        {
            float angulo = i * passoAngular;
            Vector2 direcao = Quaternion.Euler(0, 0, angulo) * Vector2.up;

            GameObject bala = Instantiate(projetilFilhoPrefab, transform.position, Quaternion.identity);
            bala.GetComponent<EnemyBullet>().direcao = direcao;
        }

        // Efeito visual/sonoro aqui
        Destroy(gameObject);
    }
}