using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChickenAI : EnemyStats
{

    private void Awake()
    {
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
    }

    void Start()
    {
        chaseRange = 20;
        attackRange = 2;
        canMoove = true;
        chaseSpeed = 2.8f;

        maxHealth = 60;
        attackRepeatTime = 2;
        attackTime = Time.time;
        damages = 5;
        
        reward = 2;
        
        health = maxHealth;       
    }

    void Update()
    {
        if (!isDead)
        {
            //recherche du joueur
            playerTransform = playerObject.transform;

            //Check for sight and attack range
            playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInChaseRange && !playerInAttackRange) Idle();
            if (playerInChaseRange && !playerInAttackRange) Chase();
            if (playerInChaseRange && playerInAttackRange) Attack();    
        }
    }

    //poursuite
    private void Chase()
    {
        agent.speed = chaseSpeed;
        enemyAnimator.SetBool("ChaseRange", true);
        enemyAnimator.SetBool("AttackRange", false);
             
        agent.destination = playerTransform.position;     
    }

    private void Attack()
    {      
        //empeche l'ennemi de traverser le joueur
        agent.destination = transform.position;
        transform.LookAt(playerTransform);

        if (Time.time > attackTime)
        {
            enemyAnimator.SetBool("AttackRange", true);
            enemyAnimator.SetBool("ChaseRange", false);

            // Appel de méthode de player pour infliger des dommages au joueur
            playerController.playerTakeDamage(damages);
            
            attackTime = Time.time + attackRepeatTime;
        }
    }

    private void Idle()
    {
        enemyAnimator.SetBool("ChaseRange", false);
        enemyAnimator.SetBool("AttackRange", false);
        agent.destination = transform.position;
    }

}
