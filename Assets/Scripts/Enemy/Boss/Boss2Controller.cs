using UnityEngine;
using System.Collections;

public class Boss2Controller : MonoBehaviour
{
    [Header("Configuração")]
    public float velocidadeMovimento = 10f;
    public float tempoEntreAtaques = 2f;

    [Header("Ataque: Espiral")]
    public GameObject projetilPrefab;
    public Transform centroDisparo;
    public int balasNoEspiral = 30;

    [Header("Ataque: Laser")]
    public GameObject objetoLaserVisual; // O Sprite esticado com Collider
    public Transform miraLaser; // Um ponto vazio que gira para mirar
    public float tempoDeMira = 1.5f;
    public float duracaoLaser = 0.5f;

    [Header("Ajustes")]
    public float offsetRotacao = 90f;
    private bool emCombate = false;

    [Header("Configuração de Spawn")]
    public bool ChefeDeFase = true; // SE FOR FALSE, ele age como inimigo comum

    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Vector3 posicaoInicial;
    private EnemyHealth Vida; // Opcional se usar no futuro

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        posicaoInicial = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();


        if (ChefeDeFase && EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.PausarSpawning(true);
        }

        StartCoroutine(CicloDeBatalha());
    }
    public void PiscarDano()
    {
        // Garante que a corrotina comece
        StartCoroutine(FlashVermelho());
    }

    IEnumerator FlashVermelho()
    {
        // Pinta de vermelho
        spriteRenderer.color = Color.red;
        // Espera um pouquinho
        yield return new WaitForSeconds(0.1f);
        // Volta para a cor normal (branco = cor original do sprite)
        spriteRenderer.color = Color.white;
    }

    IEnumerator CicloDeBatalha()
    {
        // Fase de entrada (opcional)
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // 1. Move para uma posição aleatória no topo
            Vector3 destino = posicaoInicial + new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f), 0);
            yield return StartCoroutine(MoverPara(destino));

            // 2. Escolhe o ataque
            if (Random.value > 0.5f)
            {
                yield return StartCoroutine(AtaqueLaser());
            }
            else
            {
                yield return StartCoroutine(AtaqueEspiral());
            }

            // 3. Descanso
            yield return new WaitForSeconds(tempoEntreAtaques);
        }
    }

    // Corrotina para mover suavemente até um ponto
    IEnumerator MoverPara(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadeMovimento * Time.deltaTime);
            yield return null; // Espera o próximo frame
        }
    }

    // --- ATAQUE 1: ESPIRAL ---
    IEnumerator AtaqueEspiral()
    {
        // Vai para o centro para ficar bonito
        yield return StartCoroutine(MoverPara(posicaoInicial));

        float anguloAtual = 180f;

        // Atira 30 balas girando
        for (int i = 0; i < balasNoEspiral; i++)
        {
            // Cria a bala
            GameObject bala = Instantiate(projetilPrefab, centroDisparo.position, Quaternion.identity);

            // Calcula direção baseada no ângulo atual
            Vector2 direcao = Quaternion.Euler(0, 0, anguloAtual) * Vector2.down;
            bala.GetComponent<EnemyBullet>().direcao = direcao;

            // Gira um pouco para a próxima bala (ex: 15 graus)
            anguloAtual += 15f;

            // Som de tiro aqui seria legal
            yield return new WaitForSeconds(0.05f); // Tiro muito rápido
        }
    }

    // --- ATAQUE 2: LASER ---
    IEnumerator AtaqueLaser()
    {
        float timer = 0f;

        // Fase 1: Mirando
        while (timer < tempoDeMira)
        {
            if (player != null)
            {
                Vector3 direcao = player.position - transform.position;
                float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, 0, angulo - offsetRotacao);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Fase 2: DISPARO
        objetoLaserVisual.transform.localRotation = Quaternion.identity;

        objetoLaserVisual.SetActive(true); // LIGA O LASER

        yield return new WaitForSeconds(duracaoLaser);

        objetoLaserVisual.SetActive(false); // DESLIGA

        // Fase 3: Recuperação
        transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    void OnDestroy()
    {
        // Verifica se o jogo ainda está rodando e se o Spawner existe
        if (ChefeDeFase && EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.Boss2Derrotado();
        }
    }
}