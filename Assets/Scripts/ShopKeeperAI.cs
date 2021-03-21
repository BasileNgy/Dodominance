using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ShopKeeperAI : EnemyStats
{

    private Transform attackPoint;
    private bool alreadyAttacked;

    private void Awake()
    {
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();

        List<Transform> shopKeeperTransforms = gameObject.GetComponentsInChildren<Transform>().ToList();
        attackPoint = shopKeeperTransforms.Find(transform => transform.name.Equals("AttackPointShopKeeper"));
    }

    void Start()
    {
        chaseRange = 20;
        attackRange = 2;
        canMoove = true;
        chaseSpeed = 4f;

        maxHealth = 251;
        attackRepeatTime = 2.1f;
        attackTime = Time.time;
        isAttacking = false;
        alreadyAttacked = false;
        damages = 30;

        reward = 413;

        health = maxHealth;
    }

    void Update()
    {
        if (!isDead && !gameManager.gameIsPaused)
        {
            //recherche du joueur
            playerTransform = playerObject.transform;

            //Check for sight and attack range
            playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInChaseRange && !playerInAttackRange) Idle();
            else if (playerInChaseRange && !playerInAttackRange) Chase();
            else if (playerInChaseRange && playerInAttackRange) Attack();
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

        if (!isAttacking)
        {
            isAttacking = true;

            enemyAnimator.SetBool("AttackRange", true);
            enemyAnimator.SetBool("ChaseRange", false);

            Invoke(nameof(ResetAttackCoolDown), attackRepeatTime);
        }
    }

    private void ResetAttackCoolDown()
    {
        isAttacking = false;
        alreadyAttacked = false;
    }

    public void PunchCollision()
    {
        if (isAttacking && !alreadyAttacked)
        {
            playerController.playerTakeDamage(damages);
            alreadyAttacked = true;
        }
    }

    private void Idle()
    {
        enemyAnimator.SetBool("ChaseRange", false);
        enemyAnimator.SetBool("AttackRange", false);
        agent.destination = transform.position;
    }

}
