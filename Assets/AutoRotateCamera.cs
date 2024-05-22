using UnityEngine;

public class AutoRotateCamera : MonoBehaviour
{
    public Transform target; // ī�޶� ȸ���� �߽� ������Ʈ
    public float distance = 10f; // ī�޶�� ������Ʈ ������ �Ÿ�
    public float rotationSpeed = 20.0f; // ī�޶��� ȸ�� �ӵ�

    private float currentAngle = 0.0f; // ���� ȸ�� ����

    void LateUpdate()
    {
        if (target)
        {
            // ���� ������ �ð��� ���� ������Ŵ
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f; // 360���� ������ �ٽ� 0����

            // X, Z�࿡�� ī�޶��� ���ο� ��ġ ���
            float radians = currentAngle * Mathf.Deg2Rad;
            float x = target.position.x + distance * Mathf.Cos(radians);
            float z = target.position.z + distance * Mathf.Sin(radians);

            // ī�޶��� ���ο� ��ġ ����
            Vector3 newPosition = new Vector3(x, target.position.y + 3f, z);
            transform.position = newPosition;

            // ī�޶� �׻� Ÿ���� �ٶ󺸰� ����
            transform.LookAt(target);
        }
    }
}
