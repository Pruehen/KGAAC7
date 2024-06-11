using UnityEngine;

public class Cockpit : MonoBehaviour
{
    [SerializeField] Transform Transform_Aircraft;

    // Update is called once per frame    
    void LateUpdate()
    {
        if (Transform_Aircraft == null)
            return;

        this.transform.position = Transform_Aircraft.position;
        this.transform.rotation = Transform_Aircraft.rotation;
    }
}
