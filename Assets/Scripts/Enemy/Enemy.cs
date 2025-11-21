using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float velocidade = 2f;
    public int vida = 1;

    void Update()
    {
        // Move para baixo
        transform.Translate(Vector2.up * velocidade * Time.deltaTime);

        // Se passar muito do fundo da tela (Y < -6, por exemplo), destrói
        // (Podemos melhorar isso usando a câmera depois, igual fizemos no Player)
        if (transform.position.y < -10f)
        {
            Destroy(gameObject); // Ou SetActive(false) se usarmos Pool depois
        }
    }

    // Função para receber dano
    public void ReceberDano()
    {
        vida--;
        if (vida <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        // Aqui depois colocaremos som de explosão e partículas
        Destroy(gameObject);
    }
}