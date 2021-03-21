using UnityEngine;


[System.Serializable]
public class Slot
{
    public GameObject slot;
    public HUDText txtSlot;
    public Camera fpsCam;
    public GearController gear;
    public string slotName;
    public bool slotEmpty;

    public Slot(GameObject cSlot, HUDText cTxtSlot, Camera cFpsCam)
    {
        slot = cSlot;
        txtSlot = cTxtSlot;
        fpsCam = cFpsCam;
        slotEmpty = true;
    }

    public void SetName(string gear)
    {
        slotName = gear;
        txtSlot.SetMyText(gear);
    }
}
