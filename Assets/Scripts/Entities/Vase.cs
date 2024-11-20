using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : MonoBehaviour
{
    private void Start()
    {
        transform.position = transform.position + (Vector3)Random.insideUnitCircle*0.1f + Vector3.down*0.2f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int count = Random.Range(3, 6);
        for (int i = 0; i < count; i++)
        {
            Debris.Spawn(DebriType.Vase, transform.position, UnityEngine.Random.insideUnitCircle, 1);
        }
        Destroy(gameObject);
    }
}
