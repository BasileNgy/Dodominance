using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretAI : EnemyStats
{
    //Attacking
    private bool alreadyAttacked;
    
    private GameObject projectile;
    private Transform attackPoint;
    private float upForce;

    private void Awake()
    {
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        attackPoint = GameObject.Find("AttackPointRedPanda").transform;
        projectile = Resources.Load("Prefabs/Ranger Projectile") as GameObject;
    }
    void Start()
    {
        enemyName = "Red Panda";
        attackRange = 15;
        chaseRange = 0;
        chaseSpeed = 0;

        maxHealth = 80;
        attackRepeatTime = 1.5f;
        upForce = 4;

        reward = 10;

        health = maxHealth;
    }
   
    void Update()
    {
        if (!isDead)
        {
            //actualisation des paramètres de la cible
            playerTransform = playerObject.transform;

            //Check for attack range
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInAttackRange) Idle();
            else AttackPlayer();
        }
    }

    private void Idle()
    {
        enemyAnimator.SetBool("AttackRange", false);
    }

    private void AttackPlayer()
    {
        transform.LookAt(playerTransform);

        if(!alreadyAttacked)
        {
            ///Attack code
            enemyAnimator.SetBool("AttackRange", true);

            Vector3 direction = playerTransform.position - attackPoint.position;

            GameObject bullet = Instantiate(projectile, attackPoint.position, Quaternion.identity);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            
            bullet.transform.forward = direction.normalized;
            rb.AddForce(direction, ForceMode.Impulse);
            rb.AddForce(transform.up * upForce, ForceMode.Impulse);

            ///

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), attackRepeatTime);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    
}
