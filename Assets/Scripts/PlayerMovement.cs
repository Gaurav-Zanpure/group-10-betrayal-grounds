using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float speed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false;
    private bool isShielding = false;
    public GameObject bloodEffect;
    public Collider2D attackCollider;  // ✅ Reference to attack collider

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        isAttacking = false;
        isShielding = false;
        animator.SetBool("isShielding", false);
        animator.ResetTrigger("Attack");
    }

    void Update() {
        HandleMovement();
        HandleAttack();
        HandleShielding();
    }

    void HandleMovement() {
        float move = Input.GetAxisRaw("Horizontal");

        if (!isAttacking && !isShielding) { 
            rb.linearVelocity = new Vector2(move * speed, rb.linearVelocity.y);
            animator.SetBool("isRunning", move != 0);
        } else {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleAttack() {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isShielding) {
            isAttacking = true;
            animator.SetTrigger("Attack");

            if (attackCollider != null) {
                attackCollider.enabled = true;  // ✅ Ensure attack collider is active
                Debug.Log("🗡 Player attack started!"); // ✅ Debugging attack trigger
            } else {
                Debug.LogError("⚠️ Attack Collider is missing from Player! Add it in Inspector.");
            }

            rb.linearVelocity = Vector2.zero;
            Invoke("EndAttack", 0.3f);
        }
    }

    void EndAttack() {
        isAttacking = false;

        if (attackCollider != null) {
            attackCollider.enabled = false;  // ✅ Disable attack collider after attack
        }
    }

    void HandleShielding() {
        if (Input.GetKey(KeyCode.LeftShift) && !isAttacking) {
            if (!isShielding) {
                isShielding = true;
                animator.SetBool("isShielding", true);
                rb.linearVelocity = Vector2.zero;
            }
        } else if (!Input.GetKey(KeyCode.LeftShift) && isShielding) {
            isShielding = false;
            animator.SetBool("isShielding", false);
        }
    }

    public void TakeDamage(int damage) {
        if (!isShielding) { 
            Instantiate(bloodEffect, transform.position, Quaternion.identity);
            Debug.Log("Player hit!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Enemy_Idle")) {  
            if (isAttacking) {  // ✅ Check if the player is attacking
                Debug.Log("💥 Player attacked Enemy!");  // ✅ Debug log should now appear
                EnemyAI enemy = collision.GetComponent<EnemyAI>();
                if (enemy != null) {
                    enemy.TakeDamage(1);  // ✅ Call the enemy's TakeDamage method
                } else {
                    Debug.LogError("⚠️ EnemyAI script not found on collided object!");
                }
            }
        }
    }
}
