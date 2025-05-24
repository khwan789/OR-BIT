using System.Collections;
using UnityEngine;

public abstract class ObjectPlayer : MonoBehaviour
{
    // 이동 및 입력 관련 변수들
    [SerializeField] protected float slidingTime = 1f;              // 슬라이딩 지속 시간
    [SerializeField] protected bool isJumpButtonDown = false;         // 점프 버튼 입력 상태
    [SerializeField] protected bool isSlidingButtonDown = false;      // 슬라이드 버튼 입력 상태

    protected float startDistance;                                    // 행성(planet)과의 초기 거리
    public Vector3 basePosition;                                      // 점프 시 기준 위치
    protected GameManager gameManager;                                // 게임 매니저 참조
    protected Animator animator;                                      // 플레이어 애니메이터 (주 캐릭터)
    protected ObjectPoolManager poolManager;                          // 객체 풀 매니저

    // Ghost 및 Invincibility 상태 관련 변수들
    protected float invincStateTime;                                  // 무적 상태 남은 시간
    protected bool isInvinc = false;                                  // 무적 상태 여부
    public GameObject colliderObject;                                 // 충돌 처리를 위한 오브젝트 (태그 변경용)
    public GameObject gameOverEffect;                                 // 게임 오버 효과 프리팹
    public GameObject invincTrail;

    // 이동 관련 변수들
    protected Transform planet;                                       // 행성(또는 회전 기준)의 Transform
    [SerializeField] protected float orbitSpeed;                     // 행성 주위를 도는 속도
    [SerializeField] protected float orbitBoostSpeed;                // 슬라이딩 시 도는 속도 (사용 예)
    [SerializeField] protected float originalOrbitSpeed;
    [SerializeField] protected float jumpHeight = 2f;                  // 점프 높이
    [SerializeField] protected float jumpSpeed = 5f;                   // 점프 속도
    protected bool isJumping = false;                                 // 점프 중 여부
    protected bool isSliding = false;                                 // 슬라이드 중 여부
    protected float currentDistance;                                  // 현재 행성과의 거리
    protected float targetDistance;                                   // 목표 거리 (점프 전후)
    public int direction = 1;                                         // 이동 방향 (1: 시계, -1: 반시계)
    protected float previousAngle = 0f;                               // 이전 프레임의 각도 (점수 계산용)

    // Invincibility Coroutine 참조 (중복 실행 방지)
    private Coroutine invincCoroutine;

    // Invincibility 관련
    private float invincibilityTime = 0f; // Remaining time for invincibility
    private bool isInvincActive = false;

    // Jump Sound Support
    [Header("Audio Settings")]
    protected AudioSource audioSource;   // AudioSource component

    protected virtual void Start()
    {
        // 필요한 컴포넌트 및 객체 캐싱
        gameManager = GameManager.Instance;
        poolManager = FindObjectOfType<ObjectPoolManager>();

        // 'Ground' 태그가 붙은 오브젝트를 행성으로 가정하여 캐싱
        GameObject groundObj = GameObject.FindGameObjectWithTag("Ground");
        if (groundObj != null)
        {
            planet = groundObj.transform;
        }
        else
        {
            Debug.LogError("Planet (Ground 태그)가 존재하지 않습니다!");
        }

        currentDistance = startDistance = 25;
        targetDistance = currentDistance;

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("isRunning", true);
        }

        orbitBoostSpeed = orbitSpeed * 2;
        originalOrbitSpeed = orbitSpeed;

