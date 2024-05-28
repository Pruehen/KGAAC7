using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUi : MonoBehaviour
{
    [SerializeField] Graphic _background;
    [SerializeField] List<Graphic> _childs;
    [SerializeField] bool _onlyActiveParent = true;


    private void Awake()
    {
        //_childs = transform.GetChildRecursive<RawImage>();
    }

    public void FadeIn()
    {
        _background.FadeIn(1f, _onlyActiveParent, FadeInChild);
    }

    private void FadeInChild()
    {
        foreach (var child in _childs)
        {
            child.FadeIn(1f, true);
        }
    }
    public void FadeOutResultUi()
    {
        FadeOutChildAndSelf();
    }
    private void FadeOutChildAndSelf()
    {
        bool once = false;
        foreach (var child in _childs)
        {
            if(!once)
            {
                once = true;
                child.FadeOut(1f, true, null, FadeOutSelf);
            }
            else
            {
                child.FadeOut(1f, true);
            }
        }
    }
    private void FadeOutSelf()
    {
        _background.FadeOut(1f, _onlyActiveParent);
    }
}
