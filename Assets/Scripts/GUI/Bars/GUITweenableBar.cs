using UnityEngine;
using System.Collections;

public class GUITweenableBar : MonoBehaviour {

	public tk2dClippedSprite foreground;
	public tk2dSprite background;
	public bool isVisible = true;

//	float initialSize;
	float initValue;
	float currentValue;

	public float CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	// Use this for initialization
//	void Awake ()
//	{
//		initialSize = foreground.dimensions.x;
//	}

	void OnDisable()
	{
		StopCoroutine("ResizeBar");
	}

	public void Init(float initialValue)
	{
		initValue = initialValue;
		currentValue = initialValue;

		foreground.gameObject.SetActive(true);
		background.gameObject.SetActive(true);

		// reset size
//		Vector2 dim = foreground.dimensions;
//		dim.x = initialSize;
//		foreground.dimensions = dim;

		Rect cr = foreground.ClipRect;
		cr.height = 1f;
		foreground.ClipRect = cr;

		if(!isVisible)
		{
			foreground.gameObject.SetActive(false);
			background.gameObject.SetActive(false);
		}
	}

	public void UpdateBar(float value, bool isDelta)
	{
		// avoid negative/over scaling
		if(isDelta)
		{
			currentValue += value;
		}
		else
		{
			currentValue = value;
		}

		if(currentValue < 0f) currentValue = -1f;
		if(currentValue > initValue) currentValue = initValue;

		if(gameObject.activeInHierarchy && isVisible)
			ResizeBar();
	}

	void ResizeBar()
	{
		float blah = currentValue/initValue;
		if( blah <= 0f ) blah = 0.1f;
//		Vector2 size = foreground.dimensions;
//		size.x = blah;
//		foreground.dimensions = size;
		Rect cr = foreground.ClipRect;
		cr.height = blah;
		foreground.ClipRect = cr;
	}

	public override string ToString ()
	{
			return string.Format ("[GUITweenableBar: CurrentValue={0} InitValue={1}]", CurrentValue, initValue);
	}
}
