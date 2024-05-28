using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ImageUiExtender
{
    public static void FadeIn(this Graphic image, float time, bool onlyActiveSelf = false, System.Action fadeEnd = null)
    {
        kjh.GameManager.Instance.FadeIn(image, time, onlyActiveSelf, fadeEnd);
    }
    public static void FadeOut(this Graphic image, float time, bool onlyActiveSelf = false, System.Action fadeStart = null, System.Action fadeEnd = null)
    {
        kjh.GameManager.Instance.FadeOut(image, time, onlyActiveSelf, fadeStart, fadeEnd);
    }
}

public static class TransformExtender
{
    /// <summary>
    /// ��� �ڽĿ��� ������Ʈ�� ã�� ����Ʈ�� ��ȯ����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static List<T> GetChildRecursive<T>(this Transform transform)
    {
        List<Transform> list = new List<Transform>();
        List<T> result = new List<T>();
        GetAllChild(transform, ref list);

        foreach (Transform child in list)
        {
            if (child != transform)
            {
                result.Add(child.GetComponent<T>());
            }
        }    
        return result;
    }
    /// <summary>
    /// ��� �ڽ� ���ӿ�����Ʈ�� ����Ʈ�� �����
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="list"></param>
    private static void GetAllChild(this Transform transform, ref List<Transform> list)
    {
        list.Add(transform);
        for (int i = 0; i < transform.childCount; i++) 
        {
            GetAllChild(transform.GetChild(i),ref list);
        }
    }
}
