using UnityEngine;

public class PlayerController : MonoBehaviour
{


    //Scene References
    public GameManager gameManager;

    public CharacterController controller;
    public AudioManager audioManager;
    public SlotsController slotsController;

    //HUD References
    public HUDBar barHealth;
    public HUDText txtHealth;

    public HUDBar barArmor;
    public HUDText txtArmor;

    public HUDText txtMoney;


    //Player Stats
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float maxHealth  { get; private set; }
    public float currentHealth { get; private set; }
    public float maxArmor { get; private set; }
    public float currentArmor { get; private set; }

    public int currentMoney;


    private Vector3 velocity;
    private float xMovement;
    private float zMovement;
    private float playerSpeed = 6;
    private float gravity = -9.81f;
    private float jumpHeigt = 1;
    private bool isGrounded;

    void Update()
    {
        if (!gameManager.gameIsPaused)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            xMovement = Input.GetAxis("Horizontal") * Time.deltaTime;
            zMovement = Input.GetAxis("Vertical") * Time.deltaTime;

            Vector3 playerMovement = transform.right * xMovement + transform.forward * zMovement;

            controller.Move(playerMovement * playerSpeed);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeigt * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (!gameManager.gameIsPaused)
        {
        checkIfHealthAndArmorValid();
        }
    }

    void checkIfHealthAndArmorValid()
    {
        if (currentArmor > maxArmor)
        {
            Debug.Log("Player's armor went further than max value. Reducing armor to " + maxArmor);
            currentArmor = maxArmor;
            txtArmor.SetMyText(currentArmor.ToString());
        }

        if (currentHealth > maxHealth)
        {
            Debug.Log("Player's health went further than max value. Reducing health to " + maxHealth);
            currentHealth = maxHealth;
            txtHealth.SetMyText(currentHealth.ToString());
        }
    }

    public void YouKilledAnEnemy(int reward)
    {
        PlayerEarnMoney(reward);
    }

    public void PlayerEarnMoney(int money)
    {

        currentMoney += money;
        txtMoney.SetMyText(currentMoney.ToString());
    }

    public void PlayerLooseMoney(int money)
    {
        if(currentMoney < money)
        {
            currentMoney = 0;
            txtMoney.SetMyText(currentMoney.ToString());
            return;
        }
        currentMoney -= money;
        txtMoney.SetMyText(currentMoney.ToString());
    }

    void playerGainArmor(float armor)
    {
        Debug.Log("Player gain 20 armor");
        currentArmor += 20;
        barArmor.SetMyCurrentValue(currentArmor);
        txtArmor.SetMyText(currentArmor.ToString());

    }
    
    public void playerGainHealth(float health)
    {
        Debug.Log("Player gain 20 health");
        currentHealth += 20;
        barHealth.SetMyCurrentValue(currentHealth);
        txtHealth.SetMyText(currentHealth.ToString());

    }


    public void playerTakeDamage(float damage)
    {
        if(currentArmor > 0 && damage <= currentArmor)
        {
            audioManager.Play("HitArmor");
            currentArmor -= damage;
            barArmor.SetMyCurrentValue(currentArmor);
            txtArmor.SetMyText(currentArmor.ToString());
        }
        else if (currentArmor > 0 && damage > currentArmor)
        {
            audioManager.Play("HitArmor");
            float newDamage = damage - currentArmor;
            currentArmor = 0;
            barArmor.SetMyCurrentValue(currentArmor);
            txtArmor.SetMyText(currentArmor.ToString());
            playerTakeDamage(newDamage);
        }
        else if (currentHealth > 0 && damage < currentHealth)
        {
            audioManager.Play("HitHealth");
            currentHealth -= damage;
            barHealth.SetMyCurrentValue(currentHealth);
            txtHealth.SetMyText(currentHealth.ToString());
        }
        else
        {
            currentHealth = 0;
            audioManager.Play("PlayerDeath");
            barHealth.SetMyCurrentValue(currentHealth);
            txtHealth.SetMyText(currentHealth.ToString());
            gameManager.PlayerDies();
        }
    }

    public void playerReset(Vector3 spawnPosition)
    {
        //Reset player stats
        maxHealth = 100;
        currentHealth = maxHealth;
        maxArmor = 100;
        currentArmor = 0;

        currentMoney = 0;

        gameManager.SetPlayerHUD();

        //Reset player position
        teleportPlayer(spawnPosition);
    }

    public void teleportPlayer(Vector3 newPosition)
    {

        controller.enabled = false;
        controller.transform.position = newPosition;
        controller.enabled = true;
    }

}
