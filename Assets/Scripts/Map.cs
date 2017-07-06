using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FrameShape
{
	Portrait,
	Square,
	Landscape
}

public class Map : ScriptableObject {
	public string title;
	public string author;
	public MapSize size;
	public int width;
	public int height;
	public int difficulty;
	public int timeLimit;
	public float pointsMultiplier;
	public float pointTarget;

	public string frame;

	public SlotType[] types;
	public int[] sections;
	public string[] textures;

	[HideInInspector]public int trueSlots;
	[HideInInspector]public string keyID;
	[HideInInspector]public int currentIndex;
	[HideInInspector]public bool isFinalLevel;
	public FrameShape shape;

	public void UpdateDetails(Map map)
	{
		frame = map.frame;
		types = map.types;
		sections = map.sections;
		textures = map.textures;

		size = map.size;
		width = map.width;
		height = map.height;
		difficulty = map.difficulty;
	}
}
