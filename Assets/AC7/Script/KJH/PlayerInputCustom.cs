using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputCustom : NetworkBehaviour
{
    public bool isControlable { get; set; } = true;
    public float pitchAxis {get; private set;}
    public float rollAxis {get; private set;}
    public float yawAxis {get; private set;}
    public float throttleAxis { get; private set; }
    public bool isLeftClick {get; private set;}
    Vector2 mouseDeltaPos;
    float mouseControllGain = 0.1f;

    public System.Action OnFireEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    public UnityEvent onClick_X;
    public UnityEvent onClick_R;
    public UnityEvent onClick_Fdown;
    public UnityEvent onClick_Fup;
    public UnityEvent onClick_RightMouse;
    public UnityEvent onClick_LeftMouseDown;
    public UnityEvent onClick_LeftMouseUp;
    public UnityEvent onClick_MidMouseDown;
    public UnityEvent onClick_MidMouseUp;
    public UnityEvent onClick_ESC;

    // Update is called once per frame
    private enum CustmInputTypes
    {
        LeftMouseDown,
        LeftMouseUp,
        MidMouseDown,
        MidMouseUp,
        Click_X,
        Click_R,
        Click_Fdown,
        Click_Fup,
        Click_ESC
    }
    void Update()
    {
        if(!isLocalPlayer)
        { return; }
        if(!isControlable)
        { return; } 
        if(kjh.GameManager.Instance.IsPaused)
        { return; }
        ControlSurface();

        

        if (Input.GetMouseButtonDown(0))
        {
            CommandInvoke(CustmInputTypes.LeftMouseDown);
        }
        if (Input.GetMouseButtonUp(0))
        {
            CommandInvoke(CustmInputTypes.LeftMouseUp);
        }
        if (Input.GetMouseButtonDown(1))
        {
            CommandSetMissileFireTrigger(true);
        }
        if (Input.GetMouseButtonDown(2))
        {
            CommandInvoke(CustmInputTypes.MidMouseDown);
        }
        if (Input.GetMouseButtonUp(2))
        {
            CommandInvoke(CustmInputTypes.MidMouseUp);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            CommandInvoke(CustmInputTypes.Click_X);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            CommandInvoke(CustmInputTypes.Click_R);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            CommandInvoke(CustmInputTypes.Click_Fdown);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            CommandInvoke(CustmInputTypes.Click_Fup);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CommandInvoke(CustmInputTypes.Click_ESC);
        }
    }

    [Command(requiresAuthority = false)]
    private void CommandInvoke(CustmInputTypes unityEvent)
    {
        switch (unityEvent)
        {   
            case CustmInputTypes.LeftMouseDown:
                onClick_LeftMouseDown.Invoke();
                break;
            case CustmInputTypes.LeftMouseUp:
                onClick_LeftMouseUp.Invoke();
                break;
            case CustmInputTypes.MidMouseDown:
                onClick_MidMouseDown.Invoke();
                break;
            case CustmInputTypes.MidMouseUp:
                onClick_MidMouseUp.Invoke();
                break;
            case CustmInputTypes.Click_X:
                onClick_X.Invoke();
                break;
            case CustmInputTypes.Click_R:
                onClick_R.Invoke();
                break;
            case CustmInputTypes.Click_Fdown:
                onClick_Fdown.Invoke();
                break;
            case CustmInputTypes.Click_Fup:
                onClick_Fup.Invoke();
                break;
            case CustmInputTypes.Click_ESC:
                onClick_ESC.Invoke();
                break;
        }
    }    
    
    [Command(requiresAuthority = false)]
    private void CommandSetMissileFireTrigger(bool trigger)
    {
        missileFireTrigger = trigger;
    }    

    bool missileFireTrigger = false;
    private void FixedUpdate()
    {
        if (missileFireTrigger)
        {
            onClick_RightMouse.Invoke();
            missileFireTrigger = false;
        }
    }
    void OnMouseDeltaPos(InputValue inputValue)
    {
        if (!isControlable)
        { return; }
        mouseDeltaPos = inputValue.Get<Vector2>();//인풋 벡터 받아옴                
    }

    void ControlSurface()//조종면 관련 인풋
    {
        float pitchAxisTemp = 0;
        float rollAxisTemp = 0;
        float yawAxisTemp = 0;
        float throttleAxisTemp = 0;

        if (Input.GetKey(KeyCode.A))//요 좌측
        {
            yawAxisTemp -= 1;
        }
        if (Input.GetKey(KeyCode.D))//요 우측
        {
            yawAxisTemp += 1;
        }
        if (Input.GetKey(KeyCode.W))//피치 다운
        {
            pitchAxisTemp -= 1;
        }
        if (Input.GetKey(KeyCode.S))//피치 업
        {
            pitchAxisTemp += 1;
        }
        if (Input.GetKey(KeyCode.Q))//롤 좌측
        {
            rollAxisTemp -= 1;
        }
        if (Input.GetKey(KeyCode.E))//롤 우측
        {
            rollAxisTemp += 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))//스로틀 다운
        {
            throttleAxisTemp -= 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))//스로틀 업
        {
            throttleAxisTemp += 1;
        }


        pitchAxisTemp -= mouseDeltaPos.y * mouseControllGain;
        rollAxisTemp += mouseDeltaPos.x * mouseControllGain;

        pitchAxisTemp = Mathf.Clamp(pitchAxisTemp, -1, 1);
        rollAxisTemp = Mathf.Clamp(rollAxisTemp, -1, 1);
        CommandSetControlSurface(pitchAxisTemp, rollAxisTemp, yawAxisTemp, throttleAxisTemp);
    }

    [Command (requiresAuthority = false)]
    void CommandSetControlSurface(float pitch, float roll, float yaw, float throttle)
    {
        pitchAxis = pitch;
        rollAxis = roll;
        yawAxis = yaw;
        throttleAxis = throttle;
    }

    void OnFire(InputValue inputValue)
    {

        if (!isControlable)
        { return; }
        if (inputValue.isPressed)
        {
            OnFireEvent?.Invoke();
        }
    }
}
