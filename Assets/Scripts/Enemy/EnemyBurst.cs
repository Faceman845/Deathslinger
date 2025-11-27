using UnityEngine;
using System.Collections;

public class EnemyBurst : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeDescida = 1f;

    [Header("Configuração da Rajada")]
    public GameObject projetilPrefab;
    public Transform firePointL;
    public Transform firePointR;

    public int balasPorRajada = 5;
    public float tempoEntreBalas = 0.1f;
    public float tempoEntreRajadas = 1.0f;

    private bool estaAtirando = false;
    private Transform playerTransform; // Referência para o alvo

    void Start()
    {
        // Encontra o player na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        StartCoroutine(RotinaDeRajada());
    }

    void Update()
    {
        // 1. Mover para baixo (no Mundo, independente da rotação da nave)
        transform.Translate(Vector2.down * velocidadeDescida * Time.deltaTime, Space.World);

        // 2. Rotacionar para olhar para o Player
        if (playerTransform != null)
        {
            Vector3 direcao = playerTransform.position - transform.position;
            float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

            // O "+ 90" ajusta a rotação se o sprite original apontar para cima
            transform.rotation = Quaternion.Euler(0, 0, angulo - 90);
        }

        // Limpeza
        if (transform.position.y < -10f) Destroy(gameObject);
    }

    IEnumerator RotinaDeRajada()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            estaAtirando = true;

            // Só atira se o player ainda estiver vivo
            if (playerTransform != null)
            {
                for (int i = 0; i < balasPorRajada; i++)
                {
                    AtirarSimultaneo();
                    yield return new WaitForSeconds(tempoEntreBalas);
                }
            }

            estaAtirando = false;
            yield return new WaitForSeconds(tempoEntreRajadas);
        }
    }

    void AtirarSimultaneo()
    {
        if (playerTransform == null) return;

        // Calcula a direção exata para o player neste momento
        Vector2 direcaoL = (playerTransform.position - firePointL.position).normalized;
        Vector2 direcaoR = (playerTransform.position - firePointR.position).normalized;

        // Cria a bala da esquerda
        GameObject balaL = Instantiate(projetilPrefab, firePointL.position, Quaternion.identity);
        // Aplica a direção calculada
        balaL.GetComponent<EnemyBullet>().direcao = direcaoL;
        // Ajusta a rotação da bala para ela apontar para o player visualmente
        AjustarRotacaoBala(balaL, direcaoL);

        // Cria a bala da direita
        GameObject balaR = Instantiate(projetilPrefab, firePointR.position, Quaternion.identity);
        balaR.GetComponent<EnemyBullet>().direcao = direcaoR;
        AjustarRotacaoBala(balaR, direcaoR);
    }

    // Função auxiliar para girar o sprite da bala na direção que ela vai
    void AjustarRotacaoBala(GameObject bala, Vector2 dir)
    {
        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bala.transform.rotation = Quaternion.Euler(0, 0, angulo - 90);
    }
}