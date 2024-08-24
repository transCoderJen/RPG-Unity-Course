using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool activated;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint(true);
        }
    }

    public void ActivateCheckpoint(bool withSaving)
    {
        if (withSaving)
        {
            // SaveManager.instance.gameData.lastCheckpointId = id;
            // SaveManager.instance.SaveGame();
        }
        activated = true;
        anim.SetBool("active", true);
    }
}
