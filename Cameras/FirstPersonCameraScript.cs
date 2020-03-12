using UnityEngine;

public class FirstPersonCameraScript : MonoBehaviour
{
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

    public Vector3 AngularDisplacement {
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

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        MainCamera = Camera.main;
        CameraTransform = transform;
        Rotation = CameraTransform.localRotation;
        Position = CameraTransform.localPosition;
    }

    private void Update()
    {
        SmoothlyRotate(Space.World);
        SmoothlyTranslate(Space.Self);
    }

    private void LateUpdate()
    {
        CameraTransform.localRotation = Quaternion.Slerp(CameraTransform.localRotation, Rotation, Time.deltaTime / (rotationSmoothness / 100));
        CameraTransform.localPosition = Vector3.Lerp(CameraTransform.localPosition, Position, Time.deltaTime / (movementSmoothness / 100));
        //CameraTransform.localPosition = Vector3.MoveTowards(CameraTransform.localPosition, Position, Time.deltaTime / (movementSmoothness / 100));
    }

    public void SmoothlyRotate(Space space)
    {
        // Rotation done with the mouse scrollwheel is too slow, so we need to multiply the speed by another value.
        angularDisplacement.z = Input.GetAxis("Mouse ScrollWheel") * rotationSpeed * 10f;
        angularDisplacement.x = -Input.GetAxis("Mouse Y") * rotationSpeed;
        angularDisplacement.y = Input.GetAxis("Mouse X") * rotationSpeed;

        Roll += angularDisplacement.z;
        Pitch += angularDisplacement.x;
        Yaw += CameraTransform.up.y >= 0 ? angularDisplacement.y : -angularDisplacement.y; // Yaw is inverted when the camera is upside down.

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
                Rotation = Quaternion.AngleAxis(angularDisplacement.z, CameraTransform.forward) * 
                    Quaternion.AngleAxis(angularDisplacement.x, CameraTransform.right) * 
                    Quaternion.AngleAxis(angularDisplacement.y, CameraTransform.up) * 
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
                Position += CameraTransform.TransformDirection(displacement); 
                break;
        }
    }
}

/* References:
 * https://codingchronicles.com/unity-vr-development/day-8-creating-character-first-person-shooter
 * https://answers.unity.com/questions/13561/first-person-camera-is-extremely-jerky.html
 * https://www.flipcode.com/archives/Main_Loop_with_Fixed_Time_Steps.shtml
 * https://www.gamedev.net/forums/topic/624285-smooth-Rotation-and-movement-of-camera/
 */
