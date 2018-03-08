using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

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
 //   public UILabel attackRange;
   // public UILabel attackAngel;
    public GameObject cam;//摄像机
    Vector3[] paths1To2 = { new Vector3(4, 10, -5), new Vector3(0, 10, 8), new Vector3(4, 10, 21) };//摄像机移动路径(玩家1到玩家2)
    Vector3[] paths2To1 = { new Vector3(4, 10, 21), new Vector3(8, 10, 8), new Vector3(4, 10, -5) };//摄像机移动路径(玩家2到玩家1)
    float getAttackPowerSpeed = 0.05f;
    // Use this for initialization
    void Start()
    {
        water.text = ""+ GameManager.waterPower;
    }

    // Update is called once per frame
    void Update()
    {
        water.text = "" + GameManager.waterPower;
        if (GameManager.selected_hero != null)
        {
            heroLife.text = "" + GameManager.selected_hero.GetComponent<HeroManager>().life;
            runPower.text = "" + GameManager.selected_hero.GetComponent<HeroManager>().run_power;
            attackPower.text = "" + GameManager.selected_hero.GetComponent<HeroManager>().attack_power;
        }
    }
    //面板选项触发函数
    public void operation_move()
    {
        if (!GameManager.selected_hero.gameObject.GetComponent<HeroManager>().is_moved)
        {

            this.gameObject.GetComponent<UIManager>().operation_panel.SetActive(false);
            GameManager.game_process = GameManager.Game_process.Move;
            print("移动状态");
        }
        else
        {
            print("已经移动过了");
        }
    }

    public void operation_attack()
    {
        if (!GameManager.selected_hero.gameObject.GetComponent<HeroManager>().is_attacked)
        {
            this.gameObject.GetComponent<UIManager>().operation_panel.SetActive(false);
            GameManager.game_process = GameManager.Game_process.GetAttackAngel;
            print("攻击状态");
        }
        else
        {
            print("已经攻击过了");
        }
    }

    public void operation_usekill()
    {
        /* if (GameManager.waterPower < GameManager.selected_hero.GetComponent<HeroManager>().skillWaterNeed)
         {
             print("圣水不足");
             GameManager.game_process = GameManager.Game_process.Choose_operation;
         }
         else
         {
             this.gameObject.GetComponent<UIManager>().operation_panel.SetActive(false);
             GameManager.game_process = GameManager.Game_process.Use_Skill;
             print("使用技能状态");
         }*/
    }
    public void round_over()
    {
        //取消移动范围显示
        if (GameManager.distance != null)
        {
            for (int i = 0; i < GameManager.distance.Count; i++)
            {
                int row = GridManager.instance.GetRow(GridManager.instance.GetGridIndex(((Node)GameManager.distance[i]).position)) + 1;
                int col = GridManager.instance.GetColumn(GridManager.instance.GetGridIndex(((Node)GameManager.distance[i]).position)) + 1;
                GameObject theCube = GameObject.Find("Cube " + row + col);
                theCube.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        //切换摄像头
        GameManager.resetHero(GameManager.player_side);  //重置英雄状态
        print("现在是第" + GameManager.roundIndex + "回合");
        //GameManager.player_side = (GameManager.player_side == GameManager.Player_side.player1) ? GameManager.Player_side.player2 : GameManager.Player_side.player1;  //转换玩家方
        //GridManager.instance.ClearAllObstacle();//将所有障碍物标志取消
        //GetComponent<NetworkView>().RPC("ClearAllObstacle", RPCMode.All);
        //GridManager.instance.findObstacleAndCalculateWaterPower();
        //GetComponent<NetworkView>().RPC("findObstacleAndCalculateWaterPower", RPCMode.All);
        /*  if (GameManager.player_side == GameManager.Player_side.player2)
          {
              camera1To2();
          }
          else if (GameManager.player_side == GameManager.Player_side.player1)
          {
              camera2To1();
          }*/
        if (GameManager.selected_hero)
        {
            GameManager.selected_hero.gameObject.GetComponentInChildren<Animator>().SetInteger("process", 0);
            GameManager.selected_hero = null;           //选定英雄置空
            cam.GetComponent<CameraManager>().CameraPosReset();
        }
        GameManager.game_process = GameManager.Game_process.Wait;
        this.gameObject.GetComponent<NetworkView>().RPC("RoundTurn", RPCMode.AllBuffered);
        operation_panel.SetActive(false);                     //隐藏操作面板

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
       cam.GetComponent<AudioListener>().enabled = (cam.GetComponent<Camera>().GetComponent<AudioListener>().enabled == true) ? false : true;
        if (cam.GetComponent<AudioListener>().enabled)
            auCancelIcon.SetActive(false);
        else
            auCancelIcon.SetActive(true);
    }
}
