using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimentação")]
    public float velocidade = 10f;
    public float margem = 0.5f; // Espaço para a nave não colar na borda

    [Header("Combate")]
    public Transform pontoDeDisparo;
    public float cadenciaDeTiro = 0.1f;

    // Componentes
    private Rigidbody2D rb;
    private Vector2 movimentoInput;
    private float proximoDisparo = 0f;

    // Variáveis para guardar o tamanho da tela calculado
    private Vector2 limiteMin;
    private Vector2 limiteMax;

    // Controles de Input System
    private GameControls controls;
    private bool isFiring = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new GameControls();

        // Configura os callbacks de input
        controls.Player.Move.performed += ctx => movimentoInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movimentoInput = Vector2.zero;

        // Ataque
        controls.Player.Attack.performed += ctx => isFiring = true;
        controls.Player.Attack.canceled += ctx => isFiring = false;
    }

    void Start()
    {
        CalcularLimitesDaTela();
    }

    void CalcularLimitesDaTela()
    {
        Camera cam = Camera.main;

        // Converte o canto inferior esquerdo (0,0) da visão para coordenadas do mundo (X,Y)
        limiteMin = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));

        // Converte o canto superior direito (1,1)
        limiteMax = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
    }

    void OnEnable()
    {
        controls.Player.Enable();
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Update()
    {
        if (isFiring)
        {
            Atirar();
        }
    }

    void FixedUpdate()
    {
        // 1. Move a nave
        Vector2 novaPosicao = rb.position + movimentoInput * velocidade * Time.fixedDeltaTime;

        // 2. Aplica o "Grampo" (Clamp) para não sair da tela
        // Clamp(valor, minimo + margem, maximo - margem)
        novaPosicao.x = Mathf.Clamp(novaPosicao.x, limiteMin.x + margem, limiteMax.x - margem);
        novaPosicao.y = Mathf.Clamp(novaPosicao.y, limiteMin.y + margem, limiteMax.y - margem);

        // 3. Aplica a posição final
        rb.MovePosition(novaPosicao);
    }

    void Atirar()
    {
        if (Time.time > proximoDisparo)
        {
            proximoDisparo = Time.time + cadenciaDeTiro;

            GameObject bala = ObjectPooler.Instance.GetPooledObject();
            if (bala != null)
            {
                bala.transform.position = pontoDeDisparo.position;
                bala.transform.rotation = transform.rotation;
                bala.SetActive(true);
            }
        }
    }
}