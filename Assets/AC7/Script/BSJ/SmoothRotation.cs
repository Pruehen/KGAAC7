using UnityEngine;
public class SmoothRotation : MonoBehaviour
{
    [SerializeField] float _maxRotationSpeed = 60f; // Maximum rotation speed in degrees per second
    [SerializeField] float _smoothDuration = 1.0f; // Duration of the smooth-in and smooth-out phases

    private Quaternion targetRotation;
    private Quaternion startRotation;
    private float angleRemaining;
    private float speedFactor;

    private bool isRotating = false;
    private Quaternion _currentRotation;

    public void Init(float maxRotationSpeed, float smoothDuration)
    {
        _maxRotationSpeed = maxRotationSpeed;
        _smoothDuration = smoothDuration;
    }

    void Update()
    {
        if (isRotating)
        {
            float deltaAngle = _maxRotationSpeed * Time.deltaTime * speedFactor;

            // Adjust the speed factor based on remaining angle for smooth-in and smooth-out
            if (angleRemaining < _maxRotationSpeed * _smoothDuration)
            {
                speedFactor = angleRemaining / (_maxRotationSpeed * _smoothDuration);
            }
            else
            {
                speedFactor = 1.0f;
            }

            _currentRotation = Quaternion.RotateTowards(_currentRotation, targetRotation, deltaAngle);
            angleRemaining -= deltaAngle;

            if (angleRemaining <= 0.1f) // Threshold to stop rotation
            {
                _currentRotation = targetRotation;
                isRotating = false;
            }
        }
    }

    public void UpdateTargetRotation(Quaternion newTargetRotation)
    {
        targetRotation = newTargetRotation;
        startRotation = transform.rotation;
        angleRemaining = Quaternion.Angle(startRotation, targetRotation);
        isRotating = true;
    }

    public void StartRotation()
    {
        startRotation = transform.rotation;
        angleRemaining = Quaternion.Angle(startRotation, targetRotation);
        isRotating = true;
    }
    public void StopRatation()
    {
        isRotating = false;
    }
    public Quaternion GetRotation()
    {
        return _currentRotation;
    }
}