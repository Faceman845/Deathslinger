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

    // --- Novas variáveis de controle de parada ---
    private float pontoDeParadaY;
    private bool chegouNoPonto = false;

    void Start()
    {
        timerTiro = intervaloTiro;
        CalcularPontoDeParada();
    }

    void CalcularPontoDeParada()
    {
        Camera cam = Camera.main;
        // Sorteia um ponto entre a metade (0.5) e o topo (0.8) da tela
        float alturaAleatoria = Random.Range(0.5f, 0.8f);
        Vector2 pontoNoMundo = cam.ViewportToWorldPoint(new Vector3(0.5f, alturaAleatoria, 0));
        pontoDeParadaY = pontoNoMundo.y;
    }

    void Update()
    {
        // --- FASE 1: ENTRADA ---
        if (!chegouNoPonto)
        {
            transform.Translate(Vector2.up * velocidadeDescida * Time.deltaTime);

            if (transform.position.y <= pontoDeParadaY)
            {
                chegouNoPonto = true;
            }
        }
        // --- FASE 2: COMBATE (Parado) ---
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

    void AtirarShotgun()
    {
        for (int i = 0; i < quantidadePelots; i++)
        {
            float anguloAleatorio = Random.Range(-anguloDoCone / 2f, anguloDoCone / 2f);
            // Usa Vector2.down como base
            Vector2 direcaoCalculada = Quaternion.Euler(0, 0, anguloAleatorio) * Vector2.down;

            GameObject balaObj = Instantiate(projetilPrefab, firePoint.position, Quaternion.identity);
            EnemyBullet scriptBala = balaObj.GetComponent<EnemyBullet>();

            scriptBala.direcao = direcaoCalculada;

            float velocidadeOriginal = scriptBala.velocidade;
            scriptBala.velocidade = velocidadeOriginal + Random.Range(-variacaoVelocidade, variacaoVelocidade);
        }
    }
}