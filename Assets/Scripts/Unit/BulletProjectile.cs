using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVFX;

    private Vector3 targetPosition;
    
    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 200.0f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPosition;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);

            Instantiate(bulletHitVFX, targetPosition, Quaternion.identity);
        }
    }
}
