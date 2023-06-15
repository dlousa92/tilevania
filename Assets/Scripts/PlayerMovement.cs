using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rigidBodyPlayer;
    Animator animatorPlayer;
    CapsuleCollider2D colliderPlayerBody;
    BoxCollider2D colliderPlayerFeet;
    SpriteRenderer spriteRendererPlayer;

    [Header("Movement")]
    [SerializeField] float runSpeed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float climbSpeed;

    [Header("Gravity")]
    [SerializeField] float defaultGravityScale;
    bool isTouchingGround;
    bool collidedWithEnemy = false;

    [Header("Death")]
    [SerializeField] Vector2 playerDeathLaunchValues = new Vector2(15f, 15f);
    [SerializeField] Color32 playerDeathColor;


    // Start is called before the first frame update
    void Start()
    {
        rigidBodyPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
        colliderPlayerBody = GetComponent<CapsuleCollider2D>();
        colliderPlayerFeet = GetComponent<BoxCollider2D>();
        spriteRendererPlayer = GetComponent<SpriteRenderer>();
        rigidBodyPlayer.gravityScale = defaultGravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfTouchingGround();

        if (collidedWithEnemy)
        {
            return;
        }
        CheckForPlayerDeath();
        ClimbLadder();
        PlayerRun();
    }

    void OnMove(InputValue value)
    {
        if (collidedWithEnemy) 
        {
            return;
        }

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (collidedWithEnemy) {
            return;
        }
        
        if(value.isPressed && isTouchingGround)
        {
            rigidBodyPlayer.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void CheckForPlayerDeath()
    {
        int deathLayerMasks = LayerMask.GetMask("Enemy", "Hazards");
        if (colliderPlayerBody.IsTouchingLayers(deathLayerMasks))
        {
            collidedWithEnemy = true;
            animatorPlayer.SetTrigger("playerDeath");
            rigidBodyPlayer.velocity = playerDeathLaunchValues;
            spriteRendererPlayer.color = playerDeathColor;
        }
    }

    void ClimbLadder()
    {
        int ladderLayerMask = LayerMask.GetMask("Ladders");
        bool isTouchingLadder = colliderPlayerFeet.IsTouchingLayers(ladderLayerMask);

        if (isTouchingLadder) {
            rigidBodyPlayer.gravityScale = 0f;
            rigidBodyPlayer.velocity = new Vector2(rigidBodyPlayer.velocity.x, moveInput.y * climbSpeed);

            bool isPlayerClimbing = Mathf.Abs(moveInput.y) > Mathf.Epsilon;
            animatorPlayer.SetBool("isClimbing", isPlayerClimbing);
        } else {
            rigidBodyPlayer.gravityScale = defaultGravityScale;
            animatorPlayer.SetBool("isClimbing", false);
        }
    }

    void CheckIfTouchingGround()
    {
        int groundLayerMask = LayerMask.GetMask("Ground");
        isTouchingGround = colliderPlayerFeet.IsTouchingLayers(groundLayerMask);

        if (!isTouchingGround) {
            return;
        } else {
            rigidBodyPlayer.gravityScale = defaultGravityScale;
        }
    }

    void PlayerRun()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, rigidBodyPlayer.velocity.y);
        rigidBodyPlayer.velocity = playerVelocity;

        bool isPlayerRunning = Mathf.Abs(moveInput.x) > Mathf.Epsilon;
        animatorPlayer.SetBool("isRunning", isPlayerRunning);

        FlipSprite(isPlayerRunning);
    }

    void FlipSprite(bool isPlayerRunning)
    {
        
        if (isPlayerRunning)
        {
            transform.localScale = new Vector2(Mathf.Sign(moveInput.x), transform.localScale.y);
        }
        
    }
}
