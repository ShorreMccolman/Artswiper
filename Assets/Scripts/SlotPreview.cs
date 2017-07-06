using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SlotType {
	Normal,
	Safe,
	Bomb,
	Blank
}

[ExecuteInEditMode]
public class SlotPreview : MonoBehaviour {

	public int position;
	public int x;
	public int y;
	public int section;

	public SlotType slotType;

	public Image cover;
	public GameObject bombIcon;
	public GameObject safeIcon;
}
