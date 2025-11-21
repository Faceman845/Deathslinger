using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Lista de Inimigos")]
    // Elemento 0 = Mais Fácil
    // Elemento 1 = Médio
    // Elemento 2 = Difícil
    public GameObject[] prefabsInimigos;

    [Header("Configurações de Spawn")]
    public float tempoEntreSpawns = 2f;
    public float tempoMinimo = 0.5f;
    public float redutorDeTempo = 0.05f;
    public float larguraSpawnX = 8f;

    [Header("Progressão de Dificuldade")]
    public float tempoParaAumentarNivel = 20f; // A cada 20s, desbloqueia um inimigo novo

    private float tempoAtualSpawn;
    private float alturaTelaY;
    private float tempoDeJogo = 0f;

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

    void SpawnarInimigoPorDificuldade()
    {
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-larguraSpawnX, 6, 0), new Vector3(larguraSpawnX, 6, 0));
    }
}