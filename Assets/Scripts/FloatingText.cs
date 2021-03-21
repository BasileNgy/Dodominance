using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    protected Animator takeDamageAnimator;
    private void Awake()
    {
        takeDamageAnimator = gameObject.GetComponentInChildren<Animator>();
        if(takeDamageAnimator == null)
        {
            Debug.Log("Did not find animator");
            return;
        }
        Debug.Log("PlayAnimation " + takeDamageAnimator.name);
        takeDamageAnimator.Play("takeDamageText");
    }
}
