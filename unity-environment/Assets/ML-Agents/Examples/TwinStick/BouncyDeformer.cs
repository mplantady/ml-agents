using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyDeformer : MonoBehaviour
{

#if UNITY_EDITOR
    private float randomStart;

    private void Awake()
    {
        randomStart = Random.Range(-3, 3);
    }

    void Update()
    {
        transform.localScale = new Vector3(1 + 0.1f * Mathf.Cos(randomStart + Time.realtimeSinceStartup * 5), 1 + 0.1f * Mathf.Cos(randomStart + 0.5f + Time.realtimeSinceStartup * 2), 1 + 0.1f * Mathf.Sin(randomStart + Time.realtimeSinceStartup * 5));
    }
#endif

}
