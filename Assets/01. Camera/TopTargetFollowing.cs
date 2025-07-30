using UnityEngine;

public class TopTargetFollowing : MonoBehaviour
{
    public Transform target; // 따라갈 타겟(플레이어)
    public Vector3 offset = new Vector3(0, 10, 0); // 카메라의 y축 높이와 거리
    public float followSpeed = 5f; // 따라가는 속도

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산 (플레이어 위쪽)
        Vector3 desiredPosition = target.position + offset;
        // 부드럽게 따라가기
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        // 항상 타겟을 바라보게
        transform.LookAt(target.position);
    }

    
    // 타겟을 변경하는 메서드 추가
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
