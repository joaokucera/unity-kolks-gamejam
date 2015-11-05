using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour 
{
	public InputField playerNameInputField; 
	public Button playButton; 
	public Button rankingButton; 
	public Animator playerNameAnimator;
	public Animator[] tileAnimators;

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
		playerNameAnimator.enabled = false;
		Open ();
	}

	void EnableButtons(bool enable)
	{
		playButton.interactable = enable;
		rankingButton.interactable = enable;
	}

	public void ToGame()
	{
		SoundEffectControl.Instance.PlaySound (SoundEffectClip.ButtonClick);

		string playerName = playerNameInputField.text;
		if (string.IsNullOrEmpty(playerName))
		{
			StartCoroutine("BlinkPlaceHolder");
			return;
		}
		PlayerLog.CreatePlayerLog (playerName);

		for (int i = 0; i < tileAnimators.Length; i++) 
		{
			tileAnimators[i].enabled = true;
		}

		EnableButtons (false);
		Close ("Game");
	}

	IEnumerator BlinkPlaceHolder()
	{
		playerNameAnimator.enabled = true;

		for (float i = 0; i < 1.5f; i+= Time.deltaTime) 
		{
			yield return 0;
		}

		playerNameAnimator.enabled = false;
	}

	public void ToRanking()
	{
		SoundEffectControl.Instance.PlaySound (SoundEffectClip.ButtonClick);

		EnableButtons (false);
		Close ("Ranking");
	}

	void Open()
	{
		StartCoroutine ("FadeIn");
	}
	
	void Close(string nextScene)
	{
		StartCoroutine ("FadeOut", nextScene);
	}

	IEnumerator FadeOut(string nextScene)
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
		
		GoTo (nextScene);
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
	
	void GoTo(string nextScene)
	{
		Application.LoadLevel (nextScene);
	}
}
