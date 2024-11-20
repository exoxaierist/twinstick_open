using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleHandler : MonoBehaviour
{
    [HideInInspector] public static BloodParticleHandler main;

    [SerializeField] private ParticleSystem detailParticle;
    [SerializeField] private ParticleSystem largeParticle;
    [SerializeField] private ParticleSystem detailRemain;
    [SerializeField] private ParticleSystem largeRemain;

    private void Awake()
    {
        SetSingleton();
    }

    public void Emit(Vector2 position, float scale) => Emit(position, scale, Vector2.zero);
    public void Emit(Vector2 position, float scale, Vector2 direction)
    {
        Vector3 rotation = Vector3.zero;
        if(direction.sqrMagnitude > 0)
        {
            rotation.x = -Vector2.SignedAngle(Vector2.right, direction);
            rotation.y = 90;
        }

        ParticleSystem.MainModule main = detailParticle.main;
        main.startSpeedMultiplier = 20 * scale;
        main = largeParticle.main;
        main.startSpeedMultiplier = 15 * scale;

        ParticleSystem.ShapeModule shape = detailParticle.shape;
        shape.rotation = rotation;
        shape.position = position;
        shape = largeParticle.shape;
        shape.rotation = rotation;
        shape.position = position;

        detailParticle.Emit(Mathf.CeilToInt(30 * scale));
        largeParticle.Emit(Mathf.CeilToInt(5 * scale));
    }

    public void Clear()
    {
        detailRemain.Clear();
        largeRemain.Clear();
    }

    private void SetSingleton()
    {
        if (main != null)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
    }
}
