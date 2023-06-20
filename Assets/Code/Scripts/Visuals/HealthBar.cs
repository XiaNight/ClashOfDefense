using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClashOfDefense.Game.Visuals
{
	using Base;
	public class HealthBar : MonoBehaviour
	{
		[SerializeField, SerializeReference] private IHaveHealth target;
		[SerializeField] private Image healthBarFill;
		[SerializeField] private TMP_Text healthLabel;
		private int maxHealth;
		private int currentHealth;

		private void Awake()
		{
			target = GetComponentInParent<IHaveHealth>();
			target.OnHealthChanged += (health) =>
			{
				UpdateData();
			};
		}

		public void UpdateData()
		{
			maxHealth = target.MaxHealth;
			currentHealth = target.Health;
			UpdateUI();
		}

		private void UpdateUI()
		{
			if (maxHealth == 0)
			{
				healthBarFill.fillAmount = 0;
				healthLabel.text = "0/0";
				return;
			}
			healthBarFill.fillAmount = (float)currentHealth / maxHealth;
			healthLabel.text = $"{currentHealth}/{maxHealth}";
		}
	}
}