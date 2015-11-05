using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DisplayHighscore : MonoBehaviour 
{	
	public Ranking rankingManager;
	public Text[] textHighscores;

	void Start()
	{
		for (int i = 0; i < textHighscores.Length; i++) 
		{
			textHighscores[i].text = (i+1) + "  ...loading...";

			StartCoroutine("RefreshHighscores");
		}
	}

	public void OnHighscoresDownloaded(Highscore[] highscoreArray)
	{
		for (int i = 0; i < textHighscores.Length; i++) 
		{
			textHighscores[i].text = (i+1) + ". ";

			if (highscoreArray.Length > i)
			{
				textHighscores[i].text += string.Format("({0:d}) player: <color=yellow>{1}</color> | score: <color=yellow>{2}</color>", highscoreArray[i].date, highscoreArray[i].username, highscoreArray[i].score);
			}
		}
	}

	IEnumerator RefreshHighscores()
	{
		while (true) 
		{
			rankingManager.DownloadHighscore();
			yield return new WaitForSeconds(30);
		}
	}

	public void GoToMenu()
	{
		Application.LoadLevel ("Menu");
	}
}