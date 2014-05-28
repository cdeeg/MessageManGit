using UnityEngine;
using System.Collections;

public class GUITweenableBar : MonoBehaviour {

	public tk2dSlicedSprite foreground;

	float initialSize;
	float initValue;
	float currentValue;

	public float CurrentValue
	{
		get { return currentValue; }
		set { currentValue = value; }
	}

	// Use this for initialization
	void Awake ()
	{
		initialSize = foreground.dimensions.x;
	}

	void OnDisable()
	{
		StopCoroutine("ResizeBar");
	}

	public void Init(float initialValue)
	{
		initValue = initialValue;
		currentValue = initialValue;

		// reset size
		Vector2 dim = foreground.dimensions;
		dim.x = initialSize;
		foreground.dimensions = dim;
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

		if(gameObject.activeInHierarchy)
			StartCoroutine(ResizeBar());
	}

	IEnumerator ResizeBar()
	{
		float blah = (currentValue/initValue)*initialSize;
		if( blah <= 0f ) blah = 0.1f;
		Vector2 size = foreground.dimensions;
		size.x = blah;
		foreground.dimensions = size;

		yield return null;
	}

	public override string ToString ()
	{
			return string.Format ("[GUITweenableBar: CurrentValue={0} InitValue={1}]", CurrentValue, initValue);
	}
}
