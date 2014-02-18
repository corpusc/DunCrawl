using UnityEngine;
using System.Collections;
// for saving/loading via BinaryFormatter
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



[Serializable]
public class TileData {
	public ObjectType Type;
	public int Pic;
}