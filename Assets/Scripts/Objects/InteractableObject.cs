﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {

    VRTK.VRTK_InteractableObject vrtkInteractable;

    public Matter matter;

    DestroyGlass destroyGlass;
    Manipulatable manipulatable;

    public Material redGlass;
    public Material redMetal;
    public Material redDefault;

    Collider objectCollider;
    Rigidbody objectRigidbody;
    Transform objectTransform;
    private Renderer myRenderer;
    public GameObject thisGameObject;

    public bool liftable;
    public float mass;
    public float boxVolume;
    public const float maximumLiftWeight = 1;

    public float force;
    public float highestVelocity;
    float acceleration;

    private void Awake()
    {
        objectTransform = GetComponent<Transform>();
        objectCollider = GetComponent<BoxCollider>();
        objectRigidbody = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();

        myRenderer.material = matter.matterMaterial;
    }

    void Start()
    {
        UpdateProperties();
    }

    void FixedUpdate()
    {
        highestVelocity = Mathf.Max(Mathf.Abs(objectRigidbody.velocity.x), Mathf.Abs(objectRigidbody.velocity.y), Mathf.Abs(objectRigidbody.velocity.z));
        acceleration = highestVelocity / Time.fixedDeltaTime;
        force = mass * acceleration;
    }

    public void UpdateMatter(Matter matter)
    {
        this.matter = matter;
        UpdateProperties();
    }

    void UpdateProperties()
    {
        objectTransform = GetComponent<Transform>();
        objectCollider = GetComponent<BoxCollider>();
        objectRigidbody = GetComponent<Rigidbody>();

        objectCollider.material = matter.physicMaterial;
        MassCalculation();

        if (mass > maximumLiftWeight)
        {
            liftable = false;
            if (boxVolume >= 1f)
            {
                objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }
        else
        {
            liftable = true;
        }
        
        if (!matter.isPhysical)
        {
            liftable = false;

            if (matter.name=="Hologram")
            {
                objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                objectCollider.enabled = false;
            }
            else
            {
                objectRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                objectCollider.enabled = !objectCollider.enabled;
                objectCollider.isTrigger = false;
            }
        }

        //Add if() and check if object with the matter is NOT liftable, then use a public material(red glass, red default, red metal) saved in this script
        //if (matter.name=="Glass" && !liftable)
        //{
        //    myRenderer.material = redGlass;
        //}
        //else if (matter.name == "Default" && !liftable)
        //{
        //    myRenderer.material = redDefault;
        //}
        //else if (matter.name == "Metal" && !liftable)
        //{
        //    myRenderer.material = redMetal;
        //}
        //else
        //{
        //    myRenderer.material = matter.matterMaterial;
        //}
        myRenderer.material = matter.matterMaterial;

    }

    void MassCalculation()
    {
        boxVolume = objectTransform.localScale.x * objectTransform.localScale.y * objectTransform.localScale.z;
        mass = matter.density * boxVolume;

        objectRigidbody.mass = mass;
    }
}
