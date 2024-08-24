using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("End Screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endScreen;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject[] menuItems;
    [SerializeField] private Button tryAgainButton;
    

    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;
    public UI_CraftWindow craftWindow;
    public UI_SkillTooltip skillTooltip;
    
    private void Awake()
    {
        fadeScreen.gameObject.SetActive(true);  
    }
    private void Start()
    {
        SwitchTo(inGameUI);
        characterUI.gameObject.SetActive(true);
        skillTreeUI.gameObject.SetActive(true);

        // Ensure tooltip windows are always inactive at start
        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);

        characterUI.gameObject.SetActive(false);
        skillTreeUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (PlayerManager.instance.player.isDead)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TabOrEscapePressed(optionsUI);

        if (Input.GetKeyDown(KeyCode.Tab))
            TabOrEscapePressed(characterUI);

        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);

        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(craftUI);

        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(skillTreeUI);

        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
    }
    public void SwitchTo(GameObject _menu)
    {
        DeactivateAllMenus();

        if (_menu != null)
            _menu.SetActive(true);
    }

    private void DeactivateAllMenus()
    {

        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.GetComponent<UI_FadeScreen>() == null)
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void TabOrEscapePressed(GameObject _menu)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && !inGameUI.activeSelf)
            {
                DeactivateAllMenus();
                CheckForInGameUI();
                return;
            }
        }

        SwitchTo(_menu);
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        if (_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGameUI();
            return;
        }

        SwitchTo(_menu);
    }

    public void CheckForInGameUI()
    {
        for (int i = 0; i < menuItems.Length; i++)
        {
            if (menuItems[i].gameObject.activeSelf)
                return;
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        
        fadeScreen.FadeOut();
        StartCoroutine("EndScreenCoroutine");
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1);

        endScreen.SetActive(true);
    }

    public void RestartGameButton()
    {
        GameManager.instance.RestartScene();
    }
}
