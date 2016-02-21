using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class enemie : MonoBehaviour {

    public float xFrom;
    public float xTo;
    public float Speed;
    public float radius;
    public Animator animator;
    public float attackLength = 0.5f;
    public AudioClip shieldSound;
    public bool WaitingForEnemy = false;
    [Space(8)]
    public bool debugMode = false;

    private Vector2 globalFrom;
    private Vector2 globalTo;
    private Rigidbody2D rb2d;
    private BoxCollider2D bc2d;
    private CircleCollider2D cc2d;
    private bool updateVelocityY = true;
    private bool facingRight = false;
    private GameObject follow = null;
    private bool wait = false;
    private int countWait = 0;
    private bool isDie = false;
    private bool doDie = false;
    private bool isAttack = false;
    private float timeAttack = 0;
    private AudioSource audio;

    void Start() {
        UpdateGlobal();
        this.rb2d = this.GetComponent<Rigidbody2D>();
        this.bc2d = this.GetComponent<BoxCollider2D>();
        this.cc2d = this.GetComponent<CircleCollider2D>();
        this.cc2d.radius = radius;
        audio = GetComponent<AudioSource>();
    }

    void Update() {
        if (this.timeAttack + .5f < Time.time && this.isAttack) {
            this.animator.SetBool("isAttacking", false);
            this.isAttack = false;
        }

        if (this.rb2d.velocity.y != 0)
            this.updateVelocityY = true;

        if (this.updateVelocityY && this.rb2d.velocity.y == 0) {
            this.updateYGlobal();
            this.updateVelocityY = false;
        }

        if (radius * this.bc2d.size.x != this.cc2d.radius)
            this.cc2d.radius = radius * this.bc2d.size.x;

        checkDie();
        checkPlayer();
        checkMovement();
        fixMovement();
        checkAnimState();
    }

    void UpdateGlobal() {
        globalFrom = new Vector2();
        globalFrom = new Vector2(xFrom, 0) + new Vector2(transform.position.x, this.transform.position.y);
        globalTo = new Vector2();
        globalTo = new Vector2(xTo, 0) + new Vector2(transform.position.x, this.transform.position.y);
    }

    void updateYGlobal() {
        this.globalFrom = new Vector2(globalFrom.x, this.transform.position.y);
        this.globalTo = new Vector2(globalTo.x, this.transform.position.y);
    }

    void fixMovement() {
        if (this.rb2d.velocity.x == 0 && !this.wait && (!this.WaitingForEnemy && !this.follow))
            this.rb2d.AddForce(new Vector2(0, 1), ForceMode2D.Impulse);
    }

    void checkMovement() {
        if (this.transform.position.x < this.globalFrom.x && !this.facingRight)
            flip();
        if (this.transform.position.x > this.globalTo.x && this.facingRight)
            flip();
    }

    void FixedUpdate() {
        if (!this.wait && (!this.WaitingForEnemy || this.follow))
            this.rb2d.velocity = new Vector2(this.Speed * (facingRight ? 1 : -1), this.rb2d.velocity.y);
        else if (!this.wait && ++this.countWait == 10) {
            this.wait = false;
            this.countWait = 0;
        }
    }

    void flip() {
        if (this.isDie || this.wait)
            return;

        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player")
            this.attack(coll.gameObject);
        if (coll.gameObject.tag == "core")
            flip();
        if (coll.gameObject.tag == "Enemie")
            flip();
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.tag == "Player")
            this.follow = coll.gameObject;
    }

    void OnTriggerExit2D(Collider2D coll) {
        if (coll.tag == "Player") {
            this.follow = null;
            this.wait = false;
        }
    }

    void checkPlayer() {
        if (this.follow == null)
            return;

        if (
            this.between(this.transform.position.x, this.follow.transform.position.x - this.bc2d.bounds.extents.x, this.follow.transform.position.x + this.bc2d.bounds.extents.x, true) ||
            (this.follow.transform.position.x < this.globalFrom.x && this.transform.position.x - 0.1f < this.globalFrom.x) ||
            (this.follow.transform.position.x > this.globalTo.x && this.transform.position.x + 0.1f > this.globalTo.x)
        )
            this.wait = true;
        else
            this.wait = false;

        if (this.wait)
            return;

        if (this.transform.position.x < this.follow.transform.position.x && !facingRight)
            flip();
        if (this.transform.position.x > this.follow.transform.position.x && facingRight)
            flip();

    }

    void checkAnimState() {
        this.animator.SetFloat("speed", Mathf.Abs(this.rb2d.velocity.x));
    }

    void OnDrawGizmos() {
        if (this.debugMode) {
            Gizmos.color = Color.green;
            float size = 0.3f;

            globalFrom = (Application.isPlaying) ? globalFrom : new Vector2(xFrom, 0) + new Vector2(transform.position.x, transform.position.y);
            globalTo = (Application.isPlaying) ? globalTo : new Vector2(xTo, 0) + new Vector2(transform.position.x, transform.position.y);

            //From
            Gizmos.DrawLine(globalFrom - Vector2.up * size, globalFrom + Vector2.up * size);
            Gizmos.DrawLine(globalFrom - Vector2.left * size, globalFrom + Vector2.left * size);

            //To
            Gizmos.DrawLine(globalTo - Vector2.up * size, globalTo + Vector2.up * size);
            Gizmos.DrawLine(globalTo - Vector2.left * size, globalTo + Vector2.left * size);
            /*
                        //Radius
                        UnityEditor.Handles.color = Color.green;

                        if (Application.isPlaying)
                            UnityEditor.Handles.DrawWireArc(this.transform.position + new Vector3(this.bc2d.offset.x * (this.facingRight ? -1 : 1), this.bc2d.offset.y, 0), this.transform.forward, this.transform.right, 360, this.transform.localScale.x * this.bc2d.size.x * radius);
                    **/
        }
    }

    public void die() {
        this.animator.SetBool("isDying", true);
        this.isDie = true;
    }

    void checkDie() {
        if (this.isDie && !this.doDie) {
            this.doDie = true;
            this.rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            this.bc2d.size = new Vector2(0, 0);
        }
    }

    bool between(float num, float lower, float upper, bool inclusive = false) {
        return inclusive
            ? lower <= num && num <= upper
            : lower < num && num < upper;
    }

    void attack(GameObject player) {
        this.animator.SetBool("isAttacking", true);
        this.isAttack = true;
        this.timeAttack = Time.time;
        audio.PlayOneShot(this.shieldSound);
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.takeAHit(this.transform.position.x > player.transform.position.x);
        flip();
    }
}
