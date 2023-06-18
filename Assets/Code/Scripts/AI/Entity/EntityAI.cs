using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace ClashOfDefense.Game.Entity
{
	using Base;
	using Helper;
	using Structure;

	public class EntityAI : MonoBehaviour
	{
		[Header("Movement")]
		[SerializeField] protected float speed;

		[Space]
		[Header("Path Finding")]
		[SerializeField] protected Vector2Int[] pathFinded;
		[SerializeField] protected int currentPathIndex;
		[SerializeField] protected Vector3 centralPosition = new Vector3(0, 0.5f, 0);

		[Header("Settings")]
		[SerializeField] protected EnemyData enemyData;
		public EnemyData Data { get => enemyData; }
		public Vector3 CentralPosition { get => transform.position + centralPosition; }

		[Header("Status")]
		[SerializeField] protected bool paused = false;
		[SerializeField] protected bool pauseTreaverse = false;
		[SerializeField] protected int health;
		protected Vector2Int tilePosition;
		public Vector2Int TilePosition { get => tilePosition; }


		public event UnityAction<Vector2Int> OnTreaversedTile;
		public event UnityAction OnPathEnded;
		public event UnityAction<EntityAI> OnDeath;

		protected virtual void Awake()
		{
			GameManager.Instance.OnGameStateChange += (state) =>
			{
				switch (state)
				{
					case GameState.Ended:
					case GameState.Paused:
						paused = true;
						break;
					case GameState.Playing:
						paused = false;
						break;
				}
			};
		}

		protected virtual void FixedUpdate()
		{
			if (!paused && !pauseTreaverse)
			{
				FollowPathFind();
			}
		}

		public virtual int Setup(EnemyData enemyData, Vector2Int[] path)
		{
			this.enemyData = enemyData;

			speed = enemyData.speed;
			health = enemyData.health;
			tilePosition = path[0];

			pathFinded = path;
			currentPathIndex = 0;
			transform.position = MapPositionTransformer.MapToWorldPosition(pathFinded[currentPathIndex], GameManager.Instance.tileSize);
			return pathFinded.Length;
		}

		public virtual void ProcessStructureData(IEnumerable<Building> buildings) { }

		private void FollowPathFind()
		{
			if (pathFinded != null && pathFinded.Length > 0)
			{
				Vector3 targetPosition = MapPositionTransformer.MapToWorldPosition(pathFinded[currentPathIndex], GameManager.Instance.tileSize);
				Vector3 direction = targetPosition - transform.position;
				float step = speed * Time.deltaTime;
				if (direction.magnitude < step)
				{
					currentPathIndex++;
					if (currentPathIndex >= pathFinded.Length)
					{
						pathFinded = null;
						currentPathIndex = 0;
						pauseTreaverse = true;
						OnPathEnded?.Invoke();
						return;
					}

					tilePosition = pathFinded[currentPathIndex - 1];
					OnTreaversedTile?.Invoke(tilePosition);
				}
				else
				{
					transform.position += direction.normalized * step;
				}
			}
		}

		/// <summary>
		/// Deals damage to this entity. Returns true if entity is dead.
		/// </summary>
		/// <param name="damage">damage</param>
		/// <returns>true if entity is killed</returns>
		public virtual bool DealDamage(int damage)
		{
			health -= damage;
			if (health <= 0)
			{
				OnDeath?.Invoke(this);
				Destroy(gameObject);
				return true;
			}
			return false;
		}

		protected virtual void OnDrawGizmos()
		{
			Gizmos.DrawIcon(CentralPosition, "turret.png", true);
		}
	}
}