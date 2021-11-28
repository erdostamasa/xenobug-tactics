using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform groundTransform;


    [SerializeField] float movementSpeed;
    [SerializeField] float movementTime;
    [SerializeField] float rotationAmount;
    [SerializeField] Vector3 zoomAmount;
    [SerializeField] float mouseZoomMultiplier = 1;
    [SerializeField] float maxZoom = -1;
    [SerializeField] float minZoom = -10;

    [SerializeField] Vector3 newPosition;
    [SerializeField] Quaternion newRotation;
    [SerializeField] Vector3 newZoom;

    [Header("Game end setting")]
    [SerializeField] float orbitDuration = 10f;
    [SerializeField] float snapDuration = 3f;
    [SerializeField] float zoomDuration = 4f;
    [SerializeField] Vector3 endZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    bool gameEnded = false;


    void Awake() {
        EventManager.instance.onGameEnded += GameEnded;
    }

    void Start() {
        transform.position = groundTransform.position;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    public void GameEnded() {
        gameEnded = true;
        transform.DOMove(groundTransform.position, snapDuration).SetEase(Ease.OutQuart);
        //transform.DOLocalRotate(new Vector3(0, 360, 0), 1, RotateMode.FastBeyond360).SetLoops(-1);
        transform.DORotate(new Vector3(0, -360, 0), orbitDuration, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Incremental);
        cameraTransform.DOLocalMove(endZoom, zoomDuration).SetEase(Ease.OutQuart);
    }


    void Update() {
        if (!gameEnded) {
            HandleMouseInput();
            HandleKeyboardInput();
        }
    }

    void HandleMouseInput() {
        if (Input.mouseScrollDelta.y != 0) {
            if (Input.mouseScrollDelta.y > 0 && newZoom.z < maxZoom) {
                newZoom += Input.mouseScrollDelta.y * zoomAmount * mouseZoomMultiplier;
            }

            if (Input.mouseScrollDelta.y < 0 && newZoom.z > minZoom) {
                newZoom += Input.mouseScrollDelta.y * zoomAmount * mouseZoomMultiplier;
            }
        }

        if (Input.GetMouseButtonDown(2)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry)) {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(2)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry)) {
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition = transform.position + (dragStartPosition - dragCurrentPosition);
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    void HandleKeyboardInput() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPosition += (transform.forward * movementSpeed);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            newPosition += (transform.forward * -movementSpeed);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            newPosition += (transform.right * movementSpeed);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            newPosition += (transform.right * -movementSpeed);
        }


        if (Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        if (Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        if (Input.GetKey(KeyCode.R) && newZoom.z < maxZoom) {
            newZoom += zoomAmount;
        }

        if (Input.GetKey(KeyCode.F) && newZoom.z > minZoom) {
            newZoom -= zoomAmount;
        }


        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}