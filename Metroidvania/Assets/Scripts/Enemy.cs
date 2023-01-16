using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; 
    public int direction = 1; 
    public int damage = 1; 
    public int bulletDamage = 1; 
    public int maxHealth = 1; 
    public int currentHealth; 
    public bool isInvincible = false; 
    public bool isAttacking = false; 
    public bool isDead = false; 

    private Rigidbody2D rb2d; 
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead)
        {          
            return;
        }

        
        rb2d.velocity = new Vector2(speed * direction, rb2d.velocity.y);

        if (isAttacking)
        {          
            Attack();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.tag != "Player")
        {
            direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
       
        if (collision.gameObject.tag == "Player")
        {
            //Player player = collision.gameObject.GetComponent<Player>();
            //player.TakeDamage(damage);
        }
    }

    void Attack()
    {
       
    }

    public void TakeDamage(int damage, bool isBullet)
    {
        if (isInvincible)
        {           
            return;
        }

        if (isBullet)
        {            
            currentHealth -= bulletDamage;
        }
        else
        {           
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            //Die();
        }
    }
}



