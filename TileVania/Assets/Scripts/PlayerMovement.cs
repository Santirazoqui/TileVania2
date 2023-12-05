using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbingSpeed = 10f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 30f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;

    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;
    public bool isAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }

    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("IsRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpedd = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if(playerHasHorizontalSpedd)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        bool isTouchingGround = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool isTouchingLadder = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        if(!isTouchingGround && !isTouchingLadder) 
        {
            return;
        }
        if(value.isPressed)
        {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void ClimbLadder()
    {
        if(!myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        { 
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("IsClimbing", false);
            return; 
        }
        myRigidbody.gravityScale = 0f;
        Vector2 climbingVelocity = new Vector2(this.myRigidbody.velocity.x, this.moveInput.y * this.climbingSpeed);
        this.myRigidbody.velocity = climbingVelocity;
        bool playerHasVerticalSpeed = Mathf.Abs(this.myRigidbody.velocity.y) > Mathf.Epsilon;
        this.myAnimator.SetBool("IsClimbing", playerHasVerticalSpeed);
    }

    void Die()
    {
        bool isTouchingEnemy = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies"));
        bool isTouchingSpike = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards"));
        if(isTouchingEnemy || isTouchingSpike) 
        { 
            this.isAlive = false;
            this.myAnimator.SetTrigger("Dying");
            this.myRigidbody.velocity = deathKick;
            CameraShaker.Instance.ShakeCamera(5f, .1f);
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
    }
}
