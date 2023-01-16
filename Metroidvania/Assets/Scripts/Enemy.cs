using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f; // enemy speed
    public int direction = 1; // enemy movement direction
    public int damage = 1; // damage dealt to player on contact
    public int bulletDamage = 1; // damage dealt to enemy by bullets
    public int maxHealth = 1; // maximum health of enemy
    public int currentHealth; // current health of enemy
    public bool isInvincible = false; // is the enemy invincible?
    public bool isAttacking = false; // is the enemy attacking?
    public bool isDead = false; // is the enemy dead?

    private Rigidbody2D rb2d; // enemy's Rigidbody2D component
    private SpriteRenderer spriteRenderer; // enemy's SpriteRenderer component

    void Start()
    {
        // get the enemy's Rigidbody2D and SpriteRenderer components
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // set the enemy's current health to its maximum health
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead)
        {
            // do nothing if the enemy is dead
            return;
        }

        // move the enemy horizontally
        rb2d.velocity = new Vector2(speed * direction, rb2d.velocity.y);

        if (isAttacking)
        {
            // attack the player if the enemy is attacking
            Attack();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // if the enemy collides with an object, turn around
        if (other.gameObject.tag != "Player")
        {
            direction *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // if the enemy collides with the player, deal damage to the player
        if (collision.gameObject.tag == "Player")
        {
            //Player player = collision.gameObject.GetComponent<Player>();
            //player.TakeDamage(damage);
        }
    }

    void Attack()
    {
        // attack logic goes here
    }

    public void TakeDamage(int damage, bool isBullet)
    {
        if (isInvincible)
        {
            // do nothing if the enemy is invincible
            return;
        }

        if (isBullet)
        {
            // if the enemy is hit by a bullet, reduce its health by the bullet damage
            currentHealth -= bulletDamage;
        }
        else
        {
            // reduce the enemy's current health by the amount of damage taken
            currentHealth -= damage;
        }

        if (currentHealth <= 0)
        {
            // the enemy is dead
            //Die();
        }
    }
}



