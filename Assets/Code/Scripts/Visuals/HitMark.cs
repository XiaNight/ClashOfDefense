using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashOfDefense.Game.Visuals
{
	public class HitMark : MonoBehaviour
	{
		[SerializeField] private TMPro.TMP_Text label;
		[Header("Movement")]
		[SerializeField] private float lifeTime;
		[SerializeField] private Vector3 initialDirection;
		[SerializeField] private float initialSpeed;
		[Header("Colors")]
		[SerializeField] private Color normalHitColor;
		[SerializeField] private Color criticalHitColor;
		[SerializeField] private Color healColor;
		[SerializeField] private Color shieldColor;

		[Header("Size")]
		[SerializeField] private float normalSize;
		[SerializeField] private float largeSize;

		private void Start()
		{
			Destroy(gameObject, lifeTime);
		}

		private void Update()
		{
			transform.position += initialDirection * initialSpeed * Time.deltaTime;
		}

		public void SetText(string message, HitType hitType = HitType.NormalHit, bool large = false)
		{
			label.color = hitType switch
			{
				HitType.NormalHit => normalHitColor,
				HitType.CriticalHit => criticalHitColor,
				HitType.Heal => healColor,
				HitType.Shield => shieldColor,
				_ => normalHitColor,
			};
			label.text = message;
			label.fontSize = large ? largeSize : normalSize;
		}

		public enum HitType
		{
			NormalHit,
			CriticalHit,
			Heal,
			Shield,
		}
	}
}

