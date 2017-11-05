using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

public class TwinStickAcademy : Academy 
{
    public const float areaWidth = 46f;
    public const float areaHeight = 36f;

    public List<TwinStickEnemy> enemyPrefabs = new List<TwinStickEnemy>();
    public Text debug;

    [HideInInspector]
    public TwinStickAgent spaceship;
    [HideInInspector]
    public List<TwinStickEnemy> enemies = new List<TwinStickEnemy>();
    [HideInInspector]
    public List<Vector3> edgePositions = new List<Vector3>();

    private int enemyWave;

    public override void InitializeAcademy()
    {
        if (Application.isEditor && spaceship.brain.brainType == BrainType.External)
            spaceship.brain.brainType = BrainType.Player;

        spaceship.Init(this);

        edgePositions.Add(new Vector3(-areaWidth, 0, areaHeight));
        edgePositions.Add(new Vector3(-areaWidth, 0, -areaHeight));
        edgePositions.Add(new Vector3(areaWidth, 0, areaHeight));
        edgePositions.Add(new Vector3(areaWidth, 0, -areaHeight));
        episodeCount = 0;
    }

    public override void AcademyReset()
    {
        debug.text = episodeCount.ToString();

        enemyWave = 0;

        foreach (TwinStickEnemy child in enemies)
        {
            Destroy(child.gameObject);
        }
        enemies.Clear();
    }

    public override void AcademyStep()
    {
        if (spaceship.done)
        {
            done = true;
            return;
        }

        if (currentStep % 50 == 1)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Vector3 newPos = Vector3.zero;

        if (enemyWave < 2)
        {
            float minDist = Mathf.Lerp(35, 15, Mathf.Clamp01(episodeCount / 10000f));

            do
            {
                Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));

                if (!dir.Equals(Vector3.zero))
                    newPos = spaceship.transform.position + dir.normalized * minDist;

                newPos = new Vector3(Mathf.Clamp(newPos.x, -areaWidth, areaWidth), 0, Mathf.Clamp(newPos.z, -areaHeight, areaHeight));
            }
            while (Vector3.Distance(newPos, spaceship.transform.position) < minDist + float.MinValue);
        }
        else
        {
            do
            {
                int probaEdge = (int)Mathf.Lerp(100, 2, Mathf.Clamp01(episodeCount / 10000f));

                if (UnityEngine.Random.Range(0,probaEdge) == 0)
                    newPos = RandomPositionInEdge();
                else
                    newPos = RandomPositionInArea();

            }
            while (Vector3.Distance(newPos, spaceship.transform.position) < 18);
        }

        var enemy = Instantiate(enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count())], newPos, Quaternion.identity) as TwinStickEnemy;
        enemy.Init(this);

        enemies.Add(enemy);

        enemyWave++;
    }

    private Vector3 RandomPositionInEdge()
    {
        return edgePositions[UnityEngine.Random.Range(0, edgePositions.Count())];
    }

    public Vector3 RandomPositionInArea()
    {
        return new Vector3(UnityEngine.Random.Range(-areaWidth, areaWidth), 0, UnityEngine.Random.Range(-areaHeight, areaHeight));
    }

    /*public void CollectEnemies(ref List<float> state, int max)
    {
        var nearest = enemies
            .OrderBy(x => Vector3.Distance(x.transform.position, spaceship.transform.position))
            .Take(max);

        foreach (var n in nearest)
        {
            state.Add(1);
            state.Add((spaceship.transform.position.x - n.transform.position.x) / 55f);
            state.Add((spaceship.transform.position.z - n.transform.position.z) / 40f);
        }

        for (int i = 0; i < max - nearest.Count(); i ++)
        {
            state.Add(-1);
            state.Add(-1);
            state.Add(-1);
        }
    }*/
}
