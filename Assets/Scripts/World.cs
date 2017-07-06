using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New World",menuName = "World")]
public class World : ScriptableObject {
	public string displayName;
	public Map[] maps;
}
