using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCustom : SceneSingleton<PlayerInputCustom>
{
    public float pitchAxis {get; private set;}
    public float rollAxis {get; private set;}
    public float yawAxis {get; private set;}
    public float throttleAxis { get; private set; }
    public bool isLeftClick {get; private set;}
    public bool isRightClick { get; private set; }
    Vector2 mouseDeltaPos;
    float mouseControllGain = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ControlSurface();

        if (Input.GetMouseButtonDown(0))
        {
            isLeftClick = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isLeftClick = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            isRightClick = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightClick = false;
        }
    }

    void OnMouseDeltaPos(InputValue inputValue)            
    {
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
}
