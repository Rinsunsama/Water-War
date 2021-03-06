﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }
    //摄像头移动到英雄位置后七格
    public void cameraMove(Vector3 pos, float time)
    {

        if (GameController.player_side == GameController.Player_side.player1)
        {
            //iTween.MoveTo(maincCamera, iTween.Hash("position", new Vector3(4, 10, -5), "time", 2.0f));
            iTween.MoveTo(this.gameObject, new Vector3(pos.x, 10, pos.z - 7), time);
        }
        else if (GameController.player_side == GameController.Player_side.player2)
        {
            //iTween.MoveTo(mainCamera, iTween.Hash("position", new Vector3(4, 10, 21), "time", 2.0f));
            iTween.MoveTo(this.gameObject, new Vector3(pos.x, 10, pos.z + 7), time);
        }
    }
    //判断摄像机是否在原位
    public bool IsCameraDefault()
    {

        if (GameController.player_side == GameController.Player_side.player1 && this.gameObject.transform.position == new Vector3(4, 10, -5))
        {
            return true;
        }
        else if (GameController.player_side == GameController.Player_side.player2 && this.gameObject.transform.position == new Vector3(4, 10, 21))
        {
            return true;
        }
        else
            return false;
    }
    //摄像头重置
    public void CameraPosReset()
    {
        if (!IsCameraDefault())
        {
            if (GameController.selected_hero == null)
            {
                iTween.RotateTo(this.gameObject, new Vector3(40, this.gameObject.transform.localEulerAngles.y, this.gameObject.transform.localEulerAngles.z), 2.0f);
                if (GameController.player_side == GameController.Player_side.player1)
                {
                    //iTween.MoveTo(maincCamera, iTween.Hash("position", new Vector3(4, 10, -5), "time", 2.0f));
                    iTween.MoveTo(this.gameObject, new Vector3(4, 10, -5), 2.0f);
                }
                else if (GameController.player_side == GameController.Player_side.player2)
                {
                    //iTween.MoveTo(mainCamera, iTween.Hash("position", new Vector3(4, 10, 21), "time", 2.0f));
                    iTween.MoveTo(this.gameObject, new Vector3(4, 10, 21), 2.0f);
                }
            }
        }
    }
    //摄像头拉近
    public void CameraInto()
    {
        //Vector3 cameraPosition = this.gameObject.transform.position;
        Vector3 pos = GameController.selected_hero.transform.position;
        string heroName = GameController.selected_hero.name.Substring(0, GameController.selected_hero.tag.Length - 1 - 6);//-6 = (clone)
        iTween.RotateTo(this.gameObject, new Vector3(10, this.gameObject.transform.localEulerAngles.y, this.gameObject.transform.localEulerAngles.z), 2.0f);
        //this.gameObject.transform.RotateAround(transform.position, new Vector3(1, 0, 0), -30f);
        if (GameController.player_side == GameController.Player_side.player1)
        {
            //iTween.MoveTo(this.gameObject, new Vector3(cameraPosition.x, cameraPosition.y - 3, cameraPosition.z + 3), 2.0f);
            //print(new Vector3(cameraPosition.x, 5, cameraPosition.z));
            if (heroName == "Sword")
            {
                print(heroName);
                iTween.MoveTo(this.gameObject, new Vector3(pos.x, pos.y + 3, pos.z - 5.5f), 2.0f);
            }
            else
            {
                print(heroName);
                iTween.MoveTo(this.gameObject, new Vector3(pos.x, pos.y + 3, pos.z - 4f), 2.0f);
            }
        }
        else if (GameController.player_side == GameController.Player_side.player2)
        {
            //iTween.MoveTo(this.gameObject, new Vector3(cameraPosition.x, cameraPosition.y - 3, cameraPosition.z - 3), 2.0f);
            //print(new Vector3(cameraPosition.x, 5, cameraPosition.z));
            if (heroName == "Sword")
            {
                iTween.MoveTo(this.gameObject, new Vector3(pos.x, pos.y + 3, pos.z + 5.5f), 2.0f);
            }
            else
            {
                iTween.MoveTo(this.gameObject, new Vector3(pos.x, pos.y + 3, pos.z + 4f), 2.0f);
            }
        }
    }
    //摄像头拉近
    public void CameraIntoLeft(Vector3 selectHeroPosition)
    {
        Vector3 cameraPosition = this.gameObject.transform.position;

        if (GameController.player_side == GameController.Player_side.player1)
        {
            iTween.MoveTo(this.gameObject, new Vector3(selectHeroPosition.x - 5, selectHeroPosition.y + 5, selectHeroPosition.z), 2.0f);
            this.gameObject.transform.eulerAngles = new Vector3(this.gameObject.transform.localEulerAngles.x, 90, this.gameObject.transform.localEulerAngles.z);
        }
        else if (GameController.player_side == GameController.Player_side.player2)
        {
            iTween.MoveTo(this.gameObject, new Vector3(cameraPosition.x, cameraPosition.y - 3, cameraPosition.z - 3), 2.0f);
        }
    }
    //摄像头远离(撤回原来位置)
    public void CameraOut()
    {
        Vector3 cameraPosition = this.gameObject.transform.position;

        if (GameController.player_side == GameController.Player_side.player1)
        {
            iTween.MoveTo(this.gameObject, new Vector3(cameraPosition.x, cameraPosition.y + 3, cameraPosition.z - 3), 2.0f);
            print(new Vector3(cameraPosition.x, 5, cameraPosition.z));
        }
        else if (GameController.player_side == GameController.Player_side.player2)
        {
            iTween.MoveTo(this.gameObject, new Vector3(cameraPosition.x, cameraPosition.y + 3, cameraPosition.z + 3), 2.0f);
            print(new Vector3(cameraPosition.x, 5, cameraPosition.z));
        }
    }
}
