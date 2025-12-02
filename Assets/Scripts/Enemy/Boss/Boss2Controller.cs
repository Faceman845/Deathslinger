using UnityEngine;
using System.Collections;

public class Boss2Controller : MonoBehaviour
{
    [Header("Configuração")]
    public float velocidadeMovimento = 10f;
    public float tempoEntreAtaques = 2f;
    public bool ChefeDeFase = true;

    [Header("Ataque: Espiral")]
    public GameObject projetilPrefab;
    public Transform centroDisparo;
    public int balasNoEspiral = 30;

    [Header("Ataque: Laser")]
    public GameObject objetoLaserVisual;
    public Transform miraLaser;
    public float tempoDeMira = 1.5f;
    public float duracaoLaser = 0.5f;
    public float offsetRotacao = 180f;

    private Transform player;
    private Vector3 posicaoInicialDeCombate;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        objetoLaserVisual.SetActive(false);

        Camera cam = Camera.main;
        Vector2 pontoCombate = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, 0));
        posicaoInicialDeCombate = new Vector3(pontoCombate.x, pontoCombate.y, 0);

        if (ChefeDeFase && EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.PausarSpawning(true);
        }

        StartCoroutine(CicloDeBatalha());
    }

    public void PiscarDano() { StartCoroutine(FlashVermelho()); }
    IEnumerator FlashVermelho()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    IEnumerator CicloDeBatalha()
    {
        // Enquanto ele não chegar perto da posição de combate, ele desce
        while (Vector3.Distance(transform.position, posicaoInicialDeCombate) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicaoInicialDeCombate, velocidadeMovimento * Time.deltaTime);
            yield return null;
        }

        // Garante que fique exatamente no ponto
        transform.position = posicaoInicialDeCombate;

        // Espera para o jogador ver o boss posicionado
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // Move para uma posição aleatória
            Vector3 destino = posicaoInicialDeCombate + new Vector3(Random.Range(-3f, 3f), Random.Range(-1f, 1f), 0);
            yield return StartCoroutine(MoverPara(destino));

            if (Random.value > 0.5f)
            {
                yield return StartCoroutine(AtaqueLaser());
            }
            else
            {
                yield return StartCoroutine(AtaqueEspiral());
            }

            yield return new WaitForSeconds(tempoEntreAtaques);
        }
    }

    IEnumerator MoverPara(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadeMovimento * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator AtaqueEspiral()
    {
        // Volta para o centro (Anchor Point) para atirar
        yield return StartCoroutine(MoverPara(posicaoInicialDeCombate));

        float anguloAtual = 0f;
        for (int i = 0; i < balasNoEspiral; i++)
        {
            GameObject bala = Instantiate(projetilPrefab, centroDisparo.position, Quaternion.identity);
            Vector2 direcao = Quaternion.Euler(0, 0, anguloAtual) * Vector2.down;
            bala.GetComponent<EnemyBullet>().direcao = direcao;

            // Rotaciona a bala visualmente também
            float anguloBala = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            bala.transform.rotation = Quaternion.Euler(0, 0, anguloBala - 90);

            anguloAtual += 15f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator AtaqueLaser()
    {
        float timer = 0f;
        while (timer < tempoDeMira)
        {
            if (player != null)
            {
                Vector3 direcao = player.position - transform.position;
                float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                // Gira o Boss
                transform.rotation = Quaternion.Euler(0, 0, angulo - offsetRotacao);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        objetoLaserVisual.transform.localRotation = Quaternion.identity;
        objetoLaserVisual.SetActive(true);
        yield return new WaitForSeconds(duracaoLaser);
        objetoLaserVisual.SetActive(false);

        // Reseta rotação suavemente
        // Fase 3: Recuperação (Resetar Rotação)
        objetoLaserVisual.SetActive(false);

        float t = 0;
        Quaternion rotAtual = transform.rotation;
        Quaternion rotDestino = Quaternion.Euler(0, 0, 180f);
        Quaternion rotInicial = transform.rotation;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            // Gira suavemente de onde estava para 180
            transform.rotation = Quaternion.Lerp(rotAtual, rotDestino, t);
            yield return null;
        }
        transform.rotation = rotDestino;
    }

    void OnDestroy()
    {
        if (ChefeDeFase && EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.Boss2Derrotado();
        }
    }
}