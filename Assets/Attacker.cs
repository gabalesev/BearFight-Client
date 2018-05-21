using UnityEngine;
using System.Collections;
using System;

public class Attacker : MonoBehaviour {

    public float attackDistance;
    public float attackRate;

    float lastAttackTime = 0;

    Targeter targeter;

    void Start ()
    {
        targeter = GetComponent<Targeter>();
	}	
	
	void Update () {

        if (!isReadyToAttack())
            return;

        if(isTargetDead())
        {
            targeter.ResetTarget();
            return;
        }

        if (isReadyToAttack() && targeter.IsInRange(attackDistance))
        {
            var targetId = targeter.target.GetComponent<NetworkEntity>().id;

            Debug.Log("attacking " + targeter.target.name + " id: " + targetId);

            Network.Attack(targetId);
            lastAttackTime = Time.time;
        }
	}

    private bool isTargetDead()
    {
        return targeter.target.GetComponent<Hitable>().IsDead;
    }

    bool isReadyToAttack()  {
        return Time.time - lastAttackTime > attackRate && targeter.target;
    }
}
