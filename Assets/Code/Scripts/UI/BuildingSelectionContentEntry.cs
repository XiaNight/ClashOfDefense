using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.EventSystems;

public class BuildingSelectionContentEntry : MonoBehaviour
{
	[SerializeField] private Toggle toggleButton;
	[SerializeField] private Image iconImage;
	[SerializeField] private TextMeshProUGUI nameText;

	public event UnityAction OnClick;

	public void Setup(Sprite icon, string name)
	{
		toggleButton.onValueChanged.AddListener((value) =>
		{
			if (value)
			{
				OnClick?.Invoke();
			}
		});
		iconImage.sprite = icon;
		nameText.text = name;
	}
}
