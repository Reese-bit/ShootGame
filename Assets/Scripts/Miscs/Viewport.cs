using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : Singleton<Viewport>
{
    float minX;
    float maxX;
    float minY;
    float maxY;
    float halfX;

    public float MaxX => maxX;

    // Start is called before the first frame update
    void Start()
    {
        Camera mainCamera = Camera.main;

        Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector2(0f,0f));
        Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector2(1f,1f));

        //the edge points
        minX = bottomLeft.x;
        minY = bottomLeft.y;
        maxX = topRight.x;
        maxY = topRight.y;

        halfX = mainCamera.ViewportToWorldPoint(new Vector3(0.5f,0f,0f)).x;
    }

    public Vector3 PlayerMoveablePosition(Vector3 playerPosition,float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Mathf.Clamp(playerPosition.x,minX + paddingX,maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y,minY + paddingY,maxY - paddingY);

        return position;

    }

    public Vector3 RandomlyEnemySpawnPosition(float paddingX,float paddingY)
    {
        Vector3 position =Vector3.zero;

        position.x = maxX + paddingX;
        position.y = Random.Range(minY + paddingY,maxY - paddingY);

        return position;
    }

    public Vector3 RandomEnemyRightHalfPosition(float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(halfX,maxX - paddingX);
        position.y = Random.Range(minY + paddingY,maxY - paddingY);

        return position;
    }

    public Vector3 RandomEnemyMovingPosition(float paddingX,float paddingY)
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(minX + paddingX, maxX - paddingX);
        position.y = Random.Range(minY + paddingY, maxY - paddingY);

        return position;
    }

}
