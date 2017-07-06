using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : GenericMenuButton {

	public Image frameImage;
	public Image featureImage;
	public Image lockImage;
	public GameObject ribbon;
	[HideInInspector]
	public Map levelMap;
	bool isUnlocked;

	public void Init(Map map)
	{
		levelMap = map;
		isUnlocked = PlayerProgression.HasCompletedLevel (levelMap.keyID);
		isActive = isUnlocked;
		lockImage.enabled = !isUnlocked;
		Sprite sprite =  Resources.Load<Sprite>("Frames/" + map.frame);
		if (sprite != null)
			frameImage.sprite = sprite;
		transform.localScale = Vector3.one;

		sprite = Resources.Load<Sprite>("Features/" + map.name + "Small");
		if (sprite != null)
			featureImage.sprite = sprite;

		RectTransform trans = (RectTransform)frameImage.transform;
		switch(map.shape) {
		case FrameShape.Portrait:
			trans.sizeDelta = new Vector2 (trans.sizeDelta.x, trans.sizeDelta.x * 10 / 8);
			break;
		case FrameShape.Square:
			trans.sizeDelta = new Vector2 (trans.sizeDelta.y, trans.sizeDelta.y);
			break;
		case FrameShape.Landscape:
			trans.sizeDelta = new Vector2 (trans.sizeDelta.y * 10 / 8, trans.sizeDelta.y);
			break;
		}

		int best = PlayerProgression.GetBestScore (levelMap.name);
		ribbon.SetActive (best >= levelMap.pointTarget);
	}

	public void ShowInfo()
	{
		LevelMenu.Instance.ShowLevelInfo (levelMap);
	}
	public void HideInfo()
	{
		LevelMenu.Instance.HideLevelInfo();
	}

	public void Selectlevel()
	{
		if (isUnlocked) {
			LevelMenu.Instance.SelectLevel (levelMap);
		} else {
			MenuController.Instance.ShakeTransform(transform);
		}
	}
}
