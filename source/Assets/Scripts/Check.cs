using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Check : MonoBehaviour 
{
	private Image image;
	private Image Image
	{
		get
		{
			if (image == null)
			{
				image = GetComponent<Image> ();
			}
			
			return image;
		}
	}

	public void ShowImage()
	{
		Image.enabled = true;
	}
	
	public void HideImage()
	{
		Image.enabled = false;
	}
}