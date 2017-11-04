using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameObject[] prefabs;
	public GameObject playerPrefab;
	private GameObject[] layers;
	public int mapWidth;
	public int mapHeight;
	public int numberOfLayers;

	private int[][,] layerData;

	private void InstantiateLayers(){
		layers = new GameObject[prefabs.Length];
		for( int i = 0; i < prefabs.Length; i++){
			layers[i] = Instantiate( prefabs[i], new Vector3(0, 0, i), Quaternion.identity ) as GameObject;
			UpdateLayerData(i);
		}
		
	}

	private void UpdateLayerData(int layer){
		layers[layer].GetComponent<BlockMesher>().GenerateLayer(layerData[layer], layer);
	}

	private void SetLayerData(){

		layerData = new int[numberOfLayers][,];

		for( int zi = 0; zi < layerData.Length; zi++ ){
			layerData[zi] = new int[mapWidth, mapHeight];
			for( int yi = 0; yi < layerData[0].GetLength(1); yi++ ){
				for( int xi = 0; xi < layerData[0].GetLength(0); xi++ ){
					if( zi == 0 ){
						if( yi > layerData[0].GetLength(1) - 2 )
						{
							layerData[zi][xi, yi] = 1;
						}
					}
					else
					{
						layerData[zi][xi, yi] = 0;
					}
				}
			}
		}
	}

	void Start () {

		SetLayerData();

		InstantiateLayers();
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
