using UnityEngine;
using System.Collections;

public class OnMenuUI : MonoBehaviour
{
    public GameObject camera;
    public GameObject auCancelIcon;
    public GameObject waitingLabel;
    public GameObject volumeSlider;
    //定义UI动画tween
    public TweenAlpha waitingLabelTween;
    public GameObject menuPanel;
    public GameObject networkPanel;
    public GameObject gameSettingPanel;
    //连接信息
    public int connections = 2;
    public int listenPort = 8899;
    public bool useNat = false;
    public UIInput ip;
    //开始游戏按钮触发函数
    public void OnToPrivatePanelButton()
    {
        Application.LoadLevel("GameMainAlone");
    }
    //局域网按钮触发函数
    public void OnToNetworkPanelButton()
    {
        menuPanel.SetActive(false);
        networkPanel.SetActive(true);
    }
    //局域网到主界面触发函数
    public void OnFromNetworkToMenuPanelButton()
    {
        menuPanel.SetActive(true);
        networkPanel.SetActive(false);
    }
    //点击创建服务器的按钮触发函数
    public void OnCreatServerButton()
    {
        NetworkConnectionError error = Network.InitializeServer(connections, listenPort);
        print(error);
    }
    //点击连接服务器的按钮触发函数
    public void OnConnectedToServerButton()
    {
        NetworkConnectionError error = Network.Connect(ip.value, listenPort);
        print(error);
    }
    //静音按钮
    public void OnMuteButton()
    {
        
        camera.GetComponent<AudioListener>().enabled=(camera.GetComponent<AudioListener>().enabled==true)?false:true;
        if(camera.GetComponent<AudioListener>().enabled)
            auCancelIcon.SetActive(false);
        else
            auCancelIcon.SetActive(true);
    }
    //保存设置按钮
    public void OnSaveSettingButton()
    {
        camera.GetComponent<AudioSource>().volume = volumeSlider.GetComponent<UISlider>().value;
        gameSettingPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
    //设置按钮
    public void OnGameSettingButton()
    {
        menuPanel.SetActive(false);
        gameSettingPanel.SetActive(true);
    }
    //服务器初始化函数
    void OnServerInitialized()
    {
        waitingLabel.SetActive(true); //等待游戏玩家加入。
        waitingLabelTween.PlayForward();
        print("完成初始化");
    }
    //客户端连接函数
     void OnConnectedToServer()
    {
        print("成功连接到服务器！");
        Application.LoadLevel("GameMain");
    }
    //服务端检测到客户端接入函数
   void OnPlayerConnected()
     {
         print("有玩家接入了");
         print(Network.player);
         Application.LoadLevel("GameMain");
     }
}
