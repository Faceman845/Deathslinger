using UnityEngine;

public class EnemyKamikaze : MonoBehaviour
{
    public float velocidade = 2f;
    public float velocidadeRotacao = 200f;

    private Transform target;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
    }

    void FixedUpdate()
    {
        if (target == null) return; // Se o player morreu, não faz nada

        // 1. Descobre a direção
        Vector2 direcao = (Vector2)target.position - rb.position;
        direcao.Normalize();

        // 2. Calcula o quanto precisa girar
        float valorGiro = Vector3.Cross(direcao, transform.up).z;

        // 3. Aplica rotação angular
        rb.angularVelocity = -valorGiro * velocidadeRotacao;

        // 4. Move para frente
        rb.linearVelocity = transform.up * velocidade;
    }


}