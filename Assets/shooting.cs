using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class shooting : MonoBehaviour
{
    public GameObject bullet;
    public float shootForce = 300, upwardForce;

    public float timeBetweenShooting = 0.1f, spread=3f, reloadTime=1.5f, timeBetweenShots=0;
    public int magazineSize = 30, bulletsPerTap = 1;
    public bool allowButtonHold = true;
    private int bulletsLeft, bulletsShot;

    private bool firing, readyToFire, reloading;

    public Camera fpsCam;
    public Transform attackPoint;

    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    public bool allowInvoke =true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToFire = true;
    }

    private void Update()
    {
        MyInput();

        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    private void MyInput()
    {
        if (allowButtonHold) firing = Input.GetKey(KeyCode.Mouse0);
        else firing = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) { Reload(); }

        if(readyToFire && firing && !reloading && bulletsLeft <= 0) { Reload(); }

        if(readyToFire && firing && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToFire = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 hitPoint;
        if(Physics.Raycast(ray,out hit))
        {
            hitPoint = hit.point;
        }
        else
        {
            hitPoint = ray.GetPoint(75);  //a random far point from cam
        }

        //guns attack point to hitpoint
        Vector3 directionWithoutSpread = hitPoint - attackPoint.position;

        //calc bullet spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //adding spread to bullet
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Adding force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce,ForceMode.Impulse);
        //below line for grenades
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShoot", timeBetweenShooting);
            allowInvoke = false;
        }

        if(bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShoot()
    {
        readyToFire = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
