using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public List<GameObject> playerList;
    private int currentIndex = 0;

    void Start()
    {
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

    void SetControlTo(int index)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            //playerList[i].IsControlledByPlayer = (i == index);
        }
    }
}
