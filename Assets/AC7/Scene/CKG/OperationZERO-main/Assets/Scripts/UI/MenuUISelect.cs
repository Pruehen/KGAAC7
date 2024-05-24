using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MenuUISelect : UISelect
{
    Animation anim;
    TextMeshProUGUI textMeshPro;
    [SerializeField]
    Image image;

    void Awake()
    {
        anim = GetComponent<Animation>();
        textMeshPro = GetComponent<TextMeshProUGUI>();

        image.rectTransform.pivot = new Vector3(0, 0.5f, 0);
        image.rectTransform.anchoredPosition = Vector3.zero;       
    }

    void OnEnable()
    {
        image.rectTransform.sizeDelta = textMeshPro.GetPreferredValues();
        anim.Play();
    }
}
