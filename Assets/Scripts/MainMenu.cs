using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textoHighScore;

    void Start()
    {
        // 1. Recupera o valor salvo. Se não existir nada salvo, retorna 0.
        int recorde = PlayerPrefs.GetInt("HighScore", 0);

        // 2. Atualiza o texto no canto
        if (textoHighScore != null)
        {
            textoHighScore.text = "RECORDE: " + recorde.ToString("00000");
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("Adeus mundo!");
        Application.Quit();
    }
}
