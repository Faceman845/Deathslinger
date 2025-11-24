using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float velocidadeScroll = 0.5f;

    private Renderer renderizador;
    private Vector2 offsetSalvo;

    void Start()
    {
        renderizador = GetComponent<Renderer>();
    }

    void Update()
    {
        // Calcula o deslocamento baseado no tempo
        // O "Mathf.Repeat" garante que o valor fique sempre entre 0 e 1, 
        // evitando números gigantescos que podem bugar a textura depois de horas jogando.
        float y = Mathf.Repeat(Time.time * velocidadeScroll, 1);

        // Aplica o deslocamento na textura
        // Vector2(0, y) move apenas na vertical
        offsetSalvo = new Vector2(0, y);

        renderizador.material.mainTextureOffset = offsetSalvo;
    }
}