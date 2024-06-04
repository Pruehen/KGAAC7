using UnityEngine;

public class BackUpCam : SceneSingleton<BackUpCam>
{
    public void SetActiveBackUpCam(bool value)
    {
        Debug.Log($"백업 카메라 셋 : {value}");
        this.gameObject.SetActive(value);
    }
}
