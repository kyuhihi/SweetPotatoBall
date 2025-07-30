using UnityEngine;

public class Ball : MonoBehaviour
{
    private GameObject m_BallOwner = null;

    public Vector3 GetProperCamPosition()
    {
        if (m_BallOwner != null)
        {
            // 공의 소유자가 있다면 소유자의 위치를 기준으로 공의 위치를 조정
            return m_BallOwner.transform.position + new Vector3(0, 0.5f, 0); // 예시로 Y축을 약간 올림
        }
        else
        {
            // 소유자가 없다면 기본 위치를 반환
            return transform.position;
        }
        
    }
}
