using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Mirror;

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

    enum InputEnum
    {
        LeftMouse_Down,
        LeftMouse_Up,
        RighdMouse_Click,
        X_Click,
        R_Click,
        F_Down,
        F_Up,
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isLocalPlayer) return;

        if(!isControlable)
        { return; } 
        if(kjh.GameManager.Instance.IsPaused)
        { return; }
        ControlSurface();

        if (Input.GetMouseButtonDown(0))
        {
            NetworkInvoke(InputEnum.LeftMouse_Down);
        }
        if (Input.GetMouseButtonUp(0))
        {
            NetworkInvoke(InputEnum.LeftMouse_Up);
        }
        if (Input.GetMouseButtonDown(1))
        {
            missileFireTrigger = true;
        }
        if (Input.GetMouseButtonDown(2))
        {
            onClick_MidMouseDown.Invoke();
        }
        if (Input.GetMouseButtonUp(2))
        {
            onClick_MidMouseUp.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            NetworkInvoke(InputEnum.X_Click);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            NetworkInvoke(InputEnum.R_Click);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            NetworkInvoke(InputEnum.F_Down);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            NetworkInvoke(InputEnum.F_Up);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            onClick_ESC.Invoke();
        }
    }
    void NetworkInvoke(InputEnum inputEnum)
    {
        if(!this.isServer)
        {
            switch (inputEnum)
            {
                case InputEnum.LeftMouse_Down:
                    onClick_LeftMouseDown.Invoke();
                    break;
                case InputEnum.LeftMouse_Up:
                    onClick_LeftMouseUp.Invoke();
                    break;
                case InputEnum.RighdMouse_Click:
                    onClick_RightMouse.Invoke();
                    break;
                case InputEnum.X_Click:
                    onClick_X.Invoke();
                    break;
                case InputEnum.R_Click:
                    onClick_R.Invoke();
                    break;
                case InputEnum.F_Down:
                    onClick_Fdown.Invoke();
                    break;
                case InputEnum.F_Up:
                    onClick_Fup.Invoke();
                    break;
                default:
                    break;
            }
        }

        CommandNetworkInvoke(inputEnum);
    }

    [Command(requiresAuthority = false)]
    void CommandNetworkInvoke(InputEnum inputEnum)
    {
        switch (inputEnum)
        {
            case InputEnum.LeftMouse_Down:
                onClick_LeftMouseDown.Invoke();
                break;
            case InputEnum.LeftMouse_Up:
                onClick_LeftMouseUp.Invoke();
                break;
            case InputEnum.RighdMouse_Click:
                onClick_RightMouse.Invoke();
                break;
            case InputEnum.X_Click:
                onClick_X.Invoke();
                break;
            case InputEnum.R_Click:
                onClick_R.Invoke();
                break;
            case InputEnum.F_Down:
                onClick_Fdown.Invoke();
                break;
            case InputEnum.F_Up:
                onClick_Fup.Invoke();
                break;
            default:
                break;
        }
    }


    bool missileFireTrigger = false;
    private void FixedUpdate()
    {
        if (missileFireTrigger)
        {
            NetworkInvoke(InputEnum.RighdMouse_Click);
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
        pitchAxis = 0;
        rollAxis = 0;
        yawAxis = 0;
        throttleAxis = 0;

        if (Input.GetKey(KeyCode.A))//요 좌측
        {
            yawAxis -= 1;
        }
        if (Input.GetKey(KeyCode.D))//요 우측
        {
            yawAxis += 1;
        }
        if (Input.GetKey(KeyCode.W))//피치 다운
        {
            pitchAxis -= 1;
        }
        if (Input.GetKey(KeyCode.S))//피치 업
        {
            pitchAxis += 1;
        }
        if (Input.GetKey(KeyCode.Q))//롤 좌측
        {
            rollAxis -= 1;
        }
        if (Input.GetKey(KeyCode.E))//롤 우측
        {
            rollAxis += 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))//스로틀 다운
        {
            throttleAxis -= 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))//스로틀 업
        {
            throttleAxis += 1;
        }


        pitchAxis -= mouseDeltaPos.y * mouseControllGain;
        rollAxis += mouseDeltaPos.x * mouseControllGain;

        pitchAxis = Mathf.Clamp(pitchAxis, -1, 1);
        rollAxis = Mathf.Clamp(rollAxis, -1, 1);
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
