using UnityEngine;

public class BackUpCam : SceneSingleton<BackUpCam>
{
    public void SetActiveBackUpCam(bool value)
    {
        Debug.Log($"��� ī�޶� �� : {value}");
        this.gameObject.SetActive(value);
    }
}
