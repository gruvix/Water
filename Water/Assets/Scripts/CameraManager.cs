using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour 
{
	public Transform targetDefault;
	public Transform target;
	public float smoothing = 5f;
	Vector3 offset;
	private Camera cam;
    private float targetZoom;

	// Use this for initialization
	void Start () {
		offset = transform.position - target.position;
		cam = Camera.main;
		targetZoom = cam.orthographicSize;
	}

	public void SetTarget (Transform trans)
	{
		offset = transform.position - trans.position;
		target.position = trans.position;

	}
    void Update ()
	{
		//if (!Pausa.MenuPause && !Pausa.onEvent)
		//{
		if (target == null)
		{
			target = targetDefault;
		}
		float ScrollWheelChange;
		ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
		targetZoom -= ScrollWheelChange * 2f; // Cantidad de zoom por rueda
		targetZoom = Mathf.Clamp(targetZoom, 1f, 3f); // LIMITES DE LA CAMARA
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 10);
		//}
	}

	// Update is called once per frame
	void LateUpdate () {

		Vector3 targetCamPos = target.position + offset + new Vector3(0,1,-10);
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}