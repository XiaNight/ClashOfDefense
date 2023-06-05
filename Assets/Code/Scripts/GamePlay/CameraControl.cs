using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game.Control
{
	using UnityEngine;

	// Top Down Camera Control
	public class CameraControl : MonoBehaviour
	{
		[SerializeField] private float moveSpeed = 10f;
		[SerializeField] private float zoomSpeed = 10f;
		[SerializeField] private float rotateSpeed = 10f;
		[SerializeField] private float minZoom = 10f;
		[SerializeField] private float maxZoom = 80f;

		[SerializeField] private Transform cameraTransform;

		[SerializeField] private Vector3 bounds = new Vector3(100, 100, 100);

		private float zoom = 50f;
		private float rotate = 45f;
		private float tilt = 60f;

		private void Update()
		{
			// Move
			Vector3 move = Vector3.zero;
			move += transform.forward * Input.GetAxis("Vertical");
			move += transform.right * Input.GetAxis("Horizontal");

			transform.position += move * moveSpeed * Time.deltaTime * zoom / minZoom;

			// Tilt
			tilt += Input.GetAxis("Mouse Y") * -rotateSpeed;
			tilt = Mathf.Clamp(tilt, 30, 89f);

			// Zoom
			zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
			zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
			float tiltRad = tilt * Mathf.Deg2Rad;
			cameraTransform.localPosition = new Vector3(0, Mathf.Sin(tiltRad), -Mathf.Cos(tiltRad)) * zoom;
			cameraTransform.LookAt(transform.position + Vector3.up * 2f);

			// Rotate
			rotate += Input.GetAxis("Mouse X") * rotateSpeed;
			transform.rotation = Quaternion.Euler(0, rotate, 0);
		}

		public void SetBounds(Vector3 bounds)
		{
			this.bounds = bounds;
		}
	}
}