using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Structure
{
	using Entity;

	public class Building : MonoBehaviour
	{
		public int maxHealth;
		private int health;

		public event UnityAction OnDeath;

		public virtual void ProcessEnemyData(EntityAI enemyData)
		{

		}

		public virtual void DealDamage(int damage)
		{
			health -= damage;
			if (health <= 0)
			{
				OnDeath?.Invoke();
				Destroy(gameObject);
			}
		}
	}
}