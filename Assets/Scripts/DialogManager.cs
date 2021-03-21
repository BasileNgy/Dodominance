using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 * monologue = liste de string "prononcées" par le NPC
 * monologueList = ensemble des monologues
 * choiceList = liste de choix qui seront proposées au joueur entre 2 monologues
 * dialogue = ensemble de tous les échanges entre le joueur et le npc
 */
public class DialogManager : MonoBehaviour
{
    //Interface
    private GameObject dialogPanel;
    private GameObject choicePanel;
    public CanvasManager canvasManager;
    public GameManager gameManager;

    private HUDText npcNameText;
    private HUDText dialogText;
    private HUDText choice1Text;
    private HUDText choice2Text;

    //Conversation data
    private List<List<string>> monologueList;
    private int monologueListIndex;         //index des monologues
    private List<string> monologue;
    private int monologueIndex;             //index des string à l'intérieur d'un monologue
    private List<string[]> choiceList;
    public bool dialogLaunched;
    public bool choiceEnable;
    public int choiceSelected;
    public int maxSelection = 2;
    public string npcTalking;

    private void Awake()
    {
        dialogPanel = GameObject.Find("Dialogue Box");
        choicePanel = GameObject.Find("Choices Boxs");
        npcNameText = GameObject.Find("NPC Name").GetComponent<HUDText>();
        dialogText = GameObject.Find("Dialog Text").GetComponent<HUDText>();
        choice1Text = GameObject.Find("Choice 1 text").GetComponent<HUDText>();
        choice2Text = GameObject.Find("Choice 2 text").GetComponent<HUDText>();
    }

    private void Start()
    {
        dialogPanel.SetActive(false);
        choicePanel.SetActive(false);
        dialogLaunched = false;
        choiceEnable = false;
        choiceSelected = 0;
    }

    private void Update()
    {
        if (dialogLaunched && !gameManager.gameIsPaused)
        {
            if (choiceEnable)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) && choiceSelected != 1)
                {
                    DecreaseSelected();
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && choiceSelected != maxSelection)
                {
                    IncreaseSelected();
                }
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    ApplyChoice();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return)) NextSentence();

        }
    }

    private void IncreaseSelected()
    {
        choiceSelected = 2;
        choice1Text.myText.color = Color.white;
        choice2Text.myText.color = Color.yellow;
    }

    private void DecreaseSelected()
    {
        choiceSelected = 1;
        choice2Text.myText.color = Color.white;
        choice1Text.myText.color = Color.yellow;

    }

    private void ApplyChoice()
    {
        if(npcTalking == NPCType.Clara.ToString())
        {
            if (choiceSelected == 1)
            {
                NextMonologue();
            }
            else if (choiceSelected == 2)
            {
                StopDialog();
            }

        }
        else if(npcTalking == NPCType.ShopKeeper.ToString())
        {
            if(choiceSelected == 1)
            {
                choiceEnable = false;
                choice1Text.SetMyText(null);
                choice2Text.SetMyText(null);
                choicePanel.SetActive(false);
                dialogPanel.SetActive(false);
                canvasManager.ToggleCanvas(CanvasType.ShopCanvas);

            } else if(choiceSelected == 2)
            {
                ActivateMonologue(2, 0);
                choiceEnable = false;
                choice1Text.SetMyText(null);
                choice2Text.SetMyText(null);
                choicePanel.SetActive(false);
            }
        }

    }

    public void StartDialog(string npcName, List<List<string>> conv, List<string[]> choices)
    {
        npcTalking = npcName;
        npcNameText.SetMyText(npcName);
        monologueList = conv;
        
        choiceList = choices;

        dialogLaunched = true;

        dialogPanel.SetActive(true);
        ActivateMonologue(0, 0);
    }

    public void ResumeDialog(int listIndex, int paragraphIndex)
    {
        dialogPanel.SetActive(true);
        ActivateMonologue(listIndex, paragraphIndex);

    }
    private void ActivateMonologue(int listIndex, int paragrapheIndex)
    {
        monologueListIndex = listIndex;
        monologueIndex = paragrapheIndex;
        monologue = monologueList[monologueListIndex];
        ShowText();
    }

    private void Reset()
    {
        dialogPanel.SetActive(false);
        choicePanel.SetActive(false);

        choice1Text.myText.color = Color.white;
        choice2Text.myText.color = Color.white;

        choiceSelected = 0;

        choiceEnable = false;
        monologueListIndex = 0;
        monologueIndex = 0;
        npcTalking = null;
    }

    public void StopDialog()
    {
        Reset();
        dialogLaunched = false;     
    }
    private void MakeChoice()
    {
        choiceEnable = true;
        choice1Text.SetMyText(choiceList[0][0]);
        choice2Text.SetMyText(choiceList[0][1]);
        choicePanel.SetActive(true);
    }

    private void ShowText()
    {
        StartCoroutine(TypeSentence(monologue[monologueIndex]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        Debug.Log("Npc talking : " + npcTalking);
        string phrase = "";
        dialogText.SetMyText(phrase);
        foreach(char letter in sentence.ToCharArray())
        {
            phrase += letter;
            dialogText.SetMyText(phrase);
            yield return null;
        }
    }

    private void NextSentence()
    {
        StopAllCoroutines();
        if(npcTalking == NPCType.Clara.ToString())
        {
            if (monologueIndex < monologue.Count - 1)
            {
                monologueIndex += 1;
                ActivateMonologue(monologueListIndex, monologueIndex);
            }
            else if (monologueListIndex == monologueList.Count - 1)
            {
                StopDialog();
            }
            else if (monologueListIndex < monologueList.Count - 1)
            {
                MakeChoice();
            }
        } else if (npcTalking == NPCType.ShopKeeper.ToString())
        {
            if (monologueIndex < monologue.Count - 1)
            {
                monologueIndex += 1;
                ActivateMonologue(monologueListIndex, monologueIndex);
            }
            else if ( (monologueListIndex == 1 || monologueListIndex == 2) && monologueIndex == monologue.Count - 1)
            {
                StopDialog();
            }
            else if (monologueListIndex == 0 && monologueIndex == monologue.Count - 1)
            {
                MakeChoice();
            }

        } 
    }

    private void NextMonologue()
    {
        monologueListIndex += 1;

        choiceEnable = false;
        choicePanel.SetActive(false);

        ActivateMonologue(monologueListIndex, 0);
    }

    public bool IsDialogLaunched()
    {
        return dialogLaunched;
    }
}
