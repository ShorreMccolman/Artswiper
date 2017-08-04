using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour {
	public static BoardGenerator Instance;
	void Awake()
	{Instance = this;}

	[SerializeField]
	private Transform anchor;

	private Map currentMap;
	public Map CurrentMap
	{
		get{
			return currentMap;
		}
		private set{
			if (value == null)
				Debug.LogError ("Current map was set to null");

			currentMap = value;
		}
	}

	Slot[] slots = new Slot[]{};
	Dictionary<int,List<Slot>> blockDict = new Dictionary<int, List<Slot>>();
	List<GameObject> activeLights = new List<GameObject>();

#if UNITY_EDITOR
	void Update()
	{
		// Auto win
		if (Input.GetKeyDown (KeyCode.W)) {
			HUD.Instance.running = false;
			GameController.Instance.currentState.victory = true;
			BoardGenerator.Instance.FlipBoard ();
			MenuController.OpenMenu (EndgamePopup.Instance);
		}
	}
#endif

	// This is used when time runs out or when we have cleared all of the safe spaces and want to reveal the board
	public void FlipBoard()
	{
		for(int i=0;i<slots.Length;i++) {
			slots [i].Flip (true);
		}
	}

	// When we flip an empty slot we also want to flip over each adject slot
	// isEnd forces the adjacent block to act as an empty slot to flip the entire board
	public void FlipEmptySlot(int index, bool isEnd)
	{
		if( index % CurrentMap.width != 0) {
			slots [index - 1].TryFlip (isEnd);
		}
		if(index % CurrentMap.width != (CurrentMap.width - 1)) {
			slots [index + 1].TryFlip (isEnd);
		}
		if(index / CurrentMap.width != 0) {
			slots [index - CurrentMap.width].TryFlip (isEnd);
		}
		if(index / CurrentMap.width != (CurrentMap.height - 1)) {
			slots [index + CurrentMap.width].TryFlip (isEnd);
		}
		if( index % CurrentMap.width != 0 && index / CurrentMap.width != 0) {
			slots [index - (CurrentMap.width + 1)].TryFlip (isEnd);
		}
		if( index % CurrentMap.width != 0 && index / CurrentMap.width != (CurrentMap.height - 1)) {
			slots [index + (CurrentMap.width - 1)].TryFlip (isEnd);
		}
		if( index % CurrentMap.width != (CurrentMap.width - 1) && index / CurrentMap.width != 0) {
			slots [index - (CurrentMap.width - 1)].TryFlip (isEnd);
		}
		if( index % CurrentMap.width != (CurrentMap.width - 1) && index / CurrentMap.width != (CurrentMap.height - 1)) {
			slots [index + (CurrentMap.width + 1)].TryFlip (isEnd);
		}
	}


	// This used to be used to flip an entire white block at a time. No longer used
	public void FlipBlock(int index)
	{
		List<Slot> list = blockDict [index];
		for(int i=0;i<list.Count;i++) {
			list [i].Flip ();

			int pos = list [i].position;
			if( pos % CurrentMap.width != 0) {
				slots [pos - 1].Flip ();
			}
			if(pos % CurrentMap.width != (CurrentMap.width - 1)) {
				slots [pos + 1].Flip ();
			}
			if(pos / CurrentMap.width != 0) {
				slots [pos - CurrentMap.width].Flip ();
			}
			if(pos / CurrentMap.width != (CurrentMap.height - 1)) {
				slots [pos + CurrentMap.width].Flip ();
			}
			if( pos % CurrentMap.width != 0 && pos / CurrentMap.width != 0) {
				slots [pos - (CurrentMap.width + 1)].Flip ();
			}
			if( pos % CurrentMap.width != 0 && pos / CurrentMap.width != (CurrentMap.height - 1)) {
				slots [pos + (CurrentMap.width - 1)].Flip ();
			}
			if( pos % CurrentMap.width != (CurrentMap.width - 1) && pos / CurrentMap.width != 0) {
				slots [pos - (CurrentMap.width - 1)].Flip ();
			}
			if( pos % CurrentMap.width != (CurrentMap.width - 1) && pos / CurrentMap.width != (CurrentMap.height - 1)) {
				slots [pos + (CurrentMap.width + 1)].Flip ();
			}
		}
	}

	// Clean up the board to create a new one.
	public void WipeBoard()
	{
		if (slots == null)
			return;
		
		for(int i=0;i<slots.Length;i++) {
			SlotPool.Instance.ReturnSlot (slots [i]);
		}

		for(int i=0;i<activeLights.Count;i++) {
			Destroy (activeLights [i].gameObject);
		}
		activeLights = new List<GameObject> ();
	}

	// Actually create the board
	public Board CreateBoardFromMap(Map map)
	{
		WipeBoard ();

		int lastX = map.width - 1;
		int lastY = map.height - 1;

		currentMap = map;

		List<int> sections = new List<int> ();
		for(int i=0;i<map.sections.Length;i++) {
			if (!sections.Contains (map.sections [i])) {
				sections.Add (map.sections [i]);
			}
		}

		int predetermined = 0;

		Vector3 resVector = new Vector3 (0f, 0f, 0f);
		switch(map.size) {
		case MapSize._9x6:
			resVector = new Vector3(740f,530f,70f);
			break;
		case MapSize._13x8:
			resVector = new Vector3(760f,510f,50f);
			break;
		case MapSize._21x13:
			resVector = new Vector3(740f,500f,30f);
			break;
		case MapSize._33x20:
			resVector = new Vector3(770f,510f,20f);
			break;

		case MapSize._8x8:
			resVector = new Vector3(510f,510f,50f);
			break;
		case MapSize._16x16:
			resVector = new Vector3(510f,510f,25f);
			break;
		case MapSize._30x16:
			resVector = new Vector3(860f,510f,25f);
			break;

		case MapSize._16x20:
			resVector = new Vector3(430f,510f,20f);
			break;
		}

		Sprite sprite = Resources.Load<Sprite> ("Features/" + CurrentMap.name);
		if(sprite == null)
			sprite = Resources.Load<Sprite> ("Features/default");
		HUD.Instance.feature.sprite = sprite;
		HUD.Instance.frame.rectTransform.sizeDelta = new Vector2 (resVector.x, resVector.y);

		Sprite frame = Resources.Load<Sprite>("Frames/" + map.frame + "Large");
		if (frame != null)
			HUD.Instance.frame.sprite = frame;

		// Generate Slots
		slots = new Slot[map.width * map.height];
		List<Slot> slotList = new List<Slot> ();
		for (int i = 0; i < slots.Length; i++) {
			Slot slot = SlotPool.Instance.GetSlot();
			slot.x = i % map.width;
			slot.y = i / map.width;
			slot.slotType = map.types [i];
			slot.section = map.sections [i];
			slot.position = i;
			slot.hasBomb = false;
			slot.size = (int)resVector.z;

			if (map.textures != null && map.textures.Length == slots.Length) {
				slot.texture = map.textures [i];
				slot.cover.sprite = Resources.Load<Sprite> ("Icons/" + map.textures [i]);
			}

			slot.transform.parent = anchor;
			slot.transform.localScale = Vector3.one;
			slot.transform.localPosition = new Vector3 (slot.x * resVector.z - ((CurrentMap.width - 1) * resVector.z * 0.5f), -slot.y * resVector.z + ((CurrentMap.height - 1) * resVector.z * 0.5f), 0);
			switch(map.size) {
			case MapSize._9x6:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 70f;
				break;
			case MapSize._13x8:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 50f;
				break;
			case MapSize._21x13:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 30f;
				break;
			case MapSize._33x20:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 20f;
				break;

			case MapSize._8x8:
				((RectTransform)slot.transform).sizeDelta = new Vector2 (50f, 50f);
				break;
			case MapSize._16x16:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 25f;
				break;
			case MapSize._30x16:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 25f;
				break;
			case MapSize._16x20:
				((RectTransform)slot.transform).sizeDelta = Vector2.one * 20f;
				break;
			}

			slots [i] = slot;
			if (slot.slotType == SlotType.Normal) {
				slotList.Add (slot);
			} else if (slot.slotType == SlotType.Bomb) {
				predetermined++;
				slot.hasBomb = true;
			}
		}

		// Add bombs to random slots
		for (int i = 0; i < map.difficulty - predetermined; i++) {
			Slot slot = slotList [Random.Range (0, slotList.Count)];
			slot.hasBomb = true;
			slotList.Remove (slot);
		}
		HUD.Instance.UpdateRemaining (map.difficulty);

		// Determine number of nearby bombs
		for (int i = 0; i < slots.Length; i++) {
			Slot slot = slots [i];
			if (!slot.hasBomb) {
				if (slot.x != 0) {
					if (slots [i - 1].hasBomb)
						slot.nearby++;
				}
				if (slot.x != lastX) {
					if (slots [i + 1].hasBomb)
						slot.nearby++;
				}

				if (slot.y != 0) {
					if (slots [i - CurrentMap.width].hasBomb)
						slot.nearby++;
				}
				if (slot.y != lastY) {
					if (slots [i + CurrentMap.width].hasBomb)
						slot.nearby++;
				}

				if (slot.x != 0 && slot.y != 0) {
					if (slots [i - (CurrentMap.width + 1)].hasBomb)
						slot.nearby++;
				}
				if (slot.x != 0 && slot.y != lastY) {
					if (slots [i + (CurrentMap.width - 1)].hasBomb)
						slot.nearby++;
				}
				if (slot.x != lastX && slot.y != 0) {
					if (slots [i - (CurrentMap.width - 1)].hasBomb)
						slot.nearby++;
				}
				if (slot.x != lastX && slot.y != lastY) {
					if (slots [i + (CurrentMap.width + 1)].hasBomb)
						slot.nearby++;
				}
			}
		}

		// Setup display for each slot
		for (int i = 0; i < slots.Length; i++) {
			slots [i].SetDisplay ();
		}

		// Create empty block dict and determine which block each empty square belongs to
		blockDict = new Dictionary<int, List<Slot>> ();
		List<Slot> list = new List<Slot> ();
		List<int> changes = new List<int> ();
		int index = 1;
		for (int i = 0; i < slots.Length; i++) {
			if (slots [i].IsEmpty) {
				slots [i].index = index;
				list.Add (slots [i]);

				if (i / CurrentMap.width != 0) {
					int aboveIndex = slots [i - CurrentMap.width].index;
					if (slots [i - CurrentMap.width].IsEmpty && aboveIndex != index && !changes.Contains (aboveIndex)) {
						changes.Add (aboveIndex);
					}

					if (i % CurrentMap.width != 0) {
						aboveIndex = slots [i - CurrentMap.width - 1].index;
						if (slots [i - CurrentMap.width - 1].IsEmpty && aboveIndex != index && !changes.Contains (aboveIndex)) {
							changes.Add (aboveIndex);
						}
					}
					if (i % CurrentMap.width != CurrentMap.width - 1) {
						aboveIndex = slots [i - CurrentMap.width + 1].index;
						if (slots [i - CurrentMap.width + 1].IsEmpty && aboveIndex != index && !changes.Contains (aboveIndex)) {
							changes.Add (aboveIndex);
						}
					}
				}

				if (i % CurrentMap.width == (CurrentMap.width - 1) || !slots [i + 1].IsEmpty) {
					foreach (int dex in changes) {
						foreach (Slot slot in blockDict[dex]) {
							slot.index = index;
							list.Add (slot);
						}
						blockDict.Remove (dex);
					}
					changes = new List<int> ();

					blockDict.Add (index, list);
					list = new List<Slot> ();
					index++;
				}
			}
		}


		if(GameSettings.ShowHint) {
			int[] arr = new int[blockDict.Keys.Count];
			blockDict.Keys.CopyTo (arr, 0);

			int[] highestKeys = new int[sections.Count];
			int[] highest = new int[sections.Count];
			for (int i = 0; i < arr.Length; i++) {
				List<Slot> currentList = blockDict [arr [i]];
				int sec = currentList [0].section;

				for (int j = 0; j < sections.Count; j++) {
					if (sections [j] == sec) {
						if (currentList.Count > highest [j]) {
							highest [j] = blockDict [arr [i]].Count;
							highestKeys [j] = arr [i];
						}
					}
				}
			}
			for (int i = 0; i < highestKeys.Length; i++) {
				List<Slot> listOfBlocks = blockDict [highestKeys [i]];
				Slot highlight = listOfBlocks [Random.Range (0, listOfBlocks.Count)];
				while (highlight.section != sections [i]) {
					highlight = listOfBlocks [Random.Range (0, listOfBlocks.Count)];
				}
				
				highlight.cover.sprite = SlotPool.Instance.HighlightSprite;
			}
		}

		Board board = new Board ();
		board.map = currentMap;
		board.slots = slots;
		board.remainingSpaces = CurrentMap.trueSlots - CurrentMap.difficulty;
		return board;
	}

	// Wipe the current board and reload
	public Board ReloadBoard()
	{
		if(currentMap != null) {
			return CreateBoardFromMap (currentMap);
		}

		Debug.LogError ("Tried to reload null map");
		return null;
	}

	// Add the alarm light gameobject to a list so we can destroy them all when we wipe the board
	public void AddActiveLight(GameObject light)
	{
		activeLights.Add (light);
	}
}
