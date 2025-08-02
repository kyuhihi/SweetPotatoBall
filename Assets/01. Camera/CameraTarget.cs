using UnityEngine;
using System.Collections.Generic;

public class CameraTarget : MonoBehaviour
{
    [SerializeField]
    private Ball m_Ball; // ���� Transform
    [SerializeField]
    private TopTargetFollowing m_TopTargetFollowing; // TopTargetFollowing ��ũ��Ʈ
    private string[] m_TargetTags = new string[] { "BlueTeam", "RedTeam" };
    void Start()
    {
        m_Ball = GameObject.FindFirstObjectByType<Ball>(); 
    }

    void LateUpdate()
    {
        if(m_Ball == null)
            Debug.Assert(m_Ball != null, "Ball not found! Make sure a Ball object is present in the scene.");

        transform.position = m_Ball.GetProperCamPosition();

    }
    public void CallLateUpdateManually()
    {
        LateUpdate();
        m_TopTargetFollowing.CallLateUpdateManually();
    }


}
