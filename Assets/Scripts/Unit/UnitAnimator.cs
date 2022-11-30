using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private Transform rifleTransform;
    [SerializeField] private Transform swordTransform;


    private void Awake()
    {
        if (TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveAction_OnStartMoving;
            moveAction.OnStopMoving += MoveAction_OnStopMoving;
        }

        if (TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootAction_OnShoot;
        }

        if (TryGetComponent<SwardAction>(out SwardAction swardAction))
        {
            swardAction.OnSwardActionStart += SwardAction_OnSwardActionStart;
            swardAction.OnSwardActionEnd += SwardAction_OnSwardActionEnd;

        }
    }

    private void Start()
    {
        EquipRifle();
    }


    private void MoveAction_OnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", true);
    }

    private void MoveAction_OnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool("IsWalking", false);
    }

    private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger("Shoot");

        Transform bulletPorjectileTransform = Instantiate(bulletProjectilePrefab, shootPoint.transform.position, Quaternion.identity);
        
        BulletProjectile bulletProjectile = bulletPorjectileTransform.GetComponent<BulletProjectile>();

        Vector3 targetPosition = e.targetUnit.GetWorldPosition();
        targetPosition.y = shootPoint.position.y;
        bulletProjectile.Setup(targetPosition);
    }

    private void SwardAction_OnSwardActionStart(object sender, EventArgs e)
    {
        EquipSward();
        animator.SetTrigger("SwardSlash");
    }

    private void SwardAction_OnSwardActionEnd(object sender, EventArgs e)
    {
        EquipRifle();
    }

    private void EquipSward()
    {
        swordTransform.gameObject.SetActive(true);
        rifleTransform.gameObject.SetActive(false);
    }
    
    private void EquipRifle()
    {
        swordTransform.gameObject.SetActive(false);
        rifleTransform.gameObject.SetActive(true);
    }

    
}
