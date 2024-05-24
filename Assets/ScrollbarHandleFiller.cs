using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollbarHandleFiller : MonoBehaviour
{
    public Scrollbar scrollbar;

    float targetValue;

    void Awake()
    {
        targetValue = scrollbar.size;
    }

    private void OnEnable()
    {
        scrollbar.size = 0;
    }

    public void OnScrollbarValueChanged()
    {
        scrollbar.size = Mathf.Lerp(scrollbar.size, targetValue, 5f * Time.deltaTime);
    }

    private void Update()
    {
        OnScrollbarValueChanged();
    }


}
