using UnityEngine.UI;
using UnityEngine;

public class HUDText : MonoBehaviour
{
    public Text myText;

    public void SetMyText(string myTxt)
    {
        myText.text = myTxt;
    }

    public void Hide()
    {
        myText.gameObject.SetActive(false);
    }

    public void Show()
    {
        myText.gameObject.SetActive(true);
    }
}
