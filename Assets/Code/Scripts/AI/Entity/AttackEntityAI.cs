using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Entity
{
	using System.Collections.Generic;
	using Combat;
	using Structure;
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
					if (other.tag == "Structure")
					{
						Building building = other.GetComponent<Building>();
						if (building != null)
						{
							building.DealDamage(targeting.damage);
						}
					}
				};
			}
		}
	}
}