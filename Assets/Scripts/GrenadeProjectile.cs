using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private Transform explosionVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve animationCurve;

    private Action onGrenadeBehaviourComplete;
    public static event EventHandler OnAnyGrenadeExploded;

    private Vector3 targetPosition;
    private Vector3 positionXZ;

    private float totalDistance;

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - positionXZ).normalized;

        float moveSpeed = 15f;
        positionXZ += moveDirection * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;

        float maxHeight = totalDistance / 4f;

        float positionY = animationCurve.Evaluate(distanceNormalized) * maxHeight;

        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);

        float reachedTargetDistance = 0.2f;
        if(Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliders = Physics.OverlapSphere(targetPosition, damageRadius);

            foreach(Collider collider in colliders)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    int damage = 30;
                    targetUnit.Damage(damage);
                }
                else if (collider.TryGetComponent<DestructibleCrate>(out DestructibleCrate crate))
                {
                    crate.Damage();
                }

            }

            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            Instantiate(explosionVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);
            onGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;

        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
