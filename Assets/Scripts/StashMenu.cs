using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WorldStats
{
	public World world;
	public Text worldLabel;
	public Text medalLabel;
}

public class StashMenu : Menu {
	public static StashMenu Instance;
	protected override void Awake ()
	{base.Awake (); Instance = this;}

	public Text classicWins;
	public Text adventureWins;
	public Text alarmsTriggered;
	public Text sevensFlipped;

	public WorldStats[] stats;

	protected override void OnOpen()
	{
		classicWins.text = PlayerProgression.GetClassicWins ().ToString();
		adventureWins.text = PlayerProgression.GetLevelsCompleted ().ToString();
		alarmsTriggered.text = PlayerProgression.GetBombsTriggered ().ToString ();
		sevensFlipped.text = PlayerProgression.Number7sFlipped ().ToString ();

		foreach(WorldStats stat in stats) {
			stat.worldLabel.text = PlayerProgression.GetLevelsCompleted (stat.world) + "/" + stat.world.maps.Length;
			stat.medalLabel.text = PlayerProgression.GetMedalsCompleted (stat.world) + "/" + stat.world.maps.Length;
		}
	}

	public void GoBack()
	{
		MenuController.CloseCurrent ();
	}

}
