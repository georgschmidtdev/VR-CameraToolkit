using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuousMovement : MonoBehaviour
{

    public float movementSpeedMultiplier = 1.0f;
    public float gravity = -9.81f;
    public float inputThreshold = 0.2f;
    public LayerMask collisionLayerMask;
    public XRNode inputSource;

    private float fallingSpeed;
    private Vector2 inputAxis;
    private CharacterController character;
    private XROrigin rig;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    // Update is called once per frame
    void Update()
    // Assigns values of the input method to variable "inputAxis"
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    private void FixedUpdate()
    {
        FollowHeadset();
        
        CalculateGravity();
        
        ApplyContinuousMovement();
    }

    void CalculateGravity()
    // ...
    {
        bool grounded = CheckGroundCollision();

        if (grounded)
        {
            fallingSpeed = 0.0f;
        }
        else
        {
            fallingSpeed += gravity * Time.fixedDeltaTime;
            character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
        }
    }

    void ApplyContinuousMovement()
    // Move the player according to the values of the input "inputAxis" and direction of the headset "headYaw"
    {
        Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
        if (Mathf.Abs(inputAxis.x) > inputThreshold || Mathf.Abs(inputAxis.y) > inputThreshold)
        {
            Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
            character.Move(direction * Time.fixedDeltaTime * movementSpeedMultiplier);
        }
    }

    private bool CheckGroundCollision()
    // Check if player is touching the ground
    {
        Vector3 sphereCastOrigin = transform.TransformPoint(character.center);
        float sphereCastLength = character.center.y + character.skinWidth;
        bool hit = Physics.SphereCast(sphereCastOrigin, character.radius, Vector3.down, out RaycastHit hitInfo, sphereCastLength, collisionLayerMask);
        return hit;
    }

    void FollowHeadset()
    // Make collision geometry of the character controller follow the headset position to ensure correct collision detection
    {
        character.height = rig.CameraInOriginSpaceHeight + character.skinWidth;
        Vector3 center = transform.InverseTransformPoint(rig.Camera.transform.position);
        character.center = new Vector3(center.x, character.height/2 + character.skinWidth, center.z);
    }
}
