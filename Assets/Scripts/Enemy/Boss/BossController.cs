using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Movimento")]
    public float amplitudeFlutuar = 0.5f; // O quanto ele sobe e desce
    public float frequenciaFlutuar = 1f; // A velocidade do balanço

    [Header("Ataques")]
    public Transform firePointEsquerdo;
    public Transform firePointDireito;
    public GameObject prefabTiroVertical;
    public GameObject prefabClusterBomb;

    [Header("Timers")]
    public float tempoEntreAtaques = 2f;

    private Vector3 posicaoInicial;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private EnemyHealth Vida;

    [Header("Configuração de Spawn")]
    public bool ChefeDeFase = true;

    void Start()
    {
        posicaoInicial = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        Vida = GetComponent<EnemyHealth>();

        // Avisa o Spawner para parar de mandar inimigos comuns!
        if (ChefeDeFase && EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.PausarSpawning(true);
        }

        StartCoroutine(RotinaDeCombate());
    }

    void Update()
    {
        // Move suavemente no eixo X e Y baseado no tempo
        float novoX = posicaoInicial.x + Mathf.Cos(Time.time * frequenciaFlutuar * 0.5f) * 2f; // Move pros lados
        float novoY = posicaoInicial.y + Mathf.Sin(Time.time * frequenciaFlutuar) * amplitudeFlutuar; // Move pra cima/baixo

        transform.position = new Vector3(novoX, novoY, 0);

        if (Vida.vidaTotal < 25)
        {
            tempoEntreAtaques = 0.1f;
        }
    }
    // --- SISTEMA DE DANO (FLASH VERMELHO) ---
    public void PiscarDano()
    {
        StartCoroutine(FlashVermelho());
    }

    IEnumerator FlashVermelho()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }
    // ----------------------------------------

    IEnumerator RotinaDeCombate()
    {
        while (true) // Loop infinito até morrer
        {
            yield return new WaitForSeconds(tempoEntreAtaques);

            // Sorteia ataque (50% de chance para cada)
            if (Random.value > 0.5f)
            {
                AtaqueVertical();
            }
            else
            {
                AtaqueCluster();
            }
        }
    }

    void AtaqueVertical()
    {
        // Tiro simples descendo dos dois canhões
        Instantiate(prefabTiroVertical, firePointEsquerdo.position, Quaternion.identity).GetComponent<EnemyBullet>().direcao = Vector2.down;
        Instantiate(prefabTiroVertical, firePointDireito.position, Quaternion.identity).GetComponent<EnemyBullet>().direcao = Vector2.down;
    }

    void AtaqueCluster()
    {
        if (player == null) return;

        // Cria a bomba no centro (ou em um dos canhões)
        GameObject bomba = Instantiate(prefabClusterBomb, transform.position, Quaternion.identity);

        // Manda ela ir para onde o player está AGORA
        bomba.GetComponent<ClusterBomb>().ConfigurarDestino(player.position);
    }

    // Chamado automaticamente quando o objeto é destruído (Morte do Boss)
    void OnDestroy()
    {
        // Libera os inimigos comuns de volta e DESBLOQUEIA os novos
        if (ChefeDeFase && EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.BossDerrotado();
        }
    }
}