using System.Collections;
// using System.Numerics;



using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public enum DustParticleType {
    Running,
    Landing
}
public class EntityFX : MonoBehaviour
{
    [Header("Pop Up Text")]
    [SerializeField] private GameObject popUpTextPrefab;

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
    [SerializeField] private AudioMixerGroup soundEffectsGroup;
    private AudioSource burningAudio;

    [Header("Hit FX")]
    [SerializeField] private GameObject hitFxPrefab;
    [SerializeField] private GameObject critHitFxPrefab;
    
    [Space]
    [SerializeField] private ParticleSystem runningdDustFx;
    [SerializeField] private ParticleSystem landingDustFx;

    SpriteRenderer sr;

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
        

        burningAudio = gameObject.AddComponent<AudioSource>();
        burningAudio.clip = AudioManager.instance.getSFXAudioSource(SFXSounds.burning).clip;
        burningAudio.outputAudioMixerGroup = soundEffectsGroup;  
    }
    
    public void CreatePopUpText(string _text)
    {
        float randomX = Random.Range(-.5f,.5f);
        float randomY = Random.Range(1, 3);

        Vector3 positionOffset = new Vector3(randomX, randomY);
        GameObject newText = Instantiate(popUpTextPrefab, transform.position + positionOffset, Quaternion.identity, transform);
        
        newText.GetComponent<TextMeshPro>().text = _text;
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
