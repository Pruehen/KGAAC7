using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIAnimator : MonoBehaviour
{
    Animation anim;
    [SerializeField]
    Image image;

    void Awake()
    {
        anim = GetComponent<Animation>();
        image.rectTransform.pivot = new Vector3(0, 0.5f, 0);
        image.rectTransform.anchoredPosition = Vector3.zero;
    }

    void OnEnable()
    {
        anim.Play();
    }

    private void Start()
    {
        Vector2 sizeDelta = image.rectTransform.sizeDelta;
        sizeDelta.x = 170;
        sizeDelta.y = 30;
        image.rectTransform.sizeDelta = sizeDelta;
    }
}
