using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour {

	public tk2dSprite tutZero;
	public tk2dSprite tutOne;

	public tk2dTextMesh textPressSpace;
	public float fadeIntervalSpeed = 1f;

	bool isActive;
	bool isLastSlide;

	void Start ()
	{
		tutZero.gameObject.SetActive(true);
		tutOne.gameObject.SetActive(false);

		isActive = true;
		isLastSlide = false;

		StartCoroutine(BlinkingText());
	}

	void LoadMainLevel()
	{
		Color col = textPressSpace.color;
		col.a = 1f;
		textPressSpace.color = col;
		textPressSpace.text = "Loading...";
		Application.LoadLevel("jpscene");
	}

	IEnumerator BlinkingText()
	{
		bool fadeIn = false;
		float passedTime = 0f;

		while(isActive)
		{
			if(Input.GetKeyUp(KeyCode.Space))
			{
				// show second slide if possible, else: load level
				if(isLastSlide)
				{
					isActive = false;

					yield return null;
				}
				else
				{
					tutZero.gameObject.SetActive(false);
					tutOne.gameObject.SetActive(true);
					isLastSlide = true;

					yield return null;
				}
			}

			Color col = textPressSpace.color;
			if(fadeIn) col.a = Mathf.Lerp(0f, 1f, passedTime/fadeIntervalSpeed);
			else col.a = Mathf.Lerp(1f, 0f, passedTime/fadeIntervalSpeed);

			textPressSpace.color = col;
			passedTime += Time.deltaTime;
			
			if(passedTime > fadeIntervalSpeed) { fadeIn = !fadeIn; passedTime = 0f; }

			yield return null;
		}

		yield return null;

		LoadMainLevel();
	}
}
