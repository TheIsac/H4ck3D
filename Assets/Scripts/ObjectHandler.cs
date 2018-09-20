﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ObjectHandler : MonoBehaviour {

    public ParticleManager particleManager;
    public Transform raycastOrigin;
    public Transform heldPosition;
    public ForceMode throwForceMode;
    public LayerMask layerMask = -1; 
    private Transform destination;

    public float initialVelocity;
    public float finalVelocity; 
    public float grabDistance;
    public float dropForce;
    public float repelCountdown; 
    public float throwForce;
    [HideInInspector]
    public float timeLeft;
    
    private GameObject heldObject;
    public bool pickedUp; 

	void Start () {
        particleManager = GetComponent<ParticleManager>();

        heldObject = null;
        pickedUp = false;
        timeLeft = repelCountdown;
	}
	
	// Update is called once per frame
	void Update () {

        var controllerEvents = GetComponent<VRTK_ControllerEvents>();

        if(heldObject != null)
        {
           if(heldObject.transform.position != heldPosition.transform.position)
            {
                particleManager.pull.Play();
                pickedUp = true; 
                heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, heldPosition.transform.position, Mathf.Lerp(initialVelocity, finalVelocity, Time.deltaTime));

                timeLeft -= Time.deltaTime;
                if (heldObject.transform.position == heldPosition.transform.position)
                {
                    particleManager.pull.Stop();
                }
            }

           else
            {
                heldObject.transform.position = heldPosition.position;
                 heldObject.transform.rotation = heldPosition.rotation;
            }
        }

        if(Input.GetMouseButtonDown(0) && pickedUp == false)
        {
            PickUpObject(); 
        }
        if(Input.GetMouseButtonDown(1) && (pickedUp))
        {
           if(timeLeft <= 0)
            {
                RepelObject();
            }
        }

        else if(Input.GetMouseButtonDown(0) && pickedUp)
        {
            DropObject();
        }
	}

    public void PickUpObject()
    {
        if(!pickedUp)
        {
            if(heldObject == null)
            {
                RaycastHit hit;

                if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out hit, grabDistance, layerMask) &&
                    hit.transform.gameObject.GetComponent<InteractableObject>().liftable)
                {
                    Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * 1000, Color.yellow, 1000);
                    heldObject = hit.collider.gameObject;
                    heldObject.GetComponent<Rigidbody>().isKinematic = true;
                }
                else
                {
                    Debug.DrawRay(raycastOrigin.position, raycastOrigin.forward * 1000, Color.red, 1000);
                }
            }
        }
    }

    public void RepelObject()
    {
        if (pickedUp)
        {
            particleManager.push.Emit(30);
            Rigidbody body = heldObject.GetComponent<Rigidbody>();
            body.isKinematic = false;
            body.AddForce(throwForce * transform.forward, throwForceMode);
            heldObject = null;
            pickedUp = false;
            timeLeft = repelCountdown;
        }
    }

    public void DropObject()
    {
        if(pickedUp)
        {
            particleManager.drop.Emit(15);
            Rigidbody body = heldObject.GetComponent<Rigidbody>();
            body.isKinematic = false;
            body.AddForce(dropForce * transform.forward, throwForceMode);
            heldObject = null;
            pickedUp = false;
        }
    }
}
