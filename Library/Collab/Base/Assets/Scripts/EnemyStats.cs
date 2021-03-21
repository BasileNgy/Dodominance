using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyStats : MonoBehaviour
{

    //GameManager
    public GameManager gameManager;

    //This enemy
    [SerializeField] protected EnemyStats me;
    protected string enemyName;
    //Joueur ciblé
    protected GameObject player;
    protected Transform playerTransform;
    public LayerMask whatIsPlayer;

    //Ranges
    public float chaseRange, attackRange;
    protected bool playerInChaseRange, playerInAttackRange, canMoove;
    public float chaseSpeed = 3;

    //health
    public float maxHealth;
    protected float health;
    protected bool isDead = false;
    protected bool dying = false;

    //Cooldown des attaques
    public float attackRepeatTime = 1;
    protected float attackTime;
    protected bool isAttacking = false;

    // Montant de dégâts
    public float damages;

    //Effets sonores
    public AudioManager audioManager;

    //animation de l'ennemi
    protected Animator enemyAnimator;

    //agent de navigation
    protected UnityEngine.AI.NavMeshAgent agent;

    //Récompense
    public int reward;
    private bool moneyGained = false;
 
    public void ApplyDamage(float TheDammage)
    {
        if (!isDead)
        {
            health -= TheDammage;
            print(gameObject.name + " a subit " + TheDammage + " points de dégâts || " + health + "/" + maxHealth);

            if (health <= 0)
            {
                isDead = true;
                audioManager.Play("EnemyDies");
                Dead();
            }
            else
            {
                audioManager.Play("EnemyTakesDmg");
            }
        }
    }

    public void Dead()
    {
        dying = true;
        enemyAnimator.applyRootMotion = false;
        enemyAnimator.SetBool("isDead", true);

        if (canMoove)
        {
            agent.destination = transform.position;
        }

        if(!moneyGained)
        {
            //méthode pour faire gagner de l'argent au joueur
            player.GetComponent<PlayerController>().YouKilledAnEnemy(reward);
            moneyGained = true;
        }
        Vector3 deathPosition = transform.position;
        gameManager.EnemyDied(enemyName, deathPosition);
        Destroy(transform.gameObject, 3);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
