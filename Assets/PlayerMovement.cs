using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    private bool m_IsControlled = false;
    public bool IsControlled
    {
        get { return m_IsControlled; }
        set
        {
            m_IsControlled = value;
        }
    }

    private Vector3 m_Movement;
    private Quaternion m_Rotation = Quaternion.identity;
    private Vector2 m_InputVector = Vector2.zero;
    private bool m_IsRunning = false;

    private PlayerInputActions inputActions;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        // m_Animator.applyRootMotion = false;
        m_Animator.applyRootMotion = true;
        m_Rigidbody = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        m_IsRunning = context.ReadValueAsButton();
    }

    void OnFootstep()
    {
        // Debug.Log("발소리 재생"); // 또는 AudioSource.Play(), 이펙트 등 실행
    }

    void FixedUpdate()
    {

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
        //m_Animator.SetFloat("Speed", appliedSpeed);

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