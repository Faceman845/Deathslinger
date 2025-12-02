using UnityEngine;

public class EnemyShotgun : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeDescida = 4f;

    [Header("Configuração da Shotgun")]
    public GameObject projetilPrefab;
    public Transform firePoint;
    public float intervaloTiro = 2.5f;

    [Header("Caos da Shotgun")]
    public int quantidadePelots = 8;
    public float anguloDoCone = 45f;
    public float variacaoVelocidade = 2f;

    private float timerTiro;

    public float offsetRotacao = 180f;

    private float pontoDeParadaY;
    private bool chegouNoPonto = false;
    private Transform playerTransform; // Referência ao Player

    void Start()
    {
        timerTiro = intervaloTiro;
        CalcularPontoDeParada();

        // Encontra o player na cena automaticamente
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }


    void CalcularPontoDeParada()
    {
        Camera cam = Camera.main;
        float alturaAleatoria = Random.Range(0.5f, 0.8f);
        Vector2 pontoNoMundo = cam.ViewportToWorldPoint(new Vector3(0.5f, alturaAleatoria, 0));
        pontoDeParadaY = pontoNoMundo.y;
    }

    void Update()
    {
        // 1. Sempre tenta olhar para o player
        if (playerTransform != null)
        {
            RotacionarParaPlayer();
        }

        // --- FASE 1: ENTRADA ---
        if (!chegouNoPonto)
        {
            // IMPORTANTE: Space.World para descer reto independente da rotação
            transform.Translate(Vector2.down * velocidadeDescida * Time.deltaTime, Space.World);

            if (transform.position.y <= pontoDeParadaY)
            {
                chegouNoPonto = true;
            }
        }
        // --- FASE 2: COMBATE ---
        else
        {
            timerTiro -= Time.deltaTime;
            if (timerTiro <= 0)
            {
                AtirarShotgun();
                timerTiro = intervaloTiro;
            }
        }
    }

    void RotacionarParaPlayer()
    {
        // Pega a direção: Onde o player está - Onde eu estou
        Vector3 direcao = playerTransform.position - transform.position;

        // Calcula o ângulo em graus
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

        // Aplica a rotação no eixo Z + o Offset de correção do sprite
        transform.rotation = Quaternion.Euler(0, 0, angulo - offsetRotacao);
    }

    void AtirarShotgun()
    {
        for (int i = 0; i < quantidadePelots; i++)
        {
            float anguloAleatorio = Random.Range(-anguloDoCone / 2f, anguloDoCone / 2f);

            Vector2 direcaoBase = transform.up; // Assumindo que o "nariz" é o eixo Y local

            // Aplica a rotação aleatória do cone na direção base
            Vector2 direcaoCalculada = Quaternion.Euler(0, 0, anguloAleatorio) * direcaoBase;

            GameObject balaObj = Instantiate(projetilPrefab, firePoint.position, Quaternion.identity);
            EnemyBullet scriptBala = balaObj.GetComponent<EnemyBullet>();

            scriptBala.direcao = direcaoCalculada;

            // Roda o sprite da bala para acompanhar a direção
            float anguloBala = Mathf.Atan2(direcaoCalculada.y, direcaoCalculada.x) * Mathf.Rad2Deg;
            balaObj.transform.rotation = Quaternion.Euler(0, 0, anguloBala - 90);

            // Variação de velocidade
            float velocidadeOriginal = scriptBala.velocidade;
            scriptBala.velocidade = velocidadeOriginal + Random.Range(-variacaoVelocidade, variacaoVelocidade);
        }
    }
}