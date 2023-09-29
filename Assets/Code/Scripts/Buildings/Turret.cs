using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game.Structure
{
	using ClashOfDefense.Game.Helper;
	using Combat;
	using Entity;

	public class Turret : AssultBuilding
	{
		[Header("Turret Transforms")]
		[SerializeField] protected Transform rotationBase;
		[SerializeField] protected Transform tiltBody;
		[SerializeField] private Transform barrel;
		[SerializeField] private BarrelSetting barrelSetting;

		[Header("Settings")]
		[SerializeField] protected float barrelRotationSpeed;
		[SerializeField] protected Targeting targeting;
		[SerializeField] protected Projectile projectilePrefab;
		[SerializeField] protected MuzzleFlashMode muzzleFlashMode;

		[Header("Muzzle Flash")]
		[SerializeField] protected ParticleSystem muzzelFlash;

#if UNITY_EDITOR
		[Header("Debug")]
		[SerializeField] protected bool debugMode = false;
		[SerializeField] protected Transform debugTarget;
#endif

		protected float currentRotationSpeed;

		protected override void Awake()
		{
			base.Awake();
#if UNITY_EDITOR
			if (debugMode)
			{
				canFire = true;
			}
#endif
			targeting.OnAttack += SpawnProjectile;
		}

		protected override void Start()
		{
			base.Start();
		}

		protected virtual void FixedUpdate()
		{
			AimAndFire();
		}

		private void SpawnProjectile()
		{
			Transform barrel = barrelSetting.GetBarrel();
			Vector3 direction = (targetEntity.CentralPosition - barrel.position).normalized;
			direction = targeting.CalculateSpread(direction);
			Projectile projectile = Instantiate(projectilePrefab, barrel.position, Quaternion.LookRotation(direction));
			projectile.OnHit += OnProjectileHit;
			if (muzzleFlashMode == MuzzleFlashMode.PlayWhenFired)
			{
				muzzelFlash.Play();
			}
		}

		protected virtual void OnProjectileHit(Collider collider)
		{
			if (collider.gameObject.layer == LayerManager.EnemyEntity)
			{
				EntityAI entity = collider.GetComponentInParent<EntityAI>();
				if (entity != null)
				{
					bool isKill = entity.DealDamage(targeting.damage);
					if (isKill)
					{
						UnsetTarget();
						FindNewTarget();
						veterancy++;
					}
				}
			}
		}

		protected virtual void AimAndFire()
		{
			// Gun barrel rotation
			if (barrel != null)
				barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

			// if can fire turret activates
			if (canFire && targetEntity != null)
			{
				// start rotation
				currentRotationSpeed = barrelRotationSpeed;

				// aim at enemy
				Vector3 targetPosition = targetEntity.transform.position;
				Vector3 baseTargetPostition = new Vector3(targetPosition.x, this.transform.position.y, targetPosition.z);

				rotationBase.transform.LookAt(baseTargetPostition);
				tiltBody.transform.LookAt(targetPosition);

				targeting.TryAttack();

				// start particle system 
				if (muzzleFlashMode == MuzzleFlashMode.PlayWhenFiring)
				{
					if (muzzelFlash.isPlaying != targeting.IsAtacking)
					{
						if (targeting.IsAtacking)
						{
							muzzelFlash.Play();
						}
						else
						{
							muzzelFlash.Stop();
						}
					}
				}
			}
			else
			{
				// slow down barrel rotation and stop
				currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);
				// stop the particle system
				if (muzzelFlash.isPlaying)
				{
					muzzelFlash.Stop();
				}
			}
		}

		[System.Serializable]
		protected enum MuzzleFlashMode
		{
			PlayWhenFiring,
			PlayWhenFired
		}

		[System.Serializable]
		protected struct BarrelSetting
		{
			public Transform barrelsContainer;
			private int currentBarrel;
			public Type type;
			public Transform GetBarrel()
			{
				switch (type)
				{
					case Type.Single:
						return barrelsContainer.GetChild(0);
					case Type.Sequential:
						return GetNextBarrel();
					case Type.Random:
						return barrelsContainer.GetChild(Random.Range(0, barrelsContainer.childCount));
					default:
						return null;
				}
			}
			private Transform GetNextBarrel()
			{
				if (currentBarrel >= barrelsContainer.childCount)
				{
					currentBarrel = 0;
				}
				return barrelsContainer.GetChild(currentBarrel++);
			}

			[System.Serializable]
			public enum Type
			{
				Single,
				Sequential,
				Random,
			}
		}
	}
}