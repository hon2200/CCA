//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;

//public class testmove : MonoBehaviourPun
//{
//    public Transform transform;
//    void Start()
//    {
        
//    }

//    public float speed = 5f;
//    void Update()
//    {
//        if (PhotonNetwork.IsConnected&&!photonView.IsMine) return;
//        float moveX = Input.GetAxis("Horizontal");
//        float moveZ = Input.GetAxis("Vertical");
//        Vector3 moveDir = new Vector3(moveX, 0,moveZ).normalized;
//        transform.Translate(moveDir * speed * Time.deltaTime);
//    }
//}
