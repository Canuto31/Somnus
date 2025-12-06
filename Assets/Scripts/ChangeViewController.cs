using System;
using UnityEngine;

public class ChangeViewController : MonoBehaviour
{
    public GameObject player;
    public float zToXRotation;
    public float xToZRotation;

    public void RotatePlayer(Vector3 playerForward)
    {
        if (playerForward == Vector3.back || playerForward == Vector3.forward)
        {
            player.transform.rotation = Quaternion.Euler(0, zToXRotation, 0);
        }
        else if (playerForward == Vector3.left || playerForward == Vector3.right)
        {
            player.transform.rotation = Quaternion.Euler(0, xToZRotation, 0);
        }
    }
}