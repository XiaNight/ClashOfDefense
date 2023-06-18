using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleableCollection : Toggleable
{
	[SerializeField] private Toggleable[] toggleables = null;

	private void Init()
	{
		List<Toggleable> toggleablesInChildren = new List<Toggleable>(gameObject.GetComponentsInChildren<Toggleable>(true));

		if (toggleablesInChildren.Contains(this))
		{
			toggleablesInChildren.Remove(this);
		}

		toggleables = toggleablesInChildren.ToArray();
	}

	public override void Disable()
	{
		if (toggleables == null)
		{
			Init();
		}

		foreach (Toggleable toggleable in toggleables)
		{
			toggleable.Disable();
		}
	}

	public override void Enable()
	{
		if (toggleables == null)
		{
			Init();
		}

		foreach (Toggleable togglable in toggleables)
		{
			togglable.Enable();
		}
	}
}
