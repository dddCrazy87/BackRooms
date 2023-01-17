using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GenerationState {
    Idle,
    GeneratingRooms,
    GeneratingLights,
    GeneratingSpawnRoom,
    GeneratingExitRoom
}

public class GenerationManager : MonoBehaviour
{
    // parent of the world
    [SerializeField] private Transform WorldGrid;

    // the state of generation
    private GenerationState nowState;
    
    // player and camera
    [SerializeField] private GameObject Player, MainCamera;
    
    // prefabs of the rooms
    [SerializeField] private List<GameObject> RoomTypes;
    [SerializeField] private GameObject EmptyRoom, SpawnRoom, ExitRoom;
    [SerializeField] private int GenEmptyChance;
    [SerializeField] private Slider EmptinessSlider;

    // pos of rooms
    private float nowX = 0, nowY = 0, nowZ = 0;
    private Vector3 nowPos;
    private int nowPosTracker = 0;

    // pos of Spawn room
    private Vector3 SpawnPos = new(0, 0, 0), ExitPos = new(0, 0, 0);
    private int SpawnID = 0, ExitID = 0;
    
    // prefabs of the lights
    [SerializeField] private List<GameObject> LightTypes;
    [SerializeField] private int GenLightChance;
    [SerializeField] private Slider BrightnessSlider;
    
    // size of the map and room
    [SerializeField] private int MapSize, RoomSize;
    private int MapSizeRoot = 0;

    // settings when generating world
    [SerializeField] private Slider MapSizeSlider;
    [SerializeField] private Button GenerateBotton;

    private void Update() {
        MapSize = (int)Mathf.Pow(MapSizeSlider.value, 4);
        GenEmptyChance = (int)EmptinessSlider.value;
        GenLightChance = (int)BrightnessSlider.maxValue - (int)BrightnessSlider.value;
        MapSizeRoot = (int)Mathf.Sqrt(MapSize);
    }

    // reload the scene
    public void ReloadWorld() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoNextState() {
        nowState ++;
        nowX = 0; nowY = 0; nowZ = 0;
        nowPosTracker = 0;
        nowPos = Vector3.zero;
    }

    public void GenerateWorld() {

        // Add the empty rooms into the roomtypes
        for (int i = 0; i < GenEmptyChance; i++) {
            RoomTypes.Add(EmptyRoom);
        }

        // determine the pos of the Spawn Room and Exit Room
        SpawnPos = new(0, 0, 0); ExitPos = new(0, 0, RoomSize);
        SpawnID = 0; ExitID = UnityEngine.Random.Range(0, MapSize);
        do {
            SpawnID = UnityEngine.Random.Range(0, MapSize);
        } while(SpawnID == ExitID);

        // turn off Generate botton when generating world
        GenerateBotton.interactable = false;

        int AllState = Enum.GetNames(typeof(GenerationState)).Length;
        for (int StateID = 0; StateID < AllState; StateID++) {

            for (int i = 0; i < MapSize; i++) {

                // go to the next row
                if(nowPosTracker == MapSizeRoot) {
                    nowPosTracker = 0;
                    nowX = 0; nowZ += RoomSize;
                }

                // continue when the pos is for the Spawn room or the Exit room
                if (i == SpawnID) {
                    SpawnPos =  new(nowX, nowY, nowZ);
                    continue;
                }
                else if (i == ExitID) {
                    ExitPos =  new(nowX, nowY, nowZ);
                    continue;
                }
                else nowPos = new(nowX, nowY, nowZ);

                switch (nowState) {

                    // generate rooms
                    case GenerationState.GeneratingRooms: {
                        Instantiate(RoomTypes[UnityEngine.Random.Range(0, RoomTypes.Count)]
                                    , nowPos, Quaternion.identity, WorldGrid);
                        break;
                    }

                    // generate lights
                    case GenerationState.GeneratingLights: {
                        System.Random rnd = new System.Random();
                        int rand = rnd.Next(0, GenLightChance + 1);
                        if(rand <= 1) {
                            Instantiate(LightTypes[UnityEngine.Random.Range(0, LightTypes.Count)]
                                       , nowPos, Quaternion.identity, WorldGrid);
                        }
                        break;
                    }
                }

                nowPosTracker ++;
                nowX += RoomSize;
            }
            GoNextState();

            switch (nowState) {

                // generate Spawn room
                case GenerationState.GeneratingSpawnRoom: {
                    Instantiate(SpawnRoom, SpawnPos, Quaternion.identity, WorldGrid);
                    break;
                }

                // generate Exit room
                case GenerationState.GeneratingExitRoom: {
                    Instantiate(ExitRoom, ExitPos, Quaternion.identity, WorldGrid);
                    break;
                }
            }
        }
    }

    // spawn a player
    private void SpawnPlayer() {
        Player.SetActive(false);
        Player.transform.position = SpawnPos;
        Player.SetActive(true);
        MainCamera.SetActive(false);
    }
}
