using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player: MonoBehaviour
{
    GameObject road;
    RoadAndMap roadScript;

    [SerializeField] private float CicloPorMin = 1f;
    int waypointIndex = 1;


    string dir = "direita";

    // Start is called before the first frame update
    void Start() {
        road = GameObject.Find("Road");
        roadScript = road.GetComponent<RoadAndMap>();
        transform.position = roadScript.road[waypointIndex].transform.position;
    }

    // Update is called once per frame
    void Update() {
        Move();
    }
    
    void Move() {
        transform.position = Vector2.MoveTowards(transform.position,
                                                roadScript.road[waypointIndex].transform.position,
                                                (roadScript.road.Length / (CicloPorMin * 60)) * Time.deltaTime);

        if (transform.position == roadScript.road[waypointIndex].transform.position) waypointIndex ++;
        if (waypointIndex == roadScript.road.Length) waypointIndex = 0;


        switch (dir) {
            case "direita":
                if (transform.position.y > roadScript.road[waypointIndex].transform.position.y) {
                    transform.Rotate(0.0f, 0.0f, -90.0f, Space.Self);
                    dir = "baixo";
                } else if (transform.position.y < roadScript.road[waypointIndex].transform.position.y) {
                    transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
                    dir = "cima";
                }
                break;
            case "baixo":
                if (transform.position.x > roadScript.road[waypointIndex].transform.position.x) {
                    transform.Rotate(0.0f, 0.0f, -90.0f, Space.Self);
                    dir = "esquerda";
                } else if (transform.position.x < roadScript.road[waypointIndex].transform.position.x) {
                    transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
                    dir = "direita";
                }
                break;
            case "esquerda":
                if (transform.position.y > roadScript.road[waypointIndex].transform.position.y) {
                    transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
                    dir = "baixo";
                } else if (transform.position.y < roadScript.road[waypointIndex].transform.position.y) {
                    transform.Rotate(0.0f, 0.0f, -90.0f, Space.Self);
                    dir = "cima";
                }
                break;
            case "cima":
                if (transform.position.x > roadScript.road[waypointIndex].transform.position.x) {
                    transform.Rotate(0.0f, 0.0f, 90.0f, Space.Self);
                    dir = "esquerda";
                } else if (transform.position.x < roadScript.road[waypointIndex].transform.position.x) {
                    transform.Rotate(0.0f, 0.0f, -90.0f, Space.Self);
                    dir = "direita";
                }
                break;
        }
    }

}

