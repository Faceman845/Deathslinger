using UnityEngine;

public class VFXCleaner : MonoBehaviour
{
    public float tempoDeVida = 0.5f; // Tempo suficiente para a animação tocar

    void Start()
    {
        // Agenda a destruição do objeto assim que ele nasce
        Destroy(gameObject, tempoDeVida);
    }
}