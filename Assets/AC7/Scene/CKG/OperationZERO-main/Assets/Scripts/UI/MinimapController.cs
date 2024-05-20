using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapController : SceneSingleton<MinimapController>
{
    public enum MinimapIndex
    {
        Small,
        Large,
        Entire
    }

    //public Camera minimapCamera;
    public Transform targetTrf;//미니맵이 어떤 오브젝트를 기준으로 그려질지 결정하는 트랜스폼
    public GameObject minimapIndicatorPrf;//미니맵 인디케이터 프리팹

    [Header("Follow Target / Small View Offset Ratio")]
    public Transform target;
    public float offsetRatio;
    
    [Header("Minimap sizes")]
    public float smallViewSize;
    public float smallZoomViewSize;
    public float largeViewSize;
    public float entireViewSize;

    public float zoomDistanceThreshold;
    public float zoomLerpAmount;
    bool isZooming;
    
    [Header("Icon resize factors (small = 1)")]
    public float largeViewIconResizeFactor;
    public float entireViewIconResizeFactor;
    
    [Header("Minimap UI")]
    public GameObject[] minimaps = new GameObject[3];

    Vector2 cameraSize;
    float sizeReciprocal;
    int minimapIndex;
    int savedMinimapIndex;  // saved on pause

    public MinimapIndex GetMinimapIndex()
    {
        return (MinimapIndex)minimapIndex;
    }

    public Vector2 CameraSize
    {
        get { return cameraSize; }
    }

    // Input Event
    public void ChangeMinimapView(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            minimapIndex = (++minimapIndex) % 3;
            SetCamera();
        }
    }

    public void SetPauseMinimapCamera(bool isPaused)
    {
        if(isPaused)
        {
            savedMinimapIndex = minimapIndex;
            minimapIndex = (int)MinimapIndex.Entire;
        }
        else
        {
            minimapIndex = savedMinimapIndex;
        }
        SetCamera();
    }


    // Change visibility and minimapCamera's size and culling mask
    public void SetCamera()
    {
        /*switch((MinimapIndex)minimapIndex)
        {
            case MinimapIndex.Small:
                minimapCamera.orthographicSize = smallViewSize;
                minimapCamera.cullingMask &= (1 << LayerMask.NameToLayer("Minimap"));
                break;
                
            case MinimapIndex.Large:
                minimapCamera.orthographicSize = largeViewSize;
                minimapCamera.cullingMask |= (1 << LayerMask.NameToLayer("Minimap (Player)"));
                break;
                
            case MinimapIndex.Entire:
                minimapCamera.orthographicSize = entireViewSize;
                minimapCamera.cullingMask |= (1 << LayerMask.NameToLayer("Minimap (Player)"));
                break;
        }*/

        //for(int i = 0; i < minimaps.Length; i++)
        //{
        //    minimaps[i].gameObject.SetActive(i == minimapIndex);
        //}

        //cameraSize = new Vector2(minimapCamera.orthographicSize, minimapCamera.orthographicSize * minimapCamera.aspect);
    }


    public float GetInitCameraViewSize()
    {
        //return minimapCamera.orthographicSize;
        return 0;
    }

    // Multiply orthographicSize to keep icons' original size
    public float GetIconResizeFactor()
    {
        //float size = minimapCamera.orthographicSize;

        /*(switch((MinimapIndex)minimapIndex)
        {
            case MinimapIndex.Large:
                return size * largeViewIconResizeFactor;
                
            case MinimapIndex.Entire:
                return size * entireViewIconResizeFactor;

            default:
                return size;
        }*/
        return 0;
    }

    void AdjustCameraTransform()
    {
        if(target == null) return;
        
        // Camera position adjustment
        Vector3 position;
        float cameraRotation;

        if(minimapIndex == (int)MinimapIndex.Small)
        {
            Vector3 targetForwardVector = target.forward;
            targetForwardVector.y = 0;
            targetForwardVector.Normalize();

           // position = new Vector3(target.transform.position.x, 1, target.transform.position.z)
            //               + targetForwardVector * offsetRatio * minimapCamera.orthographicSize;
            cameraRotation =  -target.eulerAngles.y;
        }
        else
        {
            if(minimapIndex == (int)MinimapIndex.Large)
            {
                position = new Vector3(target.transform.position.x, 1, target.transform.position.z);
            }
            else
            {
                position = transform.position;  // Minimap Controller must be at the center of the map
                position.y = 1;
            }
            cameraRotation = 0;
        }

       // minimapCamera.transform.position = position;
        //minimapCamera.transform.eulerAngles = new Vector3(90, 0, cameraRotation);
    }

    
    public void SetZoom(float distance)
    {
        if(distance < zoomDistanceThreshold)
        {
            isZooming = true;
        }
        else if(distance > zoomDistanceThreshold * 1.5f)
        {
            isZooming = false;
        }
    }

    void ZoomCamera()
    {
        float size = (isZooming == true) ? smallZoomViewSize : smallViewSize;
        //minimapCamera.orthographicSize = Mathf.Lerp(minimapCamera.orthographicSize, size, zoomLerpAmount * Time.deltaTime);
       // cameraSize = new Vector2(minimapCamera.orthographicSize, minimapCamera.orthographicSize * minimapCamera.aspect);
    }

    void Awake()
    {
        minimapIndex = (int)MinimapIndex.Small;
        SetCamera();
        isZooming = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        AdjustCameraTransform();

        if(minimapIndex == (int)MinimapIndex.Small)
        {
            ZoomCamera();
        }
    }
}
