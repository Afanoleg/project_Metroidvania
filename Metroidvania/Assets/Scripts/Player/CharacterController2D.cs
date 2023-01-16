using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterController2D : MonoBehaviour
{
    public float speed = 10.0f;
    public float jumpForce = 500.0f;
    public float doubleJumpForce = 300.0f;
    public float crouchSpeed = 5.0f;
    public float dashSpeed = 20.0f;
    public float dashDuration = 0.5f;
    public bool canShoot = true;
    public int maxJumps = 2;
    private int jumps = 0;
    public bool canDash = true;
    public GameObject projectile;
    public Transform shotPoint;
    public int maxHealth = 100;

    private bool facingRight = true;
    private bool isGrounded = false;
    private bool isCrouching = false;
    private bool isShooting = false;
    private bool isCrouchShooting = false;
    private bool isDashing = false;
    private int currentHealth;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Fall();
        DoubleJump();
        Crouch();
        Dash();
        Shoot();
        CrouchShoot();

        if (isGrounded)
        {
            animator.SetBool("Jump", false);
            animator.SetBool("Fall", false);
        }
        else
        {
            animator.SetBool("Jump", rb.velocity.y > 0);
            animator.SetBool("Fall", rb.velocity.y < 0);
        }

        float move = Input.GetAxis("Horizontal");
        if (isShooting || isCrouchShooting)
        {
            move = 0;
        }
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(move));
        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            jumps = 0;
        }
    }

    #region Movment
    private void Move()
    {
        float move = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(move * speed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(move));
        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void ApplyMovement()
    {
        if (isCrouching)
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * crouchSpeed, rb.velocity.y);
        }
        else if (isDashing)
        {
            rb.velocity = new Vector2(dashSpeed * transform.localScale.x, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            isGrounded = false;
            rb.AddForce(Vector2.up * jumpForce);
            animator.SetTrigger("Jump");
        }
        else if (!isGrounded)
        {
            if (rb.velocity.y > 0)
            {
                animator.SetBool("isJump", true);
            }
            else
            {
                animator.SetBool("isJump", false);
            }
        }
    }

    private void DoubleJump()
    {
        if (Input.GetButtonDown("Jump") && !isGrounded && jumps < maxJumps)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * doubleJumpForce);
            jumps++;
        }
    }


    private void Fall()
    {
        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    private void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
            animator.SetBool("isCrouching", true);
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
            animator.SetBool("isCrouching", false);
        }
    }

    private void Dash()
    {
        if (Input.GetButtonDown("Dash") && canDash)
        {
            canDash = false;
            isDashing = true;
            animator.SetTrigger("Dash");
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(0.1f);
        canDash = true;
    }
    #endregion

    #region Shooting
    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            canShoot = false;
            animator.SetTrigger("Shoot");
            StartCoroutine(ShootCoroutine());
        }
    }

    private void CrouchShoot()
    {
        if (Input.GetButtonDown("Fire1") && isCrouching && canShoot)
        {
            canShoot = false;
            animator.SetTrigger("CrouchShoot");
            StartCoroutine(CrouchShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        if (facingRight)
        {
            yield return new WaitForSeconds(0.15f);
            Vector3 shotOffset = transform.localScale.x * Vector3.right * 0.5f;
            GameObject shot = Instantiate(projectile, shotPoint.position + shotOffset, transform.rotation);
            shot.GetComponent<Rigidbody2D>().AddForce(transform.localScale.x * Vector2.right * 400);
            yield return new WaitForSeconds(0.15f);
            canShoot = true;
        }
        else
        {
            yield return new WaitForSeconds(0.15f);
            Vector3 shotOffset = transform.localScale.x * Vector3.right * 0.5f;
            GameObject shot = Instantiate(projectile, shotPoint.position + shotOffset, transform.rotation);
            shot.GetComponent<Rigidbody2D>().AddForce(transform.localScale.x * Vector2.left * 400);
            yield return new WaitForSeconds(0.15f);
            canShoot = true;
        }
    }

    IEnumerator CrouchShootCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject shot = Instantiate(projectile, shotPoint.position, transform.rotation);
        shot.GetComponent<Rigidbody2D>().AddForce(transform.localScale.x * Vector2.right * 400);
        yield return new WaitForSeconds(0.5f);
        canShoot = true;
    }
    #endregion

    #region Health Interactions
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Death();
        }
        else
        {
            GettingHit();
        }
    }

    private void GettingHit()
    {
        animator.SetTrigger("GettingHit");
    }

    private void Death()
    {
        animator.SetTrigger("Death");
        Destroy(gameObject, 1.0f);
    }
    #endregion
}


