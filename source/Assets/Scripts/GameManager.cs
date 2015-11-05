using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ExtraUI
{
	public Text levelText;

	public GameObject gameOverObject;
	public Text gameOverPlayerText;
	public Text gameOverPointsText;

	public Button[] buttons;

	public void DisableButtons ()
	{
		for (int i = 0; i < buttons.Length; i++) 
		{
			buttons[i].interactable = false;
		}
	}

	public void DisableAll ()
	{
		DisableNextLevel ();
		DisableGameOver ();
	}

	public void ActiveNextLevel(string nextLevel)
	{
		levelText.text = nextLevel;

		levelText.gameObject.SetActive (true);
	}

	public void ActiveGameOver(string playerText, string pointsText)
	{
		gameOverPlayerText.text = playerText;
		gameOverPointsText.text = pointsText;
			
		gameOverObject.SetActive (true);
	}

	private void DisableNextLevel()
	{
		levelText.gameObject.SetActive (false);
	}

	private void DisableGameOver()
	{
		gameOverObject.SetActive (false);
	}
}

public class GameManager : MonoBehaviour
{
	#region [ FIELDS ]

	private static GameManager instance;

	public ExtraUI extraUI;
	public Text levelText;
	public Image timerImage;
	public Color originalColor;
	public Text goalText;
	public Text playerText;
	public Text pointsText;

	private const string LevelString = "Level {0}";
	private const string PlayerString = "PLAYER:{0}<color=white>{1}</color>";
	private const string PointsString = "POINTS:{0}<color=white>{1}</color>";
	private const string GoalString = "GOAL:{0}<color=white>organize tiles to build a lake in</color><color=yellow> {1} </color><color=white>seconds.</color>";
	
	private float points = 0;
	private float initialLevelTime;
	private float levelTime = 100f;
	private float decrementTime = 10f;
	private int level = 1;
	private CanvasGroup canvasGroup;
	private bool fadingIn;
	private bool fadingOut;
	private float fadeTime = 1.5f;
	private bool gameStarted;

	#endregion

	#region [ PROPERTIES ]

	public static GameManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<GameManager>();
			}

			return instance;
		}
	}
	
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

	#endregion

	#region [ METHODS ]

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}

		initialLevelTime = levelTime;
		points = initialLevelTime;

		CanvasGroup.alpha = 0;
		playerText.text = string.Format (PlayerString, Environment.NewLine, PlayerLog.Name);

		Initialize ();
	}

	void Update()
	{
		if (gameStarted)
		{
			pointsText.text = string.Format(PointsString, Environment.NewLine, (int)points);
		}
	}

	void Initialize()
	{
		Open ();

		extraUI.DisableAll ();

		timerImage.fillAmount = 1;
		timerImage.color = originalColor;

		levelText.text = string.Format (LevelString, level);
		goalText.text = string.Format(GoalString, Environment.NewLine, levelTime);

		BoardManager.Instance.SetupBoard (level);

		StartCoroutine ("CountdownLevel");
	}

	void HandleTimerImage(Image image, float currentHealth, float maxHealth)
	{
		float currentValue = Map (currentHealth, 0, maxHealth, 0, 1);
		
		image.fillAmount = Mathf.Lerp (image.fillAmount, currentValue, levelTime * Time.deltaTime);
		
		if (currentHealth > maxHealth / 2)
		{
			image.color = new Color32((byte)Map(currentHealth, maxHealth / 2, maxHealth, 255, 0), 255, 0, 255);
		}
		else
		{
			image.color = new Color32(255, (byte)Map(currentHealth, 0, maxHealth / 2, 0, 255), 0, 255);
		}
	}

	float Map(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}

	IEnumerator CountdownLevel()
	{
		float time = levelTime;

		while (time > 0)
		{
			if (gameStarted)
			{
				HandleTimerImage(timerImage, time, levelTime);

				time -= Time.deltaTime;
				points -= Time.deltaTime;
			}

			yield return 0;
		}

		CheckGameOver ();
	}

	void StopCountdownLevel()
	{
		StopCoroutine ("CountdownLevel");
	}

	public void CheckLevel()
	{
		bool completed = BoardManager.Instance.CheckLevel ();

		if (completed)
		{
			NextLevel();
		}
	}

	public void CheckGameOver()
	{
		bool completed = BoardManager.Instance.CheckLevel ();

		if (completed)
		{
			NextLevel();
		}
		else
		{
			StopCountdownLevel ();

			SoundEffectControl.Instance.PlaySound (SoundEffectClip.GameOver);
			PlayerLog.UpdatePlayerLog ((int)points);

			Close(false);
		}
	}

	void NextLevel()
	{
		SoundEffectControl.Instance.PlaySound (SoundEffectClip.StageCompleted);
		points += initialLevelTime;

		StopCountdownLevel ();

		level++;
		extraUI.ActiveNextLevel (string.Format (LevelString, level));

		levelTime -= decrementTime;	
		levelTime = Mathf.Clamp (levelTime, decrementTime, initialLevelTime);

		Close (true);
	}

	void Open()
	{
		StartCoroutine ("FadeIn");
	}
	
	void Close(bool win)
	{
		StartCoroutine ("FadeOut", win);
	}

	IEnumerator FadeOut(bool win)
	{
		gameStarted = false;

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

		if (win)
		{
			Initialize ();
		}
		else
		{
			GetComponent<Canvas>().enabled = false;
			extraUI.ActiveGameOver(string.Format (PlayerString, " ", PlayerLog.Name), string.Format(PointsString, " ", (int)points));
		}
	}

	public void GoTo(string nextLevel)
	{
		SoundEffectControl.Instance.PlaySound (SoundEffectClip.ButtonClick);

		StopAllCoroutines ();
		extraUI.DisableButtons ();

		Application.LoadLevel(nextLevel);
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

		gameStarted = true;
	}

	#endregion
}