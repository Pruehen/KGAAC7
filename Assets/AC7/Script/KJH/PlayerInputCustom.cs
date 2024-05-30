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
            NetworkInvoke(onClick_LeftMouseDown);
        }
        if (Input.GetMouseButtonUp(0))
        {
            NetworkInvoke(onClick_LeftMouseUp);
        }
        if (Input.GetMouseButtonDown(1))
        {
            missileFireTrigger = true;
        }
        if (Input.GetMouseButtonDown(2))
        {
            NetworkInvoke(onClick_MidMouseDown);
        }
        if (Input.GetMouseButtonUp(2))
        {
            NetworkInvoke(onClick_MidMouseUp);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            NetworkInvoke(onClick_X);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            NetworkInvoke(onClick_R);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {            
            NetworkInvoke(onClick_Fdown);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {            
            NetworkInvoke(onClick_Fup);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {            
            NetworkInvoke(onClick_ESC);
        }
    }
    void NetworkInvoke(UnityEvent unityEvent)
    {
        unityEvent.Invoke();

        if(this.isClient)
        {
            CommandInvoke_ClientOnly(unityEvent);
        }
    }

    [Command]
    void CommandInvoke_ClientOnly(UnityEvent unityEvent)
    {
        unityEvent.Invoke();
    }

    bool missileFireTrigger = false;
    private void FixedUpdate()
    {
        if (missileFireTrigger)
        {
            NetworkInvoke(onClick_RightMouse);
            missileFireTrigger = false;
        }
    }
    void OnMouseDeltaPos(InputValue inputValue)
    {
        if (!isControlable)
        { return; }
        mouseDeltaPos = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�                
    }

    void ControlSurface()//������ ���� ��ǲ
    {
        pitchAxis = 0;
        rollAxis = 0;
        yawAxis = 0;
        throttleAxis = 0;

        if (Input.GetKey(KeyCode.A))//�� ����
        {
            yawAxis -= 1;
        }
        if (Input.GetKey(KeyCode.D))//�� ����
        {
            yawAxis += 1;
        }
        if (Input.GetKey(KeyCode.W))//��ġ �ٿ�
        {
            pitchAxis -= 1;
        }
        if (Input.GetKey(KeyCode.S))//��ġ ��
        {
            pitchAxis += 1;
        }
        if (Input.GetKey(KeyCode.Q))//�� ����
        {
            rollAxis -= 1;
        }
        if (Input.GetKey(KeyCode.E))//�� ����
        {
            rollAxis += 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))//����Ʋ �ٿ�
        {
            throttleAxis -= 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))//����Ʋ ��
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
