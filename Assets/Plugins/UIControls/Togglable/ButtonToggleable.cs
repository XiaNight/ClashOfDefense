using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggleable : Toggleable
{
	public Button button;
	public bool interactableWhen;

	private void Reset()
	{
		button = GetComponent<Button>();
	}

	public override void Enable()
	{
		button.interactable = interactableWhen;
	}

	public override void Disable()
	{
		button.interactable = !interactableWhen;
	}
}
