using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CuirasseAI : EnemyStats
{
    //attack
    private float attackLenght;
    private float punchTime;
    private float halfPunchTime;
    private bool playerAlreadyHit;

    //Range
    private Transform attackPoint;

    private void Awake()
    {
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        List<Transform> pandaTransforms = gameObject.GetComponentsInChildren<Transform>().ToList();
        attackPoint = pandaTransforms.Find(transform => transform.name.Equals("AttackPointPanda"));
    }

    void Start()
    {
        chaseRange = 15;
        attackRange = 2;
        canMoove = true;
        chaseSpeed = 4;

        maxHealth = 204;    
        attackRepeatTime = 5;
        damages = 10;
        attackLenght = 1.3f;
        playerAlreadyHit = false;

        reward = 15;
        
        health = maxHealth;
        attackTime = Time.time;
        punchTime = Time.time;
    }


    void Update()
    {
        if (!isDead)
        {
            //recherche du joueur
            playerTransform = playerObject.transform;

            //Check for sight and attack range     
            playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);        
            playerInAttackRange = Physics.CheckSphere(attackPoint.position, attackRange, whatIsPlayer);

            //peut chase uniquement s'il a finit de taper
            if (Time.time > punchTime)
            {
                isAttacking = false;
                playerAlreadyHit = false;
            }
            if (!playerInChaseRange && !playerInAttackRange) Idle();
            if (playerInChaseRange && !playerInAttackRange) Chase();
            if (playerInChaseRange && playerInAttackRange) Attack();
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
            if(playerInAttackRange)
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
}
