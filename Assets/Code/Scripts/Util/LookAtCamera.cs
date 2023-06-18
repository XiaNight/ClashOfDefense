using UnityEngine;

namespace ClashOfDefense.Game.Util
{
	public class LookAtCamera : MonoBehaviour
	{
		public bool isFlipped = false;
		public bool lockAxis = false;
		public bool cameraRotation = false;
		public bool dynamicSize = false;
		public float scaleMultiplier = 1f;
		private Camera mainCamera;

		private void Awake()
		{
			mainCamera = Camera.main;
		}
		private void Update()
		{
			Vector3 direction;
			if (cameraRotation)
			{
				direction = mainCamera.transform.forward;
			}
			else
			{
				direction = mainCamera.transform.position - transform.position;
			}
			if (lockAxis)
			{
				direction.y = 0;
			}
			if (isFlipped)
			{
				direction = -direction;
			}
			transform.rotation = Quaternion.LookRotation(direction);

			if (dynamicSize)
			{
				transform.localScale = Vector3.one * Vector3.Distance(transform.position, mainCamera.transform.position) * scaleMultiplier;
			}
		}
	}
}
