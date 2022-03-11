using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRollingThunderScript : MonoBehaviour
{
    [Header("Camera Shake")] 
    public float maxDistanceForShake;
    public float shakeDuration;
    public float shakeAmount;
    
    public float explosionShakeDuration;
    public float explosionShakeAmount;
    
    [Header("dmg variables")]
    public bool damageOverTime;
    
    private bool canDamage = true;
    public bool isActive = false;
    
    public GameObject warningParticles;
    public GameObject dangeroursParticles;

    public float waitTime;
    public int damage;

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;
        if (other.gameObject.CompareTag("Player") && canDamage)
        {
            canDamage = false;
            other.gameObject.GetComponent<BaseCharacterClass>().TakeHealth(damage);
            if(damageOverTime) Wait(waitTime, () => { canDamage = true;});
        }
    }
    public void Wait(float seconds, Action action){
        StartCoroutine(_wait(seconds, action));
    }
    
    IEnumerator _wait(float time, Action callback){
        yield return new WaitForSeconds(time);
        callback();
    }

    public virtual void SetWarningActive(bool x)
    {
        if (x)
        {
            float dis = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
            float explShadeAmount = Mathf.Lerp(0, shakeAmount, 1 - dis/maxDistanceForShake);
            float explShakeDuration = Mathf.Lerp(0, shakeAmount, 1 - dis/maxDistanceForShake);
            
            CameraShake.Shake(explShakeDuration, explShadeAmount);
        }
        warningParticles.SetActive(x);
    }
    public virtual void SetDangerActive(bool x)
    {
        
        if (x)
        {
            float dis = Vector3.Distance(GameObject.FindWithTag("Player").transform.position, transform.position);
            float explShadeAmount = Mathf.Lerp(0, explosionShakeAmount, 1 - dis/maxDistanceForShake);
            float explShakeDuration = Mathf.Lerp(0, explosionShakeDuration, 1 - dis/maxDistanceForShake);
            
            CameraShake.Shake(explShakeDuration, explShadeAmount);
        }
        isActive = x;
        canDamage = x;
        dangeroursParticles.SetActive(x);
    }
}
