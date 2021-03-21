using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public Doorway[] doorways;
	public MeshCollider meshCollider;


	public Bounds RoomBounds {
		get { return meshCollider.bounds; }
	}

}
