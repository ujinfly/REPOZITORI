using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponControll : MonoBehaviour {
    public string name;
    public int weaponId;
    public Transform shootPos;
    public bool readyToShoot,isLazer;
    public int magazineSize, magazine, shootCoast , criticalLazerMagazine;
    public float shootRate, reloadTime;
    public int fxShoot;
    private float  _reloadTimer;
    public RPC_Centr rpcc;
	// Use this for initialization
	void Start () {
        _reloadTimer = reloadTime;
        InfoUpdate();

    }
	
	// Update is called once per frame
	void Update () {
         if (isLazer )
            {
            if (!readyToShoot || magazine <magazineSize && readyToShoot) {
                LazerUpdate();
            }
            }
   	}
    void LazerUpdate()
    {
        if(magazine <magazineSize)
        {
            if(_reloadTimer <= 0)
            {
                magazine++;
                _reloadTimer = reloadTime ;
                InfoUpdate();
            }
            else
            {
                _reloadTimer -= Time.deltaTime;
            }

        }else
        {
            magazine = magazineSize;
            readyToShoot = true;
            InfoUpdate();
        }

    }
    public bool  Shoot()
    {
        bool _b = false;
        if (isLazer && readyToShoot && magazine >= shootCoast )
        {
            magazine -= shootCoast;
            if (magazine <criticalLazerMagazine)
            {
                readyToShoot = false;
            }
            _b = true;
        }
        InfoUpdate();
        return _b;
    }
    //
    public void DestroyW()
    {
        Destroy(gameObject, 0.01f);
    }
    //-----------------------------------
    //      INfoShow
    //-----------------------------------
    void InfoUpdate()
    {
        if (isLazer)
        {
            float _m = magazine;
            float _ms = magazineSize;
            float _pr =  _m/_ms;
            string _s = _pr*100 +" %";
            rpcc.WeaponUpdate(_s, _pr, readyToShoot);
        }
    }
}
