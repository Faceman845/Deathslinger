using UnityEngine;
using System.Collections.Generic; // Necessário para usar Listas

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Lista de Inimigos")]
    public GameObject[] prefabsInimigos;

    [Header("Configurações de Spawn")]
    public float tempoEntreSpawns = 2f;
    public float tempoMinimo = 0.5f;
    public float redutorDeTempo = 0.05f;
    public float larguraSpawnX = 8f;

    // Flags de controle
    private bool spawnPausado = false; // Para de criar inimigos comuns
    private bool cronometroPausado = false; // Para o relógio do jogo (Boss 2 não se aproxima)

    [Header("Chefe 1")]
    public GameObject prefabBoss1;
    public float tempoParaBoss1 = 60f; // Boss 1 aparece em 60 segundos
    private bool boss1JaNasceu = false;

    [Header("Chefe 2")]
    public GameObject prefabBoss2;
    public float tempoParaBoss2 = 180f; // Boss 2 aparece em 3 minutos (de tempo jogado real)
    private bool boss2JaNasceu = false;
    public bool boss2JaFoiDerrotado = false; // Libera o Boss 1 como mini-boss

    [Header("Progressão de Dificuldade")]
    public float tempoParaAumentarNivel = 20f;
    public GameObject[] inimigosDesbloqueaveisAposBoss;

    [Header("Dificuldade Infinita")]
    public float taxaDeCrescimento = 0.1f;

    private float tempoAtualSpawn;
    private float alturaTelaY;
    private float tempoDeJogo = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        tempoAtualSpawn = tempoEntreSpawns;

        Camera cam = Camera.main;
        Vector2 cantoSuperior = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        alturaTelaY = cantoSuperior.y + 1f;
    }

    void Update()
    {
        // 1. RELÓGIO GERAL DO JOGO
        // Só avança se não estivermos lutando contra um chefe
        if (!cronometroPausado)
        {
            tempoDeJogo += Time.deltaTime;

            // Checagem Boss 1
            if (!boss1JaNasceu && tempoDeJogo >= tempoParaBoss1)
            {
                InvocarBoss1();
            }

            // Checagem Boss 2
            if (!boss2JaNasceu && tempoDeJogo >= tempoParaBoss2)
            {
                InvocarBoss2();
            }

            if (boss2JaFoiDerrotado)
            {
                // Aumenta o multiplicador suavemente a cada frame
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.multiplicadorDificuldade += taxaDeCrescimento * Time.deltaTime;
                }
            }
        }

        // 2. SISTEMA DE SPAWN DE INIMIGOS
        if (spawnPausado) return;

        tempoAtualSpawn -= Time.deltaTime;

        if (tempoAtualSpawn <= 0)
        {
            SpawnarInimigoPorDificuldade();
            tempoAtualSpawn = tempoEntreSpawns;
            AumentarVelocidadeSpawn();
        }
    }

    // --- LÓGICA DO BOSS 1 ---
    void InvocarBoss1()
    {
        boss1JaNasceu = true;

        // Pausa tudo para o X1
        PausarSpawning(true);
        cronometroPausado = true; // TRAVA O TEMPO DO BOSS 2

        Instantiate(prefabBoss1, new Vector2(0, alturaTelaY), Quaternion.identity);
        Debug.Log("BOSS 1 CHEGOU!");
    }

    // Chamado quando o Boss 1 morre (no OnDestroy dele)
    public void BossDerrotado()
    {
        spawnPausado = false;
        cronometroPausado = false; // DESTRAVA O TEMPO (Rumo ao Boss 2)

        // Adiciona novos inimigos à lista
        var listaNova = new List<GameObject>(prefabsInimigos);
        listaNova.AddRange(inimigosDesbloqueaveisAposBoss);
        prefabsInimigos = listaNova.ToArray();

        Debug.Log("Boss 1 Derrotado! Novos inimigos desbloqueados e tempo retomado.");
    }

    // --- LÓGICA DO BOSS 2 ---
    void InvocarBoss2()
    {
        boss2JaNasceu = true;

        PausarSpawning(true);
        cronometroPausado = true; // Trava o tempo novamente

        Instantiate(prefabBoss2, new Vector2(0, alturaTelaY), Quaternion.identity);
        Debug.Log("BOSS 2 CHEGOU!");
    }

    // Chamado quando o Boss 2 morre (no OnDestroy dele)
    public void Boss2Derrotado()
    {
        spawnPausado = false;
        cronometroPausado = false; // Destrava o tempo
        boss2JaFoiDerrotado = true; // Libera o Boss 1 como inimigo comum (5%)

        Debug.Log("Boss 2 Derrotado! Modo Hardcore ativado.");
    }

    // --- UTILITÁRIOS ---

    public void PausarSpawning(bool pausar)
    {
        spawnPausado = pausar;

        // Limpa a arena destruindo inimigos comuns
        if (pausar)
        {
            // OBS: Verifique se a Tag dos seus inimigos é "Inimigo" ou "Enemy". 
            // Usei "Enemy" conforme seu código anterior, mas ajuste se necessário.
            GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var inimigo in inimigos)
            {
                // Garante que não vamos destruir o Boss que acabou de nascer
                if (inimigo.GetComponent<BossController>() == null && inimigo.GetComponent<Boss2Controller>() == null)
                {
                    EnemyHealth vida = inimigo.GetComponent<EnemyHealth>();
                    Destroy(inimigo);
                }
            }
        }
    }

    void SpawnarInimigoPorDificuldade()
    {
        if (boss2JaFoiDerrotado && Random.value <= 0.05f)
        {
            SpawnarBossAleatorioComoInimigo();
            return;
        }

        int nivelAtual = Mathf.FloorToInt(tempoDeJogo / tempoParaAumentarNivel);
        int maxIndicePermitido = Mathf.Clamp(nivelAtual, 0, prefabsInimigos.Length - 1);
        int indiceSorteado = Random.Range(0, maxIndicePermitido + 1);

        GameObject inimigoEscolhido = prefabsInimigos[indiceSorteado];
        float xAleatorio = Random.Range(-larguraSpawnX, larguraSpawnX);
        Vector2 posicaoSpawn = new Vector2(xAleatorio, alturaTelaY);

        Instantiate(inimigoEscolhido, posicaoSpawn, inimigoEscolhido.transform.rotation);
    }
    void SpawnarBossAleatorioComoInimigo()
    {
        float xAleatorio = Random.Range(-larguraSpawnX, larguraSpawnX);
        // Bosses geralmente precisam de mais espaço, então nascem um pouco mais alto
        Vector2 posicaoSpawn = new Vector2(xAleatorio, alturaTelaY);

        GameObject miniBoss;

        // 50% de chance para cada um
        if (Random.value > 0.5f)
        {
            // Spawna Boss 1
            miniBoss = Instantiate(prefabBoss1, posicaoSpawn, Quaternion.identity);

            // Desliga a flag de chefe
            BossController script = miniBoss.GetComponent<BossController>();
            if (script != null) script.ChefeDeFase = false;
        }
        else
        {
            // Spawna Boss 2
            miniBoss = Instantiate(prefabBoss2, posicaoSpawn, Quaternion.identity);

            // Desliga a flag de chefe
            Boss2Controller script = miniBoss.GetComponent<Boss2Controller>();
            if (script != null) script.ChefeDeFase = false;
        }

        Debug.Log("UM MINI-BOSS APARECEU!");
    }

    void AumentarVelocidadeSpawn()
    {
        if (tempoEntreSpawns > tempoMinimo)
        {
            tempoEntreSpawns -= redutorDeTempo;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-larguraSpawnX, 6, 0), new Vector3(larguraSpawnX, 6, 0));
    }
}