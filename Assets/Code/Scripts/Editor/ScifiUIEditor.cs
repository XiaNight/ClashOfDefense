using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class ScifiUIEditor : EditorWindow
{
	private Color targetColorScheme;
	private string targetText;
	private GameObject lastSelected;
	private bool activePreview;

	[MenuItem("ClashOfDefense/ScifiUI")]
	private static void ShowWindow()
	{
		var window = GetWindow<ScifiUIEditor>();
		window.titleContent = new GUIContent("ScifiUI");
		window.Show();
	}

	private void OnGUI()
	{
		// color picker
		targetColorScheme = EditorGUILayout.ColorField("Color Scheme", targetColorScheme);

		// get selected
		var selected = Selection.activeGameObject;

		if (lastSelected != selected)
		{
			activePreview = false;
		}

		// target text
		targetText = EditorGUILayout.TextField("Text", targetText);

		// preview checkbox
		activePreview = EditorGUILayout.Toggle("Preview", activePreview);

		// if selected has a button component
		if (selected.TryGetComponent(out Graphic button))
		{
			if (activePreview || GUILayout.Button("Set Color Scheme"))
			{
				button.color = targetColorScheme;

				Graphic glow = null;
				if (button.transform.Find("Glow")?.TryGetComponent(out glow) ?? false)
				{
					Color.RGBToHSV(targetColorScheme, out float h, out float s, out float v);
					glow.color = Color.HSVToRGB(h, s * 0.15f, 1);
				}

				Graphic rim = null;
				if (button.transform.Find("Rim")?.TryGetComponent(out rim) ?? false)
				{
					Color.RGBToHSV(targetColorScheme, out float h, out float s, out float v);
					rim.color = Color.HSVToRGB(h, s / 4, 1);
				}

				Graphic halo = null;
				if (button.transform.Find("Halo")?.TryGetComponent(out halo) ?? false)
				{
					Color.RGBToHSV(targetColorScheme, out float h, out float s, out float v);
					halo.color = Color.HSVToRGB(h, 0, 1);
				}

				Graphic text = null;
				if (button.transform.Find("MainText")?.TryGetComponent(out text) ?? false)
				{
					Color.RGBToHSV(targetColorScheme, out float h, out float s, out float v);
					text.color = Color.HSVToRGB(h, s * 0.1f, v > 0.5f && s < 0.4f ? 0 : 1);
				}

				TMP_Text textLabel = null;
				if ((bool)(button.transform.Find("MainText")?.TryGetComponent(out textLabel)))
				{
					textLabel.text = targetText;
				}

				// set dirty
				EditorUtility.SetDirty(button);
			}

			if (lastSelected != selected)
			{
				Text text = null;
				if (button.transform.Find("Text")?.TryGetComponent(out text) ?? false)
				{
					targetText = text.text;
				}
			}
		}
		else
		{
			EditorGUILayout.HelpBox("Select a GameObject with a Graphic component", MessageType.Info);
		}

		lastSelected = selected;
	}
}