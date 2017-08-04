using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour {

	public enum State
	{
		Hidden,
		Flagged,
		Open,
		Blank
	}

	public SlotType slotType;

	public bool hasBomb;
	public int nearby;

	public int index;
	public int position;
	public int x;
	public int y;
	public int size;
	public int section;
	public string texture;

	public State state = State.Hidden;

	public Text label;
	public Image image;
	public Image cover;

	bool active;
	bool hasSent;

	public bool IsEmpty
	{
		get{
			return slotType != SlotType.Blank && !hasBomb && nearby == 0;
		}
	}

	public void Reset()
	{
		hasBomb = false;
		hasSent = false;
		nearby = 0;
		index = 0;
		position = 0;
		x = 0;
		y = 0;
		size = 0;
		section = 0;
		state = State.Hidden;
		label.text = "";
		image.enabled = true;
		image.sprite = SlotPool.Instance.FrameSprite;
		image.color = Color.white;
		cover.enabled = true;
		cover.sprite = SlotPool.Instance.HiddenSprite;
	}

	public void SetDisplay()
	{
		if(slotType == SlotType.Blank) {
			label.enabled = false;
			cover.enabled = false;
			image.enabled = false;
		} else if(hasBomb) {
			label.text = "";
			image.sprite = SlotPool.Instance.BombSprite;
		} else if (nearby > 0) {
			SetNearby ();
		} else {
			label.text = "";
		}
	}

	public void SetNearby()
	{
		label.enabled = true;
		label.text = nearby.ToString ();

		if (nearby == 1) {
			label.color = new Color(40f / 255f, 34f / 255f, 219f / 255f);
		} else if (nearby == 2) {
			label.color = new Color(176f / 255f, 14f / 255f, 14f / 255f);
		} else if (nearby == 3) {
			label.color = new Color(17f / 255f, 128f / 255f, 22f / 255f);
		} else if (nearby == 4) {
			label.color = Color.cyan;
		} else if (nearby == 5) {
			label.color = Color.magenta;
		} else if (nearby == 6) {
			label.color = Color.yellow;
		} else if (nearby == 7) {
			label.color = Color.white;
		} else if (nearby == 8) {
			label.color = Color.black;
		}

	}

	void Update()
	{
		if(active && cover.enabled && !AppManager.Instance.ScreenIsCovered) {
			if (Input.GetMouseButtonUp (0) && !Input.GetKey(KeyCode.LeftControl)) {
				bool didFlip = TryFlip ();
				if(didFlip)
					SoundController.PlaySoundEffect (Sounds.FLIP);
			}

			if (Input.GetMouseButtonUp (1) || ( Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftControl) )) {
				Flag ();
			}
		}
	}

	public void ForceFlip()
	{
		if (hasSent)
			return;
		hasSent = true;

		if(state == State.Blank || !cover.enabled) {

		} else {
			Flip (true);
		}

		if(hasBomb && state != State.Flagged) {
			GameObject blast = Instantiate (Resources.Load("Particles/Blast")) as GameObject;
			blast.transform.position = transform.position;

			GameObject light = Instantiate (Resources.Load("Particles/Light" + size)) as GameObject;
			light.transform.position = transform.position + Vector3.back * 10f;

			BoardGenerator.Instance.AddActiveLight (light);
		}
			
		Invoke ("SendFlipEnd", 0.02f);
	}

	public bool TryFlip(bool isEnd = false)
	{
		if(isEnd) {
			ForceFlip();
			return false;
		}

		if(state == State.Flagged || state == State.Blank || !cover.enabled) {
			return false;
		}
			
		Flip ();

		if(hasBomb) {

			GameObject blast = Instantiate (Resources.Load("Particles/Blast")) as GameObject;
			blast.transform.position = transform.position;

			GameObject light = Instantiate (Resources.Load("Particles/Light" + size)) as GameObject;
			light.transform.position = transform.position + Vector3.back * 10f;

			BoardGenerator.Instance.AddActiveLight (light);

			PlayerProgression.TriggerBomb ();
			SoundController.PlaySoundEffect (Sounds.HIT,true);

			image.color = Color.magenta;
			GameController.HitBomb ();
			hasSent = true;
			Invoke ("SendFlipEnd", 0.2f);
		} else if (IsEmpty) {
			Invoke ("SendFlip", 0.02f);
		}

		if(nearby == 7) {
			PlayerProgression.FlipNumber7 ();
		}

		return true;
	}

	public void SendFlipEnd()
	{
		BoardGenerator.Instance.FlipEmptySlot (position,true);
	}
	public void SendFlip()
	{
		BoardGenerator.Instance.FlipEmptySlot (position,false);
	}

	public void Flag()
	{
		if(state == State.Hidden) {
			if(GameController.CurrentState.remainingFlags <= 0) {
				return;
			}

			state = State.Flagged;
			cover.sprite = SlotPool.Instance.FlagSprite;
			GameController.FlagSpace (true, hasBomb);
			SoundController.PlaySoundEffect (Sounds.FLAG);

		} else if (state == State.Flagged) {
			state = State.Hidden;
			cover.sprite = Resources.Load<Sprite> ("Icons/" + texture);
			GameController.FlagSpace (false, hasBomb);
		}
	}

	public void Flip(bool end = false)
	{
		if (state == State.Hidden) {
			state = State.Open;
			cover.enabled = false;

			if (!hasBomb && !end)
				GameController.FlipSafeSpace ();

			if (IsEmpty)
				image.enabled = false;
		} else if (state == State.Flagged) {
			if (end) {
				if (hasBomb) {
					cover.sprite = SlotPool.Instance.CheckSprite;
				} else {
					cover.sprite = SlotPool.Instance.XSprite;
				}
			}
		}

		if (end) {
			if (nearby > 0 && state != State.Flagged) {
				//StartCoroutine (DropSlot ());
			}
		}
	}

	public void Activate()
	{
		active = true;	
	}
	public void Deactivate()
	{
		active = false;
	}

	IEnumerator DropSlot()
	{
		yield return new WaitForSeconds (3f);
		float rand = Random.Range (0f, 1f);
		yield return new WaitForSeconds (rand);

		float speed = 0.1f;
		while(transform.position.y > -100f) {
			speed += 5f * Time.deltaTime;
			transform.position += new Vector3 (0f, -speed, 0f);
			yield return null;
		}
	}
}
