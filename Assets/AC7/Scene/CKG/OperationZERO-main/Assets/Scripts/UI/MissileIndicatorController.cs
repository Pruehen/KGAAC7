using UnityEngine;

public class MissileIndicatorController : SceneSingleton<MissileIndicatorController>
{
    [SerializeField]
    GameObject mslIndicatorPrf;

    public void AddMissileIndicator(Guided missile)
    {
        GameObject obj = ObjectPoolManager.Instance.DequeueObject(mslIndicatorPrf);
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<MissileIndicator>().Missile = missile;                
    }

    public void RemoveMissileIndicator(Guided missile)
    {
        //missile.GetComponent<MissileIndicator>().Missile = null;
    }
}
