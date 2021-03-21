using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class LevelBuilder_3 : MonoBehaviour
{
	public Room startRoomPrefab, endRoomPrefab, monsterRoomPrefab;
	public List<Room> roomPrefabs = new List<Room>();
	public Vector2 iterationRange = new Vector2(10, 20);

	public GameManager gameManager;
	public List<Vector3> poz = new List<Vector3>();

	List<Doorway> availableDoorways = new List<Doorway>();

	MonsterRoom monsterRoom;
	StartRoom startRoom;
	EndRoom endRoom;
	List<Room> placedRooms = new List<Room>();

	LayerMask roomLayerMask;

	public NavMeshSurface surface;

	public void GetSpawnPoint()
	{
		foreach (Room room in placedRooms)
		{
			List<Transform> spawnPoints = room.GetComponentsInChildren<Transform>().ToList();
			Transform spawnPoint1 = spawnPoints.Find(transform => transform.name.Equals("SpawnPoint1"));

			Transform spawnPoint2 = spawnPoints.Find(transform => transform.name.Equals("SpawnPoint2"));

			if (spawnPoint1 != null)
			{
				poz.Add(spawnPoint1.position);
			}
			if (spawnPoint2 != null)
			{
				poz.Add(spawnPoint2.position);
			}
		}
	}

	public void NewMap()
	{
		roomLayerMask = LayerMask.GetMask("Room");
		StartCoroutine("GenerateLevel");
	}

	IEnumerator GenerateLevel()
	{
		WaitForSeconds startup = new WaitForSeconds(1);
		WaitForFixedUpdate interval = new WaitForFixedUpdate();

		poz.Clear();

		if (startRoom)
		{
			Destroy(startRoom.gameObject);
		}
		yield return startup;

		// Place start room
		PlaceStartRoom();
		yield return interval;

		// Random iterations
		int iterations = Random.Range((int)iterationRange.x, (int)iterationRange.y);

		for (int i = 0; i < iterations; i++)
		{
			// Place random room from list
			PlaceRoom();
			Debug.Log("Place random room from list");
			yield return interval;
		}

		// Place monster room
		PlaceEndRoom();
		Debug.Log("Place End Room");
		yield return interval;

		PlaceMonsterRoom();
		Debug.Log("Place Monster Room");
		yield return interval;

		// Level generation finished
		Debug.Log("Level generation finished");
		GetSpawnPoint();
		
		gameManager.tpToArena2FromLevel2 = GameObject.Find("tpToArena2FromLevel2").transform;

		//NavMeshBuilder.BuildNavMesh();
		surface.BuildNavMesh();

		SetPositionEnenmies();
	}

	private void SetPositionEnenmies()
    {
		float rdn;
		foreach (Vector3 pos in poz)
		{
			rdn = Random.Range(0, 99);
			if (rdn > 20) gameManager.SpawnEnemy("Prefabs/Red Panda", pos);
			else gameManager.SpawnEnemy("Prefabs/Panda", pos);
		}
	}

	void PlaceStartRoom()
	{
		// Instantiate room
		startRoom = Instantiate(startRoomPrefab) as StartRoom;
		startRoom.transform.parent = this.transform;

		// Get doorways from current room and add them randomly to the list of available doorways
		AddDoorwaysToList(startRoom, ref availableDoorways);

		// Position room
		startRoom.transform.position = new Vector3(0.0f, 200.0f, 0.0f);
		startRoom.transform.rotation = Quaternion.identity;
	}

	void AddDoorwaysToList(Room room, ref List<Doorway> list)
	{
		foreach (Doorway doorway in room.doorways)
		{
			int r = Random.Range(0, list.Count);
			list.Insert(r, doorway);
		}
	}

	void PlaceRoom()
	{
		// Instantiate room
		Room currentRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)]) as Room;
		currentRoom.transform.parent = this.transform;

		// Create doorway lists to loop over
		List<Doorway> allAvailableDoorways = new List<Doorway>(availableDoorways);
		List<Doorway> currentRoomDoorways = new List<Doorway>();
		AddDoorwaysToList(currentRoom, ref currentRoomDoorways);

		// Get doorways from current room and add them randomly to the list of available doorways
		AddDoorwaysToList(currentRoom, ref availableDoorways);

		bool roomPlaced = false;

		// Try all available doorways
		foreach (Doorway availableDoorway in allAvailableDoorways)
		{
			// Try all available doorways in current room
			foreach (Doorway currentDoorway in currentRoomDoorways)
			{
				// Position room
				PositionRoomAtDoorway(ref currentRoom, currentDoorway, availableDoorway);
				Debug.Log("Checking overlap");

				// Check room overlaps
				if (CheckRoomOverlap(currentRoom))
				{
					continue;
				}

				roomPlaced = true;

				// Add room to list
				placedRooms.Add(currentRoom);

				// Remove occupied doorways
				currentDoorway.gameObject.SetActive(false);
				availableDoorways.Remove(currentDoorway);

				availableDoorway.gameObject.SetActive(false);
				availableDoorways.Remove(availableDoorway);

				// Exit loop if room has been placed
				break;
			}

			// Exit loop if room has been placed
			if (roomPlaced)
			{
				break;
			}
		}

		// Room couldn't be placed. Restart generator and try again
		if (!roomPlaced)
		{
			Destroy(currentRoom.gameObject);
			ResetLevelGenerator();
		}
	}

	void PositionRoomAtDoorway(ref Room room, Doorway roomDoorway, Doorway targetDoorway)
	{
		// Reset room position and rotation
		room.transform.position = Vector3.zero;
		room.transform.rotation = Quaternion.identity;

		// Rotate room to match previous doorway orientation
		Vector3 targetDoorwayEuler = targetDoorway.transform.eulerAngles;
		Vector3 roomDoorwayEuler = roomDoorway.transform.eulerAngles;
		float deltaAngle = Mathf.DeltaAngle(roomDoorwayEuler.y, targetDoorwayEuler.y);
		Quaternion currentRoomTargetRotation = Quaternion.AngleAxis(deltaAngle, Vector3.up);
		room.transform.rotation = currentRoomTargetRotation * Quaternion.Euler(0, 180f, 0);

		// Position room
		Vector3 roomPositionOffset = roomDoorway.transform.position - room.transform.position;
		room.transform.position = targetDoorway.transform.position - roomPositionOffset;
	}

	bool CheckRoomOverlap(Room room)
	{
		Vector3 roomPos = room.gameObject.transform.position;
		Vector3 pt1 = new Vector3(room.RoomBounds.center.x - room.RoomBounds.extents.x, room.RoomBounds.center.y + room.RoomBounds.extents.y, room.RoomBounds.center.z - room.RoomBounds.extents.z);
		Vector3 pt2 = new Vector3(room.RoomBounds.center.x - room.RoomBounds.extents.x, room.RoomBounds.center.y - room.RoomBounds.extents.y, room.RoomBounds.center.z + room.RoomBounds.extents.z);
		Vector3 pt3 = new Vector3(room.RoomBounds.center.x - room.RoomBounds.extents.x, room.RoomBounds.center.y + room.RoomBounds.extents.y, room.RoomBounds.center.z + room.RoomBounds.extents.z);
		Vector3 pt4 = new Vector3(room.RoomBounds.center.x + room.RoomBounds.extents.x, room.RoomBounds.center.y - room.RoomBounds.extents.y, room.RoomBounds.center.z - room.RoomBounds.extents.z);
		Vector3 pt5 = new Vector3(room.RoomBounds.center.x + room.RoomBounds.extents.x, room.RoomBounds.center.y + room.RoomBounds.extents.y, room.RoomBounds.center.z - room.RoomBounds.extents.z);
		Vector3 pt6 = new Vector3(room.RoomBounds.center.x + room.RoomBounds.extents.x, room.RoomBounds.center.y - room.RoomBounds.extents.y, room.RoomBounds.center.z + room.RoomBounds.extents.z);
		Vector3 pt7 = new Vector3(-5, 200, 7);
		Vector3 pt8 = new Vector3(-5, 200, -7);
		Vector3 pt9 = new Vector3(15, 200, -4);
		Vector3 pt10 = new Vector3(15, 200, 4);
		Vector3 pt11 = new Vector3(-4, 200, 2);
		Vector3 pt12 = new Vector3(-11, 200, 1);

		foreach (Room r in placedRooms)
		{
			if (r.RoomBounds.Intersects(room.RoomBounds) ||
				r.RoomBounds.Contains(roomPos) ||
				r.RoomBounds.Contains(room.RoomBounds.min)
				|| r.RoomBounds.Contains(room.RoomBounds.max)
				|| r.RoomBounds.Contains(pt1)
				|| r.RoomBounds.Contains(pt2)
				|| r.RoomBounds.Contains(pt3)
				|| r.RoomBounds.Contains(pt4)
				|| r.RoomBounds.Contains(pt5)
				|| r.RoomBounds.Contains(pt6)
				|| r.RoomBounds.Contains(pt7)
				|| r.RoomBounds.Contains(pt8)
				|| r.RoomBounds.Contains(pt9)
				|| r.RoomBounds.Contains(pt10)
				|| r.RoomBounds.Contains(pt11)
				|| r.RoomBounds.Contains(pt12))
			{
				Debug.LogError("Overlap Detected");
				return true;
			}
		}

		return false;
	}

	void PlaceMonsterRoom()
	{
		// Instantiate room
		monsterRoom = Instantiate(monsterRoomPrefab) as MonsterRoom;
		monsterRoom.transform.parent = this.transform;

		// Create doorway lists to loop over
		List<Doorway> allAvailableDoorways = new List<Doorway>(availableDoorways);
		Doorway doorway = monsterRoom.doorways[0];

		bool roomPlaced = false;

		// Get all placed room
		// Try all available doorways
		foreach (Doorway availableDoorway in allAvailableDoorways)
		{
			// Position room
			Room room = (Room)monsterRoom;
			PositionRoomAtDoorway(ref room, doorway, availableDoorway);
			Debug.Log("Checking overlap");

			if (CheckRoomOverlap(monsterRoom))
			{
				continue;
			}

			Debug.Log("Monster Room placed");
			roomPlaced = true;

			// Remove occupied doorways
			doorway.gameObject.SetActive(false);
			availableDoorways.Remove(doorway);

			availableDoorway.gameObject.SetActive(false);
			availableDoorways.Remove(availableDoorway);

			// Exit loop if room has been placed
			break;
		}

		// Room couldn't be placed. Restart generator and try again
		if (!roomPlaced)
		{
			ResetLevelGenerator();
		}
	}

	void PlaceEndRoom()
	{
		// Instantiate room
		endRoom = Instantiate(endRoomPrefab) as EndRoom;
		endRoom.transform.parent = this.transform;

		// Create doorway lists to loop over
		List<Doorway> allAvailableDoorways = new List<Doorway>(availableDoorways);
		Doorway doorway = endRoom.doorways[0];

		bool roomPlaced = false;

		// Try all available doorways
		foreach (Doorway availableDoorway in allAvailableDoorways)
		{
			// Position room
			Room room = (Room)endRoom;
			PositionRoomAtDoorway(ref room, doorway, availableDoorway);
			Debug.Log("Checking overlap");

			// Check room overlaps
			if (CheckRoomOverlap(endRoom))
			{
				continue;
			}

			Debug.Log("Room placed");
			roomPlaced = true;
			placedRooms.Add((Room)endRoom);

			// Remove occupied doorways
			doorway.gameObject.SetActive(false);
			availableDoorways.Remove(doorway);

			availableDoorway.gameObject.SetActive(false);
			availableDoorways.Remove(availableDoorway);

			// Exit loop if room has been placed
			break;
		}

		// Room couldn't be placed. Restart generator and try again
		if (!roomPlaced)
		{
			ResetLevelGenerator();
		}
	}

	void ResetLevelGenerator()
	{
		Debug.LogError("Reset level generator");

		StopCoroutine("GenerateLevel");

		// Delete all rooms
		if (startRoom)
		{
			Destroy(startRoom.gameObject);
		}

		if (endRoom)
		{
			Destroy(endRoom.gameObject);
		}

		if (monsterRoom)
		{
			Destroy(monsterRoom.gameObject);
		}

		foreach (Room room in placedRooms)
		{
			Destroy(room.gameObject);
		}

		// Clear lists
		placedRooms.Clear();
		availableDoorways.Clear();

		// Reset coroutine
		StartCoroutine("GenerateLevel");
	}
}
