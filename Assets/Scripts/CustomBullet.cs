using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    //Assignables
    private Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsPlayer;
    private GameObject player;

    //Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    //Damage 
    public float explosionDamage = 10f;
    public float explosionRange = 1.5f;

    //Lifetime
    private bool isExploded = false;
    public int maxCollisions = 0;
    public float maxLifetime = 2f;
    public bool explodeOnTouch = true;

    private int collisions = 0;
    PhysicMaterial physics_mat;

    //Effets sonores
    public AudioManager audioManager;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        Setup();
    }

    private void Update()
    {
        if(!isExploded)
        {
            //when to explode
            if (collisions > maxCollisions) Explode();

            //Count down lifetime
            maxLifetime -= Time.deltaTime;
            if (maxLifetime <= 0) Explode();
        } 
    }

    private void Explode()
    {
        if(!isExploded)
        {
            //Instanciate explosion
            if (explosion != null) Instantiate(explosion, transform.position, Quaternion.identity);
            //audioManager.Play("Explosion");

            //check for player
            if (Physics.CheckSphere(transform.position, explosionRange, whatIsPlayer))
            {
                player = GameObject.Find("FirstPersonPlayer");
                player.GetComponent<PlayerController>().playerTakeDamage(explosionDamage);
            }

            isExploded = true;

            //add a delay and destroy object
            DestroyBall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isExploded)
        {
            if(!collision.collider.CompareTag("Enemy")) collisions++;
            
            //Explode if bullet hits an ennemy directly and explodeOnTouch is activated
            if (collision.collider.CompareTag("Player") && explodeOnTouch) Explode();
        }  
    }

    private void DestroyBall()
    {
        Destroy(gameObject, 0.05f);
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        //Assign materials to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
