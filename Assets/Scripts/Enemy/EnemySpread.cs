using UnityEngine;

public class EnemySpread : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidadeDescida = 4f; // Aumentei um pouco para entrar rápido

    [Header("Configuração do Leque")]
    public GameObject projetilPrefab;
    public Transform firePoint;
    public float intervaloTiro = 2f;

    [Header("Matemática do Leque")]
    public int quantidadeBalas = 5;
    public float anguloTotalDoLeque = 90f;

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
            // Desce (lembrando que Vector2.up desce se o sprite estiver invertido)
            transform.Translate(Vector2.up * velocidadeDescida * Time.deltaTime);

            // Verifica se chegou na altura alvo
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
                AtirarLeque();
                timerTiro = intervaloTiro;
            }
        }

        // Removemos o "Destroy se sair da tela" pois ele não deve mais sair.
    }

    void AtirarLeque()
    {
        float anguloInicial = -anguloTotalDoLeque / 2f;
        float passoAngular = anguloTotalDoLeque / (quantidadeBalas - 1);

        for (int i = 0; i < quantidadeBalas; i++)
        {
            float anguloAtual = anguloInicial + (passoAngular * i);
            // Usa Vector2.down como base pois ele atira para baixo
            Vector2 direcaoCalculada = Quaternion.Euler(0, 0, anguloAtual) * Vector2.down;

            GameObject bala = Instantiate(projetilPrefab, firePoint.position, Quaternion.identity);
            bala.GetComponent<EnemyBullet>().direcao = direcaoCalculada;
        }
    }
}