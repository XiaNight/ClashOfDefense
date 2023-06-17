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
	}
}