using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoardEditor : EditorWindow {
	[MenuItem("Window/Board Editor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow (typeof(BoardEditor)).minSize = new Vector2(400f,200f);
	}

	void OnEnable()
	{
		SceneView.onSceneGUIDelegate += this.OnSceneGUI;
		isSmallBrush = true;
	}

	void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	}

	public bool isSmallBrush;
	public SlotType type;

	public MapCreator creator;
	public Texture2D square;
	public Texture2D green;
	public Texture2D red;
	public Texture2D black;

	public Sprite currentTex;
	public Texture2D currentFrame;

	void OnGUI()
	{
		if(creator == null) {
			GameObject creatorObj = GameObject.Find ("Map Creator");
			if(creatorObj == null) {
				Debug.LogError ("Could not find a map creator in the current scene, close board editor or change to editor scene");
				return;
			}

			creator = creatorObj.GetComponent<MapCreator> ();
			type = SlotType.Normal;
		}

		if(square == null) {
			square = Resources.Load ("square") as Texture2D;
		}
		if(red == null) {
			red = Resources.Load ("red") as Texture2D;
		}
		if(green == null) {
			green = Resources.Load ("green") as Texture2D;
		}
		if(black == null) {
			black = Resources.Load ("black") as Texture2D;
		}

		for(int i=0;i<(int)MapSize.Count;i++) {
			if(GUILayout.Button("Create New " + (MapSize)i)) {
				creator.size = (MapSize)i;
				creator.BuildPreview (null);
			}
		}

		EditorGUILayout.BeginHorizontal ();
		creator.id = EditorGUILayout.TextField ("ID: ", creator.id);
		if(GUILayout.Button("Load Map")) {
			Map map = Resources.Load ("Maps/" + creator.id) as Map;
			if (map != null) {
				creator.size = map.size;
				creator.difficulty = map.difficulty;
				creator.frame = map.frame;
				currentFrame = (Texture2D)Resources.Load ("Frames/" + map.frame);
			}

			creator.BuildPreview (map);
		}
		EditorGUILayout.EndHorizontal ();

		creator.difficulty = EditorGUILayout.IntField ("Difficulty: ",creator.difficulty);
		creator.title = EditorGUILayout.TextField ("Title: ", creator.title);

		currentFrame = (Texture2D)EditorGUILayout.ObjectField ("Frame",currentFrame, typeof(Texture2D),allowSceneObjects:true);
		if(currentFrame != null) {
			creator.frame = currentFrame.name;
		}

		EditorGUILayout.BeginHorizontal ();
		if(GUILayout.Button("Save Map to " + creator.id + ".asset")) {
			Map map = creator.Create ();

			if (!string.IsNullOrEmpty (creator.id)) {
				Map existing = AssetDatabase.LoadAssetAtPath<Map>("Assets/Resources/Maps/" + creator.id + ".asset");
				if(existing != null) {
					existing.UpdateDetails (map);
				} else {
					AssetDatabase.CreateAsset (map,"Assets/Resources/Maps/" + creator.id + ".asset");
				}
				AssetDatabase.SaveAssets ();
			}
		}

		if(GUILayout.Button("Discard Current Changes")) {
			creator.ClosePreview ();
		}
		EditorGUILayout.EndHorizontal ();

		if(GUILayout.Button("Reverse")) {
			foreach(SlotPreview preview in creator.slots) {
				if(preview.slotType == SlotType.Normal) {
					preview.slotType = SlotType.Blank;
					preview.cover.color = Color.black - new Color(0f,0f,0f,0.5f);
				} else if (preview.slotType == SlotType.Blank) {
					preview.slotType = SlotType.Normal;
					preview.cover.color = Color.white - new Color(0f,0f,0f,0.5f);
				}
			}
		}
		if(GUILayout.Button("Fixed Broken Refrences")) {
			FixEditor ();
		}

		GUILayout.Space (30f);

		string tool = "";
		switch(type) {
		case SlotType.Normal:
			tool = "Normal Brush";
			break;
		case SlotType.Safe:
			tool = "Safe Brush";
			break;
		case SlotType.Bomb:
			tool = "Bomb Brush";
			break;
		case SlotType.Blank:
			tool = "Eraser Brush";
			break;
		}
		GUILayout.Label ("Currently using " + (isSmallBrush ? "1x1 " : "9x9 ") + tool);

		EditorGUILayout.BeginHorizontal ();
		if(GUILayout.Button("Brush Tool")) {
			type = SlotType.Normal;
		}
		if(GUILayout.Button("Safe Tool")) {
			type = SlotType.Safe;
		}
		if(GUILayout.Button("Mine Tool")) {
			type = SlotType.Bomb;
		}
		if(GUILayout.Button("Eraser Tool")) {
			type = SlotType.Blank;
		}
		EditorGUILayout.EndHorizontal ();
		EditorGUILayout.BeginHorizontal ();
		if(GUILayout.Button("Small Brush")) {
			isSmallBrush = true;
		}
		if(GUILayout.Button("9x9 Brush")) {
			isSmallBrush = false;
		}
		EditorGUILayout.EndHorizontal ();

		currentTex = (Sprite)EditorGUILayout.ObjectField ("Sprite",currentTex, typeof(Sprite),allowSceneObjects:true);


		GUIStyle style = new GUIStyle (GUI.skin.label);
		style.padding = new RectOffset (0, 0, 0, 0);
		style.margin = new RectOffset (0, 0, 0, 0);

		if (creator.slots != null && creator.slots.Length == creator.xSize * creator.ySize) {
			EditorGUILayout.BeginHorizontal ();
			for (int i = 0; i < creator.xSize; i++) {
				EditorGUILayout.BeginVertical ();
				for (int j = 0; j < creator.ySize; j++) {
					int index = i + j * creator.xSize;
					Texture2D tex = square;
					if(creator.slots[index].slotType == SlotType.Safe) {
						tex = green;
					} else if(creator.slots[index].slotType == SlotType.Bomb) {
						tex = red;
					} else if(creator.slots[index].slotType == SlotType.Blank) {
						tex = black;
					}
					if (GUILayout.Button (tex, style, GUILayout.Width(20f),GUILayout.Height(20f))) {
						creator.slots [index].slotType =  type;
						if (currentTex != null) {
							creator.slots [index].cover.sprite = currentTex;
						}
						creator.slots [index].safeIcon.SetActive (false);
						creator.slots [index].bombIcon.SetActive (false);
						switch(type) {
						case SlotType.Normal:
							creator.slots [index].cover.color = Color.white - new Color (0f, 0f, 0f, 0.5f);
							break;
						case SlotType.Safe:
							creator.slots [index].cover.color = Color.white - new Color(0f,0f,0f,0.5f);
							creator.slots [index].safeIcon.SetActive (true);
							break;
						case SlotType.Bomb:
							creator.slots [index].cover.color = Color.white - new Color(0f,0f,0f,0.5f);
							creator.slots [index].bombIcon.SetActive (true);
							break;
						case SlotType.Blank:
							creator.slots [index].cover.color = Color.black - new Color(0f,0f,0f,0.5f);
							break;
						}
					}
				}
				EditorGUILayout.EndVertical ();
			}
			EditorGUILayout.BeginHorizontal ();
		}
	}

	void FixEditor()
	{
		GameObject[] objs = GameObject.FindGameObjectsWithTag ("Slot");
		creator.slots = new SlotPreview[objs.Length];
		foreach(GameObject obj in objs) {
			SlotPreview slot = obj.GetComponent<SlotPreview> ();
			creator.slots [slot.position] = slot;
		}
	}

	void OnSceneGUI(SceneView sceneView)
	{
		Event e = Event.current;
		if (e != null && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0) {
			Ray r = HandleUtility.GUIPointToWorldRay (e.mousePosition);
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (r, out hit)) {
				SlotPreview slot = hit.collider.GetComponent<SlotPreview> ();
				List<SlotPreview> targets = new List<SlotPreview> ();

				if(isSmallBrush) {
					targets.Add (slot);
				} else {
					targets.Add (slot);

					SlotPreview[] slots = creator.slots;
					int lastX = creator.xSize - 1;
					int lastY = creator.ySize - 1;

					if (slot.x != 0) {
						targets.Add (slots [slot.position - 1]);
					}
					if (slot.x != lastX) {
						targets.Add (slots [slot.position + 1]);
					}

					if (slot.y != 0) {
						targets.Add (slots [slot.position - creator.xSize]);
					}
					if (slot.y != lastY) {
						targets.Add (slots [slot.position + creator.xSize]);
					}

					if (slot.x != 0 && slot.y != 0) {
						targets.Add (slots [slot.position - (creator.xSize + 1)]);
					}
					if (slot.x != 0 && slot.y != lastY) {
						targets.Add (slots [slot.position + (creator.xSize - 1)]);
					}
					if (slot.x != lastX && slot.y != 0) {
						targets.Add (slots [slot.position - (creator.xSize - 1)]);
					}
					if (slot.x != lastX && slot.y != lastY) {
						targets.Add (slots [slot.position + (creator.xSize + 1)]);
					}

				}

				foreach(SlotPreview targ in targets) {
					targ.slotType = type;
					if (currentTex != null && type == SlotType.Normal) {
						targ.cover.sprite = currentTex;
					}
					targ.safeIcon.SetActive (false);
					targ.bombIcon.SetActive (false);
					switch (type) {
					case SlotType.Normal:
						targ.cover.color = Color.white - new Color (0f, 0f, 0f, 0.5f);
						break;
					case SlotType.Safe:
						targ.cover.color = Color.white - new Color (0f, 0f, 0f, 0.5f);
						targ.safeIcon.SetActive (true);
						break;
					case SlotType.Bomb:
						targ.cover.color = Color.white - new Color (0f, 0f, 0f, 0.5f);
						targ.bombIcon.SetActive (true);
						break;
					case SlotType.Blank:
						targ.cover.color = Color.black - new Color (0f, 0f, 0f, 0.5f);
						break;
					}
				}
			}
		}
	}
}
