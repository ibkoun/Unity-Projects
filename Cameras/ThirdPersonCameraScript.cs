using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraScript : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // The game object we are currently looking at.
    [SerializeField] private Vector3 offset; // The relative position of the camera to the target.
    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementSmoothness;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothness;
    [SerializeField] private ConstrainedVector3 rotationConstraints;

    private Vector3 displacement, angularDisplacement;

    public float MovementSpeed
    {
        get { return movementSpeed; }
        set { movementSpeed = value; }
    }

    public float RotationSpeed
    {
        get { return rotationSpeed; }
        set { rotationSpeed = value; }
    }

    public Vector3 Displacement
    {
        get { return displacement; }
        set { displacement = value; }
    }

    public Vector3 AngularDisplacement
    {
        get { return angularDisplacement; }
        set { angularDisplacement = value; }
    }

    public float Pitch { get; set; }

    public float Yaw { get; set; }

    public float Roll { get; set; }

    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }

    public Camera MainCamera { get; set; }

    public Transform CameraTransform { get; set; }

    public Transform TargetTransform { get; set; }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        MainCamera = Camera.main;
        if (CameraTransform == null) CameraTransform = transform;
    }

    void Start()
    {
        TargetTransform = targetObject.transform;
        Rotation = TargetTransform.localRotation;
        Position = TargetTransform.localPosition;
    }

    private void Update()
    {
        SmoothlyRotate(Space.World);
        TargetTransform.localRotation = Quaternion.Slerp(TargetTransform.localRotation, Rotation, Time.deltaTime / (rotationSmoothness / 100));
        SmoothlyTranslate(Space.Self);
        TargetTransform.localPosition = Vector3.Lerp(TargetTransform.localPosition, Position, Time.deltaTime / (movementSmoothness / 100));
        //CameraTransform.localPosition = Vector3.MoveTowards(CameraTransform.localPosition, Position, Time.deltaTime / (movementSmoothness / 100));
    }

    private void LateUpdate()
    {
        CameraTransform.localRotation = Quaternion.Slerp(CameraTransform.localRotation, Rotation, Time.deltaTime / (rotationSmoothness / 100)); // Make the camera look in the direction where the player is facing.
        CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, Position, Time.deltaTime / (movementSmoothness / 100)); // Make the camera follow the player from a fixed distance.
        CameraTransform.localPosition = TargetTransform.localPosition + (CameraTransform.localRotation * offset);
    }

    public void SmoothlyRotate(Space space)
    {
        // Rotation done with the mouse scrollwheel is too slow, so we need to multiply the speed by another value.
        angularDisplacement.z = Input.GetAxis("Mouse ScrollWheel") * rotationSpeed * 10f;
        angularDisplacement.x = -Input.GetAxis("Mouse Y") * rotationSpeed;
        angularDisplacement.y = Input.GetAxis("Mouse X") * rotationSpeed;

        Roll += angularDisplacement.z;
        Pitch += angularDisplacement.x;
        Yaw += TargetTransform.up.y >= 0 ? angularDisplacement.y : -angularDisplacement.y; // Yaw is inverted when the camera is upside down.

        rotationConstraints.z = Roll;
        Roll = rotationConstraints.z > 360 || rotationConstraints.z < -360 ? rotationConstraints.z % 360 : rotationConstraints.z;
        rotationConstraints.x = Pitch;
        Pitch = rotationConstraints.x > 360 || rotationConstraints.x < -360 ? rotationConstraints.x % 360 : rotationConstraints.x;
        rotationConstraints.y = Yaw;
        Yaw = rotationConstraints.y > 360 || rotationConstraints.y < -360 ? rotationConstraints.y % 360 : rotationConstraints.y;

        switch (space)
        {
            // Rotation in world space.
            case Space.World:
                
                Rotation = Quaternion.Euler(Pitch, Yaw, 0);
                break;
            // Rotation in local space.
            case Space.Self:
                Rotation = Quaternion.AngleAxis(angularDisplacement.z, TargetTransform.forward) *
                    Quaternion.AngleAxis(angularDisplacement.x, TargetTransform.right) *
                    Quaternion.AngleAxis(angularDisplacement.y, TargetTransform.up) *
                    Rotation;
                break;
        }
    }

    public void SmoothlyTranslate(Space space)
    {
        displacement.x = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        displacement.y = Input.GetAxis("Jump") * movementSpeed * Time.deltaTime;
        displacement.z = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        switch (space)
        {
            // Translation in world space.
            case Space.World:
                Position += displacement;
                break;
            // Translation in local space.
            case Space.Self:
                Position += TargetTransform.TransformDirection(displacement);
                break;
        }
    }

    private void OnValidate()
    {
        if (CameraTransform == null) CameraTransform = transform;
        CameraTransform.localPosition = offset;
    }
}
