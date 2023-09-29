using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game.Structure
{
	using ClashOfDefense.Game.Helper;
	using Combat;
	using Entity;

	public class AssultBuilding : Building
	{
		protected EntityAI targetEntity;
		protected bool canFire = true;

		public override void ProcessEnemyData(EntityAI enemyData, Vector2Int tilePosition)
		{
			base.ProcessEnemyData(enemyData, tilePosition);
			if (enemyData == null)
			{
				return;
			}
			if (targetEntity == null)
			{
				if (IsInDistance(enemyData))
				{
					SetTarget(enemyData);
				}
			}
		}

		protected virtual void FindNewTarget()
		{
			UnsetTarget();
			foreach (var enemy in GameManager.Instance.spawnedEnemies)
			{
				if (enemy == null)
				{
					continue;
				}
				if (IsInDistance(enemy))
				{
					SetTarget(enemy);
					break;
				}
			}
		}

		protected virtual void SetTarget(EntityAI target)
		{
			if (target == null)
			{
				return;
			}
			targetEntity = target;
			targetEntity.OnTreaversedTile += OnTargetMoved;
		}

		protected virtual void OnTargetMoved(Vector2Int position)
		{
			if (!IsInDistance(targetEntity))
			{
				UnsetTarget();
			}
		}

		protected virtual void UnsetTarget()
		{
			if (targetEntity == null)
			{
				return;
			}
			targetEntity.OnTreaversedTile -= OnTargetMoved;
			targetEntity = null;
		}

		protected bool IsInDistance(EntityAI entity)
		{
			return Vector2Int.Distance(tilePosition, entity.TilePosition) < targeting.range;
		}

		protected override void OnDrawGizmos()
		{
			base.OnDrawGizmos();
			// Draw a red sphere at the transform's position to show the firing range
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, targeting.range * GameManager.Instance.tileSize);

			if (targetEntity != null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position, targetEntity.transform.position);
			}
		}
	}
}