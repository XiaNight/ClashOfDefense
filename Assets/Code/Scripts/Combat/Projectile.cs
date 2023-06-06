using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Combat
{
	public class Projectile : MonoBehaviour
	{
		[SerializeField] private Rigidbody rb;
		public float speed;
		public event UnityAction<Collider> OnHit;

		public void Start()
		{
			rb.velocity = transform.forward * speed;
			Destroy(gameObject, 100f);
		}

		private void OnCollisionEnter(Collision other)
		{
			OnHit?.Invoke(other.collider);
			Destroy(gameObject);
		}
	}
}