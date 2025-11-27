using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI textoPontuacao;
    public TextMeshProUGUI textoVida;

    private int scoreAtual = 0;
    private string vida;

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

    // Função chamada pelos inimigos quando morrem
    public void AdicionarPontos(int pontos)
    {
        scoreAtual += pontos;
        AtualizarTexto();
    }

    public void AtualizarVida(int vidaAtual, int vidaMaxima)
    {
        textoVida.text = "HP: " + vidaAtual + "/" + vidaMaxima;
    }

    void AtualizarTexto()
    {
        // Formata o texto para ter zeros à esquerda (ex: 00150)
        textoPontuacao.text = "SCORE: " + scoreAtual.ToString("00000");
    }
}