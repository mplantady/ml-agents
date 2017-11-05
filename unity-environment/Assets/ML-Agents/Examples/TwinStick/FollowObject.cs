using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {

    public Transform target;
    public float speed = 0.1f;

	void Update () {
        transform.position = Vector3.Lerp(transform.position, target.position + Vector3.up * transform.position.y, Time.deltaTime * speed);
	}
}
