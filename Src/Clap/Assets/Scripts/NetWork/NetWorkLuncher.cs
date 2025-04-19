//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;

//public class NetWorkLuncher : MonoBehaviourPunCallbacks
//{
//    void Start()
//    {
//        PhotonNetwork.ConnectUsingSettings();
//    }

//    public override void OnConnectedToMaster()
//    {
//        base.OnConnectedToMaster();
//        PhotonNetwork.JoinOrCreateRoom("Room",new Photon.Realtime.RoomOptions() { MaxPlayers=4},default);

//    }

//    public override void OnJoinedRoom()
//    {
//        base.OnJoinedRoom();
//        PhotonNetwork.Instantiate("1",new Vector3(0,0,0),Quaternion.identity,0);
//    }
//}
