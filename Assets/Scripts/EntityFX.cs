using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    SpriteRenderer sr;
    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private float flashDuration;
    [SerializeField] private int flashCount;
    private Material originalMat;

    [Header("Ailment Colors")]
    [SerializeField] private Color chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;


    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
    }

    public void MakeTransparent(bool _transparent)
    {
        CanvasGroup slider = GetComponentInChildren<CanvasGroup>();

        if (_transparent)
        {
            sr.color = Color.clear;
            slider.alpha = 0;
        }
        else
        {
            sr.color = Color.white;
            slider.alpha = 1;
        }
    }

    private IEnumerator FlashFX()
    {
        for (int i = 1; i < flashCount; i++)
        {
            sr.material = hitMat;

            yield return new WaitForSeconds(flashDuration);

            sr.material = originalMat;
            yield return new WaitForSeconds(flashDuration);
        }
    }

    private void RedColorBlink()
    {
        if (sr.color != Color.white)
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }

    public void IgniteFxFor(float _seconds)
    {
        InvokeRepeating("IgniteColorFX", 0, .15f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        sr.color = chillColor;
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        InvokeRepeating("ShockColorFX", 0, .15f);
        Invoke("CancelColorChange", _seconds);

    }
    
    private void IgniteColorFX()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }

    private void ShockColorFX()
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }
}
