using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    Clara,
    ShopKeeper
}
public class NPC : MonoBehaviour
{
    //NPC dialogue
    public NPCType mType;
    public string npcName;
    public DialogManager dialogManager;
    public GameManager gameManager;
    private DialogData conversation;
    private List<List<string>> npcMonologsList;
    private List<string[]> npcChoiceList;

    //Player variables related
    public float dialogRange;
    public LayerMask whatIsPlayer;
    

    public void Update()
    {
        if (!gameManager.gameIsPaused)
        {
            // Si le joueur n'est plus dans la range du npc, on stoppe la conv
            bool playerInDialogRange = Physics.CheckSphere(transform.position, dialogRange, whatIsPlayer);
            if (!playerInDialogRange && npcName == dialogManager.npcTalking)
            {
                Debug.Log(npcName + " stopped talking");
                dialogManager.StopDialog();
            }
        }

    }

    public void LaunchDialog()
    {
        if(!dialogManager.IsDialogLaunched())
        {
            dialogManager.StartDialog(npcName, npcMonologsList, npcChoiceList);           
        }       
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, dialogRange);
    }

    public void SetNpcConversation(string npcName)
    {
        conversation = new DialogData(npcName);
        npcMonologsList = conversation.GetListDialog();
        npcChoiceList = conversation.GetListChoice();
    }
}
