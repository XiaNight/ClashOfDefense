using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Structure
{
	using Entity;
	using Visuals;

	public class Building : MonoBehaviour
	{
		[Header("Building Settings")]
		[SerializeField] protected HitMark hitMarkPrefab;
		[SerializeField] protected Vector3 centralPosition = new Vector3(0, 0.5f, 0);
		public Vector3 CentralPosition { get => transform.position + centralPosition; }
		[SerializeField] protected BuildingData data;
		public BuildingData Data { get => data; }

		[Header("Status")]
		[SerializeField] protected int health;
		[SerializeField] protected int veterancy;
		public Vector2Int tilePosition { get; protected set; }
		protected int currentLevelIndex;

		public event UnityAction OnDestroyed;

		protected virtual void Start()
		{
			health = data.levels[currentLevelIndex].maxHealth;
			GameManager.Instance.OnEnemyTreaversedTile += ProcessEnemyData;
		}

		public virtual void Setup(BuildingData dataInstance, Vector2Int tilePosition)
		{
			data = dataInstance;
			this.tilePosition = tilePosition;
		}
		public virtual void Setup(BuildingData dataInstance)
		{
			data = dataInstance;
		}

		public virtual void SetPosition(Vector2Int tilePosition)
		{
			this.tilePosition = tilePosition;
		}

		public virtual void Reset()
		{
			if (data.regenerateHealth)
			{
				health = data.levels[currentLevelIndex].maxHealth;
			}
		}

		public virtual void ProcessEnemyData(EntityAI enemyData, Vector2Int tilePosition)
		{

		}

		public virtual void DealDamage(int damage)
		{
			health -= damage;
			HitMark hitMark = Instantiate(hitMarkPrefab, transform.position, Quaternion.identity);
			hitMark.SetText(damage.ToString());
			if (health <= 0)
			{
				OnDestroyed?.Invoke();
				Destroy(gameObject);
			}
		}

		public void Destroy()
		{
			OnDestroyed?.Invoke();
			Destroy(gameObject);
		}

		private void OnDestroy()
		{
			GameManager.Instance.OnEnemyTreaversedTile -= ProcessEnemyData;
		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(CentralPosition, "building.png", true);
		}
	}
}