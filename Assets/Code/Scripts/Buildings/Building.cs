using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Structure
{
	using Entity;
	using Visuals;
	using Base;

	public class Building : MonoBehaviour, IHaveHealth
	{
		[Header("Building Settings")]
		[SerializeField] protected HitMark hitMarkPrefab;
		[SerializeField] protected Vector3 centralPosition = new Vector3(0, 0.5f, 0);
		public Vector3 CentralPosition { get => transform.position + centralPosition; }
		[SerializeField] protected BuildingData data;
		public BuildingData Data { get => data; }

		[field: Header("Status")]
		[SerializeField] public int health { get; protected set; }
		public int Health { get => health; }
		public int MaxHealth { get => data.levels[currentLevelIndex].maxHealth; }
		[SerializeField] protected int veterancy;
		public Vector2Int tilePosition { get; protected set; }
		protected int currentLevelIndex;
		[SerializeField] protected bool isDead = false;
		public bool IsDead { get => isDead; }


		public event UnityAction OnDead;
		public event UnityAction OnDeleted;
		public event UnityAction<int> OnHealthChanged;

		protected virtual void Awake()
		{
			GameManager.Instance.OnGameStateChange += (state) =>
			{
				if (state == GameState.Building)
				{
					Reset();
				}
			};
		}


		protected virtual void Start()
		{
			SetHealth(data.levels[currentLevelIndex].maxHealth);
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
				SetHealth(data.levels[currentLevelIndex].maxHealth);
				if (isDead)
				{
					isDead = false;
					gameObject.SetActive(true);
				}
			}
			else // If cannot regenerate health, and if it is dead, delete it
			{
				if (isDead)
				{
					Delete();
				}
			}
		}

		public virtual void ProcessEnemyData(EntityAI enemyData, Vector2Int tilePosition)
		{

		}

		public virtual void DealDamage(int damage)
		{
			HitMark hitMark = Instantiate(hitMarkPrefab, transform.position, Quaternion.identity);
			hitMark.SetText(damage.ToString());
			ChangeHealth(-damage);
			if (health <= 0)
			{
				health = 0;
				isDead = true;
				OnDead?.Invoke();
				if (data.regenerateHealth)
				{
					gameObject.SetActive(false);
				}
			}
		}

		private void SetHealth(int health)
		{
			this.health = health;
			OnHealthChanged?.Invoke(this.health);
		}

		private void ChangeHealth(int health)
		{
			this.health += health;
			OnHealthChanged?.Invoke(this.health);
		}

		public void Delete()
		{
			OnDeleted?.Invoke();
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