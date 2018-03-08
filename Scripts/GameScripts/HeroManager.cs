using UnityEngine;
using System.Collections;

public class HeroManager : MonoBehaviour {

    public int life;
    public int maxLife;
    int direction = 0;
    public float attackForwardRspeed = 2.0f;
   // public int attack_power;
    public float run_power;
    public int attack_power;
    public int attack_range;
    public int attack_angel;
    public bool is_attacked;
    public bool is_moved;
    public GameObject showAttackAngel;
    public GameObject attackForward;
    public GameObject bulletPrefab;
	// Use this for initialization
	void Start () {
        is_attacked = false;
        is_moved = false;
        
	}
	
	// Update is called once per frame
	void Update () {
        IsGameOver();
	}
    //显示攻击角度选取
    public void ShowAttackAngel()
    {
        showAttackAngel.SetActive(true);  //显示攻击范围和指针
        attackForward.SetActive(true);
        attackForwardRspeed = 5.0f;  //开始转动
        float y = attackForward.transform.eulerAngles.y;
        if (attackForward.transform.eulerAngles.y > 360 - (attack_angel / 2) - 30) y = -(360 - y);
        if (attack_angel == 360) attackForward.transform.RotateAround(transform.position, new Vector3(0, 1, 0), attackForwardRspeed);
        else
        {
            if (direction == 0)
            {
                attackForward.transform.RotateAround(transform.position, new Vector3(0, 1, 0), attackForwardRspeed);
                //print(y);
            }
            else
            {
                attackForward.transform.RotateAround(transform.position, new Vector3(0, 1, 0), -attackForwardRspeed);
                //print(y);
            }
            if (y >= attack_angel / 2)
            {
                direction = 1;
            }
            if (y <= -attack_angel / 2)
            {
                direction = 0;
            }
        }
    }
     public void HideAttackAngel()
    {
        showAttackAngel.SetActive(false);  //显示攻击范围和指针
        attackForward.SetActive(false);
    }
    [RPC]
    public void ShootBullet(float angleY,float attackDistance)
    {  //产生一个炮弹预设在发射口
        int player = int.Parse(Network.player + "");
        float attackX,attackZ;
        Vector3 attackPosition = new Vector3(0, 0, 0); 
        float midX=0,midZ=0;
        if(GameManager.player_side==GameManager.Player_side.player1)
        {
            attackX = transform.position.x + Mathf.Sin(angleY * Mathf.Deg2Rad) * attackDistance;
            attackZ = transform.position.z + Mathf.Cos(angleY * Mathf.Deg2Rad) * attackDistance;
            attackPosition = new Vector3(attackX,0, attackZ);
            midX = transform.position.x + (attackX - transform.position.x) / 2.0f;
            midZ = transform.position.z + (attackZ - transform.position.z) / 2.0f;
        }
        else if (GameManager.player_side == GameManager.Player_side.player2)
        {

            attackX = transform.position.x - Mathf.Sin(angleY * Mathf.Deg2Rad) * attackDistance;
            attackZ = transform.position.z - Mathf.Cos(angleY * Mathf.Deg2Rad) * attackDistance;
            attackPosition = new Vector3(attackX, 0, attackZ);
            midX = transform.position.x - (transform.position.x - attackX) / 2.0f;
            midZ = transform.position.z - (transform.position.z - attackZ) / 2.0f;
        }
        Vector3[] bulletPath = { transform.position, new Vector3(midX, 1, midZ), attackPosition };
        GameObject bullet = Network.Instantiate(bulletPrefab, transform.position, transform.rotation, player) as GameObject;
       //传递角度和力度
        iTween.MoveTo(bullet, iTween.Hash("path", bulletPath,
                        "movetopath", true,
                        "time", 2,
                        "easetype", iTween.EaseType.linear,
                        //"looktarget", new Vector3(4, 0, 8),
                        "lookahead", 1));
    }
        [RPC]
    public void Chop(string team, Vector3 direction)
    {
        Collider[] others = Physics.OverlapSphere(transform.position, attack_range, 1 << LayerMask.NameToLayer(team));//获取所有碰撞体 
        Rigidbody other;//刚体，通过添加力实现爆炸的视觉效果  
        Collider hero;//生命体（被攻击）
        for (int i = 0; i < others.Length; i++)
        {
            //others[i].
            if ((other = others[i].GetComponent<Rigidbody>()))
            {//检测刚体
                other.AddExplosionForce(attack_power, transform.position, attack_range, 10);//这个函数会自动根据距离给刚体衰减的力
            }
            if ((hero = others[i].GetComponent<Collider>()) != null)
            {//检测刚体
                Vector3 forwardDir = new Vector3();
                Quaternion lookAtRot = new Quaternion();
                if (GameManager.player_side == GameManager.Player_side.player1)
                {
                    forwardDir = hero.transform.position - transform.position;
                    lookAtRot = Quaternion.LookRotation(forwardDir);
                }
                else if (GameManager.player_side == GameManager.Player_side.player2)
                {
                    forwardDir = transform.position - hero.transform.position;
                    lookAtRot = Quaternion.LookRotation(forwardDir);
                }
                print("英雄到对方角度是" + lookAtRot.eulerAngles.y);
                print("攻击度角度是" + direction.y);

                if (Mathf.Abs(lookAtRot.eulerAngles.y - direction.y) <= 30.0f)
                {

                    hero.GetComponent<NetworkView>().RPC("HPControl", RPCMode.All, attack_power);
                    //显示伤害
                    string heroName = hero.name;
                    GameObject hitedTarget = GameObject.Find(heroName + "hited");
                    hitedTarget.GetComponent<NetworkView>().RPC("showHurtNum", RPCMode.All, attack_power);
                }
            }
        }
    }


