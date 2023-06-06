using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace ClashOfDefense.Game.Entity
{
	using Helper;
	using Structure;

	public class EntityAI : MonoBehaviour
	{
		[Header("Movement")]
		[SerializeField] protected float speed;

		[Space]
		[SerializeField] protected Vector2Int[] pathFinded;
		[SerializeField] protected int currentPathIndex;
		[SerializeField] protected bool paused = false;

		public event UnityAction<Vector2Int> OnTreaversedTile;
		public event UnityAction OnPathEnded;

		protected float tileSize;

		protected virtual void FixedUpdate()
		{
			if (!paused)
			{
				FollowPathFind();
			}
		}

		public virtual int Setup(Vector2Int[] path, float tileSize)
		{
			this.tileSize = tileSize;
			pathFinded = path;
			currentPathIndex = 0;
			transform.position = MapPositionTransformer.MapToWorldPosition(pathFinded[currentPathIndex], tileSize);
			return pathFinded.Length;
		}

		public virtual void ProcessStructureData(IEnumerable<Building> buildings) { }

		private void FollowPathFind()
		{
			if (pathFinded != null && pathFinded.Length > 0)
			{
				Vector3 targetPosition = MapPositionTransformer.MapToWorldPosition(pathFinded[currentPathIndex], tileSize);
				Vector3 direction = targetPosition - transform.position;
				float step = speed * Time.deltaTime;
				if (direction.magnitude < step)
				{
					currentPathIndex++;
					if (currentPathIndex >= pathFinded.Length)
					{
						pathFinded = null;
						currentPathIndex = 0;
						paused = true;
						OnPathEnded?.Invoke();
						return;
					}
					OnTreaversedTile?.Invoke(pathFinded[currentPathIndex - 1]);
				}
				else
				{
					transform.position += direction.normalized * step;
				}
			}
		}
	}
}