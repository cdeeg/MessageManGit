using UnityEngine;
using System.Collections;

public abstract class IHoverable : MonoBehaviour
{
	protected bool hovered;
	protected bool isEnabled = true;
	protected GUIMenuController myController;

	public bool IsHovered
	{
		get { return hovered; }
	}

	public bool IsEnabled
	{
		get { return isEnabled; }
	}

	public abstract void OnHover(bool isHovered, bool ignorePreviousValue = false);

	public abstract void Enable(bool enabled);

	public abstract void OnClick();

	public abstract void Reset();

	public abstract void PlayAudioClip(AudioClip clip);

	public virtual void SetController(GUIMenuController ctrl)
	{
		myController = ctrl;
	}
}
