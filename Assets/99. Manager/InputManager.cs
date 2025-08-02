using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public List<GameObject> playerList;
    public TopTargetFollowing cameraController;
    private int currentIndex = 0;

    private PlayerInputActions inputActions;

    // 해야할 일
    // 반대되는 방향키를 누를때 느리게 누른 키로 적용하기 (멈추는 일이 없게 하기 위해)

    // 현재 입력 상태 저장
    private Vector2 currentMoveInput = Vector2.zero;
    private bool isCurrentlySprinting = false;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        inputActions.Player.Pass.performed += OnPass;
    }

    void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.Disable();
    }

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("BlueTeam");

        // 이름에서 숫자를 추출해서 정렬
        System.Array.Sort(players, (x, y) =>
        {
            string xNumber = x.name.Replace("Player", "");
            string yNumber = y.name.Replace("Player", "");

            if (int.TryParse(xNumber, out int xNum) && int.TryParse(yNumber, out int yNum))
            {
                return xNum.CompareTo(yNum);
            }
            return x.name.CompareTo(y.name);
        });

        foreach (GameObject player in players)
        {
            playerList.Add(player);
        }

        // 카메라 컨트롤러가 없으면 자동으로 찾기
        if (cameraController == null)
        {
            GameObject cameraObject = GameObject.FindWithTag("MainCamera");
            if (cameraObject != null)
            {
                cameraController = cameraObject.GetComponent<TopTargetFollowing>();
            }
        }

        SetControlTo(currentIndex);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % playerList.Count;
            SetControlTo(currentIndex);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // 현재 입력 상태 저장
        currentMoveInput = context.ReadValue<Vector2>();

        if (currentIndex < playerList.Count && playerList[currentIndex] != null)
        {
            PlayerMovement playerMovement = playerList[currentIndex].GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.OnMoveInput(context);
            }
        }
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        // 현재 스프린트 상태 저장
        isCurrentlySprinting = context.performed;

        if (currentIndex < playerList.Count && playerList[currentIndex] != null)
        {
            PlayerMovement playerMovement = playerList[currentIndex].GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.OnSprintInput(context);
            }
        }
    }

    private void OnPass(InputAction.CallbackContext context)
    {
        if (currentIndex < playerList.Count && playerList[currentIndex] != null)
        {
            PlayerMovement playerMovement = playerList[currentIndex].GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.OnPassInput(context);
            }
        }
    }
    
    public void SwitchToPlayer(GameObject targetPlayer)
    {
        if (targetPlayer == null)
        {
            Debug.LogWarning("SwitchToPlayer: targetPlayer가 null입니다.");
            return;
        }

        int playerIndex = playerList.IndexOf(targetPlayer);
        if (playerIndex >= 0)
        {
            currentIndex = playerIndex;
            SetControlTo(currentIndex);
        }
        else
        {
            Debug.LogWarning($"SwitchToPlayer: {targetPlayer.name}을 playerList에서 찾을 수 없습니다.");
        }
    }
    
    void SetControlTo(int index)
    {
        if (index >= playerList.Count || playerList[index] == null)
        {
            Debug.LogWarning($"SetControlTo: 유효하지 않은 인덱스 - {index}");
            return;
        }

        // 모든 플레이어의 컨트롤 상태 설정
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] != null)
            {
                BasePlayerMovement playerMovement = playerList[i].GetComponent<BasePlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.IsControlledByPlayer = (i == index);
                }
            }
        }

        // 새 플레이어에게 현재 입력 상태 전달
        ApplyCurrentInputToPlayer(index);

        // 카메라도 현재 플레이어를 따라가도록 설정
        if (cameraController != null)
        {
            cameraController.SetTarget(playerList[index].transform);
        }

        Debug.Log($"컨트롤 전환: {playerList[index].name}");
    }

    private void ApplyCurrentInputToPlayer(int playerIndex)
    {
        if (playerIndex >= playerList.Count || playerList[playerIndex] == null)
            return;

        PlayerMovement playerMovement = playerList[playerIndex].GetComponent<PlayerMovement>();
        if (playerMovement == null)
            return;

        // 현재 움직임 입력이 있다면 새 플레이어에게 즉시 적용
        if (currentMoveInput != Vector2.zero)
        {
            // PlayerMovement에 직접 입력 값 설정하는 메서드 필요
            playerMovement.OnMoveInput(currentMoveInput);
            // playerMovement.SetDirectMoveInput(currentMoveInput);
            Debug.Log($"움직임 입력 전달: {currentMoveInput} -> {playerList[playerIndex].name}");
        }

        // 현재 스프린트 상태도 전달
        if (isCurrentlySprinting)
        {
            playerMovement.OnSprintInput(isCurrentlySprinting);
            // playerMovement.SetDirectSprintInput(isCurrentlySprinting);
            Debug.Log($"스프린트 입력 전달 -> {playerList[playerIndex].name}");
        }
    }
}
