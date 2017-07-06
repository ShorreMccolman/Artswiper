using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapSize {
	_9x6,
	_13x8,
	_21x13,
	_33x20,

	_8x8,
	_16x16,
	_30x16,

	_16x20,

	Count
}

[SelectionBase]
public class MapCreator : MonoBehaviour {

	public string id;
	public string title;
	public int difficulty;
	public int timeLimit;
	public string frame;

	public SlotPreview[] slots;

	MapSize Size;
	public MapSize size
	{
		set{
			Size = value;
			switch(Size) {
			case MapSize._9x6:
				xSize = 9;
				ySize = 6;
				cellWidth = 70f;
				cellName = "70";
				offset = -20f;
				break;
			case MapSize._13x8:
				xSize = 13;
				ySize = 8;
				cellWidth = 50f;
				cellName = "50";
				offset = -10f;
				break;
			case MapSize._21x13:
				xSize = 21;
				ySize = 13;
				cellWidth = 30f;
				cellName = "30";
				offset = 0f;
				break;
			case MapSize._33x20:
				xSize = 33;
				ySize = 20;
				cellWidth = 20f;
				cellName = "20";
				offset = 5f;
				break;

			case MapSize._8x8:
				xSize = 8;
				ySize = 8;
				cellWidth = 50f;
				cellName = "50";
				offset = -10f;
				break;
			case MapSize._16x16:
				xSize = 16;
				ySize = 16;
				cellWidth = 25f;
				cellName = "25";
				offset = 2.5f;
				break;
			case MapSize._30x16:
				xSize = 30;
				ySize = 16;
				cellWidth = 20f;
				cellName = "20";
				offset = 5f;
				break;

			case MapSize._16x20:
				xSize = 16;
				ySize = 20;
				cellWidth = 20f;
				cellName = "20";
				offset = 5f;
				break;
			}
		}
		get{
			return Size;
		}
	}
	public int xSize;
	public int ySize;
	float cellWidth;
	float offset;
	string cellName;

	public void ClosePreview()
	{
		for (int i=0;i<slots.Length;i++) {
			if(slots[i] != null && slots[i].gameObject != null)
				DestroyImmediate (slots[i].gameObject);
		}

		slots = new SlotPreview[]{ };
	}

	public void BuildPreview(Map map)
	{
		if(slots != null) {
			ClosePreview ();
		}

		if (map != null) {
			difficulty = map.difficulty;
			timeLimit = map.timeLimit;
			title = map.title;
		}
		slots = new SlotPreview[xSize * ySize];
		for (int i = 0; i < slots.Length; i++) {
			GameObject obj = Instantiate (Resources.Load ("SlotPreview" + cellName)) as GameObject;
			SlotPreview slot = obj.GetComponent<SlotPreview> ();
			slot.x = i % xSize;
			slot.y = i / xSize;
			slot.transform.parent = transform;
			slot.transform.position = transform.position + new Vector3 (slot.x * cellWidth - ((xSize - 1) * cellWidth / 2), -slot.y * cellWidth + offset, 0);
			slot.position = i;

			if (map != null) {
				slot.slotType = map.types [i];
				if(map.textures != null && map.textures.Length == slots.Length) {
					slot.cover.sprite = Resources.Load<Sprite> ("Icons/" + map.textures [i]);
				}
				if (map.types [i] == SlotType.Blank) {
					slot.cover.color = Color.black - new Color (0f, 0f, 0f, 0.5f);
				} else if (map.types [i] == SlotType.Bomb) {
					slot.cover.color = Color.white - new Color(0f,0f,0f,0.5f);
					slot.bombIcon.SetActive (true);
				} else if (map.types [i] == SlotType.Safe) {
					slot.cover.color = Color.white - new Color(0f,0f,0f,0.5f);
					slot.safeIcon.SetActive (true);
				} else {
					slot.cover.color = Color.white - new Color(0f,0f,0f,0.5f);
				}
				slot.section = map.sections [i];
			}

			slots [i] = slot;
		}
	}

	public Map Create()
	{
		Map map = new Map ();
		map.title = title;
		map.width = xSize;
		map.height = ySize;
		map.size = size;
		map.difficulty = difficulty;
		map.timeLimit = timeLimit;
		map.frame = frame;
		map.types = new SlotType[slots.Length];
		map.sections = new int[slots.Length];
		map.textures = new string[slots.Length];
		int trueSlots = 0;
		for (int i=0;i<slots.Length;i++) {
			if(slots [i].slotType != SlotType.Blank) {
				trueSlots++;
			}
			map.types [i] = slots [i].slotType;
			map.sections [i] = slots [i].section;
			map.textures [i] = slots [i].cover.sprite.name;
			DestroyImmediate (slots[i].gameObject);
		}
		map.trueSlots = trueSlots;
		slots = new SlotPreview[]{ };

		return map;
	}
}
