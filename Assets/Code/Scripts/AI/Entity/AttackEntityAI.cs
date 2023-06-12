using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Entity
{
	using System.Collections.Generic;
	using Combat;
	using Structure;
	using Helper;
	public class AttackEntityAI : EntityAI
	{
		public Targeting targeting;
		public Transform muzzle;

		private Building targetingBuilding;

		protected override void FixedUpdate()
		{
			base.FixedUpdate();
			if (targetingBuilding != null)
			{
				AttackBuilding();
			}
		}

		public override void ProcessStructureData(IEnumerable<Building> buildings)
		{
			base.ProcessStructureData(buildings);
			float minDistance = float.MaxValue;
			foreach (var building in buildings)
			{
				if (building == null)
				{
					continue;
				}
				float distance = Vector3.Distance(transform.position, building.transform.position);
				if (distance < targeting.range * tileSize && distance < minDistance)
				{
					targetingBuilding = building;
					minDistance = distance;
				}
			}
			if (targetingBuilding != null)
			{
				this.paused = true;
				targetingBuilding.OnDeath += () =>
				{
					this.paused = false;
					targetingBuilding = null;
				};
			}
		}

		private void AttackBuilding()
		{
			if (targeting.TryAttack())
			{
				Projectile projectile = Instantiate(targeting.projectile, muzzle.position, Quaternion.LookRotation(targetingBuilding.transform.position - transform.position));
				projectile.OnHit += (Collider other) =>
				{
					Debug.Log(LayerManager.Structure);
					if (other.gameObject.layer == LayerManager.Structure)
					{
						Building building = other.GetComponentInParent<Building>();
						if (building != null)
						{
							Debug.Log("Hit building");
							building.DealDamage(targeting.damage);
						}
						else
						{
							Debug.Log($"Hit something else, {other.gameObject.name}");
						}
					}
					else
					{
						Debug.Log($"Hit something else then structure, {other.gameObject.layer}");
					}
				};
			}
		}
	}
}