using UnityEngine;


[System.Serializable]
public class Arsenal
{

    public string myName;
    public PlayerController player;
    public float effect;
    public float totalEffect;
    public float initialEffect;

    public Arsenal(string cName, PlayerController cPlayer, float cEffect, float cInitialEffect)
    {
        myName = cName;
        player = cPlayer;
        effect = cEffect;
        initialEffect = cInitialEffect;
        totalEffect = initialEffect;
    }
}
