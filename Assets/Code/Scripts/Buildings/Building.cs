using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Structure
{
	using Entity;
	using Visuals;

	public class Building : MonoBehaviour
	{
		[SerializeField] private HitMark hitMarkPrefab;
		public int maxHealth;
		private int health;

		public event UnityAction OnDeath;

		private void Start()
		{
			health = maxHealth;
		}

		public virtual void ProcessEnemyData(EntityAI enemyData)
		{

		}

		public virtual void DealDamage(int damage)
		{
			health -= damage;
			HitMark hitMark = Instantiate(hitMarkPrefab, transform.position, Quaternion.identity);
			hitMark.SetText(damage.ToString());
			if (health <= 0)
			{
				OnDeath?.Invoke();
				Destroy(gameObject);
			}
		}
	}
}