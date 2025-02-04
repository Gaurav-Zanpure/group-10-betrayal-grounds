using UnityEngine;

public class EnemyAttack : MonoBehaviour {
    public int damage = 1;
    private bool isAttacking = false;
    private Animator animator;
    private Transform player;
    private EnemyAI enemyAI;

    void Start() {
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null) {
            player = playerObject.transform;
        }

        enemyAI = GetComponent<EnemyAI>();
    }

    void Update() {
        if (player == null) return;

        if (Mathf.Abs(transform.position.x - player.position.x) < 0.5f && !isAttacking) {
            Invoke("StartAttack", 0.5f);
        }
    }

    void StartAttack() {
        if (isAttacking || enemyAI.isDead) return;
        isAttacking = true;
        animator.SetTrigger("Attack");

        if (player != null) {
            player.GetComponent<PlayerMovement>().TakeDamage(damage);
        }

        Invoke("ResetAttack", 1f);
    }

    void ResetAttack() {
        isAttacking = false;
        if (enemyAI.isDead) return;
        StartAttack();
    }
}
