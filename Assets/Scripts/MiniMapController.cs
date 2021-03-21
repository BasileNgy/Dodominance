using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{

    public Transform player;
    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y += 10;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);

    }
}
