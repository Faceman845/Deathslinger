using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vidaTotal = 3;

    [Header("Efeitos Visuais")]
    public GameObject prefabExplosao;

    // Função genérica para receber dano
    public void ReceberDano(int dano = 1)
    {
        vidaTotal -= dano;

        if (vidaTotal <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        if (prefabExplosao != null)
        {
            Instantiate(prefabExplosao, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}