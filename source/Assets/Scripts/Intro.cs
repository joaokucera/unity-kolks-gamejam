using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Intro : MonoBehaviour 
{
	public Text text; 

	private CanvasGroup canvasGroup;
	private bool fadingIn;
	private bool fadingOut;
	private float fadeTime = 1.5f;

	private CanvasGroup CanvasGroup
	{
		get
		{
			if (canvasGroup == null)
			{
				canvasGroup = GetComponent<CanvasGroup>();
			}
			
			return canvasGroup;
		}
	}

	void Awake()
	{
		CanvasGroup.alpha = 0;
	}

	void Start()
	{
		Open ();
	}

	void Update()
	{
		if (Input.anyKeyDown && text.gameObject.activeInHierarchy)
		{
			SoundEffectControl.Instance.PlaySound (SoundEffectClip.ButtonClick);
			text.gameObject.SetActive(false);

			Close();
		}
	}

	void Open()
	{
		StartCoroutine ("FadeIn");
	}

	void Close()
	{
		StartCoroutine ("FadeOut");
	}

	IEnumerator FadeOut()
	{
		if (!fadingOut)
		{
			fadingOut = true;
			fadingIn = false;
			StopCoroutine("FadeIn");
			
			float startAlpha = CanvasGroup.alpha;
			float rate = 1f / fadeTime;
			float progress = 0f;
			
			while (progress < 1f) 
			{
				CanvasGroup.alpha = Mathf.Lerp(startAlpha, 0, progress);
				progress += rate * Time.deltaTime;
				
				yield return 0;
			}
			
			CanvasGroup.alpha = 0;
			fadingOut = false;
		}

		GoToMenu ();
	}

	IEnumerator FadeIn()
	{
		if (!fadingIn)
		{
			fadingIn = true;
			fadingOut = false;
			StopCoroutine("FadeOut");
			
			float startAlpha = CanvasGroup.alpha;
			float rate = 1f / fadeTime;
			float progress = 0f;
			
			while (progress < 1f) 
			{
				CanvasGroup.alpha = Mathf.Lerp(startAlpha, 1, progress);
				progress += rate * Time.deltaTime;
				
				yield return 0;
			}
			
			CanvasGroup.alpha = 1f;
			fadingIn = false;
		}
	}

	void GoToMenu()
	{
		Application.LoadLevel ("Menu");
	}
}