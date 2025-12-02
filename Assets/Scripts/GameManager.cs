using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("HUD (Interface de Jogo)")]
    public TextMeshProUGUI textoPontuacao;
    public TextMeshProUGUI textoVida;

    [Header("Tela de Game Over")]
    public GameObject painelGameOver; 
    public TextMeshProUGUI textoScoreFinal; 
    public TextMeshProUGUI textoHighScore;  

    private int scoreAtual = 0;

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

    public void AdicionarPontos(int pontos)
    {
        scoreAtual += pontos;
        AtualizarTexto();
    }

    public void AtualizarVida(int vidaAtual)
    {
        // Garante que não mostre vida negativa
        if (vidaAtual < 0) vidaAtual = 0;
        textoVida.text = "HP: " + vidaAtual;
    }

    void AtualizarTexto()
    {
        textoPontuacao.text = "SCORE: " + scoreAtual.ToString("00000");
    }

    public void GameOver()
    {
        // 1. Ativa o painel
        painelGameOver.SetActive(true);

        // 2. Esconde a UI de jogo para ficar mais limpo (opcional)
        textoPontuacao.gameObject.SetActive(false);
        textoVida.gameObject.SetActive(false);

        // 3. Lógica de Salvar Recorde
        int recordeAtual = PlayerPrefs.GetInt("HighScore", 0);

        if (scoreAtual > recordeAtual)
        {
            recordeAtual = scoreAtual;
            PlayerPrefs.SetInt("HighScore", recordeAtual);
            PlayerPrefs.Save(); // Força o salvamento no disco
        }

        // 4. Atualiza os textos do Painel
        textoScoreFinal.text = "PONTOS: " + scoreAtual.ToString("00000");
        textoHighScore.text = "RECORDE: " + recordeAtual.ToString("00000");
        Time.timeScale = 0f; 
    }

    // Botão "Tentar de Novo"
    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Botão "Menu"
    public void VoltarParaMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}