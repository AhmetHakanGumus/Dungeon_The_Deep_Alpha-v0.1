using UnityEngine;

public enum EnemyState { Patrol, Investigate, Chase, Attack }

public class EnemyFSM : FSMBase<EnemyState>
{
    [Header("Rastgele Devriye Alanı")]
    public float patrolRadius = 4f;
    public float waitAtPoint = 1.5f;

    [Header("Hareket Hızları")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float investigateSpeed = 2.5f;

    [Header("Saldırı Ayarları")]
    public float attackRange = 1.5f; 
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Takip Kaybı")]
    public float losePlayerDelay = 0.5f;

    // ── Bileşenler ──
    DirectionController directionController; 
    FieldOfView fieldOfView;
    PlayerHealth playerHealth;
    Transform player;
    Animator anim;
    Rigidbody2D rb; 

    // ── Dahili Değişkenler ──
    Vector2 startPosition;
    Vector2 randomPatrolTarget;
    float waitTimer;
    float attackTimer;
    float losePlayerTimer;
    Vector2 investigatePosition;
    Vector2 lastPosition;

    // ── Çarpışma ve Takılma Değişkenleri ──
    float stuckCheckTimer = 0.5f;
    Vector2 lastStuckPos;
    bool isTouchingPlayer = false; 

    void Start()
    {
        directionController = GetComponent<DirectionController>(); 
        fieldOfView = GetComponent<FieldOfView>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerHealth>();
        }

        lastPosition = transform.position;
        lastStuckPos = transform.position;
        startPosition = transform.position;
        PickNewRandomPatrolPoint();

        ChangeState(EnemyState.Patrol);
    }

    protected override void UpdateState()
    {
        // 1. Orijinal hareket algılama
        bool isMoving = Vector2.Distance(transform.position, lastPosition) > 0.001f;

        // ── YENİ: EĞER SALDIRIYORSA YÜRÜMEYİ İPTAL ET (İtilse bile) ──
        if (currentState == EnemyState.Attack)
        {
            isMoving = false; 
        }

        if (anim != null)
        {
            anim.SetBool("Walk", isMoving);

            // ── YENİ: BAKIŞ YÖNÜ KONTROLÜ ──
            if (currentState == EnemyState.Attack && player != null)
            {
                // Saldırıdayken itilse bile ZORLA oyuncuya baksın
                Vector2 lookDir = (player.position - transform.position).normalized;
                anim.SetFloat("Horizontal", lookDir.x);
                anim.SetFloat("Vertical", lookDir.y);
            }
            else if (isMoving)
            {
                // Normal yürüyüşte hareket ettiği yöne baksın
                Vector2 moveDir = ((Vector2)transform.position - lastPosition).normalized;
                anim.SetFloat("Horizontal", moveDir.x);
                anim.SetFloat("Vertical", moveDir.y);
            }
        }
        
        lastPosition = transform.position;

        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
        if (stuckCheckTimer > 0f) stuckCheckTimer -= Time.deltaTime; 

        switch (currentState)
        {
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Investigate: UpdateInvestigate(); break;
            case EnemyState.Chase: UpdateChase(); break;
            case EnemyState.Attack: UpdateAttack(); break;
        }
    }

    protected override void OnEnterState(EnemyState newState, EnemyState fromState)
    {
        switch (newState)
        {
            case EnemyState.Investigate:
                investigatePosition = fieldOfView.LastKnownPlayerPosition;
                losePlayerTimer = 0f;
                break;
            case EnemyState.Patrol:
                startPosition = transform.position;
                PickNewRandomPatrolPoint();
                break;
        }
    }

    protected override void OnExitState(EnemyState state) { }

    void PickNewRandomPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        randomPatrolTarget = startPosition + randomOffset;
        waitTimer = waitAtPoint; 
        
        stuckCheckTimer = 0.5f; 
        lastStuckPos = transform.position;
    }

    void UpdatePatrol()
    {
        if (fieldOfView.CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (Vector2.Distance(transform.position, randomPatrolTarget) < 0.2f)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                PickNewRandomPatrolPoint();
            }
        }
        else
        {
            MoveTo(randomPatrolTarget, patrolSpeed);

            if (stuckCheckTimer <= 0f)
            {
                if (Vector2.Distance(transform.position, lastStuckPos) < 0.5f)
                {
                    PickNewRandomPatrolPoint(); 
                }
                
                lastStuckPos = transform.position;
                stuckCheckTimer = 0.5f; 
            }
        }
    }

    void UpdateInvestigate()
    {
        if (fieldOfView.CanSeePlayer())
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        MoveTo(investigatePosition, investigateSpeed);

        if (Vector2.Distance(transform.position, investigatePosition) < 0.3f)
            ChangeState(EnemyState.Patrol);
    }

    void UpdateChase()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange || isTouchingPlayer)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        if (!fieldOfView.CanSeePlayer())
        {
            losePlayerTimer += Time.deltaTime;
            if (losePlayerTimer >= losePlayerDelay)
                ChangeState(EnemyState.Investigate);
        }
        else
        {
            losePlayerTimer = 0f;
        }

        MoveTo(player.position, chaseSpeed);
    }

   void UpdateAttack()
    {
        if (player == null) return;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (directionController != null)
            directionController.LookAt(player.position);

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > attackRange && !isTouchingPlayer)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!fieldOfView.CanSeePlayer() && dist > attackRange * 1.5f && !isTouchingPlayer)
        {
            ChangeState(EnemyState.Investigate);
            return;
        }

        if (attackTimer <= 0f)
        {
            PerformAttack();
            attackTimer = attackCooldown;
        }
    }

    void MoveTo(Vector2 targetPos, float speed)
    {
        if (rb != null)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.deltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

        if (directionController != null && Vector2.Distance(transform.position, targetPos) > 0.1f)
        {
            directionController.LookAt(targetPos);
        }
    }

    void PerformAttack()
    {
        if (anim != null) anim.SetTrigger("Attack");
        Debug.Log($"[{name}] Saldırdı! Hasar: {attackDamage}");
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
        else if (currentState == EnemyState.Patrol)
        {
            if (stuckCheckTimer <= 0f) 
            {
                PickNewRandomPatrolPoint(); 
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        if (Application.isPlaying)
        {
            Gizmos.DrawWireSphere(startPosition, patrolRadius);
            Gizmos.DrawLine(transform.position, randomPatrolTarget);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, patrolRadius);
        }
    }
}