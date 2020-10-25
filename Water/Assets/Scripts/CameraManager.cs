using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {
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
		ParticleSystem rain = GetComponent<ParticleSystem>();
	}
    void Update ()
	{
		//if (!Pausa.MenuPause && !Pausa.onEvent)
		//{
			float ScrollWheelChange;
			ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
			targetZoom -= ScrollWheelChange * 2f; // Cantidad de zoom por rueda
			targetZoom = Mathf.Clamp(targetZoom, 1f, 3f); // LIMITES DE LA CAMARA
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 10);
		//}
	}

	// Update is called once per frame
	void LateUpdate () {
		Vector3 targetCamPos = target.position + offset;
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}