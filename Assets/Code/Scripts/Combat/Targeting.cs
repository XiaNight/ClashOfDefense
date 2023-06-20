using UnityEngine;
using UnityEngine.Events;

namespace ClashOfDefense.Game.Combat
{
	[System.Serializable]
	public struct Targeting
	{
		public int damage;
		public int range;

		[Header("Attack")]
		public float cooldown;
		private float lastAttack;
		private bool isAttacking;
		public bool IsAtacking { get => isAttacking; }

		[Header("Burst")]
		public int shots;
		public float shotInterval;
		private float lastShotTime;
		private int shotsRemain;

		[Header("Accuracy")]
		// Spread in meters at 10m
		public AnimationCurve accuracyCurve;
		public float currentSpread;
		public float spreadCooldown;

		public event UnityAction OnAttack;

		public void TryAttack()
		{
			if (!isAttacking)
			{
				if (Time.time - lastAttack > cooldown)
				{
					lastAttack = Time.time;
					isAttacking = true;
					shotsRemain = shots;
				}
			}
			if (isAttacking)
			{
				if (Time.time - lastShotTime > shotInterval)
				{
					currentSpread -= (Time.time - lastShotTime) * spreadCooldown;
					if (currentSpread < 0.1f)
					{
						currentSpread = 0.1f;
					}
					lastShotTime = Time.time;
					--shotsRemain;
					if (shotsRemain <= 0)
					{
						isAttacking = false;
					}
					OnAttack?.Invoke();
				}
			}
		}

		public Vector3 CalculateSpread(Vector3 direction, float spreadMultiplier = 1)
		{
			float spread = accuracyCurve.Evaluate(currentSpread) * spreadMultiplier;
			direction = (direction * 10 + Random.insideUnitSphere * spread).normalized;
			currentSpread += 1;
			return direction;
		}
	}
}