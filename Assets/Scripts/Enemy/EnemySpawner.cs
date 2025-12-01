using UnityEngine;

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
    private bool spawnPausado = false;

    [Header("Chefes Recorrentes")]
    public GameObject prefabBoss1;
    public bool boss2JaFoiDerrotado = false;

    [Header("Progressão de Dificuldade")]
    public float tempoParaAumentarNivel = 20f;
    public GameObject[] inimigosDesbloqueaveisAposBoss;
    private float tempoAtualSpawn;
    private float alturaTelaY;
    private float tempoDeJogo = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        tempoAtualSpawn = tempoEntreSpawns;

        // Calcula topo da tela
        Camera cam = Camera.main;
        Vector2 cantoSuperior = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        alturaTelaY = cantoSuperior.y + 1f;
    }

    void Update()
    {
        if (spawnPausado) return;

        // Conta o tempo total de jogo para saber a dificuldade
        tempoDeJogo += Time.deltaTime;

        // Lógica do Timer de Spawn
        tempoAtualSpawn -= Time.deltaTime;

        if (tempoAtualSpawn <= 0)
        {
            SpawnarInimigoPorDificuldade();

            // Reseta o timer com o tempo atual (que vai diminuindo)
            tempoAtualSpawn = tempoEntreSpawns;

            // Deixa o jogo mais rápido
            AumentarVelocidadeSpawn();
        }
    }

    public void PausarSpawning(bool pausar)
    {
        spawnPausado = pausar;

        // Opcional: Destruir inimigos comuns que sobraram na tela para limpar a arena pro X1
        if (pausar)
        {
            GameObject[] inimigos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var inimigo in inimigos)
            {
                // Não destrói o Boss (verifique se tem o script BossController)
                if (inimigo.GetComponent<BossController>() == null)
                {
                    // Usa a função de explodir sem pontos que criamos antes, ou Destroy direto
                    Destroy(inimigo);
                }
            }
        }
    }

    void SpawnarBossComoInimigoComum()
    {
        float xAleatorio = Random.Range(-larguraSpawnX, larguraSpawnX);
        Vector2 posicaoSpawn = new Vector2(xAleatorio, alturaTelaY);

        GameObject miniBoss = Instantiate(prefabBoss1, posicaoSpawn, Quaternion.identity);

        // --- O TRUQUE MÁGICO ---
        // Acessamos o script e dizemos: "Ei, você NÃO é o chefe da fase, não pare o jogo!"
        BossController scriptBoss = miniBoss.GetComponent<BossController>();
        if (scriptBoss != null)
        {
            scriptBoss.ChefeDeFase = false;
        }

        Debug.Log("CUIDADO! O Boss 1 apareceu como inimigo comum!");
    }

    void SpawnarInimigoPorDificuldade()
    {

        if (boss2JaFoiDerrotado && Random.value <= 0.05f)
        {
            SpawnarBossComoInimigoComum();
            return; // Sai da função para não spawnar outro inimigo junto neste frame
        }

        // 1. Calcula o nível atual baseado no tempo
        int nivelAtual = Mathf.FloorToInt(tempoDeJogo / tempoParaAumentarNivel);

        // 2. Garante que não vamos tentar acessar um inimigo que não existe no array
        int maxIndicePermitido = Mathf.Clamp(nivelAtual, 0, prefabsInimigos.Length - 1);

        // 3. Sorteia um inimigo entre o Básico (0) e o Máximo Desbloqueado
        int indiceSorteado = Random.Range(0, maxIndicePermitido + 1); // +1 porque o Range de int é exclusivo no final

        // 4. Instancia
        GameObject inimigoEscolhido = prefabsInimigos[indiceSorteado];

        float xAleatorio = Random.Range(-larguraSpawnX, larguraSpawnX);
        Vector2 posicaoSpawn = new Vector2(xAleatorio, alturaTelaY);
        Instantiate(inimigoEscolhido, posicaoSpawn, inimigoEscolhido.transform.rotation);

    }

    void AumentarVelocidadeSpawn()
    {
        if (tempoEntreSpawns > tempoMinimo)
        {
            tempoEntreSpawns -= redutorDeTempo;
        }
    }


    public void BossDerrotado()
    {
        spawnPausado = false; // Volta a spawnar

        // Cria uma lista temporária
        var listaNova = new System.Collections.Generic.List<GameObject>(prefabsInimigos);

        // Adiciona os desbloqueáveis
        listaNova.AddRange(inimigosDesbloqueaveisAposBoss);

        // Atualiza o array principal
        prefabsInimigos = listaNova.ToArray();

        Debug.Log("Novos inimigos desbloqueados!");
    }
    public void Boss2Derrotado()
    {
        boss2JaFoiDerrotado = true;
        // Lógica para desbloquear mais coisas...
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-larguraSpawnX, 6, 0), new Vector3(larguraSpawnX, 6, 0));
    }
}