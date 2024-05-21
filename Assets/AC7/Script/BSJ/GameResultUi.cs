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
        _background.FadeIn(1f, FadeInChild);
        if( _onlyActiveParent )
        {
            for (int i = 0; i < _background.transform.childCount; i++)
            {
                _background.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void FadeInChild()
    {
        foreach (var child in _childs)
        {
            child.FadeIn(1f);
        }
    }
}
