using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private Transform playerTransform;

	private float playerXPos;
	private float playerYPos;

	// Use this for initialization
	void Start () {
		playerTransform = this.transform;
		
		playerXPos = playerTransform.position.x;
		playerYPos = playerTransform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		playerXPos = playerTransform.position.x;
		playerYPos = playerTransform.position.y;
	}
}
