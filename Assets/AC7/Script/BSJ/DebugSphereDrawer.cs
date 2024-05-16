using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSphereDrawer : SceneSingleton<DebugSphereDrawer>
{
    private static List<Vector3> poss = new();
    private static List<float> radis = new();
    private static List<Color> colors = new();

    private void Start()
    {
        DebugExtender.Init(this);
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < poss.Count; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawSphere(poss[i], radis[i]);
            poss.Clear();
            radis.Clear();
            colors.Clear();
        }
    }
    public void AddSphere(Vector3 position, float radius, Color color)
    {
        poss.Add(position);
        radis.Add(radius);
        colors.Add(color);
    }

    //public void AddSphereForDuration(Vector3 position, float radius, float duration, Color color)
    //{
    //    StartCoroutine(AddLoop(position, radius, duration, color));
    //}
    //private IEnumerator AddLoop(Vector3 position, float radius, float duration, Color color)
    //{
    //    float initTime = Time.time;
    //    while(initTime + duration < Time.time)
    //    {
    //        AddSphere(position, radius, color);
    //        yield return null;
    //    }

    //}
}

public static class DebugExtender
{
    static DebugSphereDrawer _dsp;

    public static void Init(DebugSphereDrawer dsp)
    {
        _dsp = dsp;
    }

    public static void DrawSphere(this Vector3 position, float radius, Color color)
    {
        _dsp.AddSphere(position, radius, color);
    }
    //public static void DrawSphereDuration(this Vector3 position, float radius, float duration, Color color)
    //{
    //    _dsp.AddSphereForDuration(position, radius, duration, color);
    //}

}