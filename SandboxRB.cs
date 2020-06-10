using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxRB : MonoBehaviour
{

    private Vector2 movement;
    private Vector3 finalMovement;
    private float currentSpeed;
    private float maxSpeed;
    private int playerZ = -18;

    // physics variables
    private Vec2 feetVector;
    private ColliderLimits cLimits;
    private Vector2 forceVector;
    private float horizontalMomentum;
    private float tileGrip;
    private float colliderOffsetX;
    private float colliderOffsetY;
    private float upStep;
    public bool isGrounded;

    public float currentGrip;

    // DEBUG
    public GameObject cTesterPrefab;
    private GameObject[] cTesters;



    // inspector variables

    public float gravityForce;

    public float friction;

    void Start()
    {
        forceVector = new Vector2(0, 0);
        tileGrip = 1;
        feetVector = new Vec2(5, 5);
        gravityForce = -gravityForce;
        colliderOffsetX = transform.GetComponent<BoxCollider2D>().size.x / 2;
        colliderOffsetY = transform.GetComponent<BoxCollider2D>().size.y / 2;
        upStep = 0.02f;
        currentSpeed = 0;
        movement = new Vector2();
        finalMovement = new Vector3(0, 0, playerZ);

        if(Debugging.debuggingMovement)
        {
            CreateColliderTesters();
        }
    }

   

    public Vector2 MoveCharacter(int h_Axis, float speed)
    {
        isGrounded = CheckIfGrounded();
        cLimits = GetColliderLimits();
        // h is 1, 0, or -1. if it's 0, speed will be 0, otherwise it'll be speed in case of 1, -speed in case of -1;
        maxSpeed = speed * h_Axis;
        // calculate tile grip
        //

        // account for friction and grip
        ModifyCurrentSpeed();

        movement.x = (currentSpeed / 10) * Time.deltaTime + forceVector.x * Time.deltaTime;


        forceVector.x -= 10f * Time.deltaTime;
        if (forceVector.x < 0)
            forceVector.x = 0;

        movement.y = (gravityForce * Time.deltaTime + forceVector.y * Time.deltaTime); //  + jumpForce * Time.deltaTime 

        forceVector.y += gravityForce / 30;

        if (forceVector.y < 0)
            forceVector.y = 0;

        if(movement.x > 0)
        {
            CheckRightCollisions();
        }

        if(movement.x < 0)
        {
            CheckLeftCollisions();
        }

        if(movement.y <= 0)
        {
            CheckDownCollision();
        }

        if(movement.y > 0)
        {
            CheckUpCollision();
        }

        if(Debugging.debuggingMovement)
        {
            Debugging.xMovement = movement.x;
        }

        finalMovement.x = transform.position.x + movement.x;
        finalMovement.y = transform.position.y + movement.y;

        transform.position = finalMovement;

        return movement;
        //
 
       // movement.y = (gravityForce * Time.deltaTime) + jumpForce * Time.deltaTime; //* downwardsAcceleration *
    }

    private void ModifyCurrentSpeed()
    {


        feetVector = World.WorldToTileCoords(transform.position.x, cLimits.yMin - 0.02f);

        tileGrip = GetTileGrip();
        tileGrip *= 100;

        currentGrip = tileGrip;

        if (maxSpeed > currentSpeed)
        {
            currentSpeed += tileGrip * Time.deltaTime;

            if (currentSpeed > maxSpeed) currentSpeed = maxSpeed;
        }

        if(maxSpeed < currentSpeed)
        {
            currentSpeed -= tileGrip * Time.deltaTime;

            if (currentSpeed < maxSpeed) currentSpeed = maxSpeed;
        }


    }

    public void AddForce(Vector2 _forceVector)
    {
        forceVector = _forceVector;
    }

    private float GetTileGrip()
    {
        return World.GetTileGrip(MapData.mapArray[feetVector.x, feetVector.y].tileType);
        /*
        switch(MapData.mapArray[feetVector.x, feetVector.y].tileType)
        {
            case 5: // stone
                return 0.2f;

            default:
                return 1;
        }
        */

    }

    public bool IsGrounded()
    {
        return isGrounded;
    }


    private ColliderLimits GetColliderLimits()
    {
        float xMin = transform.position.x - colliderOffsetX;
        float xMax = transform.position.x + colliderOffsetX;
        float yMin = transform.position.y - colliderOffsetY;
        float yMax = transform.position.y + colliderOffsetY;

        return new ColliderLimits(xMin, xMax, yMin, yMax);
    }

    private void CheckRightCollisions()
    {
        Vec2 topRight = World.WorldToTileCoords(cLimits.xMax, cLimits.yMax - 0.05f); // +0.03f
        Vec2 bottomRight = World.WorldToTileCoords(cLimits.xMax, cLimits.yMin + 0.05f);

        if (MapData.mapArray[topRight.x, topRight.y].IsSolid() || MapData.mapArray[topRight.x, topRight.y - 1].IsSolid())
        {
            movement.x = 0;
            return;
        }
        if (MapData.mapArray[bottomRight.x, bottomRight.y].IsSolid())
        {
            if (!MapData.mapArray[bottomRight.x, bottomRight.y + 1].IsSolid() && !MapData.mapArray[topRight.x, topRight.y].IsSolid() && !MapData.mapArray[topRight.x, topRight.y + 1].IsSolid() && !MapData.mapArray[topRight.x - 1, topRight.y + 1].IsSolid())
            {
                movement.y = upStep;
                return;
            }

            movement.x = 0;

        }
    }

    private void CheckLeftCollisions()
    {
        Vec2 topLeft = World.WorldToTileCoords(cLimits.xMin, cLimits.yMax - 0.05f); //  - 0.03f
        Vec2 bottomLeft = World.WorldToTileCoords(cLimits.xMin, cLimits.yMin + 0.05f); // + 0.05f

        if (MapData.mapArray[topLeft.x, topLeft.y].IsSolid() || MapData.mapArray[topLeft.x, topLeft.y - 1].IsSolid())
        {
            movement.x = 0;
            return;
        }
        if (MapData.mapArray[bottomLeft.x, bottomLeft.y].IsSolid())
        {
            if (!MapData.mapArray[bottomLeft.x, bottomLeft.y + 1].IsSolid() && !MapData.mapArray[topLeft.x, topLeft.y].IsSolid() && !MapData.mapArray[topLeft.x, topLeft.y + 1].IsSolid() && !MapData.mapArray[topLeft.x + 1, topLeft.y + 1].IsSolid())
            {
                movement.y = upStep;
                return;
            }

            movement.x = 0;

        }
    }

    private void CheckUpCollision()
    {
        Vec2 topRight = World.WorldToTileCoords(cLimits.xMax - 0.02f, cLimits.yMax + 0.03f); //  - 0.02f
        Vec2 topLeft = World.WorldToTileCoords(cLimits.xMin + 0.02f, cLimits.yMax + 0.03f);


        if (MapData.mapArray[topLeft.x, topLeft.y].IsSolid() || MapData.mapArray[topRight.x, topRight.y].IsSolid())
        {
            //jumpForce = 0;
           // movement.y = 0;
            forceVector.y = 0;
        }

    }

    private void CheckDownCollision()
    {

        Vec2 bottomRight = World.WorldToTileCoords(cLimits.xMax - 0.03f, cLimits.yMin + movement.y);
        Vec2 bottomLeft = World.WorldToTileCoords(cLimits.xMin + 0.03f, cLimits.yMin + movement.y);



        if (MapData.mapArray[bottomLeft.x, bottomLeft.y].IsSolid() || MapData.mapArray[bottomRight.x, bottomRight.y].IsSolid())
        {

            float yMax = Mathf.Max(World.TileToWorldYMax(bottomLeft.y), World.TileToWorldYMax(bottomRight.y));

            // because the tiles you walk on most of the time aren't full squares
            // yMax -= 0.02f;

            if (cLimits.yMin + movement.y < yMax)
            {

                movement.y = yMax - cLimits.yMin;
            }
            else
            {
                movement.y = 0;
            }



        }

    }

    private bool CheckIfGrounded()
    {

        ColliderLimits _colliderLimits = GetColliderLimits();

        Vec2 bottomLeft = World.WorldToTileCoords(_colliderLimits.xMin + 0.03f, _colliderLimits.yMin - 0.03f); // Necessary the way this works
        Vec2 bottomRight = World.WorldToTileCoords(_colliderLimits.xMax - 0.03f, _colliderLimits.yMin - 0.03f);

        if (MapData.mapArray[bottomLeft.x, bottomLeft.y].IsSolid() || MapData.mapArray[bottomRight.x, bottomRight.y].IsSolid())
            return true;
        return false;
    }

    void Update()
    {
        if(Debugging.debuggingMovement)
        {
            UpdateDebugColliders();
        }
        
    }

    // DEBUG //

    private void UpdateDebugColliders()
    {

        cTesters[0].transform.position = new Vector3(cLimits.xMin, cLimits.yMin, 30);
        cTesters[1].transform.position = new Vector3(cLimits.xMax, cLimits.yMin, 30);
        cTesters[2].transform.position = new Vector3(cLimits.xMax, cLimits.yMax, 30);
        cTesters[3].transform.position = new Vector3(cLimits.xMin, cLimits.yMax, 30);
    }

    private void CreateColliderTesters()
    {
        cTesters = new GameObject[4];

        cTesters[0] = Instantiate(cTesterPrefab);
        cTesters[1] = Instantiate(cTesterPrefab);
        cTesters[2] = Instantiate(cTesterPrefab);
        cTesters[3] = Instantiate(cTesterPrefab);
    }
}
