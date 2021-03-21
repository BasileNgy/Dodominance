using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLoadingLogo : MonoBehaviour
{
    private RectTransform crossImage;
    private float speedRotation = 0;

    void Awake()
    {
        crossImage = gameObject.GetComponent<RectTransform>();

        speedRotation = 150;
    }


    // Update is called once per frame
    void Update()
    {
        //Rotation
        //crossImage.rotation.SetEulerRotation(0, Time.deltaTime, 0);
        //crossImage.rotation.eulerAngles = 
        crossImage.Rotate(new Vector3(0,0,1) * -speedRotation * Time.deltaTime);
    }
}
