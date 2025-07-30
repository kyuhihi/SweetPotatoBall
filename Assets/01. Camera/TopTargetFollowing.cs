using UnityEngine;

public class TopTargetFollowing : MonoBehaviour
{
    public Transform target; // 따라갈 타겟(플레이어)
    public Vector3 offset = new Vector3(0, 10, 0); // 카메라의 y축 높이와 거리
    public float followSpeed = 5f; // 따라가는 속도
    public float rotationSpeed = 5f; // 회전 속도

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산 (플레이어 위쪽)
        Vector3 desiredPosition = target.position + offset;
        // 부드럽게 따라가기
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // y축 회전 값 고정
        transform.rotation = Quaternion.Euler(smoothedRotation.eulerAngles.x, transform.rotation.eulerAngles.y, smoothedRotation.eulerAngles.z);
    }

    public void CallLateUpdateManually()
    {
        if (target == null) return;
        


        // 목표 위치 계산 (플레이어 위쪽)
        Vector3 desiredPosition = target.position + offset;
        // 부드럽게 따라가기
        transform.position = desiredPosition;

        // 목표 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // y축 회전 값 고정
        transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetRotation.eulerAngles.z);
    }
}
