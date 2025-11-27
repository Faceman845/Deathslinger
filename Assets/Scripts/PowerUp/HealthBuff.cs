using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/HealthBuff")]
public class HealthBuff : PowerupEffect
{
    public int amount;

    public override void Apply(GameObject target)
    {
        target.GetComponent<PlayerHealth>().vidaAtual += amount;
        GameManager.Instance.AtualizarVida(target.GetComponent<PlayerHealth>().vidaAtual, target.GetComponent<PlayerHealth>().vidaMaxima);
    }
}
