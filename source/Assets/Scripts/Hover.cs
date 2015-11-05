using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hover : MonoBehaviour 
{
	private Animator animator;
	private Image image;

	private Animator Animator
	{
		get
		{
			if (animator == null)
			{
				animator = GetComponent<Animator> ();
			}

			return animator;
		}
	}

	public Image Image
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

	public void EnableAnimatorAndImage(Color color)
	{
		Animator.enabled = true;

		Image.color = color;
		Image.enabled = true;
	}

	public void DisableAnimatorAndImage()
	{
		Animator.enabled = false;

		Image.enabled = false;
	}

	public void DisableAnimator()
	{
		Animator.enabled = false;
	}
}