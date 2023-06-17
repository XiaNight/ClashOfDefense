using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game.Control
{
	using UnityEngine;
	using UnityEngine.Events;

	// Top Down Camera Control
	public class CameraControl : MonoBehaviour
	{
		// Singleton
		private static CameraControl instance;
		public static CameraControl Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<CameraControl>();
				}
				return instance;
			}
		}
		[SerializeField] private float moveSpeed = 10f;
		[SerializeField] private float zoomSpeed = 0.15f;
		[SerializeField] private float rotateSpeed = 10f;
		[SerializeField] private float minZoom = 10f;
		[SerializeField] private float maxZoom = 80f;

		[SerializeField] private Transform cameraTransform;

		[SerializeField] private Vector3 bounds = new Vector3(100, 100, 100);

		private float zoom = 50f;
		private float rotate = 45f;
		private float tilt = 60f;

		public event UnityAction<int, Vector3> onMouseClicked;

		private void Start()
		{
			UpdateCameraTileRotation();
		}

		private void Update()
		{
			Movement();

			if (Input.GetMouseButtonDown(0))
			{
				if (MouseRayPosition(out Vector3 position))
					onMouseClicked?.Invoke(0, position);
			}
			if (Input.GetMouseButtonDown(1))
			{
				if (MouseRayPosition(out Vector3 position))
					onMouseClicked?.Invoke(1, position);
			}
		}

		private bool MouseRayPosition(out Vector3 position)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000))
			{
				position = hit.point;
				return true;
			}
			position = Vector3.zero;
			return false;
		}

		private void Movement()
		{
			if (Input.GetMouseButton(2))
			{
				UpdateCameraTileRotation();
			}

			// Move
			Vector3 move = Vector3.zero;
			move += transform.forward * Input.GetAxis("Vertical");
			move += transform.right * Input.GetAxis("Horizontal");

			transform.position += move * moveSpeed * Time.deltaTime * zoom / minZoom;

			// Zoom
			zoom += zoom * zoomSpeed * -Input.GetAxis("Mouse ScrollWheel");
			zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
			float tiltRad = tilt * Mathf.Deg2Rad;
			cameraTransform.localPosition = new Vector3(0, Mathf.Sin(tiltRad), -Mathf.Cos(tiltRad)) * zoom;
			cameraTransform.LookAt(transform.position + Vector3.up * 2f);
		}

		private void UpdateCameraTileRotation()
		{
			// Tilt
			tilt += Input.GetAxis("Mouse Y") * -rotateSpeed;
			tilt = Mathf.Clamp(tilt, 30, 89f);

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