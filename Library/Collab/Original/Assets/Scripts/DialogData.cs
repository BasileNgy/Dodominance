using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class DialogData
{
    private List<string> storyDialog; 
    private List<string> tutoDialog;
    private List<string[]> choiceList;
    private List<List<string>> monologsList;

    public DialogData()
    {
        storyDialog = new List<string>();
        tutoDialog = new List<string>();
        choiceList = new List<string[]>();
        monologsList = new List<List<string>>();

        //monologue sur l'histoire
        storyDialog.Add("Tu es enfin reveillé ? On a pas beaucoup de temps, je vais t'expliquer ce qu'on fait là ");
        storyDialog.Add("On a été kidnappé par un dodo géant, je n'ai pas vraiment compris tout ce qu'il voulait");
        storyDialog.Add("Apparemment il veut détruire l'éspèce humaine avec son armée d'animaux disparus ou en voie d'extinction");
        storyDialog.Add("Tu dois le stopper ! Traverse les différents étages de ce complexe et empêche le de réaliser son plan");
        storyDialog.Add("Avant de partir, as-tu besoin que je t'expliques les modules dont tu disposes ?");

        choiceList.Add(new string[] { "Oui s'il-te-plaît", "Non merci j'y vais" });

        //monologue sur le tuto
        tutoDialog.Add("Tout d'abord, tu peux te déplacer avec ZQSD, viser avec la souris et cliquer pour tirer");
        tutoDialog.Add("Tu peux également interagir avec ton envrionment avec E");
        tutoDialog.Add("En bas à gauche, tu peux voir ta barre de vie et d'armure, évite de mourir je t'en prie");
        tutoDialog.Add("il y a aussi le montant d'argent qur tu possèdes, tu peux récupérer des pièces en tuant des animaux");
        tutoDialog.Add("Ensuite en haut à gauche, il y a une mini-carte, sers-t'en pour te repérer dans ce complexe");
        tutoDialog.Add("En bas à droite, ce sont tes munitions, elles sont illimitées mais pas ton chargeur, fais attention");
        tutoDialog.Add("Enfin, en bas, il y a tes emplacement d'armes, tu peux changer d'arme avec la molette de la souris");
        tutoDialog.Add("C'est bon, je t'ai tout présenté, maintenant dépêche toi !");

        monologsList.Add(storyDialog);
        monologsList.Add(tutoDialog);
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
