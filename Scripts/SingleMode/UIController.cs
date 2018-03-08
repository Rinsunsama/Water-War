using UnityEngine;
using System.Collections;

public class UIController : MonoBehaviour {

    public GameObject operation_panel;
    public GameObject round_panel_over;
    public GameObject getAttackPower;
    public GameObject gameOverPanel;
    public GameObject auCancelIcon;
    //

    public UILabel water;
    public UILabel heroLife;
    public UILabel runPower;
    public UILabel attackPower;
    public UILabel attackRange;
    public UILabel attackAngel;
    public GameObject cam;//摄像机
    Vector3[] paths1To2 = { new Vector3(4, 10, -5), new Vector3(0, 10, 8), new Vector3(4, 10, 21) };//摄像机移动路径(玩家1到玩家2)
    Vector3[] paths2To1 = { new Vector3(4, 10, 21), new Vector3(8, 10, 8), new Vector3(4, 10, -5) };//摄像机移动路径(玩家2到玩家1)
    float getAttackPowerSpeed = 0.05f;
    // Use this for initialization
    void Start()
    {
        water.text = "圣水：" + GameController.waterPower;
    }

    // Update is called once per frame
    void Update()
    {
        water.text = "" + GameController.waterPower;
        if (GameController.selected_hero != null)
        {
            heroLife.text = ""+GameController.selected_hero.GetComponent<HeroController>().life;
            runPower.text = ""+GameController.selected_hero.GetComponent<HeroController>().run_power;
            attackPower.text = ""+GameController.selected_hero.GetComponent<HeroController>().attack_power;
        }
    }
    //面板选项触发函数
    public void operation_move()
    {
        if (!GameController.selected_hero.gameObject.GetComponent<HeroController>().is_moved)
        {

            this.gameObject.GetComponent<UIController>().operation_panel.SetActive(false);
            GameController.game_process = GameController.Game_process.Move;
            print("移动状态");
        }
        else
        {
            print("已经移动过了");
        }
    }

    public void operation_attack()
    {
        if (!GameController.selected_hero.gameObject.GetComponent<HeroController>().is_attacked)
        {
            this.gameObject.GetComponent<UIController>().operation_panel.SetActive(false);
            GameController.game_process = GameController.Game_process.GetAttackAngel;
            print("攻击状态");
        }
        else
        {
            print("已经攻击过了");
        }
    }

    public void operation_usekill()
    {
        /* if (GameController.waterPower < GameController.selected_hero.GetComponent<HeroController>().skillWaterNeed)
         {
             print("圣水不足");
             GameController.game_process = GameController.Game_process.Choose_operation;
         }
         else
         {
             this.gameObject.GetComponent<UIController>().operation_panel.SetActive(false);
             GameController.game_process = GameController.Game_process.Use_Skill;
             print("使用技能状态");
         }*/
    }
    public void round_over()
    {
        //取消移动范围显示
        if (GameController.distance != null)
        {
            for (int i = 0; i < GameController.distance.Count; i++)
            {
                int row = GridManager.instance.GetRow(GridManager.instance.GetGridIndex(((Node)GameController.distance[i]).position)) + 1;
                int col = GridManager.instance.GetColumn(GridManager.instance.GetGridIndex(((Node)GameController.distance[i]).position)) + 1;
                GameObject theCube = GameObject.Find("Cube " + row + col);
                theCube.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        //切换摄像头
        GameController.resetHero(GameController.player_side);  //重置英雄状态
        //GameController.player_side = (GameController.player_side == GameController.Player_side.player1) ? GameController.Player_side.player2 : GameController.Player_side.player1;  //转换玩家
        //GridManager.instance.ClearAllObstacle();
        //GridManager.instance.findObstacleAndCalculateWaterPower();
        if (GameController.selected_hero)
        {
            GameController.selected_hero.gameObject.GetComponentInChildren<Animator>().SetInteger("process", 0);
            GameController.selected_hero = null;           //选定英雄置空
        }
        GameController.game_process = GameController.Game_process.Wait;
        this.gameObject.GetComponent<GameController>().RoundTurn();
        print("现在是第" + GameController.roundIndex + "回合");
        operation_panel.SetActive(false);                     //隐藏操作面板
        //旋转摄像头
        if (GameController.player_side == GameController.Player_side.player2)
        {
            camera1To2();
        }
        else if (GameController.player_side == GameController.Player_side.player1)
        {
            camera2To1();
        }
    }
    public void camera1To2()
    {
        iTween.MoveTo(cam, iTween.Hash("path", paths1To2,
            "movetopath", true,
            "time", 5,
            "easetype", iTween.EaseType.linear,
            "looktarget", new Vector3(4, 0, 8),
            "lookahead", 1));
    }
    //摄像机移动(玩家2到玩家1)
    public void camera2To1()
    {
        iTween.MoveTo(cam, iTween.Hash("path", paths2To1,
            "movetopath", true,
            "time", 5,
            "easetype", iTween.EaseType.linear,
            "looktarget", new Vector3(4, 0, 8),
            "lookahead", 1));
    }
    //画出摄像机移动路径
    //void OnDrawGizmos()
    //{
    //    iTween.DrawLine(paths, Color.blue);
    //    iTween.DrawPath(paths, Color.red);
    //}

    //滚动蓄力槽
    public void GetAttackPower()
    {

        if (getAttackPower.GetComponent<UISlider>().value >= 1f || getAttackPower.GetComponent<UISlider>().value <= 0f)
        {

            getAttackPowerSpeed = -getAttackPowerSpeed;
        }

        getAttackPower.GetComponent<UISlider>().value += getAttackPowerSpeed;
    }

    public void OnGameOverButton()
    {
        Application.Quit();
    }
    public void OnAuButton()
    {
        cam.GetComponent<AudioListener>().enabled = (cam.GetComponent<AudioListener>().enabled == true) ? false : true;
        if (cam.GetComponent<AudioListener>().enabled)
            auCancelIcon.SetActive(false);
        else
            auCancelIcon.SetActive(true);
    }
}
