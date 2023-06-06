using UnityEngine;

namespace ClashOfDefense.Game.Combat
{
	[System.Serializable]
	public struct Targeting
	{
		public int damage;
		public int range;
		public Projectile projectile;

		[Header("Attack")]
		public float cooldown;
		private float lastAttack;
		private bool isAttacking;

		[Header("Burst")]
		public int shots;
		public float shotInterval;
		private float lastShotTime;
		private int shotsRemain;

		public bool TryAttack()
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
					lastShotTime = Time.time;
					--shotsRemain;
					if (shotsRemain <= 0)
					{
						isAttacking = false;
					}
					return true;
				}
			}
			return false;
		}
	}
}