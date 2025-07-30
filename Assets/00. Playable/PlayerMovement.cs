using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : BasePlayerMovement
{
    public float turnSpeed = 20f;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;

    private Vector3 m_Movement;
    private Quaternion m_Rotation = Quaternion.identity;
    private Vector2 m_InputVector = Vector2.zero;
    private bool m_IsRunning = false;


    protected override void Start()
    {
        base.Start();

        // 필요한 초기화 코드가 있다면 여기에 추가
    }

    protected override void Update()
    {
        base.Update();
        // 필요한 업데이트 로직이 있다면 여기에 추가
    }


    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        // m_Animator.applyRootMotion = false;
        m_Animator.applyRootMotion = true;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // InputManager에서 호출할 메서드들
    public void OnMoveInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!IsControlledByPlayer) return;
        m_InputVector = context.ReadValue<Vector2>();
    }

    public void OnSprintInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!IsControlledByPlayer) return;
        m_IsRunning = context.ReadValueAsButton();
    }


    // private void OnMove(InputAction.CallbackContext context)
    // {
    //         if (!IsControlledByPlayer) return;
    //     m_InputVector = context.ReadValue<Vector2>();
    // }

    // private void OnSprint(InputAction.CallbackContext context)
    // {
    //     if (!IsControlledByPlayer) return;
    //     m_IsRunning = context.ReadValueAsButton();
    // }

    void OnFootstep()
    {
        // Debug.Log("발소리 재생"); // 또는 AudioSource.Play(), 이펙트 등 실행
    }

    void FixedUpdate()
    {
        if (!IsControlledByPlayer)
        {
            // 조작되지 않는 플레이어는 입력값을 0으로 초기화
            m_InputVector = Vector2.zero;
            m_IsRunning = false;
        }

        float horizontal = m_InputVector.x;
        float vertical = m_InputVector.y;

        // 이동 입력 벡터 계산
        Vector3 rawMovement = new Vector3(horizontal, 0f, vertical);
        float inputMagnitude = rawMovement.magnitude;

        // 방향만 필요한 m_Movement는 정규화
        m_Movement = rawMovement.normalized;

        float currentSpeed = m_IsRunning ? runSpeed : walkSpeed;

        // 애니메이터에 전달할 속도 (입력 강도에 따라)
        float appliedSpeed = inputMagnitude * currentSpeed;

        // 애니메이터에 부드럽게 속도 전달
        m_Animator.SetFloat("Speed", appliedSpeed, 0.1f, Time.deltaTime);

        if (inputMagnitude > 0.01f)
        {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }
    }

    void OnAnimatorMove()
    {
        float currentSpeed = m_IsRunning ? runSpeed : walkSpeed;
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * currentSpeed * Time.deltaTime);
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}