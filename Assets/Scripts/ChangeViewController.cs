using System;
using UnityEngine;

public class ChangeViewController : MonoBehaviour
{
    public GameObject player;
    public float rotationValue;

    public void RotatePlayer()
    {
        player.transform.rotation = Quaternion.Euler(0, rotationValue, 0);
    }
}
