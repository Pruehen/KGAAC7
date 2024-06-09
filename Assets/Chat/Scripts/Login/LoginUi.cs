using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginUi : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField InputField_userId;
    [SerializeField] private TMPro.TMP_InputField InputField_userPassword;
    [SerializeField] private TMPro.TMP_Dropdown DropDown_AircraftName;

    [SerializeField] private GameObject OnLoginNextUi;
    [SerializeField] private GameObject OnFailAnimation;
    [SerializeField] private PlayerData playerData;
    // =================================================OnButtunClickInvoke
    private void Start()
    {
        InputField_userId.Select();
    }

    public void OnLogin()
    {
        if (Login.RequestLogin(InputField_userId.text, InputField_userPassword.text))
        {
            Debug.Log("Success");
            playerData = FindAnyObjectByType<PlayerData>();
            playerData.Init(InputField_userId.text);
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
    public void OnSetAircraft()
    {
        int selectedIndex = DropDown_AircraftName.value;
        playerData.SetPlayerAircraft(DropDown_AircraftName.
            options[selectedIndex].text);
    }
}
