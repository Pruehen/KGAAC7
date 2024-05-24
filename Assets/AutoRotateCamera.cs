using UnityEngine;

public class AutoRotateCamera : MonoBehaviour
{
    public Transform target; // 카메라가 회전할 중심 오브젝트
    public float distance = 7.9f; // 카메라와 오브젝트 사이의 거리
    public float rotationSpeed = 20.0f; // 카메라의 회전 속도

    private float currentAngle = 0.0f; // 현재 회전 각도

    void LateUpdate()
    {
        if (target)
        {
            // 현재 각도를 시간에 따라 증가시킴
            currentAngle += rotationSpeed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f; // 360도를 넘으면 다시 0도로

            // X, Z축에서 카메라의 새로운 위치 계산
            float radians = currentAngle * Mathf.Deg2Rad;
            float x = target.position.x + distance * Mathf.Cos(radians);
            float z = target.position.z + distance * Mathf.Sin(radians);

            // 카메라의 새로운 위치 설정
            Vector3 newPosition = new Vector3(x, target.position.y + 7f, z);
            transform.position = newPosition;
            
            // 카메라가 항상 타겟을 바라보게 설정
            transform.LookAt(target);
        }
    }
    void Start()
    {
        if (target)
        {
            Debug.Log("Initial target position: " + target.position);
        }
        Debug.Log("Initial camera position: " + transform.position);
    }

}
