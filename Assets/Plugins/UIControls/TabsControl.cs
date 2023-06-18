using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class TabsControl : MonoBehaviour
{
	[SerializeField] private Tab[] tabs;
	[SerializeField] private int initialTab = -1;

	public delegate void OnTabSelectedHandler(int tabIndex);
	public event OnTabSelectedHandler OnTabSelected;

	public int CurrentTab { get; private set; }

	private void Awake()
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			int iCopy = i;
			tabs[i].tabButton.onClick.AddListener(() =>
			{
				ActivateTab(iCopy);
			});
		}

		CurrentTab = -1;
		if (initialTab >= 0)
		{
			ActivateTab(initialTab);
		}
	}

	public void ActivateTab(int index)
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			tabs[i].toggleable.Disable();
		}
		tabs[index].toggleable.Enable();
		CurrentTab = index;
		OnTabSelected?.Invoke(index);
	}

	public void AddTab(Tab tab)
	{
		Array.Resize(ref tabs, tabs.Length + 1);
		tabs[tabs.Length - 1] = tab;
		tab.transform.SetParent(transform, false);
	}

	public void RemoveAllTabs()
	{
		for (int i = 0; i < tabs.Length; i++)
		{
			Destroy(tabs[i].gameObject);
		}
		tabs = new Tab[0];
	}

	public bool GetTab(int index, out Tab tab)
	{
		if (index < 0 || index >= tabs.Length)
		{
			tab = null;
			return false;
		}
		tab = tabs[index];
		return true;
	}
}
