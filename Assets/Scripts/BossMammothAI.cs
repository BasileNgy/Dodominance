using UnityEngine;

public class BossMammothAI : EnemyStats
{
    //Components
    private Rigidbody rb;

    private float speedRotation;

    private bool alreadyTakeDamage;

    //Timer and boolean state
    private bool isBraking;
    private bool isDashing;
    private bool isCharging;
    private float chargeDuration;
    private float dashDuration;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
    }

    void Start()
    {
        canMoove = false;
        attackRange = 50;
        speedRotation = 5;

        dashDuration = 1.3f;
        chargeDuration = 1;
        attackRepeatTime = 3f;
        damages = 40;       
        maxHealth = 1100;

        reward = 150;

        alreadyTakeDamage = false;
        isCharging = true;
        isDashing = false;
        isBraking = false;

        health = maxHealth;
    }

    void Update()
    {
        if(!isDead)
        {
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInAttackRange)
            {
                if (isCharging)
                {
                    //Rotation
                    Vector3 direction = playerObject.transform.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, speedRotation * Time.deltaTime);

                    Invoke(nameof(ResetChargeCoolDown), chargeDuration);
                }
                else if (!isDashing)
                {
                    Dash();

                    isDashing = true;
                    Invoke(nameof(ResetDashCoolDown), attackRepeatTime);
                }
                else if (isBraking)
                {
                    StoppingDash();
                }
            }
        }
        else
        {
            rb.velocity *= 0f;
        }     
    }

    private void Dash()
    {
        rb.AddForce(transform.forward * 30, ForceMode.VelocityChange);
        isBraking = false;
        alreadyTakeDamage = false;

        enemyAnimator.SetBool("Dash", true);

        Invoke(nameof(StoppingDash), dashDuration);
    }

    private void StoppingDash()
    {
        rb.velocity *= 0.95f;
        rb.angularVelocity *= 0.5f;
        isBraking = true;
        Invoke(nameof(StoppingDashAnimation), 1);
    }


    private void StoppingDashAnimation()
    {
        enemyAnimator.SetBool("Dash", false);
        isCharging = true;
    }

    private void ResetDashCoolDown()
    {
        isDashing = false;
    }

    private void ResetChargeCoolDown()
    {
        isCharging = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && isDashing && !alreadyTakeDamage)
        {
            playerController.playerTakeDamage(damages);

            alreadyTakeDamage = true;
        }
    }

}
