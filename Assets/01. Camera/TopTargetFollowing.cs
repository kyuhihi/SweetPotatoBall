using UnityEngine;

public class TopTargetFollowing : MonoBehaviour
{
    public Transform target; // ���� Ÿ��(�÷��̾�)
    public Vector3 offset = new Vector3(0, 10, 0); // ī�޶��� y�� ���̿� �Ÿ�
    public float followSpeed = 5f; // ���󰡴� �ӵ�

    void LateUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ ��� (�÷��̾� ����)
        Vector3 desiredPosition = target.position + offset;
        // �ε巴�� ���󰡱�
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
        // �׻� Ÿ���� �ٶ󺸰�
        transform.LookAt(target.position);
    }

    
    // Ÿ���� �����ϴ� �޼��� �߰�
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
