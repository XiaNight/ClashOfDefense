using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CameraObjectIconRendering : EditorWindow
{
	private Vector2Int textureSize = new Vector2Int(512, 512);
	private float greenScreenThreshold = 0.7f;

	[MenuItem("ClashOfDefense/CameraObjectIconRendering")]
	private static void ShowWindow()
	{
		var window = GetWindow<CameraObjectIconRendering>();
		window.titleContent = new GUIContent("CameraObjectIconRendering");
		window.Show();
	}

	private void OnGUI()
	{
		textureSize = EditorGUILayout.Vector2IntField("Texture Size", textureSize);
		greenScreenThreshold = EditorGUILayout.Slider("Green Screen Threshold", greenScreenThreshold, 0, 1);

		if (GUILayout.Button("Render"))
		{
			Render();
		}
	}

	private void Render()
	{
		RenderTexture renderTexture = new RenderTexture(textureSize.x, textureSize.y, 24);
		Camera.main.targetTexture = renderTexture;
		Texture2D texture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGBA32, false);
		Camera.main.Render();
		RenderTexture.active = renderTexture;
		texture.ReadPixels(new Rect(0, 0, textureSize.x, textureSize.y), 0, 0);
		texture.Apply();

		// remove green screen
		Color[] pixels = texture.GetPixels();
		for (int i = 0; i < pixels.Length; ++i)
		{
			if (pixels[i].g > greenScreenThreshold)
			{
				pixels[i].a = 0;
				pixels[i].g = 0;
			}
		}
		texture.SetPixels(pixels);
		texture.Apply();

		Camera.main.targetTexture = null;
		RenderTexture.active = null;
		DestroyImmediate(renderTexture);

		// save the texture to file
		byte[] bytes = texture.EncodeToPNG();

		// get active project path
		string path = Application.dataPath;

		foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
			path = AssetDatabase.GetAssetPath(obj);
			if (!path.EndsWith(".png"))
			{
				path = path.Substring(0, path.LastIndexOf('/')) + "/icon.png";
			}
			if (System.IO.Directory.Exists(path))
			{
				break;
			}
		}

		// save to assets folder
		System.IO.File.WriteAllBytes(path, bytes);

		// refresh the asset folder
		AssetDatabase.Refresh();
	}
}