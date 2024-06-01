using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : Singleton<LevelManager>
{
    [Header("Config")]
    [SerializeField] private RoomTemplate roomTemplates;
    [SerializeField] private DungeonLibrary dungeonLibrary;
    
    public RoomTemplate RoomTemplates => roomTemplates;
    public DungeonLibrary DungeonLibrary => dungeonLibrary;
    public GameObject SelectedPlayer { get; set; }
    
    private Room currentRoom;
    private int currentLevelIndex;
    private int currentDungeonIndex;
    private GameObject currentDungeonGO;

    private List<GameObject> currentLevelChestItems = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        CreatePlayer();
    }

    private void Start()
    {
        CreateDungeon();
    }

    private void CreatePlayer()
    {
        if (GameManager.Instance.Player != null)
        {
            SelectedPlayer = Instantiate(GameManager.Instance.Player.PlayerPrefab);
        }
    }
    
    private void CreateDungeon()
    {
        currentDungeonGO = Instantiate(dungeonLibrary.Levels[currentLevelIndex]
            .Dungeons[currentDungeonIndex], transform);
        currentLevelChestItems = new List<GameObject>
            (dungeonLibrary.Levels[currentLevelIndex].ChestItems.AvailableItems);
    }

    private void ContinueDungeon()
    {
        currentDungeonIndex++;
        if (currentDungeonIndex > 
            dungeonLibrary.Levels[currentLevelIndex].Dungeons.Length - 1)
        {
            currentDungeonIndex = 0;
            currentLevelIndex++;
        }
        
        Destroy(currentDungeonGO);
        CreateDungeon();
        PositionPlayer();
    }

    private void PositionPlayer()
    {
        Room[] dungeonRooms = currentDungeonGO.GetComponentsInChildren<Room>();
        Room entranceRoom = null;
        for (int i = 0; i < dungeonRooms.Length; i++)
        {
            if (dungeonRooms[i].RoomType == RoomType.RoomEntrance)
            {
                entranceRoom = dungeonRooms[i];
            }
        }

        if (entranceRoom != null)
        {
            if (SelectedPlayer != null)
            {
                SelectedPlayer.transform.position = entranceRoom.transform.position;
            }   
        }
    }

    public GameObject GetRandomItemForChest()
    {
        int randomIndex = Random.Range(0, currentLevelChestItems.Count);
        GameObject item = currentLevelChestItems[randomIndex];
        currentLevelChestItems.Remove(item);
        return item;
    }
    
    private IEnumerator IEContinueDungeon()
    {
        UIManager.Instance.FadeNewDungeon(1f);
        yield return new WaitForSeconds(2f);
        ContinueDungeon();
        UIManager.Instance.FadeNewDungeon(0f);
    }
    
    private void PlayerEnterEventCallback(Room room)
    {
        currentRoom = room;
        if (currentRoom.RoomCompleted == false)
        {
            currentRoom.CloseDoors();
        }
    }

    private void PortalEventCallback()
    {
        StartCoroutine(IEContinueDungeon());
    }
    
    private void OnEnable()
    {
        Room.OnPlayerEnterEvent += PlayerEnterEventCallback;
        Portal.OnPortalEvent += PortalEventCallback;
    }

    private void OnDisable()
    {
        Room.OnPlayerEnterEvent -= PlayerEnterEventCallback;
        Portal.OnPortalEvent -= PortalEventCallback;
    }
}
