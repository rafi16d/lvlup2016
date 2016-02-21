using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour {

    [Header("Movements")]
    public float maxSpeed = 10.0f;
    public float Gravity = 9.87f;
    public float JumpStrength = 4.0f;
    public float ControlEffectivenessAir = 0.5f;

    [Header("Animation")]
    public Animator animator;
    public AudioClip runningSound;

    [Header("Extension")]
    public bool WallFriction = true;
    [Space(8)]
    public bool WallJump = true;
    public Vector2 WallJumpStrength = Vector2.one * 5;
    [Space(8)]
    public bool DownJump = true;
    public float downJumpStrength = 5;
    [Space(8)]
    public bool PlayerCrash = true;
    public float PlayerCrashForce1 = 5;
    public float PlayerCrashForce2 = 10;
    public float PlayerAfterCrashSleepTime = 1;
    [Space(8)]
    public bool DoubleJump = true;
    public float DoubleJumpStrength = 4f;
    [Space(8)]
    public bool Sprint = true;
    public float SpeedMultiplier = 2;
    [Range(0.1f, 2)]
    public float timeAfterKeyRelease = 0.5f;

    [Header("Miscellaneous")]
    public bool debugMode = false;
    public bool showColliderState = false;

    [Header("Autre")]
    public float limitLineYBottom = -10f;
    public GameObject pauseScreen;
    //public GameObject GameOverScreen;

    [HideInInspector()]
    public Rigidbody2D rb2d;

    [HideInInspector()]
    public Collider2D coll2D;

    [HideInInspector()]
    public PlayerInput playerInput;

    private Vector2 frameMovement = Vector2.zero;
    private playerCollision collisionState;

    private bool isFalling = false;
    private bool hasBeenDownJump = false;
    private bool hasBeenYStopped = false;
    private float lastJump = 0;
    private int playerCrashLevel = 0;
    private float sleep = 0;
    private bool doubleJumpUsed = false;
    private bool facingRight = false;
    private int life = 3;
    private bool isAttacking = false;
    private float timeAttack = 0;
    private Vector2 startPos;
    private AudioSource audio;
    private bool isRunningSoundPlaying = false;

    void Start() {
        Time.timeScale = 1.0f;
        this.startPos = this.transform.position;
        this.playerInput.update();
        this.rb2d = this.GetComponent<Rigidbody2D>();
        this.coll2D = this.GetComponent<Collider2D>();
        this.collisionState = new playerCollision(this.debugMode);
        this.audio = this.GetComponent<AudioSource>();
    }

    void Update() {
        this.playerInput.update();
        this.checkMovement();
        this.checkDebug();
        this.checkFlip();
        this.checkAttack();
        this.checkSound();
    }

    void checkFlip() {
        if (this.playerInput.rawX == 1 && this.facingRight)
            flip();
        else if (this.playerInput.rawX == -1 && !this.facingRight)
            flip();
    }

    void flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void FixedUpdate() {
        if (this.life == 0) {
            //GameOverScreen.SetActive(true);
            SceneManager.LoadScene("gameover");
            // Time.timeScale = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }

        if (isFalling) {
            this.gameObject.layer = 9;
        }
        this.rb2d.velocity = new Vector2(this.frameMovement.x, this.rb2d.velocity.y);
    }



    void checkMovement() {
        //Must the player sleep ?
        if (Time.time <= this.sleep)
            return;

        checkPlayerState();

        //input x
        this.frameMovement.x = this.playerInput.x * this.maxSpeed * ((this.collisionState.noCollision) ? this.ControlEffectivenessAir : 1);

        //jump
        if (this.collisionState.isGrounded && this.playerInput.jump && lastJump + 0.1 <= Time.time)
            jump(this.JumpStrength);

        //player touch the top or the ground
        if ((this.collisionState.top || this.collisionState.isGrounded) && !this.hasBeenYStopped) {
            this.frameMovement.y = 0;
            this.hasBeenYStopped = true;
        } else if (this.hasBeenYStopped)
            this.hasBeenYStopped = false;

        //is player under the least Y allowed ?
        if (this.transform.position.y < this.limitLineYBottom) {
            SceneManager.LoadScene("gameover");
            Debug.Log("Falling");
        }

        fixMovement();

        // extension

        if (this.WallFriction)
            checkWallFriction();

        if (this.WallJump)
            checkWallJump();

        if (this.DownJump)
            checkDownJump();

        if (this.PlayerCrash)
            checkPlayerCrash();

        if (this.DoubleJump)
            checkDoubleJump();

        if (this.Sprint)
            checkSprint();

        //var anim
        this.animator.SetFloat("speed", Mathf.Abs(this.rb2d.velocity.x));

        if (this.isFalling) {
            this.animator.SetBool("isJumping", false);
            this.animator.SetBool("isFalling", true);
        }

        if (this.collisionState.isGrounded) {
            this.animator.SetBool("isFalling", false);
            this.animator.SetBool("isOnGround", true);
        }
    }

    void OnCollisionStay2D(Collision2D coll) {
        this.collisionState.updatePlayerCollisionState(this.coll2D, coll, playerCollision.ENTER);
    }

    void OnCollisionExit2D(Collision2D coll) {
        this.collisionState.updatePlayerCollisionState(this.coll2D, coll, playerCollision.EXIT);
    }

    public Collider2D getColl2D() {
        return this.coll2D;
    }

    void checkPlayerState() {
        if (this.rb2d.velocity.y < 0)
            this.isFalling = true;
        else
            this.isFalling = false;
    }

    void jump(float Strenght) {
        this.rb2d.velocity = new Vector2(this.rb2d.velocity.x, 0);
        this.rb2d.AddForce(new Vector2(0, Strenght), ForceMode2D.Impulse);
        this.lastJump = Time.time;
        this.animator.SetBool("isJumping", true);

        this.gameObject.layer = 10;

    }

    void fixMovement() {
        // For unknown reason, the player stop moving sometime when he is between two collider. This line solve this problem.
        if (this.rb2d.velocity.x == 0 && !this.collisionState.left && !this.collisionState.right && this.playerInput.rawX != 0)
            this.rb2d.AddForce(new Vector2(0, 0.2f), ForceMode2D.Impulse);
    }

    void checkWallFriction() {
        if ((this.collisionState.left || this.collisionState.right) && this.isFalling)
            this.rb2d.gravityScale = (this.playerInput.rawX != 0) ? 0.1f : 0.3f;
        else
            this.rb2d.gravityScale = 1;
    }

    void checkWallJump() {
        if (this.collisionState.left && this.isFalling && this.playerInput.rawX == 1 && this.playerInput.rawY == 1) {
            this.rb2d.AddForce(new Vector2(this.WallJumpStrength.x, this.WallJumpStrength.y), ForceMode2D.Impulse);
            this.playerInput.addAction(true, false, false, true);
        }


        if (this.collisionState.right && this.isFalling && this.playerInput.rawX == -1 && this.playerInput.rawY == 1) {
            this.rb2d.AddForce(new Vector2(-(this.WallJumpStrength.x), this.WallJumpStrength.y), ForceMode2D.Impulse);
            this.playerInput.addAction(true, false, true, false);
        }

    }

    void checkDownJump() {
        if (!this.collisionState.isGrounded && this.playerInput.rawY == -1 && !this.hasBeenDownJump) {
            this.rb2d.AddForce(new Vector2(0, -(this.downJumpStrength)), ForceMode2D.Impulse);
            this.frameMovement.x = 0;
            this.hasBeenDownJump = true;
            this.playerInput.addAction(false, true, false, false);
        } else if (this.collisionState.isGrounded && this.hasBeenDownJump)
            this.hasBeenDownJump = false;
    }

    void checkPlayerCrash() {
        if (this.rb2d.velocity.y <= -(this.PlayerCrashForce1) && this.rb2d.velocity.y > -(this.PlayerCrashForce2) && this.playerCrashLevel < 1)
            this.playerCrashLevel = 1;
        else if (this.rb2d.velocity.y <= -(this.PlayerCrashForce2) && this.playerCrashLevel < 2)
            this.playerCrashLevel = 2;
        if (this.collisionState.isGrounded && this.playerCrashLevel != 0) {
            switch (this.playerCrashLevel) {
                case 1:
                    // set a animation here (it's when the player fall on the ground)
                    break;
                case 2:
                    // set a animation here (it's when the player fall hard on the ground)
                    this.sleepPlayer(this.PlayerAfterCrashSleepTime);
                    break;
            }
            this.playerCrashLevel = 0;
        }
    }

    public void sleepPlayer(float timeInSeconds) {
        this.sleep = Time.time + timeInSeconds;
        this.rb2d.velocity = Vector2.zero;
        this.frameMovement = Vector2.zero;
    }

    void checkDoubleJump() {
        if (!this.doubleJumpUsed && this.playerInput.isUpAvailable() && this.collisionState.noCollision && this.isFalling) {
            jump(this.DoubleJumpStrength);
            this.doubleJumpUsed = true;
        }

        if (this.doubleJumpUsed && this.collisionState.isGrounded)
            this.doubleJumpUsed = false;
    }

    void checkSprint() {
        if (this.playerInput.rawX == 1 && this.playerInput.lastReleaseRightTime + this.timeAfterKeyRelease >= Time.time && !this.collisionState.right)
            this.frameMovement.x *= this.SpeedMultiplier;

        if (this.playerInput.rawX == -1 && this.playerInput.lastReleaseLeftTime + this.timeAfterKeyRelease >= Time.time && !this.collisionState.left)
            this.frameMovement.x *= this.SpeedMultiplier;
    }

    void checkDebug() {
        if (this.showColliderState && this.debugMode) {
            Debug.Log("Top: " + this.collisionState.top);
            Debug.Log("Bottom: " + this.collisionState.bottom);
            Debug.Log("Left: " + this.collisionState.left);
            Debug.Log("Right: " + this.collisionState.right);
        }
    }

    public void takeAHit(bool fromRight) {
        this.rb2d.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
        LifeDisplay.setLife(--this.life);
    }

    public void checkAttack() {
        if (this.playerInput.attack && !this.isAttacking) {
            this.isAttacking = true;
            this.timeAttack = Time.time;
            this.animator.SetBool("isAttacking", true);

            RaycastHit2D hit = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y) + (this.facingRight ? Vector2.left : Vector2.right), Vector2.zero);
            if (hit.collider != null && hit.rigidbody.gameObject.tag == "Enemie") {
                enemie enemie = hit.rigidbody.gameObject.GetComponent<enemie>();
                enemie.die();
            }
        }

        if (this.isAttacking && this.timeAttack + 0.467f < Time.time) {
            this.isAttacking = false;
            this.animator.SetBool("isAttacking", false);
        }
    }

    public void setGameObjectHashCodeAsDestroy(int hashCode) {
        this.collisionState.removeFromDictionnaryHashCode(hashCode);
    }

    void checkSound() {
        if (this.frameMovement.x != 0 && !this.isRunningSoundPlaying && this.collisionState.isGrounded) {
            this.audio.loop = true;
            this.audio.clip = this.runningSound;
            this.audio.Play();
            this.isRunningSoundPlaying = true;
        } else if ((this.frameMovement.x == 0 && this.isRunningSoundPlaying) || !this.collisionState.isGrounded) {
            this.audio.Stop();
            this.isRunningSoundPlaying = false;
        }
    }

    public void stopSound() {
        this.audio.Stop();
    }
}

