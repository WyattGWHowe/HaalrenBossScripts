using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLazerScript : MonoBehaviour
{
    public LineRenderer _lineRenderer;
    public LineRenderer _lineRendererTwo;
    public LayerMask mask; //Used to only hit player
    public LayerMask playerMask;
    public GameObject lazerEnd;
    public GameObject lazerTrail;
    public GameObject lazerStart;

    private float hitCooldownMax = 0.1f; //Set higher for there to be greater time between hits
    private float hitCooldownTimer = 0.0f;

    public float maxDis;
    // Start is called before the first frame update
    void Start()
    {
        lazerEnd = Instantiate(lazerEnd);
        lazerStart = Instantiate(lazerStart);
        
    }

    // Update is called once per frame
    void Update()
    {
        //Update hitCooldownTimer to see if player can be hit
        if (hitCooldownTimer > 0)
        {
            hitCooldownTimer -= Time.deltaTime;
        }
        
        //Debug.DrawRay(transform.position, transform.forward * 100,Color.cyan);

        //Shoot a sphere cast out to see if the lazer collides with the player or object
        if (Physics.SphereCast(transform.position, 1.0f,transform.forward, out var hit, maxDis, mask))
        {
            if (hit.transform.gameObject.CompareTag("Player") && hitCooldownTimer <= 0)
            {
                hit.transform.gameObject.GetComponent<BaseCharacterClass>().TakeHealth(4);
                hitCooldownTimer = hitCooldownMax;
            }
            
            //Now set the lazer to where it hit the player or object, and make the lazer look in that direction also
            float dis = Vector3.Distance(transform.position, hit.point);
            Vector3 newPos = new Vector3(0,0,dis);
            _lineRenderer.SetPosition(0, transform.localPosition);
            _lineRenderer.SetPosition(1, newPos );
            _lineRendererTwo.SetPosition(0, transform.localPosition);
            _lineRendererTwo.SetPosition(1, newPos );
            lazerEnd.transform.position = hit.point;
            lazerEnd.transform.rotation = Quaternion.LookRotation(-hit.normal);
            lazerStart.transform.position = _lineRenderer.GetPosition(0) + transform.position;
            lazerStart.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
            lazerTrail.transform.position = hit.point;
            lazerTrail.transform.rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);

        }

        //Adds more height to the spherecast for hitting the player. More consistent on the collisions but sometime messes up.
        //Needs another look at down the line
        //if (Physics.SphereCast(transform.position + new Vector3(0,0.5f,0), 1.0f, transform.forward, out var hitPlayer, maxDis,playerMask))
        //{
        //    print(hitPlayer.collider.gameObject.name);
        //    if (hitPlayer.transform.gameObject.CompareTag("Player") && hitCooldownTimer <= 0)
        //    {
        //        hitPlayer.transform.gameObject.GetComponent<BaseCharacterClass>().TakeHealth(4);
        //        hitCooldownTimer = hitCooldownMax;
        //    }
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<BaseCharacterClass>().TakeHealth(20);
        }
    }
}
