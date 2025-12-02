using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Lista de PowerUps")]
    public GameObject[] prefabsPowerUps;

    [Header("Configurações de Spawn")]
    public float tempoEntreSpawns = 12f;
    public float tempoMinimo = 1f;
    public float redutorDeTempo = 0.05f;
    public float larguraSpawnX = 8f;

    [Header("Progressão de Dificuldade")]
    public float tempoParaAumentarNivel = 40f; // similar aos inimigos a cada 40s, desbloqueia um PowerUp novo

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

        // 2. Garante que não vamos tentar acessar um PowerUp que não existe no array
        int maxIndicePermitido = Mathf.Clamp(nivelAtual, 0, prefabsPowerUps.Length - 1);

        // 3. Sorteia um inimigo entre o Básico (0) e o Máximo Desbloqueado
        int indiceSorteado = Random.Range(0, maxIndicePermitido + 1); // +1 porque o Range de int é exclusivo no final

        // 4. Instancia
        GameObject powerUpEscolhido = prefabsPowerUps[indiceSorteado];
        float xAleatorio = Random.Range(-larguraSpawnX, larguraSpawnX);
        Vector2 posicaoSpawn = new(xAleatorio, alturaTelaY);

        Instantiate(powerUpEscolhido, posicaoSpawn, powerUpEscolhido.transform.rotation);
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
