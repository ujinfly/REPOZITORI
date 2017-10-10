using UnityEngine;
using System.Collections;

public class MyChar : MonoBehaviour {
	public RPC_Centr rpcc;
	private Transform tr;
	[SerializeField]private Transform tr_shoot;
	public float netSynxUpdateTime,minimalUpdateDistance;
	private Vector3 oldPos,oldRR;
	public int idOnServer;
	[SerializeField]private Transform tr_cam,tr_veapon;
	//---ray cast
	public LayerMask lm;
	public float rayL;
	public GameObject pricel;
	[SerializeField]LineRenderer line_cam_target,line_weapon_target;
	[SerializeField]private Transform  tr_pricel;
    private WeaponControll weaponC;
    private bool haveWeapon;
    private int currentWeapon;
    
	void Start () {
        currentWeapon = -1;
		tr = transform ;
		oldRR = tr.eulerAngles ;
		oldPos = tr.position ;

		InvokeRepeating ("NetSynx",rpcc.netUpdeatTime,rpcc.netUpdeatTime);
        //tr_pricel = pricel.GetComponent <Transform>();
        OnChengWeapon(0);

    }


	void Update () {
		CameraRayCast();
		tr_veapon .localEulerAngles = tr_cam .localEulerAngles ;
		if(Input .GetButtonDown ("Fire1") && !Input.GetKey(KeyCode.LeftShift) && haveWeapon ){
            //shoot
            if (weaponC.Shoot ()) {
                RayCastShoot();
            }
		}

	}
	void CameraRayCast(){
		RaycastHit hit;
		//float _range = 0;
		Vector3 _dir = tr_cam .TransformDirection(Vector3.forward);
		if (Physics.Raycast(tr_cam.position ,_dir , out hit, rayL ,lm )){
			//distanceToGround = hit.distance;
			//pricel .SetActive (true);
			tr_pricel.position = hit.point ;
			line_cam_target.SetPosition (0,tr_cam .position - new Vector3 (0,0.1f,0));
			line_cam_target.SetPosition (1,hit.point );
		}else {
			//pricel .SetActive (false);
			//tr_pricel.position = hit.point ;
			tr_pricel.position = tr_cam .position +_dir*rayL  ;
			line_cam_target.SetPosition (0,tr_cam .position - new Vector3 (0,0.1f,0));
			line_cam_target.SetPosition (1,tr_pricel.position);
		}
		
	}
	void RayCastShoot(){
        
		RaycastHit hit;
		//float _range = 0;
		tr_shoot .LookAt (tr_pricel );
		Vector3 _dir = tr_shoot .TransformDirection(Vector3.forward);
        //--переменные для рпс
        Vector3 _sh_pos;
        int _sh_bulletId;
        
        //......
        if (Physics.Raycast(tr_shoot.position ,_dir , out hit, rayL*2 ,lm )){
            _sh_pos = tr_shoot.position;
            
            line_weapon_target .SetPosition (0,tr_shoot.position);
			line_weapon_target .SetPosition (1,hit.point);
		}else {
            _sh_pos = tr_shoot.position;

            line_weapon_target .SetPosition (0,tr_shoot.position);
			line_weapon_target .SetPosition (1,tr_shoot.position + _dir * rayL *1.1f);
		}
        //rpc to server
        rpcc.OnPlayerShootClient(_sh_pos, _dir, rayL*1.1f, 0, idOnServer);
            //OnPlayerShootClient(Vector3 _shootPos,Vector3 _dir,float _range ,int shootId,int idos)

    }
    void NetSynx(){
		if( Vector3.Distance(oldPos ,tr.position )>minimalUpdateDistance ){
			oldPos = tr.position ;
			//RPCC
			rpcc.MyCharSynxPos (oldPos);

		}
		if(minimalUpdateDistance < Vector3 .Distance (tr.eulerAngles ,oldRR )){
			rpcc.MyCharSynxRR (tr.eulerAngles.y,tr_veapon .localEulerAngles.x);
			oldRR = tr.eulerAngles ;
		}

	}
    //-----
    //  FX
    //-----
    public void ShootFX(GameObject fx)
    {
        GameObject _fx = Instantiate(fx, tr_shoot) as GameObject;
        _fx.transform.localPosition = Vector3.zero;
        _fx.transform.localRotation = Quaternion.identity;
    }
    //----------
    //      DEAD
    //----------
    public void Dead()
    {
        Destroy(gameObject, 0.01f);

    }
    //--------------------
    //  WEAPON
    //--------------
    //
    public void OnChengWeapon(int newWeaponId)
    {
        if (currentWeapon != newWeaponId)
        {
            if (currentWeapon >=0) {
                weaponC.DestroyW();
            }
            //
            GameObject _w = Instantiate(rpcc.weapons[newWeaponId].gom, tr_veapon)as GameObject ;
            _w.transform.localPosition = Vector3.zero;
            _w.transform.localRotation = Quaternion.identity;
            weaponC = _w.GetComponent<WeaponControll>();
            haveWeapon = (rpcc.weapons[newWeaponId].isWeapon);
            tr_shoot = weaponC.shootPos;
            currentWeapon = newWeaponId;
            weaponC.rpcc = rpcc;
        }


    }
}