    [RPC]
    public  void AnimationPlayState(int playState)
    {
        this.gameObject.GetComponentInChildren<Animator>().SetInteger("process", playState); 
    }
    [RPC]
    public void HPControl(int demage)
    {
        this.life -= demage;
        float temp =life+0.0f;
        if (GameObject.Find(this.name.Replace("(Clone)", "")))
        GameObject.Find(this.name.Replace("(Clone)","")).GetComponent<UISlider>().value =temp/maxLife;
    }

    public void IsGameOver()
    {
        if (life <= 0)
        {
          int hero1Num= GameObject.FindGameObjectsWithTag("Player1_hero").Length;
          int hero2Num= GameObject.FindGameObjectsWithTag("Player2_hero").Length;
              if (name == "Base1(Clone)")
            {
                GetComponent<NetworkView>().RPC("ToGameOver", RPCMode.All, 2);  //玩家2获胜
            }
            else if (name == "Base2(Clone)")
            {
                GetComponent<NetworkView>().RPC("ToGameOver", RPCMode.All, 1);//玩家1获胜
            }
              else if ((hero1Num == 1 && hero2Num == 2) || (hero1Num == 2 && hero2Num == 1))
              {
                  GetComponent<NetworkView>().RPC("ToGameOver", RPCMode.All, 0);//平局
              }
            this.gameObject.GetComponentInChildren<Animator>().SetInteger("process", 6);
            GameObject.Destroy(this.gameObject, 2f);
            GameObject.Destroy(GameObject.Find(name.Replace("(Clone)", "")));  //摧毁血条   

        }
    }
    [RPC]
    public void ToGameOver(int winOrLost)
    {
        GameManager.whoWin = winOrLost;
        GameManager.game_process = GameManager.Game_process.Game_over;
    }

    public void RotateIt()
    {
        if (tag == "Player1_hero")
        {
            iTween.RotateTo(this.gameObject, new Vector3(0, 0, 0), 3f);
        }
        if (tag == "Player2_hero")
        {
            iTween.RotateTo(this.gameObject, new Vector3(0, 180, 0), 3f);
        }
    }
    //播放音效
    [RPC]
    public void attackAudioPlay()
    {
       this.gameObject.GetComponent<AudioSource>().Play();//播放攻击音效
    }

}
