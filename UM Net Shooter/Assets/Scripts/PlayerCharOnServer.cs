using UnityEngine;
using System.Collections;

public class PlayerCharOnServer : MonoBehaviour {
	public RPC_Centr rpcc;
	private Transform tr;
	private Vector3 oldPos,newPos;
	private  float interSpeed,interTime;
	public int idOnServer;
	// Use this for initialization
	void Start () {
		tr = transform ;
		oldPos = tr.position ;
	}
	
	// Update is called once per frame
	void Update () {
		tr.position = Vector3 .MoveTowards (tr.position ,newPos ,interSpeed *Time .deltaTime );
		
		
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
	}
	//-----------------------------
	//		входной урон / лечение
	//-----------------------------
	public void IncomingDamage(float damage){
		//rpcc.
		rpcc.ServerPlayerGetDamage(damage,idOnServer ,tr.position ,0);

	}
	//----------------
	//		DIE
	//----------------
	public void Die(){
		Destroy (gameObject ,0.02f);
	}
}
