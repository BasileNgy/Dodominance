using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HitboxShopKeeper : MonoBehaviour
{
    private ShopKeeperAI parentScript;
    private Transform parent;

    private void Awake()
    {
        parentScript = gameObject.GetComponentInParent<ShopKeeperAI>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            parentScript.PunchCollision();
        }
    }
}
