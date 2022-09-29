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

	void Start () {
		offset = transform.position - target.position;
		cam = Camera.main;
		targetZoom = cam.orthographicSize;
	}

	void Update ()
	{
		if (target == null)
		{
			target = targetDefault;
		}
		float ScrollWheelChange;
		ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
		targetZoom -= ScrollWheelChange * 2f; // Cantidad de zoom por rueda
		targetZoom = Mathf.Clamp(targetZoom, 5f, 10f); // LIMITES DE LA CAMARA
		cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * 10);
	}

	void LateUpdate () {

		Vector3 targetCamPos = target.position + offset + new Vector3(0, 0.5f, -10);
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}