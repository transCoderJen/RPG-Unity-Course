using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private SFXSounds soundToPlay;
    private bool isSoundPlaying;

    private void Start()
    {
        // Check if the player is already inside the trigger area at the start of the game
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Player>() != null)
            {
                PlaySound();
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            PlaySound();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            if (AudioManager.instance != null)
                StopSound();
        }
    }

    private void PlaySound()
    {
        if (!isSoundPlaying)
        {
            AudioManager.instance.PlaySFX(soundToPlay, null);
            isSoundPlaying = true;
        }
    }

    private void StopSound()
    {
        if (isSoundPlaying)
        {
            AudioManager.instance.FadeOutSFX(soundToPlay, .1f, .25f);
            isSoundPlaying = false;
        }
    }
}