class playerCollision {

    public const bool ENTER = false;
    public const bool EXIT = true;

    private Dictionary<int, string> historyColl = new Dictionary<int, string>();

    public bool top = false;
    public bool bottom = false;
    public bool left = false;
    public bool right = false;

    private bool debugMode;

    public playerCollision(bool debugMode) {
        this.debugMode = debugMode;
    }

    public bool isGrounded {
        get { return this.bottom; }
    }

    public bool noCollision {
        get { return (!top && !bottom && !left && !right); }
    }

    public void reset() {
        this.top = false;
        this.bottom = false;
        this.left = false;
        this.right = false;
    }

    public void updatePlayerCollisionState(Collider2D player, Collision2D coll, bool isExit) {
        Renderer rend = coll.gameObject.GetComponent<Renderer>();

        //Check type (is it enter or exit action ?)
        if (isExit == true) {
            if (this.debugMode)
                rend.material.color = Color.white;

            removeHashCode(coll.gameObject.GetHashCode());
            return;
        }

        if (this.debugMode)
            rend.material.color = Color.red;

        //Check right
        if (
            player.transform.position.x < coll.transform.position.x - coll.collider.bounds.extents.x + coll.collider.offset.x && (
                coll.collider.bounds.Contains(new Vector3(coll.transform.position.x, player.transform.position.y + player.bounds.extents.y, 0)) ||
                coll.collider.bounds.Contains(new Vector3(coll.transform.position.x, player.transform.position.y - player.bounds.extents.y + 0.1f, 0)) ||
                coll.collider.bounds.Contains(new Vector3(coll.transform.position.x, player.transform.position.y, 0))
            )
        ) {
            this.historyColl[coll.gameObject.GetHashCode()] = "right";
        }

        //Check left
        if (
         player.transform.position.x > coll.transform.position.x + coll.collider.bounds.extents.x + coll.collider.offset.x && (
             coll.collider.bounds.Contains(new Vector3(coll.transform.position.x, player.transform.position.y + player.bounds.extents.y, 0)) ||
             coll.collider.bounds.Contains(new Vector3(coll.transform.position.x, player.transform.position.y - player.bounds.extents.y, 0)) ||
             coll.collider.bounds.Contains(new Vector3(coll.transform.position.x, player.transform.position.y, 0))
         )
        ) {
            this.historyColl[coll.gameObject.GetHashCode()] = "left";
        }
        /* Utilities.drawPointer2D(new Vector2(player.transform.position.x + player.bounds.extents.x, coll.transform.position.y));
         Utilities.drawPointer2D(new Vector2(player.transform.position.x - player.bounds.extents.x, coll.transform.position.y));
         Utilities.drawPointer2D(new Vector2(player.transform.position.x, coll.transform.position.y));*/
        //Check bottom
        //Check bottom
        if (
            player.transform.position.y > coll.transform.position.y + coll.collider.bounds.extents.y + coll.collider.offset.y && (
                Physics2D.Raycast(player.transform.position + new Vector3(player.bounds.extents.x, 0, 0), Vector2.down, coll.collider.bounds.extents.y) ||
                Physics2D.Raycast(player.transform.position - new Vector3(player.bounds.extents.x, 0, 0), Vector2.down, coll.collider.bounds.extents.y) ||
                Physics2D.Raycast(player.transform.position, Vector2.down, coll.collider.bounds.extents.y)
            )
        ) {
            this.historyColl[coll.gameObject.GetHashCode()] = "bottom";
        }
        //Check top
        if (
            player.transform.position.y < coll.transform.position.y - coll.collider.bounds.extents.y - coll.collider.offset.y && (
                coll.collider.bounds.Contains(new Vector3(player.transform.position.x + player.bounds.extents.x, coll.transform.position.y, 0)) ||
                coll.collider.bounds.Contains(new Vector3(player.transform.position.x - player.bounds.extents.x, coll.transform.position.y, 0)) ||
                coll.collider.bounds.Contains(new Vector3(player.transform.position.x, coll.transform.position.y, 0))
            )
        ) {
            this.historyColl[coll.gameObject.GetHashCode()] = "top";
        }

        if (this.historyColl.ContainsKey(coll.gameObject.GetHashCode()))
            switch (this.historyColl[coll.gameObject.GetHashCode()]) {
                case "bottom": this.bottom = true; break;
                case "top": this.top = true; break;
                case "right": this.right = true; break;
                case "left": this.left = true; break;
            }
    }

