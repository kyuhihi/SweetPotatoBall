using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameObject m_BallOwner = null;

    public Vector3 GetProperCamPosition()
    {
        if (m_BallOwner != null)
        {
            // ���� �����ڰ� �ִٸ� �������� ��ġ�� �������� ���� ��ġ�� ����
            return m_BallOwner.transform.position + new Vector3(0, 0.5f, 0); // ���÷� Y���� �ణ �ø�
        }
        else
        {
            // �����ڰ� ���ٸ� �⺻ ��ġ�� ��ȯ
            return transform.position;
        }
        
    }
}
