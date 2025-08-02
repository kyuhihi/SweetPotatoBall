using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public List<GameObject> playerList;
    public TopTargetFollowing cameraController;
    private int currentIndex = 0;

    private PlayerInputActions inputActions;



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
            // "Player1", "Player2" 형식이라고 가정
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
        if (currentIndex < playerList.Count)
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
        if (currentIndex < playerList.Count)
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
        if (currentIndex < playerList.Count)
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
        int playerIndex = playerList.IndexOf(targetPlayer);
        if (playerIndex >= 0)
        {
            currentIndex = playerIndex;
            SetControlTo(currentIndex);
        }
    }
    
    void SetControlTo(int index)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            BasePlayerMovement playerMovement = playerList[i].GetComponent<BasePlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.IsControlledByPlayer = (i == index);
            }

            // 카메라도 현재 플레이어를 따라가도록 설정
            if (cameraController != null && index < playerList.Count)
            {
                cameraController.SetTarget(playerList[index].transform);
            }
        }
    }
}
