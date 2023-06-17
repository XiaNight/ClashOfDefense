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
		public Projectile projectilePrefab;
		public Transform muzzle;

		private Building targetingBuilding;

		private void Awake()
		{
			targeting.OnAttack += SpawnProjectile;
		}

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
				float distance = Vector3.Distance(transform.position, building.CentralPosition);
				if (distance < targeting.range * GameManager.Instance.tileSize && distance < minDistance)
				{
					targetingBuilding = building;
					minDistance = distance;
				}
			}
			if (targetingBuilding != null)
			{
				this.paused = true;
				targetingBuilding.OnDestroyed += () =>
				{
					this.paused = false;
					targetingBuilding = null;
				};
			}
		}

		private void AttackBuilding()
		{
			targeting.TryAttack();
		}

		private void SpawnProjectile()
		{
			Projectile projectile = Instantiate(projectilePrefab, muzzle.position, Quaternion.LookRotation(targetingBuilding.CentralPosition - transform.position));
			projectile.OnHit += (Collider other) =>
			{
				if (other.gameObject.layer == LayerManager.Structure)
				{
					Building building = other.GetComponentInParent<Building>();
					if (building != null)
					{
						building.DealDamage(targeting.damage);
					}
				}
			};
		}
	}
}