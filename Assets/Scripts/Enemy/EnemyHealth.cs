using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vidaTotal = 3;

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
        Destroy(gameObject);
    }
}