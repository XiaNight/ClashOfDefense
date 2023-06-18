using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(ToggleableCollection))]
public class Tab : MonoBehaviour
{
	public Button tabButton;
	public ToggleableCollection toggleable;
	private void Reset()
	{
		tabButton = GetComponent<Button>();
		toggleable = GetComponent<ToggleableCollection>();
	}
}
