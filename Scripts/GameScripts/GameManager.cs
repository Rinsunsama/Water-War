using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject allPanel;
   // public CameraMove camMover;
    public GameObject mainCamera;
    public static int player1Life;
    public static int player2Life;
    public static Player_side player_side;
    public static Game_process game_process;
    public static GameObject selected_hero;//被选中英雄
    public GameObject selectEffect;
    public static GameObject goal_position;//目标位置
    public static float waterPower;          //圣水
    public static int roundIndex = 0;          //当前游戏回合编号
    public ArrayList pathArray;//路径
    public static ArrayList distance;
    public static Vector3[] waterEyes = { new Vector3(1, 0, 3), new Vector3(4, 0, 8), new Vector3(7, 0, 13) };//泉眼位置
    public static Vector3 player1HolySpring = new Vector3(4, 2, 0);
    public static Vector3 player2HolySpring = new Vector3(4, 2, 16);
    static float angleY;          //角度
    static Vector3 attackForwardEulerAngles;
    static float attackDistance;    //攻击力度
    int networkPlayer;
    Vector3[] wayPoints;
    int pointi;
    bool moveProcessOver = false;//移动过程结束（暂时没用上）
    public static int whosRound;  //标记是谁的回合
    public static int whoWin = 0;
    public GameObject win;
    public GameObject lost;
    public GameObject draw;
    //游戏进程
    public enum Game_process
    {
        Wait, //不是自己的回合，等待阶段
        Choose_hero,  //选择英雄
        Choose_operation, //选择操作
        Move,           //移动
        GetAttackAngel,          //获取攻击角度
        GetAttackPower,        //获取攻击力度
        Use_Skill,           //使用技能
        Game_over        //游戏结束
    }
    //游戏玩家
    public enum Player_side
    {
        player1,     //游戏玩家1
        player2      //游戏玩家2
    }

    // Use this for initialization
    void Start()
    {
        whosRound = 0;
        waterPower = 5;
        roundIndex = 1;
        //获取当前的networkPlayer
        networkPlayer = int.Parse(Network.player + "");
        player_side = Player_side.player1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (game_process)
        {
            case Game_process.Wait:
                wait();
                break;
            case Game_process.Choose_hero:
                choose_hero();
                break;
            case Game_process.Choose_operation:
                choose_operation();
                break;
            case Game_process.Move:
                move();
                break;
            case Game_process.GetAttackAngel:
                getAttackAngel();
                break;
            case Game_process.GetAttackPower:
                getAttackPower();
                break;
            case Game_process.Game_over:
                gameOver();
                break;
            default:
                break;
            //实时获取当前选取英雄的坐标
        }
    }

    //选取英雄函数
    public void choose_hero()
    {
        if (selected_hero != null)
        {
            selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 0);
            selected_hero = null;
            mainCamera.GetComponent<CameraManager>().CameraPosReset();
        }   
        allPanel.SetActive(true);
        this.gameObject.GetComponent<UIManager>().getAttackPower.SetActive(false);
        this.gameObject.GetComponent<UIManager>().operation_panel.SetActive(false);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit) && (hit.transform.tag == "Player1_hero" || hit.transform.tag == "Player2_hero"))
        {
            if (hit.transform.gameObject.GetComponent<HeroManager>().is_attacked)
                print("该英雄已经行动完毕");
            else
            {
                selectEffect.GetComponent<AudioSource>().Play();
                if (player_side == Player_side.player1)
                {
                    if (hit.transform.tag == "Player1_hero")
                    {
                        selected_hero = hit.transform.gameObject;
                        //camMover.Follow(selected_hero.transform);
                        mainCamera.GetComponent<CameraManager>().cameraMove(selected_hero.transform.position, 2.0f);
                        print("你选的是玩家1的英雄");
                        print("可移动格数:" + Convert.ToInt32(waterPower / selected_hero.gameObject.GetComponent<HeroManager>().run_power));
                        game_process = Game_process.Choose_operation;
                    }
                }
                else
                {
                    if (hit.transform.tag == "Player2_hero")
                    {
                        selected_hero = hit.transform.gameObject;
                        mainCamera.GetComponent<CameraManager>().cameraMove(selected_hero.transform.position, 2.0f);
                        print("你选的是玩家2的英雄");
                        game_process = Game_process.Choose_operation;
                    }
                }
            }
        }
    }
    //选取操作函数
    public void choose_operation()
    { //改变选定英雄动画to选定待定在状态。
         if (selected_hero)
            selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 1);
        this.gameObject.GetComponent<UIManager>().operation_panel.SetActive(true);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            if (player_side == Player_side.player1 && hit.transform.tag == "Player1_hero")
            {
                selected_hero = hit.transform.gameObject;
               mainCamera.GetComponent<CameraManager>().cameraMove(selected_hero.transform.position, 2.0f);
            }
            else if (player_side == Player_side.player2 && hit.transform.tag == "Player2_hero")
            {
                selected_hero = hit.transform.gameObject;
                mainCamera.GetComponent<CameraManager>().cameraMove(selected_hero.transform.position, 2.0f);
            }
            else if (hit.transform.tag != "Operation_panle" && !HitTestUI())
            {
                selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 0);
                game_process = Game_process.Choose_hero;
                selected_hero = null;           //置空选定英雄
                mainCamera.GetComponent<CameraManager>().CameraPosReset();
                print("返回选取英雄阶段");
            }
        }
       
      
    }
    //移动函数
    public void move()
    {
        if (!selected_hero.gameObject.GetComponent<HeroManager>().is_moved)
        {
            //GetComponent<NetworkView>().RPC("ClearAllObstacle", RPCMode.All);          //清楚障碍物
            GetComponent<NetworkView>().RPC("findObstacleAndCalculateWaterPower", RPCMode.All);//计算障碍物
            //显示可移动范围
            Node heroNode = new Node(selected_hero.transform.position);
            distance = AStar.checkDistance(heroNode, (int)(waterPower / selected_hero.gameObject.GetComponent<HeroManager>().run_power));
            for (int i = 0; i < distance.Count; i++)
            {
                int row = GridManager.instance.GetRow(GridManager.instance.GetGridIndex(((Node)distance[i]).position)) + 1;
                int col = GridManager.instance.GetColumn(GridManager.instance.GetGridIndex(((Node)distance[i]).position)) + 1;
                GameObject theCube = GameObject.Find("Cube " + row + col);
                theCube.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            //射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
            {
                //目标位置gameobject类型
                goal_position = hit.transform.gameObject;
                //目标位置node类型
                Node goalNode = new Node(goal_position.transform.position);
                //if (hit.transform.tag != "Obstacle")
                if (!GridManager.instance.IsObstacle(GridManager.instance.CalculateAtNodes(goalNode)))
                {
                    int moveDistance = FindPathAndReturnDistance();
                    if (moveDistance * selected_hero.gameObject.GetComponent<HeroManager>().run_power > waterPower)
                    {
                        print("圣水不足");
                        game_process = Game_process.Choose_operation;
                    }
                    else
                    {

                        selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 2);
                        //开始移动
                        MoveToNode();

                        print("选择位置成功");
                        //camMover.Follow(goal_position.transform);
                        selected_hero.gameObject.GetComponent<HeroManager>().is_moved = true;
                        waterPower -= moveDistance * selected_hero.gameObject.GetComponent<HeroManager>().run_power;

                    }
                    //取消移动显示范围
                    for (int i = 0; i < distance.Count; i++)
                    {
                        int row = GridManager.instance.GetRow(GridManager.instance.GetGridIndex(((Node)distance[i]).position)) + 1;
                        int col = GridManager.instance.GetColumn(GridManager.instance.GetGridIndex(((Node)distance[i]).position)) + 1;
                        GameObject theCube = GameObject.Find("Cube " + row + col);
                        theCube.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
                //爆炸点
                else
                {
                    print("该单元格为障碍物，无法到达！");
                    //取消攻击显示范围
                    for (int i = 0; i < distance.Count; i++)
                    {
                        int row = GridManager.instance.GetRow(GridManager.instance.GetGridIndex(((Node)distance[i]).position)) + 1;
                        int col = GridManager.instance.GetColumn(GridManager.instance.GetGridIndex(((Node)distance[i]).position)) + 1;
                        GameObject theCube = GameObject.Find("Cube " + row + col);
                        theCube.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                    game_process = Game_process.Choose_operation;
                }
            }

        }
    }
    //攻击函数
    public void getAttackAngel()
    {
        if (selected_hero.GetComponent<HeroManager>().is_attacked == false)
        {
            selected_hero.GetComponent<HeroManager>().ShowAttackAngel();
            if (Input.GetMouseButtonDown(0))
            {
                selected_hero.GetComponent<HeroManager>().attackForwardRspeed = 0;
                angleY = selected_hero.GetComponent<HeroManager>().attackForward.transform.eulerAngles.y;
                attackForwardEulerAngles = selected_hero.GetComponent<HeroManager>().attackForward.transform.eulerAngles;
                string heroName = selected_hero.name.Substring(0, selected_hero.tag.Length - 1 - 6);//-6 = (clone)
                if (heroName == "Sword")
                {
                    //剑士直接攻击
                    if (GameManager.player_side == GameManager.Player_side.player1)
                    {
                        selected_hero.GetComponent<HeroManager>().Chop("Team2", attackForwardEulerAngles);
                        selected_hero.transform.eulerAngles = new Vector3(0, attackForwardEulerAngles.y, 0);//英雄转向箭头方向
                        selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 3);
                        selected_hero.GetComponent<NetworkView>().RPC("attackAudioPlay", RPCMode.All);
                        
                    }
                    else if (GameManager.player_side == GameManager.Player_side.player2)
                    {
                        selected_hero.GetComponent<HeroManager>().Chop("Team1", attackForwardEulerAngles);
                        selected_hero.transform.eulerAngles = new Vector3(0, attackForwardEulerAngles.y + 180.0f, 0);//英雄转向箭头方向
                        selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 3);
                        selected_hero.GetComponent<NetworkView>().RPC("attackAudioPlay", RPCMode.All);
                    }
                    selected_hero.GetComponent<HeroManager>().HideAttackAngel();//隐藏攻击显示的范围
                    selected_hero.GetComponent<HeroManager>().is_attacked = true;
                    //HeroTurnToFront();//英雄转向对面
                    game_process = Game_process.Choose_hero;

                }
                else
                {
                    //其它英雄出现蓄力槽
                    game_process = Game_process.GetAttackPower;
                }
            }
        }
    }
    public void getAttackPower()
    {
        if (selected_hero.GetComponent<HeroManager>().is_attacked == false)
        {
            GetComponent<UIManager>().getAttackPower.SetActive(true);
            GetComponent<UIManager>().GetAttackPower();
            if (Input.GetMouseButtonDown(0))
            {
                selected_hero.GetComponent<HeroManager>().is_attacked = true;
                attackDistance = selected_hero.GetComponent<HeroManager>().attack_range * GetComponent<UIManager>().getAttackPower.GetComponent<UISlider>().value;
                selected_hero.GetComponent<HeroManager>().HideAttackAngel();//隐藏攻击显示的范围
                GetComponent<UIManager>().getAttackPower.SetActive(false);
                //发射炮弹
                
                selected_hero.GetComponent<HeroManager>().ShootBullet(angleY, attackDistance);
                if (GameManager.player_side == GameManager.Player_side.player1)
                {
                    selected_hero.transform.eulerAngles = new Vector3(0, attackForwardEulerAngles.y, 0);//英雄转向箭头方向
                    selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 3);
                    selected_hero.GetComponent<NetworkView>().RPC("attackAudioPlay", RPCMode.All);
                }
                else if (GameManager.player_side == GameManager.Player_side.player2)
                {
                    selected_hero.transform.eulerAngles = new Vector3(0, attackForwardEulerAngles.y + 180.0f, 0);//英雄转向箭头方向
                    selected_hero.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 3);
                   selected_hero.GetComponent<NetworkView>().RPC("attackAudioPlay", RPCMode.All);
                }
                //HeroTurnToFront();//英雄转向对面
               
            }
        }
    }

    /*  //使用技能函数
      public void use_skill()
      {
          switch (selected_hero.GetComponent<HeroManager>().skillName)
          {
              case "咆哮":
                  Skill_control.dragonKnight_name();
                  break;
              case "天火":
                  Skill_control.mage_name();
                  break;
              case "神圣之光":
                  Skill_control.pastor_name();
                  break;
              case "无技能":
                  Skill_control.null_skill();
                  break;
              default:
                  break;
          }
      }*/
    //寻找路径函数
    //void FindPath()
    //{
    //    ////Assign StartNode and Goal Node
    //    Node startNode = new Node(new Vector3(selected_hero.transform.position.x, 0f, selected_hero.transform.position.z));
    //    Node goalNode = new Node(new Vector3(goal_position.transform.position.x, 0f, goal_position.transform.position.z));

    //    pathArray = AStar.FindPath(startNode, goalNode);

    //}
    //寻找路径（并返回距离）
    int FindPathAndReturnDistance()
    {
        //AStar Calculated Path
        pathArray = new ArrayList();

        Node startNode = new Node(new Vector3(selected_hero.transform.position.x, 0f, selected_hero.transform.position.z));
        Node goalNode = new Node(new Vector3(goal_position.transform.position.x, 0f, goal_position.transform.position.z));
        pathArray = AStar.FindPath(startNode, goalNode);

        if (pathArray == null)
        {
            Debug.Log("No way to move!");
            return 0;
        }

        pointi = 0;
        wayPoints = new Vector3[pathArray.Count];
        //print(pathArray.Count);
        for (int i = 0; i < pathArray.Count; i++)
        {
            wayPoints[i] = new Vector3(((Node)pathArray[i]).position.x, 0.5f, ((Node)pathArray[i]).position.z);
        }
        return pathArray.Count - 1;
    }
    //void showAllCanMove(Node start, int maxDistance)
    //{
    //    ArrayList allNode = new ArrayList();
    //    allNode =  AStar.checkDistance(start, maxDistance);
    //    //print(allNode.Count);
    //    //for (int i = 0; i < allNode.Count; i++)
    //    //{
    //    //    print(((Node)allNode[i]).position + " + " + ((Node)allNode[i]).distance);
    //    //}
    //}
    //移动

    //重置英雄状态（是否移动，是否攻击过）
    public static void resetHero(Player_side player_side)
    {
        //获取当前玩家的英雄
        GameObject[] heros = (player_side == Player_side.player1) ? GameObject.FindGameObjectsWithTag("Player1_hero") : GameObject.FindGameObjectsWithTag("Player2_hero");
        foreach (GameObject data in heros)
        {
            data.gameObject.GetComponent<HeroManager>().is_attacked = false;
            data.gameObject.GetComponent<HeroManager>().is_moved = false;
        }
    }
    //public ArrayList attackRange(Node heroNode, int attackDistance)
    //{
    //    ArrayList attackNodes = new ArrayList();
    //    for (int i = 0; i < attackDistance; i++)
    //    {
    //        for (int j = 0; i + j <= attackDistance; j++)
    //        {
    //            Node tempNode = new Node(new Vector3(heroNode.position.x + i, 0.5f, heroNode.position.z + j));
    //            attackNodes.Add(tempNode);
    //        }
    //    }
    //    return attackNodes;
    //}
    public static bool inAttackRange(ArrayList nodes, Node node)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            //print(((Node)nodes[i]).position.x + " + " + ((Node)nodes[i]).position.z);
            //print(node.position.x + " + " + node.position.z);
            if (((Node)nodes[i]).position.x == node.position.x && ((Node)nodes[i]).position.z == node.position.z)
                return true;
        }
        return false;
    }
    //防UI穿透 
    private bool HitTestUI()
    {
        /********以下两种判读都可以*******/
        //如果在主Camer上也挂一个UICamera，这两种判读会一直返回真（为什么呢？？？）  

        //如果有碰到NGUI对象，返回真  
        if (UICamera.hoveredObject != null) { return true; }

        print("UICamera.Raycast___" + UICamera.Raycast(Input.mousePosition));
        //如果从UICamer到当前鼠标位置的射线碰到了NGUI对象，返回真  
        //if(UICamera.Raycast(Input.mousePosition)) { return true; }  

        return false;
    }

    //等待函数
    void wait()
    {
        //当是自己的回合时，跳转到选择英雄阶段，否则禁用UI和GameManager的操作。
        if (networkPlayer == whosRound)
        {
            game_process = Game_process.Choose_hero;
        }
        else
        {
            allPanel.SetActive(false);

        }
    }
    [RPC]
    public void RoundTurn()
    {
        GameManager.whosRound = (whosRound == 0) ? 1 : 0;
        GameManager.player_side = (GameManager.player_side == Player_side.player1) ? Player_side.player2 : Player_side.player1;
        GameManager.waterPower = (GameManager.roundIndex > 6) ? 10 : 5 + GameManager.roundIndex - 1;        //计算圣水量
        GameManager.roundIndex += (GameManager.player_side == GameManager.Player_side.player1) ? 0 : 1;  //计算回合数  
        print("谁的回合：" + whosRound);
    }

    //游戏结束函数
    public void gameOver()
    {
        allPanel.SetActive(false);
        GetComponent<UIManager>().gameOverPanel.SetActive(true);
        if(whoWin == 0)
        {
            draw.SetActive(true);
        }
        else if (whoWin == int.Parse(Network.player + "") + 1)
        {
            win.SetActive(true);
        }
        else
        {
            lost.SetActive(true);
        }
    }
    public static void HeroTurnToFront()
    {
        if (selected_hero != null)
        {
            if (selected_hero.tag == "Player1_hero")
            {
                iTween.RotateTo(selected_hero, new Vector3(0, 0, 0), 3f);
            }
            if (selected_hero.tag == "Player2_hero")
            {
                iTween.RotateTo(selected_hero, new Vector3(0, 180, 0), 3f);
            }
        }
    }
    public bool IsMoveOver(Vector3 nowPosition)
    {
        if (nowPosition.x == goal_position.transform.position.x && nowPosition.x == goal_position.transform.position.z)
            return true;
        else
            return false;
    }
    void MoveToNode()
    {
        if (pointi < pathArray.Count)
        {
            iTween.MoveTo(selected_hero, iTween.Hash(
                "position", wayPoints[pointi],
                //"path", wayPoints,
                "time", 0.2f,
                "easetype", "linear",
                "movetopath", true,
                "looktarget", wayPoints[pointi],
                "oncomplete", "MoveToNode",
                "oncompletetarget", this.gameObject
                ));
            pointi++;
            mainCamera.GetComponent<CameraManager>().cameraMove(selected_hero.transform.position, 0.2f);
        }
        else
        {//英雄转向对面
            HeroTurnToFront();
            game_process = Game_process.Choose_operation;
        }
    }
}

