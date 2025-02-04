using UnityEngine;

public class EnemyAI : MonoBehaviour {
    public float moveSpeed = 2f;
    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public int health = 3;
    public bool isDead = false;
    private bool isAttacking = false;
    public bool hasStoppedMoving { get; private set; } = false;
    public GameObject bloodEffect;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer.color = Color.red;

        // 🔴 Ensure isDead is false at start (Prevents auto-death)
        isDead = false;
        animator.SetBool("isDead", false);  // ✅ Make sure enemy is NOT dead at the start
        animator.Rebind();
        animator.Update(0);

        animator.SetBool("isRunning", true);
        rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
    }

    void FixedUpdate() {
        if (isDead || player == null) return;

        float distance = Mathf.Abs(transform.position.x - player.position.x);
        Debug.Log($"🚀 Enemy Velocity: {rb.linearVelocity.x}, {rb.linearVelocity.y} | 🔍 Distance to Player: {distance}");

        if (!isAttacking && distance < 2.9f) {  
            Debug.Log("🛑 Distance condition met. Calling StopAndGoIdle()...");
            StopAndGoIdle();
        } else if (!hasStoppedMoving) {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            animator.SetBool("isRunning", true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Debug.Log("🛑 Enemy detected Player, stopping...");
            StopAndGoIdle();
        } 

        if (collision.CompareTag("PlayerAttack")) {  
            Debug.Log("💥 Player attacked Enemy!");
            TakeDamage(1);
        }
    }

    void StopAndGoIdle() {
        if (!isDead && !hasStoppedMoving) {
            Debug.Log("🛑 Enemy stopped moving and is now idle!");
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            animator.SetBool("isRunning", false);
            animator.Play("Enemy_idle");  
            hasStoppedMoving = true;
            isAttacking = true;
            InvokeRepeating("StartAttack", 0.5f, 1f);
        }
    }

    void StartAttack() {
        if (isDead || !isAttacking) return;  
        animator.SetTrigger("Attack");

        if (player != null) {
            player.GetComponent<PlayerMovement>().TakeDamage(1);
        }
    }

    public void TakeDamage(int damage) {
        if (isDead) return;

        health -= damage;  // ✅ Decrease health
        Debug.Log($"💥 Enemy took {damage} damage! Remaining Health: {health}");

        animator.SetTrigger("TakeHit");  // ✅ Play TakeHit animation
        isAttacking = false;  // ✅ Stop attacking temporarily
        CancelInvoke("StartAttack");  // ✅ Stop attack loop momentarily

        if (health <= 0) {
            SpawnBloodEffect();
            Die();
        } else {
            Invoke("ResumeAttacking", 1.5f); // ✅ Enemy pauses for 1.5 seconds before attacking again
        }
    }

    void ResumeAttacking() {
        if (!isDead) {
            Debug.Log("⚔️ Enemy is ready to attack again!");
            isAttacking = true;
            InvokeRepeating("StartAttack", 0.5f, 1f);
        }
    }

    void SpawnBloodEffect() {
        if (bloodEffect != null) {
            Instantiate(bloodEffect, transform.position, Quaternion.identity);
        } else {
            Debug.LogError("⚠️ Blood Effect is missing! Assign it in the Inspector.");
        }
    }

    void Die() {
        if (!isDead) {
            isDead = true;
            animator.SetBool("isDead", true);  // ✅ Play Death animation

            rb.linearVelocity = Vector2.zero;  // ✅ Stop all movement
            rb.constraints = RigidbodyConstraints2D.FreezeAll;  // ✅ Freeze position

            Debug.Log("💀 Enemy Died!");

            // 🔴 Wait for the Death animation duration before destroying the enemy
            float deathAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, deathAnimLength);  
        }
    }
}
