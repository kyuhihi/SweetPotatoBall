using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : BasePlayerMovement
{
    public float turnSpeed = 20f;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;

    private Vector3 m_Movement;//디버그
    private Quaternion m_Rotation = Quaternion.identity;
    private Vector2 m_InputVector = Vector2.zero;
    private bool m_IsRunning = false;

    public enum IdleWalkRunEnum
    {
        Idle = 0,
        Walk = 1,
        Run = 2
    }
    private IdleWalkRunEnum m_eLocomotionState = IdleWalkRunEnum.Idle;
    [SerializeField]
    private bool m_IsBackGo = false;



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
    public void OnPassInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!IsControlledByPlayer) return;

        if (context.performed)
        {
            // TacticsManager에게 패스 요청
            if (TacticsManager.Instance != null)
            {
                TacticsManager.Instance.RequestPass(gameObject);
            }
        }
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
        //m_Animator.SetFloat("Speed", appliedSpeed, 0.1f, Time.deltaTime);
        SetCurrentLocomotionState(appliedSpeed);

        if (inputMagnitude > 0.01f)
        {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }
        OnPlayerMove();

        SetAnimatorParameters(inputMagnitude);

    }

    private void SetAnimatorParameters(float inputMagnitude)
    {
        if (Keyboard.current.f1Key.wasPressedThisFrame)//나중에 바꿔라
        {
            m_IsBackGo = !m_IsBackGo; // F1 키로 뒤로 가기 토글
        }

        m_Animator.SetInteger("IdleWalkRunEnum", (int)m_eLocomotionState);

        float localAngle = GetLocalMoveFromWorld();
        Vector2 directionVector = Get8DirectionVector(localAngle);
        float fAnimatorSpeedX = directionVector.x * (int)m_eLocomotionState;
        float fAnimatorSpeedY = directionVector.y * (int)m_eLocomotionState;

        m_Animator.SetBool("GoBack", m_IsBackGo);
        fAnimatorSpeedX = m_IsBackGo ? fAnimatorSpeedX : fAnimatorSpeedX;
        fAnimatorSpeedY = m_IsBackGo ? fAnimatorSpeedY : fAnimatorSpeedY;

        if (inputMagnitude > 0f)
        {
            m_Animator.SetFloat("SpeedX", fAnimatorSpeedX, 0.01f, Time.deltaTime);
            m_Animator.SetFloat("SpeedY", fAnimatorSpeedY, 0.01f, Time.deltaTime);
        }
    }

    private bool IsTargetBehindPlayer(Transform m_Target)
    {
        if (m_Target == null) return false;

        // 플레이어의 정면 벡터
        Vector3 forward = transform.forward;
        // 플레이어에서 타겟까지의 방향 벡터
        Vector3 toTarget = (m_Target.position - transform.position).normalized;

        // 내적값이 0보다 작으면 뒤쪽
        float dot = Vector3.Dot(forward, toTarget);
        return dot < 0f;
    }
    private float GetLocalMoveFromWorld()
    {
        if (m_Movement.sqrMagnitude < 0.0001f)
            return 0f;

        // 내적값을 이용해 각도 계산 (y축 평면상)
        Vector3 forward = transform.forward;
        Vector3 move = m_Movement;

        // y축 평면 투영
        forward.y = 0f;
        move.y = 0f;

        forward.Normalize();
        move.Normalize();

        float dot = Vector3.Dot(forward, move);
        float angle = Mathf.Acos(Mathf.Clamp(dot, -1f, 1f)) * Mathf.Rad2Deg;

        // 방향(좌/우) 판별을 위해 cross product 사용
        float cross = Vector3.Cross(forward, move).y;
        if (cross < 0)
            angle = -angle;


        return angle; // -180 ~ 180도
    }
    private Vector2 Get8DirectionVector(float angle)
    {
        // -180 ~ 180 범위의 angle을 0~360으로 변환
        angle = (angle + 360f) % 360f;

        // 8방향 각도 구간에 따라 분기
        if (angle >= 337.5f || angle < 22.5f)
            return new Vector2(0f, 1f);         // 0도 (정면)
        else if (angle >= 22.5f && angle < 67.5f)
            return new Vector2(1f, 1f);         // 45도
        else if (angle >= 67.5f && angle < 112.5f)
            return new Vector2(1f, 0f);         // 90도 (오른쪽)
        else if (angle >= 112.5f && angle < 157.5f)
            return new Vector2(1f, -1f);        // 135도
        else if (angle >= 157.5f && angle < 202.5f)
            return new Vector2(0f, -1f);        // 180도, -180도 (뒤)
        else if (angle >= 202.5f && angle < 247.5f)
            return new Vector2(-1f, -1f);       // -135도
        else if (angle >= 247.5f && angle < 292.5f)
            return new Vector2(-1f, 0f);        // -90도 (왼쪽)
        else // angle >= 292.5f && angle < 337.5f
            return new Vector2(-1f, 1f);        // -45도
    }
    private void SetCurrentLocomotionState(float appliedSpeed)
    {

        if (Mathf.Abs(appliedSpeed - runSpeed) < 0.01f)
        {
            m_eLocomotionState = IdleWalkRunEnum.Run;
        }
        else if (Mathf.Abs(appliedSpeed - walkSpeed) < 0.01f)
        {
            m_eLocomotionState = IdleWalkRunEnum.Walk;
        }
        else
        {
            m_eLocomotionState = IdleWalkRunEnum.Idle;
        }

    }

    void OnPlayerMove()
    {
        float currentSpeed = m_IsRunning ? runSpeed : walkSpeed;
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * currentSpeed * Time.deltaTime);
        if (!m_IsBackGo)
        {
            m_Rigidbody.MoveRotation(m_Rotation);
        }
    }
}