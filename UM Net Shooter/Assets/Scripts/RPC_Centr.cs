using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class RPC_Centr : MonoBehaviour {
	public PlayersOnServer[] playersOnServer;
	public PlayersOnClient[] playersOnClient;

	//данные игрока--------------------------
	//\\\\/\\\\\\\///_+_+_+_+_+_+_+_+_+__+_+_
	public int myIdOnServer;
	public string myNickName;
	public float myHp,myMaxHP,myXp,myXpToNextLVL;
	[SerializeField]private	GameObject playerFPC;
	[SerializeField]private GameObject otherPlayer;
	private MyChar myChar;
	[SerializeField]private Transform scaler_hp;
	[SerializeField]private Text	t_hp ,t_magazine;
    [SerializeField] private RawImage ri_pricel;
    [SerializeField] private Image i_shkala;
	//данные для сервера---------------------
	//,,+,+,+,+,,+,+,+,+,+,+,+,+,+,+,+,+,+,+,
	private bool s_gameIsStarted;
	[SerializeField]private GameObject playerOnServer;
    [SerializeField]private LayerMask lm;
	//общие данные---------------------------
	//*,*,*,*,,*,,,,,***,,*,*,*,*,,*,,-*-,-*,
	[SerializeField]private  GameObject [] fxs;
	public int port;
	private bool inMenu;
	public string ip;
	public float netUpdeatsPerSecond,netUpdeatTime;

	[SerializeField]private Transform spawnPoint;

	private NetworkView nv;
	[SerializeField]private GameObject menu_start,menu_server,menu_client,menu_picChar,menu_loading,menu_player,camera_main;
    //---FX
    public ShootFxCollection[] shootFxs;
    //          WEAPON
    public Weapon[] weapons;
    //....................................................................................
	void Start () {
		myIdOnServer = -1;
		ip= "127.0.0.1";
		port = 21017;
		netUpdeatTime = 1/netUpdeatsPerSecond ;

		nv = GetComponent <NetworkView>();
		MenuStart();
	}
	

	void Update () {
	
	}
	//--------------------------------------
	// 				МЕНЮ +++++--- 
	//--------------------------------------
	private void MenuStart(){
		menu_start .SetActive (true);
		menu_server .SetActive (false);
		menu_client .SetActive (false);
		menu_picChar .SetActive (false);
		menu_loading .SetActive (false);
		menu_player .SetActive (false);
	}
	private void MenuLoad (){
		menu_start .SetActive (false);
		menu_server .SetActive (false);
		menu_client .SetActive (false);
		menu_picChar .SetActive (false);
		menu_loading .SetActive (true);
		menu_player .SetActive (false);
	}
	private void MenuOnConnected (){
		menu_start .SetActive (false);
		menu_server .SetActive (false);
		menu_client .SetActive (true);
		menu_picChar .SetActive (true);
		menu_loading .SetActive (false);
		menu_player .SetActive (false);
	}
	private void MenuStartServer(){
		menu_start .SetActive (false);
		menu_server .SetActive (true);
		menu_client .SetActive (false);
		menu_picChar .SetActive (false);
		menu_loading .SetActive (false);
		menu_player .SetActive (false);
	}
	private void MenuOnCharCreated(){
		menu_start .SetActive (false);
		menu_server .SetActive (false);
		menu_client .SetActive (false);
		menu_picChar .SetActive (false);
		menu_loading .SetActive (false);
		menu_player .SetActive (true);
		//menu_player
	}
    //--------------------------------------
    //      PLAYER MENU 
    //--------------------------------------
    // настройки при старте / выборе оружия
    //для каждого вида орижия свои надстройки
    public void NewWeaponUpdate(int _weaponId)
    {

    }
    //обновление информации от оружия напрямую
    public void WeaponUpdate(string _magazineText,float _s_magazine,bool _ready)
    {
        t_magazine.text = _magazineText;
        i_shkala.fillAmount = _s_magazine;
        if (_ready)
        {
            i_shkala.color = new Vector4(0,1,0,0.3f);
        }
        else
        {
            i_shkala.color = new Vector4(1, 0, 0, 0.3f);
        }

    }
	//--------------------------------------
	//	до подключения /создание сервера
	//--------------------------------------
	public void Button_CreatServer(){
		//
		MenuLoad ();
		Network.InitializeServer (20,port);
	}
	public void Button_ConnectToServer(){
		MenuLoad ();
		Network .Connect (ip,port);
	}
	public void ChengIp(string _ip){
		ip = _ip ;
	}
	//-------------------------------------
	// неудачное подключение к серверу
	void OnFailedToConnect(NetworkConnectionError error) {
		MenuStart();
	}
	//......................................
	//--------------------------------------
	//		Сервер создан 
	//--------------------------------------
	void OnServerInitialized()
	{
		MenuStartServer();
	}
	//--------------------------------------
	// Игрок Подключился к серверу
	//--------------------------------------
	void OnPlayerConnected(NetworkPlayer _player){
		if(Network .isServer ){
		int _newId = BusyIdOnServer ();
			if(_newId > -1){
		playersOnServer[_newId].вusy = true ;
		playersOnServer[_newId].np = _player ;
		//rpc которое дает разрешение на вход
		nv.RPC ("PlayerConnected",_player ,_newId );
		//rpc reviev all
				RevievAllPlayersForPlayer(_newId);
		}
		}
	}
	[RPC]public void PlayerConnected (int _idOnServer){
		myIdOnServer = _idOnServer;
		MenuOnConnected ();
		//RPC c name

	}
	[RPC]public void PlayerSetName(string newName,int _id){
		playersOnServer[_id].name = newName;
		//RPC for ALL
		//рпс в котором всем сообщается о имени подключившегося игрока
		nv.RPC ("ServerMessegeToAll",RPCMode.All,0,name,_id);
	}
	//--Игрок получает данные об других огроках
	void RevievAllPlayersForPlayer(int idos){

		for (int i = 0;i <22;i++ ){
			if(i != idos && playersOnServer[i].вusy && playersOnServer[i].alive ){
				//rpc
				//CreatOherPlayerChar(int _idos,int _chid,Vector3 _spawnPos,float spawnR)
				nv.RPC ("CreatOherPlayerChar",playersOnServer[idos].np,
				        i,playersOnServer[i].charGoId,
				        playersOnServer[i].pos,
				        playersOnServer[i].r_telo);
				
			}
		}
	}
	//
	//----------------------------------------
	// серверные сообщения		++++++++++++++
	//========================================
	[RPC]public void ServerMessegeToAll(int messegeTipe,string  messegeText,int param){
		switch (messegeTipe){
			case 0:
			//выдаем серое информативное сообщение
			//
			playersOnClient[param].name = messegeText; 
			break ;
		}

	}
	//=========================================


	//----------------------------------------
	// СОЗДАНИЕ ПЕРСОНАЖА
	//----------------------------------------
	public void ButtonPicCharerter(int charId){
		nv.RPC("PlayerCallCreatChar",RPCMode.Server,myIdOnServer ,charId);
	}
	//создание персонажа на сервере
	[RPC]public void PlayerCallCreatChar(int _idos,int _chid ,NetworkMessageInfo nmi){
		if(nmi.sender == playersOnServer[_idos].np && !playersOnServer[_idos].alive && Network.isServer )
		{

			playersOnServer[_idos].alive = true ;
			playersOnServer[_idos].hp = 100;
			//creat char on server
			GameObject _char = Instantiate (playerOnServer,spawnPoint.position ,Quaternion .identity )as GameObject ;
			playersOnServer[_idos].playerChar = _char.GetComponent <PlayerCharOnServer>();
			playersOnServer[_idos].playerChar.rpcc = this ;
			playersOnServer[_idos].pos = spawnPoint.position;
            playersOnServer[_idos].playerChar.idOnServer = _idos;
            //creatChar On CLIENT
            nv.RPC ("CreatMyChar",playersOnServer[_idos].np,_chid,playersOnServer[_idos].pos,spawnPoint.rotation.y);
			//creat Char for ohers players
			for (int i = 0;i <22;i++ ){
				if(i != _idos && playersOnServer[i].вusy ){
					//rpc
					//CreatOherPlayerChar(int _idos,int _chid,Vector3 _spawnPos,float spawnR)
					nv.RPC ("CreatOherPlayerChar",playersOnServer[i].np,_idos,_chid,playersOnServer[_idos].pos,spawnPoint.rotation.y);
                    nv.RPC("HpSynh", playersOnServer[i].np, i, playersOnServer[i].hp);
                }
			}
			//end creat char
		}
	}
	//создание своего персонажа на клиенте
	[RPC]public void CreatMyChar(int _chid,Vector3 _spawnPos,float spawnR){
		GameObject _mch;
		_mch = Instantiate (playerFPC,_spawnPos,Quaternion.identity )as GameObject ;
		_mch .transform .eulerAngles = new Vector3 (0,spawnR,0);
		//---
		myChar = _mch .GetComponent <MyChar>();
		myChar .rpcc = this ;
        myChar.idOnServer = myIdOnServer;
		camera_main .SetActive (false);
		MenuOnCharCreated();
	}
	//создание другого игрока на клиенте
	[RPC]public void CreatOherPlayerChar(int _idos,int _chid,Vector3 _spawnPos,float spawnR){
		GameObject _mch;
		_mch = Instantiate (otherPlayer,_spawnPos,Quaternion.identity )as GameObject ;
		_mch .transform .eulerAngles = new Vector3 (0,spawnR,0);
		//--- PlaerCharOnClient 
		playersOnClient[_idos].playerChar = _mch .GetComponent <PlaerCharOnClient>();
		playersOnClient[_idos].playerChar.rpcc = this ;
		playersOnClient[_idos].pos = _spawnPos;
		playersOnClient[_idos].r_telo = spawnR;
		playersOnClient[_idos].alive = true;
		playersOnClient[_idos].playerGO = _mch ;
		playersOnClient[_idos].playerChar.idOnServer = _idos;
       

    }
	//........................................
	//берем номер свободного места в мкассиве
	//busy - занятый
	int BusyIdOnServer()
	{
		int _id = -1;
		for (int i= 0;i<22;i++){
			if (!playersOnServer[i].вusy){
				_id = i;
				break ;
			}
		}
		return _id ;
	}
	//-----------------------------------------
	//	ПЕРЕМЕЩЕНИЕ ПЕРСОНАЖЕЙ 
	//-----------------------------------------
	//данные с клиента
	//
	public void MyCharSynxPos (Vector3 _pos){
		nv.RPC ("PlayerPosSynxFromClient",RPCMode.Server,_pos ,myIdOnServer);

	}
	public void MyCharSynxRR (float _rtelo,float _rhead){
		nv.RPC ("PlayerRRSynxFromClient",RPCMode.Server,_rtelo ,_rhead ,myIdOnServer);
		
	}
	//данные принимает сервер 
	[RPC]public void PlayerPosSynxFromClient(Vector3 newPos,int idos){
		if(playersOnServer[idos].alive && playersOnServer[idos].вusy){
			playersOnServer[idos].pos = newPos;
			//playersOnServer[idos].r_telo = rtelo;
			//playersOnServer[idos].r_head = rhead;
			//-- Synx OnNetworkSynxPos(newPos,rtelo,rhead);
			playersOnServer[idos].playerChar .OnNetworkSynxPos(newPos);
			//playersOnServer[idos].playerChar .OnNetworkSynxPos(rtelo,rhead);
			//-- Synx to Other pleyers
			nv.RPC ("PlPoSyFrServer",RPCMode.Others,newPos,idos);
		}

	}
	[RPC]public void PlayerRRSynxFromClient(float rtelo,float rhead,int idos){
		if(playersOnServer[idos].alive && playersOnServer[idos].вusy){
			//playersOnServer[idos].pos = newPos;
			playersOnServer[idos].r_telo = rtelo;
			playersOnServer[idos].r_head = rhead;
			//-- Synx OnNetworkSynxPos(newPos,rtelo,rhead);
			//playersOnServer[idos].playerChar .OnNetworkSynxPos(newPos);
			playersOnServer[idos].playerChar .OnNetworkSynxPos(rtelo,rhead);
			//-- Synx to Other pleyers
			nv.RPC ("PlRRSyFrServer",RPCMode.Others,rtelo,rhead,idos);
		}
		
	}

	//данные принимает игрок
	[RPC]public void PlPoSyFrServer(Vector3 newPos,int idos){
		if (myIdOnServer != -1 && myIdOnServer != idos){
			playersOnClient[idos].pos = newPos;
			//playersOnClient[idos].r_telo = rtelo;
			//playersOnClient[idos].r_head = rhead;
			//-----------------------------------
			playersOnClient[idos].playerChar .OnNetworkSynxPos(newPos);
			//playersOnClient[idos].playerChar .OnNetworkSynxPos(rtelo,rhead);
		}
	}
	[RPC]public void PlRRSyFrServer(float rtelo,float rhead,int idos){
		if (myIdOnServer != -1 && myIdOnServer != idos){
			//playersOnClient[idos].pos = newPos;
			playersOnClient[idos].r_telo = rtelo;
			playersOnClient[idos].r_head = rhead;
			//-----------------------------------
			//playersOnClient[idos].playerChar .OnNetworkSynxPos(newPos);
			playersOnClient[idos].playerChar .OnNetworkSynxPos(rtelo,rhead);
		}
	}
	//-------------------------------
	// ИГРОК ВЫХОДИТ С СЕРВЕРА
	//-------------------------------
	void OnPlayerDisconnected(NetworkPlayer nplayer) {
		for(int i= 0 ;i <22;i++){
			if(playersOnServer[i].np == nplayer && playersOnServer[i].вusy ){
				playersOnServer[i].вusy = false  ;
				PlayerDeadOnServer(i);

			}
		}
	}
	//-------------------------------
	//	игрок делает выстрел
	//-------------------------------
	//клиент
	public void OnPlayerShootClient(Vector3 _shootPos,Vector3 _dir,float _range ,int shootId,int idos){
        nv.RPC("PlShonServer",RPCMode.Server, _shootPos, _dir, _range, shootId, idos);

	}
    //rpc SERVER
    [RPC]void PlShonServer(Vector3 _shootPos, Vector3 _dir, float _range, int shootId, int idos)
    {
        
        //проверка на тип снаряда и прочие проверки
        if (shootFxs[shootId].isRay) {
            RaycastHit hit;
            //......
            Debug.Log("PLAYER : " + idos.ToString() + " стреляет из " + shootFxs[shootId].name );
            //......
            //....ОТПРАВЛЯЕМ И СОДАЕМ ФХ НА МЕСТЕ ВЫСТРЕЛА
            //FXSynxW(int idos,int fxid)
            nv.RPC("FXSynxW", RPCMode.All, idos, shootFxs[shootId].fxShoot);
            //....
            if (Physics.Raycast(_shootPos, _dir, out hit, _range, lm))
            {
                
                string _tag = hit.collider.tag;
                if (_tag == "Player")
                {
                    //на игреке на сервере висит скрип который отвечает за урон по себе и автоматом заносит данные.
                    //IncomingDamage(урон + / лечение -);
                    PlayerCharOnServer _pcon = hit.collider.GetComponent<PlayerCharOnServer>();
                    if (_pcon.idOnServer != idos)
                    {
                        _pcon.IncomingDamage(shootFxs[shootId].startDamage);

                        nv.RPC("FXSynxh", RPCMode.All,hit.point , shootFxs[shootId].fxHitPleyer);
                    }
                    //....
                    Debug.Log("PLAYER  : " + idos.ToString() + " in " + _pcon.idOnServer.ToString());
                    //....
                }else
                //если попали не в игрока
                {
                    nv.RPC("FXSynxh", RPCMode.All, hit.point, shootFxs[shootId].fxHit);
                }

            }
            else
            {

                Debug.Log("PLAYER not hit : " + idos.ToString());
            }

        }
    }
    //...............................
    //-------------------------------
    //		Игрок получает урон
    //-------------------------------
    //сервер
    public void ServerPlayerGetDamage(float damage,int idos,Vector3 hitFxPos,int hitFxId ){
		if(playersOnServer[idos].alive ){
			playersOnServer[idos].hp -= damage ;
			//other hp synh
			//hit FX RPC
			for(int i= 0 ;i <22;i++){
				if(playersOnServer[i].вusy ){
					//hp
					nv.RPC ("HpSynh",playersOnServer[i].np ,idos ,playersOnServer[idos].hp);
					nv.RPC ("FXSynxh",playersOnServer[i].np ,hitFxPos,hitFxId);
				}
			}
            if (playersOnServer[idos].hp<=0)
            {
                PlayerDeadOnServer(idos);
            }
		}
	}
	//клиент синхронизация хп
	[RPC]void HpSynh(int idos,float _hp ){
		if(idos == myIdOnServer ){
			t_hp .text = "HP : " + _hp;
			scaler_hp.localScale  = new Vector3 (_hp/100.0f,1,1);
		}
	}
	//-------------------------------
	//		FX 
	//-------------------------------
	//on client RPC
	[RPC]void FXSynxh(Vector3 _pos,int fxid){
		Instantiate (fxs[fxid],_pos,Quaternion.identity );
	}
    //создаем спецэфект в оружие игрока
    [RPC]void FXSynxW(int idos,int fxid)
    {
        if (!Network.isServer ) {
            if (idos == myIdOnServer) {
                myChar.ShootFX(fxs[fxid]);
            } else
            {
                playersOnClient[idos].playerChar.ShootFX(fxs[fxid]);
            }
        }
    }
	//...............................
	//-------------------------------
	//	смерть персонажа
	//-------------------------------
	void PlayerDeadOnServer(int idos){
		if (playersOnServer[idos].alive){
		playersOnServer[idos].alive = false ;
		playersOnServer[idos].playerChar.Die();
		//rpc to clients
			nv.RPC ("PlayerDeadOnServerToClientSynh",RPCMode.Others,idos);
            nv.RPC("FXSynxh", RPCMode.Others, playersOnServer[idos].pos, 3);
        }
	}
    //персонаж умирает на клиенте
	[RPC]public void PlayerDeadOnServerToClientSynh(int idos){
        if (idos != myIdOnServer)
        {
            if (playersOnClient[idos].alive)
            {
                playersOnClient[idos].alive = false;
                playersOnClient[idos].playerChar.Die();
            }
        }else
        {
            OnMyCharDead();
        }
	}
    //смерть моего персонажа
    public void OnMyCharDead()
    {
        myChar.Dead();
        camera_main.SetActive(true);
        MenuOnConnected();
        HpSynh(myIdOnServer, 100);
    }
}
[Serializable]
public struct PlayersOnServer {
	public string name;
	public bool alive,вusy;
	public NetworkPlayer np;
	public Vector3 pos;
	public float r_telo,r_head;
	public float hp;
	public PlayerCharOnServer playerChar;
	public int charGoId;
    public int weaponId;
}
[Serializable]
public struct PlayersOnClient {
	public string name;
	public bool alive;
	public int idOnServer;
	//public NetworkPlayer np;
	public Vector3 pos;
	public float r_telo,r_head;
	public GameObject playerGO;
	public PlaerCharOnClient playerChar;
	public int charGoId;
    public int weaponId;
}
[Serializable]
public struct ShootFxCollection
{
    public string name;
    public int  fxShoot;
    public int  fxHit;
    public int fxHitPleyer;
    public bool isRay;
    public float startDamage;

}
[Serializable]
public struct Weapon
{
    public  string name;
    public GameObject gom,goc;
    public bool isLaser;
    public bool isWeapon;
    
   
    public int shootId;
}