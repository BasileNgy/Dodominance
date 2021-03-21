using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChickenAI : EnemyStats
{

    // Start is called before the first frame update
    void Start()
    {
        chaseRange = 20;
        attackRange = 2;
        chaseSpeed = 2.8f;
        maxHealth = 60;
        attackRepeatTime = 2;
        damages = 5;
        reward = 2;

        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        player = GameObject.Find("FirstPersonPlayer");
        health = maxHealth;
        attackTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            //recherche du joueur
            playerTransform = player.transform;

            //Check for sight and attack range
            playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInChaseRange && !playerInAttackRange) Idle();
            if (playerInChaseRange && !playerInAttackRange) Chase();
            if (playerInChaseRange && playerInAttackRange) Attack();    
        }
        else
        {
            agent.destination = transform.position;
            Dead();
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
            player.GetComponent<PlayerController>().playerTakeDamage(damages);
            
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
