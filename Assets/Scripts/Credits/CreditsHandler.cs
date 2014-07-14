using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CreditsHandler : MonoBehaviour {

	public tk2dTextMesh category;
	public tk2dTextMesh credits;
	public tk2dTextMesh exitInfo;
	public string fileName = "Credits";
	public float waitBetweenCategories = 1f;

	List<CreditsObject> cObjects;
	int index;
	bool atEnd;

	#region structs
	struct CreditsObject
	{
		public string Category {get; set;}
		public string Names {get; set;}

		public CreditsObject(string categ, string names)
		{
			Category = categ;
			Names = names;
		}
	}
	#endregion

	void Start ()
	{
		category.text = "";
		credits.text = "";

		atEnd = false;
		index = 0;
		cObjects = new List<CreditsObject>();

		TextAsset txt = Resources.Load(fileName) as TextAsset;
		string[] all = txt.text.Split(';');

		for(int x=0; x<all.Length; x++)
		{
			string[] tmp = all[x].Split(':');
			CreditsObject obj = new CreditsObject(tmp[0].Trim(), tmp[1].Replace(',', '\n').Trim());
			cObjects.Add(obj);
		}

		StartCoroutine(TypeCredits());
		StartCoroutine(FadeExitText());
	}

	IEnumerator FadeExitText()
	{
		float timePassed = 0f;
		
		bool fadeIn = false;
		float fadeTimePassed = 0f;
		float fadeTimeDur = 1f;
		float thresh = 0.5f;
		
		while(!atEnd)
		{
			timePassed += Time.deltaTime;
			
			Color col = exitInfo.color;
			
			if(fadeIn) col.a = Mathf.Lerp(thresh, 1f, fadeTimePassed/fadeTimeDur);
			else col.a = Mathf.Lerp(1f, thresh, fadeTimePassed/fadeTimeDur);
			
			if(fadeTimePassed > fadeTimeDur) { fadeTimePassed = 0f; fadeIn = !fadeIn; }
			
			exitInfo.color = col;
			fadeTimePassed += Time.deltaTime;
			
			yield return null;
		}
	}

	void BackToMenu()
	{
		StopAllCoroutines();
		Application.LoadLevel("MainMenu");
	}
	
	IEnumerator TypeCredits()
	{
		category.text = cObjects[index].Category; 
		char[] names = cObjects[index].Names.ToCharArray();
		StringBuilder allText = new StringBuilder();
		int currentIndex = 0;
		
		float timePassed = 0f;
		float randVari = Random.Range(0f, 0.5f);
		
		while(!atEnd)
		{
			if(Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Space))
			{
				atEnd = true;
				continue;
			}

			if(timePassed > randVari)
			{
				randVari = Random.Range(0f, 0.5f);
				timePassed = 0f;
				allText.Append(names[currentIndex]);
				credits.text = allText.ToString();
				
				if(currentIndex == names.Length-1)
				{
					index++;
					if(index == cObjects.Count)
					{
						atEnd = true;
					}
					else
					{
						yield return new WaitForSeconds(waitBetweenCategories);
						credits.text = "";
						allText = new StringBuilder();
						category.text = cObjects[index].Category; 
						names = cObjects[index].Names.ToCharArray();
						currentIndex = 0;
					}
				}
				else
				{
					currentIndex++;
				}
				
				yield return null;
			}
			else
			{
				timePassed+=Time.deltaTime;

				yield return null;
			}
		}

		yield return null;

		BackToMenu();
	}
}