        // Get or add an AudioSource component for playing sounds
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (gameManager.isRevived)
        {
            ClearCollectablesInRange();
        }
    }

    protected virtual void Update()
    {
        if (gameManager.isPlaying)
        {
            // PC 입력 처리 (키보드 입력)
            HandleInput();

            MoveAroundPlanet();
            AlignToSurface();
            HandleMovement();
        }
    }

    // PC 입력 처리: 키보드 입력을 통해 슬라이드 및 점프 상태 업데이트
    protected virtual void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            isSlidingButtonDown = true;
        if (Input.GetKeyUp(KeyCode.DownArrow))
            isSlidingButtonDown = false;
        if (Input.GetKeyDown(KeyCode.Space))
            isJumpButtonDown = true;
        if (Input.GetKeyUp(KeyCode.Space))
            isJumpButtonDown = false;
    }

    // UI 입력 이벤트 (PlayerInputManager)를 통한 입력 처리
    protected virtual void OnEnable()
    {
        PlayerInputManager.OnJumpInput += JumpUp;
        PlayerInputManager.OnSlideInput += SlideDown;
    }
    protected virtual void OnDisable()
    {
        PlayerInputManager.OnJumpInput -= JumpUp;
        PlayerInputManager.OnSlideInput -= SlideDown;
    }

    // 행성을 중심으로 이동 및 점수 갱신
    protected virtual void MoveAroundPlanet()
    {
        float aroundSpeed = orbitSpeed;

        transform.RotateAround(planet.position, Vector3.back, direction * aroundSpeed * (gameManager.speedMultiplier - gameManager.slowDownValue) * Time.deltaTime);

        Vector3 directionToPlanet = (transform.position - planet.position).normalized;
        transform.position = planet.position + directionToPlanet * currentDistance;
        basePosition = planet.position + directionToPlanet * startDistance;

        float angle = Vector3.SignedAngle(Vector3.right, directionToPlanet, Vector3.forward);
        float distanceMoved = Mathf.Abs(angle - previousAngle) * currentDistance * Mathf.Deg2Rad;
        gameManager.currentScore += distanceMoved;
        previousAngle = angle;
    }

    // 행성 표면에 맞춰 플레이어의 회전 조정
    protected virtual void AlignToSurface()
    {
        Vector3 directionToPlanet = (planet.position - transform.position).normalized;
        float angle = Mathf.Atan2(directionToPlanet.y, directionToPlanet.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    // 슬라이드, 점프 등의 이동 상태 처리
    protected virtual void HandleMovement()
    {
        if (isSlidingButtonDown)
        {
            StartSliding();
        }
        else
        {
            StopSliding();
        }

        if (isJumpButtonDown && !isJumping)
        {
            StartCoroutine(Jump());
        }
    }

    // 점프 동작 Coroutine
    protected virtual IEnumerator Jump()
    {
        isJumping = true;

        // Play jump sound at the start of the jump
        if (audioSource != null)
        {
            AudioManager.Instance.PlaySFX(SFXType.Jump);
        }

        animator.SetBool("isJumping", true);
        animator.SetBool("isSliding", false);

        float jumpProgress = 0f;
        float duration = jumpHeight / (jumpSpeed * gameManager.speedMultiplier);
        float originalDistance = currentDistance;

        while (jumpProgress < 1f)
        {
            jumpProgress += Time.deltaTime / duration;
            float height = Mathf.SmoothStep(0, jumpHeight, Mathf.Sin(jumpProgress * Mathf.PI));
            currentDistance = originalDistance + height;
            yield return null;
        }

        currentDistance = originalDistance;
        isJumping = false;
        animator.SetBool("isJumping", false);
    }

    protected virtual void StartSliding()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
    }

    protected virtual void StopSliding()
    {
        isSliding = false;
        animator.SetBool("isSliding", false);
    }

    protected virtual void InvincState(float additionalTime = 1f)
    {
        isInvinc = true;
        // Extend invincibility time
        invincibilityTime += additionalTime;
        invincTrail.SetActive(true);
        // Only start countdown if it's not already active
        if (!isInvincActive)
        {
            isInvincActive = true;
            orbitSpeed = orbitBoostSpeed; // Set to boosted speed
            InvokeRepeating(nameof(InvincibilityCountdown), 1f, 1f);
        }
    }

    private void InvincibilityCountdown()
    {
        invincibilityTime -= 1f;

        if (invincibilityTime <= 0)
        {
            CancelInvoke(nameof(InvincibilityCountdown));
            orbitSpeed = originalOrbitSpeed; // Reset to normal speed
            
            // Delay removal of invincibility state by 0.5 seconds
            Invoke(nameof(RemoveInvincibility), 0.5f);   
        }
    }

    private void RemoveInvincibility()
    {
        isInvinc = false;
        isInvincActive = false;
        invincTrail.SetActive(false);
    }

    // 게임 오버 효과 후 GameOver 호출
    protected virtual IEnumerator WaitForGameOver(GameObject effect)
    {
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            while (ps.IsAlive())
            {
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
        gameManager.GameOver();
        Destroy(effect);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Collectable collectable = collision.gameObject.GetComponentInParent<Collectable>();

        if (collision.gameObject.CompareTag("Collectable"))
        {
            if (collectable != null)
            {
                gameManager.IncreaseScore(collectable.GetScoreAmount());
                gameManager.IncreaseGold(collectable.GetGoldAmount());
                collectable.ReturnToPool(transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(SFXType.Dead);

            if (!isInvinc)
            {
                GameObject effect = Instantiate(gameOverEffect, transform.position, Quaternion.identity);
                effect.transform.parent = null;
                if (Application.internetReachability != NetworkReachability.NotReachable && gameManager.currentRound >= 2 && !gameManager.isAskingRevive)
                {
                    gameManager.revivePos = transform.position;
                    gameManager.isAskingRevive = true;
                    Destroy(gameObject);
                }
                else
                {
                    gameManager.StartCoroutine(WaitForGameOver(effect));
                }
                Destroy(gameObject);
            }
            else if (isInvinc)
            {
                StartCoroutine(poolManager.GenerateDestroyEffect(collision.gameObject.transform.position));
                // Safety check: call ObstacleReturnToPool only if collectable is not null.
                if (collectable != null)
                {
                    collectable.ReturnToPool(transform.position);
                }
            }
        }
        else if (collision.gameObject.CompareTag("InvincSkill"))
        {
            InvincState();
            AudioManager.Instance.PlaySFX(SFXType.Invinc);
            if (collectable != null)
            {
                collectable.ReturnToPool();
            }
        }
        else if (collision.gameObject.CompareTag("SlowDown"))
        {
            gameManager.slowDownValue += collectable.slowDown;
            AudioManager.Instance.PlaySFX(SFXType.Invinc);
            if (collectable != null)
            {
                collectable.ReturnToPool(transform.position);
            }
        }
    }

    // UI 또는 외부에서 호출 가능한 입력 처리 메서드 (PlayerInputManager 이벤트용)
    public virtual void JumpUp(bool _jump)
    {
        isJumpButtonDown = _jump;
    }
    public virtual void SlideDown(bool _slide)
    {
        isSlidingButtonDown = _slide;
    }
    public void ClearCollectablesInRange()
    {
        // This will find all enabled Collectable scripts in the scene
        Collectable[] allCollectables = FindObjectsOfType<Collectable>();

        Vector3 myPos = transform.position;
        float rSqr = 20 * 20;

        foreach (var c in allCollectables)
        {
            // squared distance check is slightly faster than Vector3.Distance
            if ((c.transform.position - myPos).sqrMagnitude <= rSqr)
            {
                // call whatever your pooling method is
                c.ReturnToPool();
            }
        }
    }
}
