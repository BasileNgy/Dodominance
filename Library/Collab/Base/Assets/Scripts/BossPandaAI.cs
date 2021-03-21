using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPandaAI : EnemyStats
{
    //Attacking
    private bool alreadyAttackedFireBall = false;
    private bool alreadyAttackedZone = false;

    public GameObject projectile;
    public Transform attackPoint;
    public Transform zonePoint;
    public Transform playerTrsfm;
    public float upForce;
    public float forwardForce;

    public float fireballCoolDown = 5;
    public float zoneCoolDown = 1;

    void Start()
    {
        enemyName = "Boss Panda";
        canMoove = true;

        playerObject = GameObject.Find("FirstPersonPlayer");
        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        health = maxHealth;
    }

    void Update()
    {
        if (!isDead)
        {
            //actualisation des paramètres de la cible
            playerTransform = playerObject.transform;

            //Check for attack range
            playerInAttackRange = Physics.CheckSphere(zonePoint.position, attackRange, whatIsPlayer);

            if (!alreadyAttackedFireBall)
            {
                FireBall();
            }

            if(!playerInAttackRange)
            {
                Chase();          
            }
            else
            {
                Idle();
            }
            zoneAttack();
        }
        else
        {
            Dead();
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
        ///Attack code
        //enemyAnimator.SetBool("Fireball", true);

        // Création de 3 fireball

        Vector3 direction = playerTransform.position - attackPoint.position;      
        GameObject bullet = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        bullet.transform.forward = direction.normalized;
        rb.AddForce(direction * forwardForce, ForceMode.Impulse);
        rb.AddForce(transform.up * upForce, ForceMode.Impulse);


        direction = new Vector3(playerTransform.position.x +2, playerTransform.position.y, playerTransform.position.z) - attackPoint.position;
        GameObject bullot = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody bb = bullot.GetComponent<Rigidbody>();
        bullot.transform.forward = direction.normalized;
        bb.AddForce(direction * forwardForce, ForceMode.Impulse);
        bb.AddForce(transform.up * upForce, ForceMode.Impulse);


        direction = new Vector3(playerTransform.position.x -2, playerTransform.position.y, playerTransform.position.z) - attackPoint.position;
        GameObject bullt = Instantiate(projectile, attackPoint.position, Quaternion.identity);
        Rigidbody bbb = bullt.GetComponent<Rigidbody>();
        bullot.transform.forward = direction.normalized;
        bbb.AddForce(direction * forwardForce, ForceMode.Impulse);
        bbb.AddForce(transform.up * upForce, ForceMode.Impulse);
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

    public override void OnDrawGizmosSelected()
    {
        //base.OnDrawGizmosSelected();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(zonePoint.position, attackRange);
    }
}
