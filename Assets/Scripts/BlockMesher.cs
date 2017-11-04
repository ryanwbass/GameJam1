using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BlockMesher : MonoBehaviour {

	private Mesh mesh;
	private MeshRenderer rend; 
	private TileCamera tileCamera;

	[SerializeField]
	private float tileSize = 32.0f;

	[SerializeField]
	private float tileScale = 1.0f;

	[SerializeField]
	private int order = 0;

	private int[,] mapData;
	private int tileCount = 0;

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvCoordinates = new List<Vector2>();

	[SerializeField]
	private int textureTileWidth = 1;

	[SerializeField]
	private int textureTileHeight = 1;

	private void ConstructTile( int xi, int yi, int texture )
	{
		Vector3 v0 = new Vector3( xi, yi, 0 );
		Vector3 v1 = new Vector3( xi + 1, yi, 0 );
		Vector3 v2 = new Vector3( xi + 1, yi - 1, 0 );
		Vector3 v3 = new Vector3( xi, yi - 1, 0 );

		vertices.Add(v0);
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);

		int i0 = ( tileCount * 4);
		int i1 = ( tileCount * 4) + 1;
		int i2 = ( tileCount * 4) + 2;
		int i3 = ( tileCount * 4) + 3;

		triangles.Add(i0);
		triangles.Add(i1);
		triangles.Add(i3);

		triangles.Add(i1);
		triangles.Add(i2);
		triangles.Add(i3);

		float textureX = texture % this.textureTileWidth;
		float textureY = Mathf.Floor( texture / this.textureTileHeight );

		float uPerPixel = 1.0f / ( ( this.tileSize + 4.0f ) * (float)this.textureTileWidth );
		float vPerPixel = 1.0f / ( ( this.tileSize + 4.0f ) * (float)this.textureTileHeight );
	
		float uPerTile = this.tileSize * uPerPixel;
		float uPerTilePadded = ( this.tileSize + 4.0f ) * uPerPixel; 

		float vPerTile = this.tileSize * vPerPixel;
		float vPerTilePadded = ( this.tileSize + 4.0f ) * vPerPixel; 

		float left = ( textureX * uPerTilePadded ) + ( 2.0f * uPerPixel);
		float right = left + uPerTile;
		float top = ( textureY * vPerTilePadded ) + ( 2.0f * vPerPixel );
		float bottom = top + vPerTile;

		uvCoordinates.Add( new Vector2( left, bottom ) );
		uvCoordinates.Add( new Vector2( right, bottom ) );
		uvCoordinates.Add( new Vector2( right, top ) );
		uvCoordinates.Add( new Vector2( left, top ) );

		tileCount++;
	}
	
	private void GenerateMesh()
	{
		for( int yi = 0; yi < mapData.GetLength(1); yi++ )
		{
			for( int xi = 0; xi < mapData.GetLength(0); xi++ )
			{
				int tileValue = mapData[xi, yi];
				if( tileValue != 0 ){
					this.ConstructTile( xi, -yi, tileValue );
				}
			}
		}

		mesh.Clear();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();

		mesh.uv = uvCoordinates.ToArray();

		mesh.RecalculateNormals();

		vertices.Clear();
		triangles.Clear();
		uvCoordinates.Clear();
		calculateCameraBounds();

		tileCount = 0;
	}

	private void calculateCameraBounds(){
		tileCamera.movementBoundaryTop = tileCamera.worldOrigin.y;
		tileCamera.movementBoundaryBottom = tileCamera.worldOrigin.y - (mesh.bounds.size.y * this.transform.localScale.y);
		tileCamera.movementBoundaryLeft = tileCamera.worldOrigin.x;
		tileCamera.movementBoundaryRight = tileCamera.worldOrigin.x + (mesh.bounds.size.x * this.transform.localScale.x);
	}

	void Awake()
	{
		this.mesh = this.GetComponent<MeshFilter>().mesh;
		this.rend = this.GetComponent<MeshRenderer>();

		float scale = this.tileSize * this.tileScale;

		this.transform.localScale = new Vector3( scale, scale, 1.0f );
	}

	public void GenerateLayer (int[,] layerData, int layerNumber) {
		this.mapData = layerData;
		order = layerNumber;

		tileCamera = Camera.main.GetComponent<TileCamera>();
		
		this.GenerateMesh();

		this.transform.position = new Vector3( -rend.bounds.size.x/2, mapData.GetLength(1) * tileSize - Camera.main.orthographicSize, -(0.1f * this.order) );	
	}
	
	void Update () {
		
	}
}
