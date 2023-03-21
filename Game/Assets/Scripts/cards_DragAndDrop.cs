using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cards_DragAndDrop : MonoBehaviour
{

    private bool moving;
    private float startPosX;
    private float startPosY;

    private Vector3 resetPosition;

    private GameObject TableCard;
    private GameObject road_go;
    private RoadAndMap road;
    private bool snap = false;
    private bool snapOff = false;
    private bool finishSnap = false;
    private Vector3 roadPosition;
    private Vector3 roadScale;

    [SerializeField] private string tipoCard;

    void Start() {
        resetPosition = this.transform.position;
        TableCard = GameObject.Find("TableCard");
        road_go = GameObject.Find("Road");
        road = road_go.GetComponent<RoadAndMap>();
    }
 
    void Update(){
        if (!finishSnap) {
            if (moving) {
                Vector3 mousePos;
                mousePos = Input.mousePosition;
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                this.gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, this.gameObject.transform.localPosition.z);
                if (Mathf.Abs(this.transform.localPosition.x - TableCard.transform.localPosition.x) <= 1f &&
                    Mathf.Abs(this.transform.localPosition.y - TableCard.transform.localPosition.y) <= 1f) {
                    roadScale = this.transform.localScale;
                } else {
                    roadScale = new Vector3(1, 1, 1);
                }
                if (tipoCard.Equals("Road")) {
                    for (int i = 0; i < road.road.Length; i++) {
                        if (Mathf.Abs(this.transform.localPosition.x - road.road[i].transform.localPosition.x) <= 0.5f &&
                            Mathf.Abs(this.transform.localPosition.y - road.road[i].transform.localPosition.y) <= 0.5f) {
                            this.transform.localPosition = road.road[i].transform.localPosition;
                            snapOff = true;
                            roadPosition = road.road[i].transform.position;
                        }
                    }
                } else {
                    for (int i = 0; i < road.map.Length; i++) {
                        if (Mathf.Abs(this.transform.localPosition.x - road.map[i].transform.localPosition.x) <= 0.5f &&
                            Mathf.Abs(this.transform.localPosition.y - road.map[i].transform.localPosition.y) <= 0.5f) {
                            this.transform.localPosition = road.map[i].transform.localPosition;
                            snapOff = true;
                            roadPosition = road.map[i].transform.position;
                        }
                    }
                }

                if (snapOff) snap = true;
                this.transform.localScale = roadScale;
            }
        }
    }

    private void OnMouseDown() {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos;
            mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            startPosX = mousePos.x  - this.transform.localPosition.x;
            startPosY = mousePos.y  - this.transform.localPosition.y;
            moving = true;
        }
    }
    private void OnMouseUp() {
        moving = false;
        if (snap) {
            this.transform.position = roadPosition;
            finishSnap = true;
        } else {
            this.transform.position = resetPosition;
        }
        snap = false;
        snapOff = false;
        roadPosition = new Vector3(0, 0, 0);
    }

}
