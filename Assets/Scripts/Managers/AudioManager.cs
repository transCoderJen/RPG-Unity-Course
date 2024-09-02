using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum SFXSounds{
    attack1,
    attack2,
    attack3,
    bankai,
    burning,
    checkpoint,
    chronosphere,
    click,
    clockTick,
    clockTick2,
    clock,
    death_screen,
    evilVoice,
    fireMagic,
    footsteps,
    girlSigh2,
    granfatherClock,
    grilSigh,
    itemPickup,
    monster_breathing,
    monster_growl,
    mosnter_growl_1,
    openChest,
    quickTimeEventKey,
    skeleton_bones,
    spell1,
    spell,
    swordThrow2,
    throwSword,
    thunderStrike,
    wind_sounds,
    womanSigh2,
    womanSigh3,
    womanSigh,
    womanStruggle2,
    womanStruggle
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private float sfxMinimumDistance;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;

    public bool playBGM;
    private int bgmIndex;

    private bool canPlaySFX;

    public AudioSource getSFXAudioSource(SFXSounds _sfxSound)
    {
        int index = (int)_sfxSound;

        return sfx[index];
    }
    
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
        
        Invoke("AllowSFX", 1);
    }

    private void Update()
    {
        if(!playBGM)
            StopAllBGM();
        else
        {
            if(!bgm[bgmIndex].isPlaying)
                PlayRandomBGM();
        }
    }

    public void PlaySFX(SFXSounds _sfxSound, Transform _source)
    {
        if (!canPlaySFX)
            return;

        int index = (int)_sfxSound;

        if(sfx[index].isPlaying && (int)SFXSounds.thunderStrike != index)
            return;

        if (_source != null && Vector2.Distance(PlayerManager.instance.player.transform.position, _source.position) > sfxMinimumDistance)
            return;
            
        if (index < sfx.Length)
        {
            sfx[index].pitch = Random.Range(.85f, 1.1f);
            sfx[index].Play();
        }
    }

    public void StopSFX(SFXSounds _sfxSound) 
    {
        int index = (int)_sfxSound;
        sfx[index].Stop();
    }

   public void FadeOutSFX(SFXSounds _sfxSound, float _decreaseBy, float _duration)
    {
        int index = (int)_sfxSound;
        StartCoroutine(FadeOut(sfx[index], _decreaseBy, _duration));
    }

    private IEnumerator FadeOut(AudioSource _audio, float _decreaseBy, float _duration)
    {
        if (_audio == null)
            yield break;

        float defaultVolume = _audio.volume;

        while (_audio.volume > .1f)
        {
            if (_audio == null) // Check if the audio source still exists
                yield break;

            _audio.volume -= _audio.volume * _decreaseBy;
            yield return new WaitForSeconds(_duration);

            if (_audio.volume <= .1f)
            {
                if (_audio != null)
                {
                    _audio.Stop();
                    _audio.volume = defaultVolume;
                }
                break;
            }
        }
    }

    public void PlayNextBGM()
    {
        bgmIndex += 1;
        if (bgmIndex > bgm.Length - 1)
            bgmIndex = 0;
        PlayBGM(bgmIndex);
    }

    public void PlayRandomBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }

    public void PlayBGM(int _index)
    {
        bgmIndex = _index;
        StopAllBGM();

        bgm[bgmIndex].Play();
    }

    public void StopAllBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }

    private void AllowSFX() => canPlaySFX = true;
}
