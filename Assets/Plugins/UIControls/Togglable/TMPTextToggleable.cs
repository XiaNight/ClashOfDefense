using UnityEngine;
using TMPro;

public class TMPTextToggleable : Toggleable
{
	public TMP_Text label;

	public bool overrideText = true;
	public string enabledText;
	public string disabledText;

	public bool overrideColor = false;
	public Color enabledColor;
	public Color disabledColor;

	private void Reset()
	{
		label = GetComponent<TMP_Text>();
	}

	public override void Disable()
	{
		if (overrideText)
		{
			label.text = disabledText;
		}
		if (overrideColor)
		{
			label.color = disabledColor;
		}
	}

	public override void Enable()
	{
		if (overrideText)
		{
			label.text = enabledText;
		}
		if (overrideColor)
		{
			label.color = enabledColor;
		}
	}
}
