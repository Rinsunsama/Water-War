using UnityEngine;
using System.Collections;

public class bulletController : MonoBehaviour {
    public int maxAttackPower;
    public int maxAHitRange;
    public GameObject effectPrefab;
    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, 2.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Bomb(string team)//爆炸函数
    {
        Collider[] others = Physics.OverlapSphere(transform.position, maxAHitRange, 1 << LayerMask.NameToLayer(team));//获取所有碰撞体
        Rigidbody other;//刚体，通过添加力实现爆炸的视觉效果
        Collider hero;//生命体
        for (int i = 0; i < others.Length; i++)
        {
            //others[i].
            if ((other = others[i].GetComponent<Rigidbody>()))
            {//检测刚体
                other.AddExplosionForce(maxAttackPower, transform.position, maxAHitRange, 10);//这个函数会自动根据距离给刚体衰减的力
            }
            if ((hero = others[i].GetComponent<Collider>()) != null /*&& (!life.IsAP() || weapon.IsAP)*/)
            //fe.IsAP()意指生命体是否具有‘装甲’，weapon.IsAP判断武器是否‘穿甲’
            { //如果装甲，则需要能够穿甲的武器才能计算伤害， 
                //通过计算武器的杀伤范围与物体和爆炸点的距离来计算伤害，实现距离衰减
                Vector2 myOBJ = new Vector2(transform.position.x, transform.position.z);
                Vector2 hitOBJ = new Vector2(hero.transform.position.x, hero.transform.position.z);
                int temp = (int)(maxAttackPower * (1 - Vector2.Distance(myOBJ, hitOBJ) / maxAHitRange));
                if (temp > 0)
                {
                    hero.transform.gameObject.GetComponent<HeroController>().HPControl(temp / 2);
                    //hero.transform.gameObject.GetComponent<NetworkView>().RPC("HPControl", RPCMode.All, temp / 2);
                    print("伤害是" + temp);
                    hero.transform.GetComponent<HeroController>().AnimationPlayState(4);
                    //hero.transform.GetComponent<NetworkView>().RPC("AnimationPlayState", RPCMode.All, 4);
                    //根据距离衰减判断伤害值  
                    //通过物体到爆炸点的距离与武器爆炸范围的比值，实现衰减效果
                    //显示伤害
                    string heroName = hero.transform.name;
                    print(heroName);
                    if (GameObject.Find(heroName + "hited"))
                    {
                        GameObject hitedTarget = GameObject.Find(heroName + "hited");
                        hitedTarget.GetComponent<showHurt>().showHurtNum(temp / 2);
                        //hitedTarget.GetComponent<NetworkView>().RPC("showHurtNum", RPCMode.All, temp / 2);
                    }
                }
            }
        }
    }
    void OnTriggerEnter(Collider hitObject)
    {
        //产生爆炸效果
        if (hitObject.name == "Plane")
        {
            GameObject effect = GameObject.Instantiate(effectPrefab, transform.position, Quaternion.identity) as GameObject;
            if (GameController.player_side == GameController.Player_side.player1)
            {
                Bomb("Team2");
            }
            else if (GameController.player_side == GameController.Player_side.player2)
            {
                Bomb("Team1");
            }
            GameController.HeroTurnToFront();
            //if (networkPlayer == GameController.whosRound)
            GameController.game_process = GameController.Game_process.Choose_hero;
        }
    }

    [RPC]
    public void DestoryMyself(GameObject x, float destoryTime)
    {
        GameObject.Destroy(x, destoryTime);
    }
}
