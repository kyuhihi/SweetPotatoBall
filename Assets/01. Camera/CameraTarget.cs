using UnityEngine;
using System.Collections.Generic;

public class CameraTarget : MonoBehaviour
{

    public Transform m_Ball; // 공의 Transform
    private string[] m_TargetTags = new string[] { "BlueTeam", "RedTeam" };
    private GameObject[] m_TargetObjects; // 타겟들의 GameObject 배열
    public float m_SmoothSpeed = 0.125f; // 카메라 움직임의 부드러움
    public float m_Padding = 2f; // 카메라 여유 공간
    public float m_CameraHeight = 10f; // 카메라의 고정 높이

    private Camera m_Camera;

    void Start()
    {
        m_Camera = Camera.main;
        // 모든 타겟의 GameObject를 찾아 배열에 저장
        m_TargetObjects = FindTargetsByTags(m_TargetTags);
    }

    void LateUpdate()
    {
        if (m_Ball == null || m_TargetObjects == null || m_TargetObjects.Length == 0) return;


        // 공 소유자가 없으면 기본 중심점으로 이동
        Vector3 targetPosition;

        // 공 소유자를 따라감
        targetPosition = new Vector3(gameObject.transform.position.x, m_CameraHeight, gameObject.transform.position.z);

        // 카메라 위치를 부드럽게 이동
        transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, m_SmoothSpeed);
    }

    private GameObject[] FindTargetsByTags(string[] tags)
    {
        // 지정된 태그에 해당하는 모든 GameObject를 찾음
        List<GameObject> targets = new List<GameObject>();
        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            targets.AddRange(objects);
        }
        return targets.ToArray();
    }


}
