using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerFX : EntityFX
{
    [Header("Screen Shake FX")]
    [SerializeField] private float shakeMultiplier;
    public Vector3 lightShakePower;
    public Vector3 heavyShakePower;
    private CinemachineImpulseSource screenShake;

    [Header("After Image FX")]
    // [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float fadeDuration;
    public float afterImageRate;

    Player player;

    protected override void Start()
    {
        base.Start();
        screenShake = GetComponent<CinemachineImpulseSource>();
        player = PlayerManager.instance.player;
    }

    public void ScreenShake(Vector3 _shakePower)
    {
        screenShake.m_DefaultVelocity = new Vector3(_shakePower.x * player.facingDir, _shakePower.y) * shakeMultiplier;
        screenShake.GenerateImpulse();
    }

    public void CreateAfterImageFX(Transform playerTransform)
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
        StartCoroutine(FadeAfterImageFX(newAfterImage));
    }

    IEnumerator FadeAfterImageFX(GameObject _afterImage)
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
}
