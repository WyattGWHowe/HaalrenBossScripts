using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(NavMeshAgent))]
public class HaalrenHealerScript : BaseCharacterClass
{
    
    //AI decs
    private NavMeshAgent agent;
    private Animator anim;
    private HaalrenScript owner;
    private float drainCooldown;
    private int amountToDrain;
    private bool healingHaalren = false;



    [Header("DebugStuff")] 
    public Vector3 destin;
    public bool setup = false;

    public void setUpHealer(Vector3 des, float _drainCooldown, int _amountToDrain, GameObject _owner)
    {
        agent = GetComponent<NavMeshAgent>();
        owner = _owner.GetComponent<HaalrenScript>();
        anim = GetComponent<Animator>();
        drainCooldown = _drainCooldown;
        amountToDrain = _amountToDrain;
        destin = des;
        agent.isStopped = true;
        agent.destination = des;
        Wait(5.0f, () =>
        {
            setup = true;
            agent.isStopped = false;
        });

    }

    private void Update()
    {
        if (!setup) return;
        if (agent.remainingDistance < 3.0f && !healingHaalren)
        {
            agent.isStopped = true;
            healingHaalren = true;
            HealHaalren();
        }

        if (IsDead())
        {
            anim.SetTrigger("Death");
            Destroy(gameObject, 5.0f);
            healingHaalren = false;
            setup = false;
        }
    }

    private void HealHaalren()
    {
        if (!healingHaalren) return;
        int healthChange = amountToDrain < GetCurrentHealth() ? amountToDrain : GetCurrentHealth();
        owner.AddHealth(healthChange);
        TakeHealth(healthChange);
        anim.SetTrigger("Drain");
        Wait(drainCooldown,() => HealHaalren());
    }
    
    public void Wait(float seconds, Action action){
        StartCoroutine(_wait(seconds, action));
    }
    
    IEnumerator _wait(float time, Action callback){
        yield return new WaitForSeconds(time);
        callback();
    }
}