    public void removeFromDictionnaryHashCode(int hashCode) {
        removeHashCode(hashCode);
    }

    private void removeHashCode(int hashCode) {
        if (this.historyColl.ContainsKey(hashCode)) {
            switch (this.historyColl[hashCode]) {
                case "bottom": this.bottom = false; break;
                case "top": this.top = false; break;
                case "right": this.right = false; break;
                case "left": this.left = false; break;
            }
            this.historyColl.Remove(hashCode);
        }
    }

}

public struct PlayerInput {
    public float x;
    public float y;
    public float rawX;
    public float rawY;
    //----------------------------
    public float lastReleaseUpTime;
    public float lastReleaseDownTime;
    public float lastReleaseLeftTime;
    public float lastReleaseRightTime;
    //----------------------------
    private bool actionUp;
    private bool actionDown;
    private bool actionLeft;
    private bool actionRight;
    //----------------------------
    private bool lastReleaseUpBool;
    private bool lastReleaseDownBool;
    private bool lastReleaseLeftBool;
    private bool lastReleaseRightBool;
    //----------------------------
    public bool jump {
        get {
            return this.rawY == 1 || Input.GetKeyDown(KeyCode.Space);
        }
    }
    public bool attack {
        get {
            return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E);
        }
    }

    public void update() {
        this.x = Input.GetAxis("Horizontal");
        this.y = Input.GetAxis("Vertical");
        this.rawX = Input.GetAxisRaw("Horizontal");
        this.rawY = Input.GetAxisRaw("Vertical");
        this.checkActionRelease();
        this.checkLastTimeRelease();
    }

    public void addAction(bool up, bool down, bool left, bool right) {
        if (up)
            this.actionUp = true;
        if (down)
            this.actionDown = true;
        if (left)
            this.actionLeft = true;
        if (right)
            this.actionRight = true;
    }

    private void checkActionRelease() {
        if (this.actionUp && (this.rawY != 1 || !Input.GetKeyDown(KeyCode.Space)))
            this.actionUp = false;

        if (this.actionDown && this.rawY != -1)
            this.actionDown = false;

        if (this.actionLeft && this.rawX != -1)
            this.actionLeft = false;

        if (this.actionRight && this.rawX != 1)
            this.actionRight = false;

    }

    private void checkLastTimeRelease() {
        if ((this.rawY != 1 || !Input.GetKeyDown(KeyCode.Space)) && !this.lastReleaseUpBool) {
            this.lastReleaseUpTime = Time.time;
            this.lastReleaseUpBool = true;
        } else if (this.rawY == 1)
            this.lastReleaseUpBool = false;

        if (this.rawY != -1 && !this.lastReleaseDownBool) {
            this.lastReleaseDownTime = Time.time;
            this.lastReleaseDownBool = true;
        } else if (this.rawY == -1)
            this.lastReleaseDownBool = false;

        if (this.rawX != -1 && !this.lastReleaseLeftBool) {
            this.lastReleaseLeftTime = Time.time;
            this.lastReleaseLeftBool = true;
        } else if (this.rawX == -1)
            this.lastReleaseLeftBool = false;

        if (this.rawX != 1 && !this.lastReleaseRightBool) {
            this.lastReleaseRightTime = Time.time;
            this.lastReleaseRightBool = true;
        } else if (this.rawX == 1)
            this.lastReleaseRightBool = false;
    }

    public bool isUpAvailable() {
        return !this.actionUp && (this.rawY == 1 || Input.GetKeyDown(KeyCode.Space));
    }

    public bool isDownAvailable() {
        return !this.actionDown && this.rawY == -1;
    }

    public bool isLeftAvailable() {
        return !this.actionLeft && this.rawX == -1;
    }

    public bool isRightAvailable() {
        return !this.actionRight && this.rawX == 1;
    }
}