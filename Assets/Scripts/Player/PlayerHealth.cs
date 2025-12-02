using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int vidaMaxima = 3; // Vida máxima do jogador (Quantidade de Hits que pode tomar até ser destruido)
    public float tempoInvencibilidade = 2f; // Tempo de invencibilidade após receber dano

    public int vidaAtual; // Vida atual do jogador
    private bool estaInvencivel = false; // Flag para controlar invencibilidade

    private SpriteRenderer spriteRenderer; // Referência ao SpriteRenderer para fazer o pisca-pisca
    private Collider2D playerCollider; // Referência ao colisor do jogador

    [Header("Efeitos Visuais")]
    public GameObject prefabExplosao;

    void Start()
    {
        vidaAtual = vidaMaxima;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        GameManager.Instance.AtualizarVida(vidaAtual);
    }

    public void ReceberDano(int dano)
    {
        // Se estiver invencível, ignora o dano
        if (estaInvencivel) return;

        vidaAtual -= dano;
        GameManager.Instance.AtualizarVida(vidaAtual);
        Debug.Log($"Vida do Player: {vidaAtual}");

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            // Inicia a rotina de ficar piscando
            StartCoroutine(FicarInvencivel());
        }
    }

    void Morrer()
    {
        Debug.Log("GAME OVER!");

        if (prefabExplosao != null)
        {
            Instantiate(prefabExplosao, transform.position, Quaternion.identity);
        }

        GameManager.Instance.GameOver();

        gameObject.SetActive(false);
    }

    // Rotina para ficar invencível e piscar
    IEnumerator FicarInvencivel()
    {
        estaInvencivel = true;

        // Loop para fazer o sprite piscar
        for (float i = 0; i < tempoInvencibilidade; i += 0.2f)
        {
            spriteRenderer.enabled = false; 
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.enabled = true; 
            yield return new WaitForSeconds(0.1f); 
        }

        estaInvencivel = false;
    }

    // Detectar colisão física
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ReceberDano(1);
        }
    }

    // Detectar colisão por trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se encostou no corpo de um inimigo
        if (collision.CompareTag("Enemy"))
        {
            ReceberDano(1);
            if (prefabExplosao != null)
            {
                Instantiate(prefabExplosao, collision.gameObject.transform.position, Quaternion.identity);
            }
            // Destroi o inimigo ao tocar
            Destroy(collision.gameObject);
        }
    }
}