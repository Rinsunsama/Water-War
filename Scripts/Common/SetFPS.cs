using UnityEngine;

using System.Collections;

public class SetFPS : MonoBehaviour
{
    public int FPS;
    void Awake()
    {

        Application.targetFrameRate = FPS;//此处限定60帧

    }

}