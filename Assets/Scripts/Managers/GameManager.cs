using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    [SerializeField] private Checkpoint[] checkpoints;
    private string lastCheckpointId;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostSoulsPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
    
    private void Start()
    {
        checkpoints = FindObjectsByType<Checkpoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    public void RestartScene()
    {  
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void LoadData(GameData _data) => StartCoroutine(LoadWithDelay(_data));

    public void SaveData(ref GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = PlayerManager.instance.player.transform.position.x;
        _data.lostCurrencyY = PlayerManager.instance.player.transform.position.y;
        if(FindLastActiveCheckpoint() != null)
            _data.lastCheckpointId = FindLastActiveCheckpoint().id;
        else
            _data.lastCheckpointId = null;
        _data.checkpoints.Clear();

        foreach(Checkpoint checkpoint in checkpoints)
        {
            _data.checkpoints.Add(checkpoint.id, checkpoint.activated);
        }
    }

    private void LoadCheckpoints(GameData _data)
    {
        foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
        {
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (checkpoint.id == pair.Key && pair.Value)
                {
                    bool withSaving = false;
                    checkpoint.ActivateCheckpoint(withSaving);
                }
            }
        }
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (_data.lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostSoulsPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    private IEnumerator LoadWithDelay(GameData _data)
    {
        yield return new WaitForSeconds(.1f);

        LoadCheckpoints(_data);
        PlacePlayerAtCheckpoint(_data);
        LoadLostCurrency(_data);
    }

    private void PlacePlayerAtCheckpoint(GameData _data)
    {
        if (_data.lastCheckpointId == null)
            return;

        lastCheckpointId = _data.lastCheckpointId;

        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (lastCheckpointId == checkpoint.id)
                PlayerManager.instance.player.transform.position = checkpoint.transform.position;
        }
    }

    private Checkpoint FindLastActiveCheckpoint()
    {
        float furthestXPos = -Mathf.Infinity;
        Checkpoint closestCheckpoint = null;

        foreach(Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.transform.position.x > furthestXPos && checkpoint.activated)
            {
                furthestXPos = checkpoint.transform.position.x;
                closestCheckpoint = checkpoint;
            } 
        }

        return closestCheckpoint;
    }
}
