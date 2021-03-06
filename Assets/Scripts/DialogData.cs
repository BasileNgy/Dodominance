using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DialogData
{
    private List<string[]> choiceList;
    private List<List<string>> monologsList;

    public DialogData(string npc)
    {
        choiceList = new List<string[]>();
        monologsList = new List<List<string>>();

        switch (npc)
        {
            case "Clara":
                List<string> storyDialog = new List<string>();
                List<string> tutoDialog = new List<string>();

                //monologue sur l'histoire
                storyDialog.Add("Tu es enfin reveillé ? On n'a pas beaucoup de temps, je vais t'expliquer ce qu'on fait là.");
                storyDialog.Add("On a été kidnappés par un dodo géant. Je n'ai pas vraiment compris tout ce qu'il voulait.");
                storyDialog.Add("Apparemment, il veut détruire l'espèce humaine avec son armée d'animaux disparus ou en voie d'extinction.");
                storyDialog.Add("Tu dois le stopper ! Traverse les différents étages de ce complexe et empêche-le de réaliser son plan.");
                storyDialog.Add("Avant de partir, as-tu besoin que je t'expliques les modules dont tu disposes ?");

                choiceList.Add(new string[] { "Oui s'il-te-plaît", "Non merci j'y vais" });

                //monologue sur le tuto
                tutoDialog.Add("Tout d'abord, tu peux te déplacer avec ZQSD, viser avec la souris et cliquer pour tirer.");
                tutoDialog.Add("Tu peux également interagir avec ton envrionment avec E.");
                tutoDialog.Add("En bas à gauche, tu peux voir ta barre de vie et d'armure. Evite de mourir, je t'en prie.");
                tutoDialog.Add("Il y a aussi le montant d'argent qur tu possèdes. Tu peux récupérer des pièces en tuant des animaux.");
                tutoDialog.Add("Ensuite, en haut à gauche, il y a une mini-carte, sers-t'en pour te repérer dans ce complexe");
                tutoDialog.Add("En bas à droite, ce sont tes munitions. Elles sont illimitées, mais pas ton chargeur. Fais attention!");
                tutoDialog.Add("Enfin, en bas, il y a tes emplacement d'armes. Tu peux changer d'arme avec la molette de la souris.");
                tutoDialog.Add("C'est bon, je t'ai tout présenté. Maintenant dépêche toi !");

                monologsList.Add(storyDialog);
                monologsList.Add(tutoDialog);
                break;

            case "ShopKeeper":
                List<string> greatingsDialog = new List<string>();
                List<string> noShopDialog = new List<string>();
                List<string> purchaseDialog = new List<string>();
                greatingsDialog.Add("Bien le bonjour, aventurier ! Désires-tu consulter ma marchandise ?");
                monologsList.Add(greatingsDialog);
                choiceList.Add(new string[] { "Oui s'il-te-plaît", "Non merci j'y vais" });
                purchaseDialog.Add("C'est un plaisir de faire avec avec toi, Messire. Reviens quand tu veux !");
                monologsList.Add(purchaseDialog);
                noShopDialog.Add("Hors de ma vue, paysan!");
                monologsList.Add(noShopDialog);
                break;

            default:
                Debug.Log("No sentence to load for "+npc);
                break;
        }
    }

    public List<List<string>> GetListDialog()
    {
        return monologsList;
    }

    public List<string[]> GetListChoice()
    {
        return choiceList;
    }
}
