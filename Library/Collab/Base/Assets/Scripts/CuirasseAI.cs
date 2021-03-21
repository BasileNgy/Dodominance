using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuirasseAI : EnemyStats
{
    //attack
    public float attackLenght = 1.3f;
    private float punchTime;
    private float halfPunchTime;
    private bool playerAlreadyHit = false;

    //Range
    public Transform attackPoint;
    private bool playerInDamageRange;

    // Start is called before the first frame update
    void Start()
    {
        enemyName = "Panda";
        chaseRange = 15;
        attackRange = 3;
        chaseSpeed = 4;
        maxHealth = 204;
        attackRepeatTime = 5;
        damages = 10;
        reward = 15;
        canMoove = true;

        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        health = maxHealth;
        attackTime = Time.time;
        punchTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            //recherche du joueur
            playerTransform = playerObject.transform;

            //Check for sight and attack range
            playerInDamageRange = Physics.CheckSphere(attackPoint.position, attackRange - 1f, whatIsPlayer);           
            playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);        
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            //peut chase uniquement s'il a finit de taper
            if (Time.time > punchTime)
            {
                isAttacking = false;
                playerAlreadyHit = false;
            }
            if (!playerInChaseRange && !playerInDamageRange) Idle();
            if (playerInChaseRange && !playerInDamageRange) Chase();
            if (playerInChaseRange && playerInDamageRange) Attack();
        }
    }

    //poursuite
    private void Chase()
    {   
        if(!isAttacking)
        {
            agent.speed = chaseSpeed;
            enemyAnimator.SetBool("ChaseRange", true);
            enemyAnimator.SetBool("AttackRange", false);

            agent.destination = playerTransform.position;
        }
          
    }

    private void Attack()
    {
        //empeche l'ennemi de traverser le joueur
        agent.destination = transform.position;

        //Cooldown des attacks 
        if (Time.time > attackTime)
        {
            enemyAnimator.SetBool("AttackRange", true);
            Debug.Log("Attack");

            isAttacking = true;
            //cooldown
            attackTime = Time.time + attackRepeatTime;
            //durée coup de patte
            punchTime = Time.time + attackLenght; 
            halfPunchTime = Time.time + attackLenght/1.5f;
        }
        else if(Time.time > halfPunchTime && !playerAlreadyHit && isAttacking)
        {          
            if(playerInDamageRange)
            {
                // Appel de méthode de player pour infliger des dommages au joueur
                playerController.playerTakeDamage(damages);
                playerAlreadyHit = true;
            } 
        }
    }

    private void Idle()
    {
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        enemyAnimator.SetBool("ChaseRange", false);
        enemyAnimator.SetBool("AttackRange", false);
        agent.destination = transform.position;
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange -1f);
    }
}
