using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPool : MonoBehaviour {
	public static SlotPool Instance;
	void Awake()
	{Instance = this;}

	public GameObject slotPrefab;
	public int slotPool;
	Stack<Slot> availableSlots = new Stack<Slot>();

	[HideInInspector]public Sprite FrameSprite;
	[HideInInspector]public Sprite BombSprite;
	[HideInInspector]public Sprite HiddenSprite;
	[HideInInspector]public Sprite FlagSprite;
	[HideInInspector]public Sprite CheckSprite;
	[HideInInspector]public Sprite XSprite;
	[HideInInspector]public Sprite HighlightSprite;

	void Start()
	{
		BombSprite = Resources.Load<Sprite> ("Icons/Alarm");
		HiddenSprite = Resources.Load<Sprite> ("Icons/Hidden");
		FrameSprite = Resources.Load<Sprite> ("Icons/ButtonNew");
		FlagSprite = Resources.Load<Sprite> ("Icons/Flag");
		CheckSprite = Resources.Load<Sprite> ("Icons/Check");
		XSprite = Resources.Load<Sprite> ("Icons/X");
		HighlightSprite = Resources.Load<Sprite> ("Icons/Highlight");
	}


	public void InstantiateSlotPool()
	{
		availableSlots = new Stack<Slot>();
		for(int i=0;i<slotPool;i++) {
			GameObject obj = Instantiate (slotPrefab) as GameObject;
			Slot slot = obj.GetComponent<Slot> ();
			slot.transform.parent = transform;
			slot.gameObject.SetActive (false);
			availableSlots.Push (slot);
		}
	}

	public void ReturnSlot(Slot slot)
	{
		slot.gameObject.SetActive (false);
		slot.transform.parent = transform;
		availableSlots.Push (slot);
	}

	public Slot GetSlot() {
		Slot slotToUse = null;
		slotToUse = availableSlots.Pop ();
		slotToUse.gameObject.SetActive (true);
		slotToUse.Reset ();
		return slotToUse;
	}
}
