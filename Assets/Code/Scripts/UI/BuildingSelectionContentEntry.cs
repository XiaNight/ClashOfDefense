using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

namespace ClashOfDefense.Game
{
	public class BuildingSelectionContentEntry : MonoBehaviour
	{
		[SerializeField] private ToggleableCollection toggleableCollection;
		[SerializeField] private Button toggleButton;
		[SerializeField] private Image iconImage;
		[SerializeField] private TextMeshProUGUI nameText;

		public event UnityAction OnClick;

		public void Setup(Sprite icon, string name)
		{
			toggleButton.onClick.AddListener(() =>
			{
				OnClick?.Invoke();
				toggleableCollection.Enable();
			});
			iconImage.sprite = icon;
			nameText.text = name;
		}

		public void Disable()
		{
			toggleableCollection.Disable();
		}

		public void SetInteractable(bool interactable)
		{
			toggleButton.interactable = interactable;
		}
	}
}