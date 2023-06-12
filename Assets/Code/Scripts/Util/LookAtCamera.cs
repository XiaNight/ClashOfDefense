using UnityEngine;

namespace ClashOfDefense.Game.Util
{
	public class LookAtCamera : MonoBehaviour
	{
		public bool isFlipped = false;
		public bool lockAxis = false;
		private Camera mainCamera;

		private void Awake()
		{
			mainCamera = Camera.main;
		}
		private void Update()
		{
			Vector3 direction = mainCamera.transform.position - transform.position;
			if (lockAxis)
			{
				direction.y = 0;
			}
			if (isFlipped)
			{
				direction = -direction;
			}
			transform.rotation = Quaternion.LookRotation(direction);
		}
	}
}
