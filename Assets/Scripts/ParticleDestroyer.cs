﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke ("Destroy", 1f);
	}

	public void Destroy()
	{
		Destroy (this.gameObject);
	}

}
