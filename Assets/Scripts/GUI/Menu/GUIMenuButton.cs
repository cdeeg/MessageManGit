using UnityEngine;
using System.Collections;

public class GUIMenuButton : IHoverable
{
	const float INCREASE_SIZE_TWEEN = .5f;
	const float TWEEN_TIME = 1.3f;

	public tk2dSlicedSprite background;
	public tk2dTextMesh buttonText;
	public tk2dSlicedSprite indicator;

	public string onClickMethod;

	public Color enabledColor = new Color(0f, 0f, 1f);
	public Color disabledColor = new Color(1f, 0f, 0f);

	public Color enabledColorText = new Color(1f, 1f, 1f);
	public Color disabledColorText = new Color(.3f, .3f, .3f);

	public bool isActive = true;

	bool isDoneTweening = true;
	bool tweening = false;
	Vector2 initialSizeBg;
	Vector2 increaseMaxSize;

	AudioSource audioSour;

	float passedTime = 0f;

	void Awake()
	{
		indicator.gameObject.SetActive(false);
		audioSour = GetComponent<AudioSource>();
	}

	void Start()
	{
		if(background == null)
		{
			Debug.LogWarning("GUIMenuButton: No background sprite found!");
		}

		if(buttonText == null)
		{
			Debug.LogWarning("GUIMenuButton: No text mesh found!");
		}

		if(string.IsNullOrEmpty(onClickMethod))
		{
			Debug.LogWarning("GUIMenuButton: No function string found!");
		}

		initialSizeBg = background.dimensions;
		increaseMaxSize = new Vector2(initialSizeBg.x * INCREASE_SIZE_TWEEN + initialSizeBg.x, initialSizeBg.y);


		Enable(isActive);
	}

	public override void PlayAudioClip(AudioClip clip)
	{
		audioSour.PlayOneShot(clip);
	}

	#region Coroutines
	IEnumerator OnHoverTween()
	{
		passedTime = 0f;
		tweening = true;
		while(!isDoneTweening)
		{
			if(hovered)
			{
				Vector2 currentSize = background.dimensions;
				currentSize = Vector2.Lerp(currentSize, increaseMaxSize, passedTime);
				background.dimensions = currentSize;
				passedTime += Time.deltaTime;
				if(passedTime > TWEEN_TIME)
					isDoneTweening = true;
			}
			else
			{
				Vector2 currentSize = background.dimensions;
				currentSize = Vector2.Lerp(currentSize, initialSizeBg, passedTime);
				background.dimensions = currentSize;
				passedTime += Time.deltaTime;
				if(passedTime > TWEEN_TIME)
					isDoneTweening = true;
			}

			yield return null;
		}
		tweening = false;
	}
	#endregion

	#region implemented abstract members of IHoverable

	public override void OnHover (bool isHovered, bool ignorePreviousValue = false)
	{
		if(isHovered != hovered || ignorePreviousValue)
		{
			hovered = isHovered;
			indicator.gameObject.SetActive(hovered);
			isDoneTweening = false;
			if(!tweening)
			{
				StartCoroutine(OnHoverTween());
			}
			passedTime = 0f;
		}
	}

	public override void Enable (bool enabled)
	{
		if(isEnabled != enabled)
		{
			isEnabled = enabled;

			if(isEnabled) background.color = enabledColor;
			else background.color = disabledColor;

			if(isEnabled) buttonText.color = enabledColorText;
			else buttonText.color = disabledColorText;

		}
	}

	public override void OnClick ()
	{
		if(myController != null)
			myController.SendMessage(onClickMethod);
	}

	public override void Reset ()
	{
		StopAllCoroutines();
		tweening = false;
		isDoneTweening = true;
		background.dimensions = initialSizeBg;
	}

	#endregion
}
