using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Panda,
    RedPanda,
    Chicken,
    ShopKeeper,
    BossPanda,
    BossMammoth
}

[System.Serializable]
public class EnemyStats : MonoBehaviour
{

    //GameManager
    public GameManager gameManager;

    public EnemyType myType;
    //Joueur ciblé
    public GameObject playerObject;
    public PlayerController playerController;
    public Transform playerTransform;
    protected LayerMask whatIsPlayer = 1024;

    //Ranges
    protected float chaseRange, attackRange;
    protected bool playerInChaseRange, playerInAttackRange, canMoove;
    protected float chaseSpeed;

    //health
    protected float maxHealth;
    protected float health;
    public bool isDead = false;
    protected bool dying = false;

    //Cooldown des attaques
    protected float attackRepeatTime;
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
            playerController.YouKilledAnEnemy(reward);
            moneyGained = true;
        }
        Vector3 deathPosition = transform.position;
        gameManager.EnemyDied(this, deathPosition);
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
