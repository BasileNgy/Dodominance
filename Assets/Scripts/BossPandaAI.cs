using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPandaAI : EnemyStats
{
    //Attack
    private bool alreadyAttackedFireBall;
    private bool alreadyAttackedZone;

    private GameObject projectile;
    private Transform attackPoint;
    private Transform zonePoint;
    private float upForce;
    private float forwardForce;

    private float fireballCoolDown;
    private float zoneCoolDown;

    private void Awake()
    {
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        projectile = Resources.Load("Prefabs/Boss Panda Projectile") as GameObject;
        zonePoint = GameObject.Find("ZonePointBossPanda").transform;
        attackPoint = GameObject.Find("AttackPointBossPanda").transform;
        
    }

    void Start()
    {
        canMoove = true;
        chaseRange = 50;
        attackRange = 5;
        chaseSpeed = 6;

        maxHealth = 800;
        fireballCoolDown = 5;
        zoneCoolDown = 1; 
        upForce = 10;
        forwardForce = 1;
        alreadyAttackedFireBall = false;
        alreadyAttackedZone = false;
        
        reward = 100;

        health = maxHealth;
    }

    void Update()
    {
        if (!isDead)
        {
            playerInChaseRange = Physics.CheckSphere(transform.position, chaseRange, whatIsPlayer);

            if (playerInChaseRange)
            {
                //actualisation des paramètres de la cible
                playerTransform = playerObject.transform;

                //Check for attack range
                playerInAttackRange = Physics.CheckSphere(zonePoint.position, attackRange, whatIsPlayer);

                if (!alreadyAttackedFireBall)
                {
                    FireBall();
                }

                if (!playerInAttackRange)
                {
                    Chase();
                }
                else
                {
                    Idle();
                }
                zoneAttack();
            }
        }
    }

    private void Idle()
    {
        agent.destination = transform.position;
        enemyAnimator.SetBool("Chasing", false);
        enemyAnimator.SetBool("FireBall", false);
    }

    private void Chase()
    {
        agent.speed = chaseSpeed;
        agent.destination = playerTransform.position;
        enemyAnimator.SetBool("Chasing", true);
    }

    private void FireBall()
    {
        
        agent.destination = transform.position;
        enemyAnimator.SetBool("FireBall", false);

        // Création de 3 fireball à trajet aléatoire

        Vector3 direction = playerTransform.position - attackPoint.position;      
        GameObject bullet = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.forward = direction.normalized;
        rb.AddForce(direction * forwardForce, ForceMode.Impulse);
        rb.AddForce(transform.up * upForce, ForceMode.Impulse);


        Vector3 direction1 = new Vector3(playerTransform.position.x+2, playerTransform.position.y, playerTransform.position.z) - attackPoint.position;
        GameObject bullet1 = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody rb1 = bullet1.GetComponent<Rigidbody>();
        bullet1.transform.forward = direction1.normalized;
        rb1.AddForce(direction1 * forwardForce, ForceMode.Impulse);
        rb1.AddForce(transform.up * upForce, ForceMode.Impulse);


        Vector3 direction2 = new Vector3(playerTransform.position.x -2, playerTransform.position.y, playerTransform.position.z) - attackPoint.position;
        GameObject bullet2 = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
        bullet2.transform.forward = direction2.normalized;
        rb2.AddForce(direction2 * forwardForce, ForceMode.Impulse);
        rb2.AddForce(transform.up * upForce, ForceMode.Impulse);
        ///

        alreadyAttackedFireBall = true;
        Invoke(nameof(ResetAttackFireBall), fireballCoolDown);
        
    }

    private void zoneAttack()
    {
        if (!alreadyAttackedZone)
        {
            if(playerInAttackRange)
            {
                playerController.playerTakeDamage(damages);

                alreadyAttackedZone = true;
                Invoke(nameof(ResetAttackZone), zoneCoolDown);
            }  
        }    
    }

    private void ResetAttackFireBall()
    {
        alreadyAttackedFireBall = false;
    }
    private void ResetAttackZone()
    {
        alreadyAttackedZone = false;
    }
}
