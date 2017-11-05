using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinStickEnemy : MonoBehaviour {

    public float speed = 9;
    public TwinStickAcademy academy;
    private Rigidbody rigidBody;

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void FixedUpdate () 
    {
        rigidBody.velocity = (academy.spaceship.transform.position - transform.position).normalized * speed;
	}

    public void Init(TwinStickAcademy a)
    {
        academy = a;
    }
}
