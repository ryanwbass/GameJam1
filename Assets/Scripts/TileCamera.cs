using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCamera : MonoBehaviour {

	[SerializeField]
	private float pixelsPerUnit = 1.0f;

	[SerializeField]
	private float tileWidth = 32.0f;

	[SerializeField]
	private float tilesPerSecond = 8.0f;

	private float zoomFactor = 1.0f;
	private float orthographicSize = 1.0f;

	private bool zoomInProgress = false;

	public float movementBoundaryTop = 0.0f;
	public float movementBoundaryBottom = 0.0f;
	public float movementBoundaryLeft = 0.0f;
	public float movementBoundaryRight = 0.0f;

	private Vector3 initialTopLeft = Vector3.zero;
	public Vector3 worldOrigin { get { return this.initialTopLeft; } }

	private Camera gameCamera;

	private void CalculateOrthoSize()
	{
		float scale = this.pixelsPerUnit * this.zoomFactor;

		this.orthographicSize = this.gameCamera.pixelHeight / scale / 2.0f;
	}

	private void HandleInputs()
	{
		this.HandleZoomInput();
		this.HandleMovementInput();
	}

	private void HandleMovementInput()
	{
		float cameraSpeed = this.tileWidth * this.tilesPerSecond;

		Vector3 topLeft = this.gameCamera.ScreenToWorldPoint( new Vector3(0, Screen.height, 0) );
		Vector3 bottomRight = this.gameCamera.ScreenToWorldPoint( new Vector3(Screen.width, 0, 0) );

		float top = topLeft.y;
		float bottom = bottomRight.y;
		float left = topLeft.x;
		float right = bottomRight.x;

		if( Input.GetKey(KeyCode.LeftArrow) && left > this.movementBoundaryLeft )
		{
			this.transform.Translate( Vector3.left * cameraSpeed * Time.deltaTime );
		}

		if( Input.GetKey(KeyCode.RightArrow) && right < this.movementBoundaryRight )
		{
			this.transform.Translate( Vector3.right * cameraSpeed * Time.deltaTime );
		}

		if (Input.GetKey(KeyCode.UpArrow) && top < this.movementBoundaryTop )
		{
			this.transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.DownArrow) && bottom > this.movementBoundaryBottom )
		{
			this.transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime);
		}
	}	
	

	private void HandleZoomInput(){
		if( !this.zoomInProgress)
		{

			bool pageUp = Input.GetKeyDown( "page up" );
			bool pageDown = Input.GetKeyDown( "page down" );
		
			if( pageUp && !pageDown )
			{
				this.zoomFactor += ( this.zoomFactor < 5.0 ) ? 1.0f : 0.0f;
			}
			else if( !pageUp && pageDown )
			{
				this.zoomFactor -= ( this.zoomFactor > 1.0 ) ? 1.0f : 0.0f;
			}

			if( pageUp || pageDown )
			{
				this.Zoom();
			}

		}
	}

	private void Zoom()
	{
		this.zoomInProgress = true;

		this.CalculateOrthoSize();

		this.StartCoroutine(this.AnimatedZoom());
	}

	private IEnumerator AnimatedZoom()
	{
		print("test");
		float start = this.gameCamera.orthographicSize;
		float duration = 0.5f;
		float timer = duration;
		while ( timer > 0.0f )
		{
			float target = this.orthographicSize;
			float progress = 1.0f - ( timer / duration );

			this.gameCamera.orthographicSize = Mathf.Lerp( start, target, progress );

			timer -= Time.deltaTime;
			yield return null;
		}
		this.gameCamera.orthographicSize = orthographicSize;
		
		this.zoomInProgress = false;
	}

	void Awake()
	{
		this.gameCamera = this.GetComponent<Camera>();

		this.CalculateOrthoSize();
			
		this.gameCamera.orthographicSize = this.orthographicSize;

		this.initialTopLeft = this.gameCamera.ScreenToWorldPoint( new Vector3( 0, Screen.height, 0 ) );
	}

	void Update()
	{
		this.HandleInputs();
	}
}
