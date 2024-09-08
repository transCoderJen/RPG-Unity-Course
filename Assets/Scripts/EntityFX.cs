using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public enum DustParticleType {
    Running,
    Landing
}
public class EntityFX : MonoBehaviour
{
    SpriteRenderer sr;

    [Header("AFter Image FX")]
    // [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float fadeDuration;
    public float afterImageRate;

    [Header("Flash FX")]
    [SerializeField] private Material hitMat;
    [SerializeField] private float flashDuration;
    [SerializeField] private int flashCount;
    private Material originalMat;

    [Header("Ailment Colors")]
    [SerializeField] private Color chillColor;
    [SerializeField] private Color[] igniteColor;
    [SerializeField] private Color[] shockColor;

    [Header("Ailment Particles")]
    [SerializeField] private ParticleSystem igniteFx;
    [SerializeField] private ParticleSystem chillFx;
    [SerializeField] private ParticleSystem shockFx;

    [Header("Ailment Audio")]
    private AudioSource burningAudio;
    public AudioMixerGroup soundEffectsGroup;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private GameObject critHitFxPrefab;
    
    [Space]
    [SerializeField] private ParticleSystem runningdDustFx;
    [SerializeField] private ParticleSystem landingDustFx;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;

        burningAudio = gameObject.AddComponent<AudioSource>();
        burningAudio.clip = AudioManager.instance.getSFXAudioSource(SFXSounds.burning).clip;
        burningAudio.outputAudioMixerGroup = soundEffectsGroup;
    }

    public void createAfterImageFX(Transform playerTransform)
    {
        // Create a new GameObject to hold the afterimage
        GameObject newAfterImage = new GameObject("AfterImage");

        // Set the new GameObject's position and rotation to match the player
        newAfterImage.transform.position = playerTransform.position;
        newAfterImage.transform.rotation = playerTransform.rotation;

        // Add a SpriteRenderer component to the new GameObject
        SpriteRenderer afterImageRenderer = newAfterImage.AddComponent<SpriteRenderer>();

        // Get the SpriteRenderer from the player
        SpriteRenderer playerSpriteRenderer = playerTransform.GetComponentInChildren<SpriteRenderer>();

        // Copy the sprite and other properties from the player's SpriteRenderer
        afterImageRenderer.sprite = playerSpriteRenderer.sprite;
        afterImageRenderer.color = playerSpriteRenderer.color;
        afterImageRenderer.flipX = playerSpriteRenderer.flipX;
        afterImageRenderer.flipY = playerSpriteRenderer.flipY;
        afterImageRenderer.sortingLayerID = playerSpriteRenderer.sortingLayerID;
        afterImageRenderer.sortingOrder = playerSpriteRenderer.sortingOrder;

        // Start the fade coroutine
        StartCoroutine(fadeAfterImageFX(newAfterImage));
    }

    IEnumerator fadeAfterImageFX(GameObject _afterImage)
    {
        SpriteRenderer spriteRenderer = _afterImage.GetComponent<SpriteRenderer>();
        Color startColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Ensure the alpha is set to 0 after the loop
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(_afterImage);
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
        igniteFx.Play();
        burningAudio.Play();
        InvokeRepeating("IgniteColorFX", 0, .15f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFxFor(float _seconds)
    {
        chillFx.Play();
        sr.color = chillColor;
        Invoke("CancelColorChange", _seconds);
    }

    public void ShockFxFor(float _seconds)
    {
        shockFx.Play();
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
        StopAllParticleFx();
        StopAllFxAudio();
    }

    public void StopAllFxAudio()
    {
        burningAudio.Stop();
    }

    private void StopAllParticleFx()
    {
        igniteFx.Stop();
        chillFx.Stop();
        shockFx.Stop();
    }

    public void CreateHitFx(Transform _target)
    {
        float zRotation = Random.Range(-90, 90);
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        GameObject newHitFx = Instantiate(hitFxPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity);

        newHitFx.transform.Rotate(new Vector3(0, 0, zRotation));

        Destroy(newHitFx, .5f);
    }

    public void CreateCritHitFx(Transform _target, int _facingDir)
    {
        float xRotation = Random.Range(0, 60);
        float zRotation = Random.Range(-30, 30);
        float yRotation;

        if (_facingDir == -1)
            yRotation = 180;
        else
            yRotation = 0;

        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        GameObject newCritHitFx = Instantiate(critHitFxPrefab, _target.position + new Vector3(xPosition, yPosition), Quaternion.identity);

        newCritHitFx.transform.Rotate(new Vector3(xRotation, yRotation, zRotation));

        Destroy(newCritHitFx, .5f);
    }

    public void CreateDustParticles(DustParticleType _particleType)
    {
        switch (_particleType)
        {
            case DustParticleType.Running:
                if (!runningdDustFx.isPlaying)
                    runningdDustFx.Play();        
                return;
            
            case DustParticleType.Landing:
                if (!landingDustFx.isPlaying)
                    landingDustFx.Play();        
                return;
        }
    }
}
