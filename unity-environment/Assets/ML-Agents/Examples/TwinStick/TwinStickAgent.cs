using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TwinStickAgent : Agent
{


    private const int scanRaycastEnemies = 16;
    private const int scanRaycastEnemiesLength = 10;
    private const int scanLargeRaycastEnemies = 16;
    private const int scanLargeRaycastEnemiesLength = 35;
    private const int scanRaycastWall = 8;
    private const int scanRaycastWallLength = 15;

    private float maxSpeed = 26;
    private float speed = 3f;
    private Rigidbody rigidBody;
    private bool kill = false;
    private TwinStickAcademy academy;
    public Transform visual;

    public void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();

        CollectRaycasts(state, (1 << 0), scanRaycastEnemies, scanRaycastEnemiesLength);
        CollectRaycasts(state, (1 << 5), scanLargeRaycastEnemies, scanLargeRaycastEnemiesLength, Mathf.PI / scanLargeRaycastEnemies);
        CollectRaycasts(state, (1 << 8), scanRaycastWall, scanRaycastWallLength);

        return state;
    }

    private void CollectRaycasts(List<float> state, int mask, float raysToShoot, float scanDistance, float angleOffset = 0)
    {
        float angle = angleOffset;
        for (int i = 0; i < raysToShoot; i++)
        {
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / raysToShoot;

            Vector3 dir = new Vector3(x, 0, z);
            RaycastHit hit;
            float result = -1;
            if (Physics.Raycast(transform.position, dir, out hit, scanDistance, mask))
            {
                result = 1 - (hit.distance / scanDistance);
            }

            state.Add(result);
        }
    }

    public void Update()
    {
        if (!kill && rigidBody.velocity.magnitude > 0.1f)
            visual.rotation = Quaternion.LookRotation(rigidBody.velocity);
    }

    public void Init(TwinStickAcademy a)
    {
        academy = a; 
    }

    public override void AgentStep(float[] act)
    {
        if (kill)
        {
            reward = -1;
            done = true;
            return;
        }


        int input = (int)act[0];
        Vector3 newVelocity = rigidBody.velocity;
        if (input == 1) 
        {
            newVelocity = new Vector3(rigidBody.velocity.x + speed, 0, rigidBody.velocity.z);
        }

        if (input == 2)
        {
            newVelocity = new Vector3(rigidBody.velocity.x - speed, 0, rigidBody.velocity.z);
        }

        if (input == 3)
        {
            newVelocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z + speed);
        }

        if (input == 4)
        {
            newVelocity = new Vector3(rigidBody.velocity.x, 0, rigidBody.velocity.z - speed);
        }

        if (newVelocity.magnitude > maxSpeed)
        {
            newVelocity = newVelocity.normalized * maxSpeed;
        }

        rigidBody.velocity = newVelocity;

        reward += 0.1f * GetSpatialAttenuation(transform.position);
    }

    private float GetSpatialAttenuation(Vector3 pos, bool attenuateNearWall = true)
    {
        float attenuation = 1;

        if (attenuateNearWall)
        {
            float borderX = 1 - Mathf.InverseLerp(TwinStickAcademy.areaWidth - 5, TwinStickAcademy.areaWidth, Math.Abs(pos.x));
            float borderZ = 1 - Mathf.InverseLerp(TwinStickAcademy.areaHeight - 5, TwinStickAcademy.areaHeight, Math.Abs(pos.z));
            attenuation = Math.Min(borderX, borderZ);
        }


        foreach (var e in academy.enemies)
        {
            float distance = Vector3.Distance(e.transform.position, pos);

            if (distance < 12)
            {
                attenuation -= 0.1f * (1 - distance / 12f);
            }
        }

        return attenuation;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("killZone"))
            kill = true;
    }

    public override void AgentReset()
    {
        kill = false;

        int proba = (int)Mathf.Lerp(1000, 2, Mathf.Clamp01(academy.episodeCount / 10000f));

        transform.position = (UnityEngine.Random.Range(0, proba) == 0)?academy.RandomPositionInArea():new Vector3(0, 0, 0);
        rigidBody.velocity = Vector3.zero;
        visual.GetComponent<TrailRenderer>().Clear();

    }

    #region Debug functions
    public void OnDrawGizmos()
    {
        if (rigidBody == null)
            return;

        List<float> states = CollectState();


        float angle = 0;
        for (int i = 0; i < scanRaycastEnemies; i++)
        {
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / scanRaycastEnemies;

            Vector3 start = new Vector3(transform.position.x + x * 2, 0, transform.position.z + z * 2);

            Vector3 dir = new Vector3(x, 0, z);
            Vector3 end = transform.position + dir * scanRaycastEnemiesLength * (1 - states[i]);

            if (states[i] > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(start, end);
            }   
            else
            {
                Gizmos.color = new Color(1, 1, 1, 0.1f);
                Gizmos.DrawLine(start, transform.position + dir * scanRaycastEnemiesLength);
            }   
        }

        angle = Mathf.PI / scanLargeRaycastEnemies;
        for (int i = 0; i < scanLargeRaycastEnemies; i++)
        {
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / scanLargeRaycastEnemies;

            Vector3 start = new Vector3(transform.position.x + x * 2, 0, transform.position.z + z * 2);

            Vector3 dir = new Vector3(x, 0, z);
            Vector3 end = transform.position + dir * scanLargeRaycastEnemiesLength * (1 - states[i + scanRaycastEnemies]);

            if (states[i + scanRaycastEnemies] > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(start, end);
            }
            else
            {
                Gizmos.color = new Color(1, 1, 1, 0.1f);
                Gizmos.DrawLine(start, transform.position + dir * scanLargeRaycastEnemiesLength);
            }
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < scanRaycastWall; i++)
        {
            float x = Mathf.Sin(angle);
            float z = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / scanRaycastWall;

            Vector3 start = new Vector3(transform.position.x + x * 2, 0, transform.position.z + z * 2);

            Vector3 dir = new Vector3(x, 0, z);
            Vector3 end = transform.position + dir * scanRaycastWallLength * (1 - states[i + scanRaycastEnemies + scanLargeRaycastEnemies]);

            if (states[i + scanRaycastEnemies + scanLargeRaycastEnemies] > 0)
                Gizmos.DrawLine(start, end);
        }

        for (int x = (int)-TwinStickAcademy.areaWidth; x < TwinStickAcademy.areaWidth; x += 4)
        {
            for (int z = (int)-TwinStickAcademy.areaHeight; z < TwinStickAcademy.areaHeight; z += 4)
            {
                Vector3 start = new Vector3(x, 0, z);
                float attenuation = GetSpatialAttenuation(start, false);

                if (attenuation < 1)
                {
                    Gizmos.color = new Color(1, 0, 0, 1 - attenuation);
                    Gizmos.DrawLine(start, new Vector3(x + 2, 1, z + 2));
                    Gizmos.DrawLine(start, new Vector3(x - 2, 1, z + 2));
                    Gizmos.DrawLine(start, new Vector3(x - 2, 1, z - 2));
                    Gizmos.DrawLine(start, new Vector3(x + 2, 1, z - 2));
                }
            } 
        }

    }
    #endregion

}
