using UnityEngine;
using System.Collections;

public class PlaerCharOnClient : MonoBehaviour {
	public RPC_Centr rpcc;
	private Transform tr;
	private Vector3 oldPos,newPos;
	private  float interSpeed,interTime;
	public int idOnServer;
	[SerializeField]
	Animator anim;
	[SerializeField]
	Transform tr_weapon,tr_head,tr_shoot,tr_weaponMesh;
    // 
    public int currentWeapon;
    private GameObject currentWeaponGO;

	void Start () {
        currentWeapon = -1;
		tr = transform ;
		oldPos = tr.position ;
       // ChengWeapon(0);
	}
	
	// Update is called once per frame
	void Update () {
		tr.position = Vector3 .MoveTowards (tr.position ,newPos ,interSpeed *Time .deltaTime );
		anim .SetFloat ("speed",interSpeed);

	}
	public void OnNetworkSynxPos(Vector3 _newPos){
		//tr.position = _newPos ;
		oldPos = tr.position ;
		newPos = _newPos ;
		interSpeed = (Vector3 .Distance (newPos ,oldPos ))/rpcc.netUpdeatTime ;

	}
	public void OnNetworkSynxPos(float rtelo,float rhead){
		//tr.position = _newPos ;

		tr.eulerAngles= new Vector3 (0,rtelo,0);
		tr_weapon .localEulerAngles = new Vector3 (-rhead ,0,0);
        tr_head.localEulerAngles = new Vector3(-rhead, 0, 0);
    }
	//----------------
	//		DIE
	//----------------
	public void Die(){
		Destroy (gameObject ,0.02f);
	}
    //----------------
    //      FX shoot
    //----------------
    public void ShootFX(GameObject fx)
    {
        GameObject _fx = Instantiate(fx, tr_shoot) as GameObject ;
        _fx.transform.localPosition = Vector3.zero;
        _fx.transform.localRotation = Quaternion.identity;
    }
    public void ChengWeapon(int wid)
    {
        if (currentWeapon != wid)
        {
            if(currentWeapon >= 0)
            {
                Destroy(currentWeaponGO);
            }
            currentWeaponGO = Instantiate(rpcc.weapons[wid].goc, tr_weaponMesh) as GameObject;
            currentWeaponGO.transform.localPosition = Vector3.zero;
            currentWeaponGO.transform.localRotation = Quaternion.identity;

        }
    }
}
