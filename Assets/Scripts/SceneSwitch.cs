using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitch : MonoBehaviour
{   
    private void OnTriggerEnter(Collider Col)
    {
        if (Col.CompareTag("Player"))
        Col.transform.position = new Vector3 (0.0f, 40.0f, 0.0f);
    }
}