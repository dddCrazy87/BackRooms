using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GenerationState {
    Idle,
    GeneratingRooms,
    GeneratingLights
}

public class GenerationManager : MonoBehaviour
{
    // parent of the world
    [SerializeField] private Transform WorldGrid;
    
    // prefabs of the rooms
    [SerializeField] private List<GameObject> RoomTypes;
    [SerializeField] private GameObject EmptyRoom;
    [SerializeField] private int GenEmptyChance;
    [SerializeField] private Slider EmptinessSlider;

    // prefabs of the lights
    [SerializeField] private List<GameObject> LightTypes;
    [SerializeField] private int GenLightChance;
    [SerializeField] private Slider BrightnessSlider;
    
    // size of the map and room
    [SerializeField] private int MapSize = 16;
    [SerializeField] private float RoomSize = 7;

    // settings when generating world
    [SerializeField] private Slider MapSizeSlider;
    [SerializeField] private Button GenerateBotton;

    private int MapSizeRoot = 0;
    private float nowX = 0, nowY = 0, nowZ = 0;
    private Vector3 nowPos;
    private int nowPosTracker = 0;
    private GenerationState nowState;

    private void Update() {
        MapSize = (int)Mathf.Pow(MapSizeSlider.value, 4);
        GenEmptyChance = (int)EmptinessSlider.value;
        GenLightChance = (int)BrightnessSlider.maxValue - (int)BrightnessSlider.value;
        MapSizeRoot = (int)Mathf.Sqrt(MapSize);
    }

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

        GenerateBotton.interactable = false;

        int AllState = Enum.GetNames(typeof(GenerationState)).Length;
        for (int StateID = 0; StateID < AllState; StateID++) {

            for (int i = 0; i < GenEmptyChance; i++) {
                RoomTypes.Add(EmptyRoom);
            }

            for (int i = 0; i < MapSize; i++) {

                if(nowPosTracker == MapSizeRoot) {
                    nowPosTracker = 0;
                    nowX = 0;
                    nowZ += RoomSize;
                }
                nowPos = new(nowX, nowY, nowZ);

                switch (nowState) {
                    case GenerationState.GeneratingRooms: {
                        Instantiate(RoomTypes[UnityEngine.Random.Range(0, RoomTypes.Count)]
                                    , nowPos, Quaternion.identity, WorldGrid);
                        break;
                    }

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
        }
    }


}
