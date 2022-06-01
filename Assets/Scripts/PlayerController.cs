using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float verticalInitialSpeed;
    public float horizontalMaxSpeed;
    public float accelerationTime;
    public float maxJumpDuration;
    public float jumpIncrementSpeed;

    public const string topCheckerName = "Player Top Checker";

    private const KeyCode leftKey = KeyCode.LeftArrow;
    private const KeyCode rightKey = KeyCode.RightArrow;
    private const KeyCode upKey = KeyCode.Space;
    private const string animationStateVarName = "motionState";
    private const float jumpVelocityTolerance = 0.1F;

    private MainController mainController;
    private SpriteRenderer spriteRenderer;
    private new Rigidbody2D rigidbody2D;
    private Animator animator;
    private AudioSource audioSource;

    private Vector3 horizontalForceIncrement;
    private Vector3 jumpImpulseIncrement;
    private Vector3 jumpForceIncrement;
    private float jumpDurationCounter;
    private int currentAnimationStatus = 0;
    private bool isFacingRight = true;
    private HashSet<Collider2D> terrainColliders;

    private bool[] upKeyPressed = {false,false};
    
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        horizontalForceIncrement = new Vector3(horizontalMaxSpeed/accelerationTime,0,0);   
        jumpImpulseIncrement = new Vector3(0,verticalInitialSpeed,0);
        jumpForceIncrement = new Vector3(0,jumpIncrementSpeed/maxJumpDuration,0);

        jumpDurationCounter = -1F;
        upKeyPressed[0] = false;
        upKeyPressed[1] = false;
        currentAnimationStatus = 0;
        isFacingRight = true;
        terrainColliders = new HashSet<Collider2D>();
    }

    void Start() {
        mainController = GameObject.FindGameObjectWithTag(Utils.MainControllerTag).GetComponent<MainController>();
    }

    void FixedUpdate() {
        UpdateKey();
        MoveHorizontally();
        MoveVertically();
    }

    void Update() {
        Animate(); 
    }  

    private void MoveHorizontally() {
        if (!IsPressedHorizontally()) {
            if (Math.Abs(GetHorizontalSpeed())<jumpVelocityTolerance) rigidbody2D.velocity = new Vector2(0F,rigidbody2D.velocity.y);
            return;
        }

        float currentSpeed = GetHorizontalSpeed() * GetHorizontalAxis();
        
        if (currentSpeed<horizontalMaxSpeed) {
            rigidbody2D.AddForce(horizontalForceIncrement * GetHorizontalAxis());
        }
    }

    private void MoveVertically() {
        if (terrainColliders.Count==0) {
            if (jumpDurationCounter < -0.5F) return;
            if ((!Input.GetKey(upKey))||(jumpDurationCounter > maxJumpDuration)) {
                jumpDurationCounter = -1F;
                return;
            }
            jumpDurationCounter += Time.deltaTime;
            rigidbody2D.AddForce(jumpForceIncrement);
            return;
        }
        if (!CanJump()) return;
        if (!IsUpKeyUp()) return;
        rigidbody2D.AddForce(jumpImpulseIncrement,ForceMode2D.Impulse);
        jumpDurationCounter = 0F;
        PlayJumpSound();
    }

    private bool IsPressedHorizontally() {
        return Input.GetKey(leftKey) ^ Input.GetKey(rightKey);
    }

    private float GetHorizontalAxis() {
        if (!IsPressedHorizontally()) return 0F;
        return Input.GetKey(rightKey)? 1F : -1F;
    }

    private float GetHorizontalSpeed() {
        return rigidbody2D.velocity.x;
    }

    private bool IsMovingHorizontally() {
        return Math.Abs(GetHorizontalSpeed()) >= Utils.epsilon;
    }

    private void UpdateKey() {
        upKeyPressed[0] = upKeyPressed[1];
        upKeyPressed[1] = Input.GetKey(upKey);
    }

    private bool IsUpKeyUp() {
        return (!upKeyPressed[0]) && upKeyPressed[1];
    }

    public void onChildTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(Utils.TerrainTag)
            || other.gameObject.CompareTag(Utils.PlatformTag)) {
            terrainColliders.Add(other);
        }
    }

    public void onChildTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag(Utils.TerrainTag)
            || other.gameObject.CompareTag(Utils.PlatformTag)) {
            terrainColliders.Remove(other);
        }
    }

    public bool CanJump() {
        return (rigidbody2D.velocity.y<=Utils.epsilon) && (terrainColliders.Count>0);
    }

    private int GetAnimationStateNumber(string state) {
        switch (state) {
            case "Idle":
                return 0;
            case "Jumping":
                return 1;
            case "Running":
                return 2;
            case "Skidding":
                return 3;
            default:
                throw new NotImplementedException();
        }
    }

    private void Animate() {
        if (IsMovingHorizontally()) {
            isFacingRight = GetHorizontalSpeed() >= 0F;
        }

        int nextAnimationStatus = GetNextAnimationStatus();
        if (nextAnimationStatus!=currentAnimationStatus) {
            currentAnimationStatus = nextAnimationStatus;
            animator.SetInteger(animationStateVarName,currentAnimationStatus);
        }

        spriteRenderer.flipX = (!isFacingRight) ^ (currentAnimationStatus==GetAnimationStateNumber("Skidding"));
    }

    private int GetNextAnimationStatus() {
        if (!CanJump()) {
            return GetAnimationStateNumber("Jumping");
        }
        if (!IsMovingHorizontally()) {
            return GetAnimationStateNumber("Idle");
        }
        if (!IsPressedHorizontally()) {
            return GetAnimationStateNumber("Skidding");
        }
        string status = 10000F * GetHorizontalSpeed() * GetHorizontalAxis() > 0F? "Running" : "Skidding";
        return GetAnimationStateNumber(status);
    }

    public void PlayJumpSound() {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
