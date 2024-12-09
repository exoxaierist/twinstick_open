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
        SoundSystem.Play(SoundSystem.MISC_SHATTER.GetRandom(), transform.position, 0.3f);
        for (int i = 0; i < count; i++)
        {
            Debris.Spawn(DebriType.Vase, transform.position, UnityEngine.Random.insideUnitCircle, 1);
        }
        Destroy(gameObject);
    }
}
