using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GenerationManager : MonoBehaviour
{
    // parent of the world
    [SerializeField] private Transform WorldGrid;
    
    // prefab of the rooms
    [SerializeField] private List<GameObject> RoomTypes;
    [SerializeField] private GameObject EmptyRoom;
    [SerializeField] private int GenEmptyChance;
    [SerializeField] private Slider EmptinessSlider;
    
    // size of the map and room
    [SerializeField] private int MapSize = 16;
    [SerializeField] private float RoomSize = 7;

    [SerializeField] private Slider MapSizeSlider;
    [SerializeField] private Button GenerateBotton;

    private int MapSizeRoot = 0;
    private float nowX = 0, nowY = 0, nowZ = 0;
    private Vector3 nowPos;
    private int nowPosTracker = 0;

    private void Update() {
        MapSize = (int)Mathf.Pow(MapSizeSlider.value, 4);
        GenEmptyChance = (int)EmptinessSlider.value;
        MapSizeRoot = (int)Mathf.Sqrt(MapSize);
    }

    public void ReloadWorld() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GenerateWorld() {

        GenerateBotton.interactable = false;

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
            Instantiate(RoomTypes[Random.Range(0, RoomTypes.Count)], nowPos, Quaternion.identity, WorldGrid);

            nowPosTracker ++;
            nowX += RoomSize;
        }
    }

}
