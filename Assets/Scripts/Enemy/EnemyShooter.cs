using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeDescida = 4f;

    [Header("Combate")]
    public Transform pontoDeDisparo;
    public GameObject projetilPrefab;
    public float intervaloTiro = 1.5f;

    private float timerTiro;
    private Transform playerTransform;

    // Novas variáveis para controlar a parada
    private float pontoDeParadaY;
    private bool chegouNoPonto = false;

    void Start()
    {
        timerTiro = intervaloTiro;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        CalcularPontoDeParada();
    }

    void CalcularPontoDeParada()
    {
        Camera cam = Camera.main;

        // Sorteia um ponto entre 60% (0.6) e 90% (0.9) da altura da tela.
        float alturaAleatoria = Random.Range(0.6f, 0.9f);

        // Converte essa porcentagem para posição no mundo (Y)
        Vector2 pontoNoMundo = cam.ViewportToWorldPoint(new Vector3(0.5f, alturaAleatoria, 0));
        pontoDeParadaY = pontoNoMundo.y;
    }

    void Update()
    {
        // --- FASE 1: ENTRADA NA TELA ---
        if (!chegouNoPonto)
        {
            // Move para baixo (Lembrete: Vector2.up desce porque seu sprite está invertido)
            transform.Translate(Vector2.up * velocidadeDescida * Time.deltaTime);

            // Verifica se já desceu o suficiente (se o Y atual é menor ou igual ao alvo)
            if (transform.position.y <= pontoDeParadaY)
            {
                chegouNoPonto = true; // Ativa o modo torreta
            }
        }

        // --- FASE 2: COMBATE (Só atira se já chegou e o player existe) ---
        else if (playerTransform != null)
        {
            // Opcional: Faz o inimigo girar suavemente para olhar pro player
            RotacionarParaOPlayer();

            timerTiro -= Time.deltaTime;
            if (timerTiro <= 0)
            {
                Atirar();
                timerTiro = intervaloTiro;
            }
        }
    }

    void RotacionarParaOPlayer()
    {
        // Matemática simples para olhar para o alvo
        Vector3 direcao = playerTransform.position - transform.position;
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

        // Aplica a rotação (subtraindo 90 graus para alinhar corretamente)
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angulo - 90));
    }

    void Atirar()
    {
        // Define a origem do disparo
        Vector3 origem = (pontoDeDisparo != null) ? pontoDeDisparo.position : transform.position;

        // Cria a bala na posição do 'pontoDeDisparo'
        GameObject bala = Instantiate(projetilPrefab, origem, Quaternion.identity);

        // Calcula a direção para o player
        Vector2 direcaoParaPlayer = playerTransform.position - origem;

        bala.GetComponent<EnemyBullet>().direcao = direcaoParaPlayer.normalized;
    }

    public void ReceberDano()
    {
        Destroy(gameObject);
    }
}