using UnityEngine;
using System.Collections;

public class GUIMenuController : MonoBehaviour {

	public tk2dCamera cam;
	public IHoverable[] menuItems;
	public bool ignoreMouse = true;

	public AudioClip menuSound;
	public AudioClip buttonClick;

	IHoverable lastHoveredItem;
	int hoverIndex;

	void Start()
	{
		Init();
	}

	public void Init()
	{
		for(int x=0; x<menuItems.Length; x++)
		{
			menuItems[x].SetController(this);
		}
		
		if(menuItems.Length > 0)
		{
			if(lastHoveredItem != null)
				lastHoveredItem.OnHover(false);

			menuItems[0].OnHover(true);
			hoverIndex = 0;
			lastHoveredItem = menuItems[0];
		}
	}

	public void ResetAllItems()
	{
		for(int x=0; x<menuItems.Length; x++)
			menuItems[x].Reset();
	}

	void Update ()
	{
		if(!ignoreMouse)
		{
			Ray ray = cam.camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if(hit.collider.gameObject.tag == "HoverItem")
				{
					lastHoveredItem = hit.collider.gameObject.GetComponent<IHoverable>();
					if(lastHoveredItem != null && !lastHoveredItem.IsHovered && lastHoveredItem.IsEnabled)
						lastHoveredItem.OnHover(true);
				}
			}
			else
			{
				if(lastHoveredItem != null)
				{
					lastHoveredItem.OnHover(false);
					lastHoveredItem = null;
				}
			}

			if(Input.GetMouseButtonUp(0) && lastHoveredItem != null && lastHoveredItem.IsEnabled)
				lastHoveredItem.OnClick();
		}
		else
		{
			if(Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
			{
				lastHoveredItem.OnHover(false);
				lastHoveredItem = FindPreviousMenuItem();
				lastHoveredItem.PlayAudioClip(menuSound);
				lastHoveredItem.OnHover(true);
			}

			if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
			{
				lastHoveredItem.OnHover(false);
				lastHoveredItem = FindNextMenuItem();
				lastHoveredItem.PlayAudioClip(menuSound);
				lastHoveredItem.OnHover(true);
			}

			if(Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.Space))
			{
				lastHoveredItem.PlayAudioClip(buttonClick);
				lastHoveredItem.OnClick();
			}
		}
	}

	#region Navigation
	IHoverable FindPreviousMenuItem()
	{
		bool found = false;
		while(!found)
		{
			hoverIndex--;
			if(hoverIndex < 0) hoverIndex = (menuItems.Length-1);
			
			if(menuItems[hoverIndex].IsEnabled)
				found = true;
		}
		return menuItems[hoverIndex];
	}

	IHoverable FindNextMenuItem()
	{
		bool found = false;
		while(!found)
		{
			hoverIndex++;
			if(hoverIndex > (menuItems.Length-1)) hoverIndex = 0;
			
			if(menuItems[hoverIndex].IsEnabled)
				found = true;
		}

		return menuItems[hoverIndex];
	}
	#endregion
}
