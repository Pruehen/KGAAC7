using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUi : MonoBehaviour
{
    [SerializeField] Graphic _background;
    [SerializeField] List<Graphic> _childs;


    private void Awake()
    {
        //_childs = transform.GetChildRecursive<RawImage>();
    }

    public void FadeIn()
    {
        _background.FadeIn(1f);
        _background.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void FadeInChild()
    {
        foreach (var child in _childs)
        {
            child.FadeIn(1f);
        }
    }
}
