using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class Shoter : MonoBehaviour, IShoter
{
    [HideInInspector] private bool isShootingPossible = true;
    [HideInInspector] private BulletScript bullet;
    [HideInInspector] private Transform bulletSpawner;
    [HideInInspector] private Rigidbody recoilObject;
    [HideInInspector] private float bulletSpeed;
    [HideInInspector] private float reloadingTime;
    [HideInInspector] private float recoilBack;
    [HideInInspector] const float BULLET_SPEED_DEFAULT = 1f;

    public void Initialize(BulletScript _bullet, Transform _bulletSpawner, Rigidbody _recoilObject, 
        float _bulletSpeed, float _reloadingTime, float _recoilBack)
    {
        bullet = _bullet;
        bulletSpawner = _bulletSpawner;
        recoilObject = _recoilObject;
        bulletSpeed = _bulletSpeed;
        reloadingTime = _reloadingTime;
        recoilBack = _recoilBack;
        if (bulletSpeed <= 0) bulletSpeed = BULLET_SPEED_DEFAULT;
    }

    public void TryShot()
    {
        if (!isShootingPossible) return;
        isShootingPossible = false;
        GameObject bulletSpawned = Instantiate(bullet.gameObject, bulletSpawner.position, bulletSpawner.rotation);
        bulletSpawned.GetComponent<Rigidbody>().AddForce(-bulletSpawner.gameObject.transform.up * bulletSpeed);
        Recoil();
        Reloading();
    }

    public void Recoil()
    {
        recoilObject.gameObject.GetComponent<Rigidbody>().AddForce(bulletSpawner.gameObject.transform.up * recoilBack);
    }

    public void Reloading()
    {
        StartCoroutine(ReloadingCoroutine());
    }

    IEnumerator ReloadingCoroutine()
    {
        yield return new WaitForSeconds(reloadingTime);
        isShootingPossible = true;
    }
}