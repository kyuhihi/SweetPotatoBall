using UnityEngine;

public class TopTargetFollowing : MonoBehaviour
{
    public Transform target; // ���� Ÿ��(�÷��̾�)
    public Vector3 offset = new Vector3(0, 10, 0); // ī�޶��� y�� ���̿� �Ÿ�
    public float followSpeed = 5f; // ���󰡴� �ӵ�
    public float rotationSpeed = 5f; // ȸ�� �ӵ�

    void LateUpdate()
    {
        if (target == null) return;

        // ��ǥ ��ġ ��� (�÷��̾� ����)
        Vector3 desiredPosition = target.position + offset;
        // �ε巴�� ���󰡱�
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // ��ǥ ȸ�� ���
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // y�� ȸ�� �� ����
        transform.rotation = Quaternion.Euler(smoothedRotation.eulerAngles.x, transform.rotation.eulerAngles.y, smoothedRotation.eulerAngles.z);
    }

    public void CallLateUpdateManually()
    {
        if (target == null) return;
        


        // ��ǥ ��ġ ��� (�÷��̾� ����)
        Vector3 desiredPosition = target.position + offset;
        // �ε巴�� ���󰡱�
        transform.position = desiredPosition;

        // ��ǥ ȸ�� ���
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // y�� ȸ�� �� ����
        transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetRotation.eulerAngles.z);
    }
}
