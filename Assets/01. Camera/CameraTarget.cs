using UnityEngine;
using System.Collections.Generic;

public class CameraTarget : MonoBehaviour
{

    public Transform m_Ball; // ���� Transform
    private string[] m_TargetTags = new string[] { "BlueTeam", "RedTeam" };
    private GameObject[] m_TargetObjects; // Ÿ�ٵ��� GameObject �迭
    public float m_SmoothSpeed = 0.125f; // ī�޶� �������� �ε巯��
    public float m_Padding = 2f; // ī�޶� ���� ����
    public float m_CameraHeight = 10f; // ī�޶��� ���� ����

    private Camera m_Camera;

    void Start()
    {
        m_Camera = Camera.main;
        // ��� Ÿ���� GameObject�� ã�� �迭�� ����
        m_TargetObjects = FindTargetsByTags(m_TargetTags);
    }

    void LateUpdate()
    {
        if (m_Ball == null || m_TargetObjects == null || m_TargetObjects.Length == 0) return;


        // �� �����ڰ� ������ �⺻ �߽������� �̵�
        Vector3 targetPosition;

        // �� �����ڸ� ����
        targetPosition = new Vector3(gameObject.transform.position.x, m_CameraHeight, gameObject.transform.position.z);

        // ī�޶� ��ġ�� �ε巴�� �̵�
        transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, m_SmoothSpeed);
    }

    private GameObject[] FindTargetsByTags(string[] tags)
    {
        // ������ �±׿� �ش��ϴ� ��� GameObject�� ã��
        List<GameObject> targets = new List<GameObject>();
        foreach (string tag in tags)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            targets.AddRange(objects);
        }
        return targets.ToArray();
    }


}
