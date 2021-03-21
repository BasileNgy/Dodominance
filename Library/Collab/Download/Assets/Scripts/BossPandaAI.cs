using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPandaAI : EnemyStats
{
    //Attack
    private bool alreadyAttackedFireBall;
    private bool alreadyAttackedZone;

    private GameObject projectile;
    private Transform attackPoint;
    private Transform zonePoint;
    private float upForce;
    private float forwardForce;

    private float fireballCoolDown;
    private float zo