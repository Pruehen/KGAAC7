using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUi : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField InputField_userId;
    [SerializeField] private TMPro.TMP_InputField InputField_userPassword;

    [SerializeField] private GameObject OnLoginNextUi;
    [SerializeField] private GameObject OnFailAnimation;
    // =================================================OnButtunClickInvoke
    public void OnLogin()
    {
        if (Login.RequestLogin(InputField_userId.text, InputField_userPassword.text))
        {
            Debug.Log("Success");
            PlayerData playerDate = FindAnyObjectByType<PlayerData>();
            playerDate.SetPlayerAircraft("F16");
            playerDate.Init(InputField_userId.text);
            gameObject.SetActive(false);
            OnLoginNextUi.SetActive(true);
        }
        else
        {
            Debug.Log("Fail");
            OnFailAnimation.SetActive(true);
            OnFailAnimation.GetComponent<Animation>().Play();
        }
    }
    // =================================================OnButtunClickInvoke
    public void OnRegister()
    {
        if (Login.RequestInsert(InputField_userId.text, InputField_userPassword.text))
        {
            Debug.Log("Success");
        }
        else
        {
            Debug.Log("Fail");
        }
    }
}
