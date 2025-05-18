using UnityEngine;
using System.Collections;
using Assets.Scripts.RedPacket;
using UnityEngine.UI;

public class AppStatusBar : MonoBehaviour
{
    [Tooltip("状态栏是否显示状态及通知")]
    public bool statusBar;
    [Tooltip("状态栏样式")]
    public AndroidStatusBar.States states = AndroidStatusBar.States.TranslucentOverContent;
    //  public Button[] button;
    // Use this for initialization
    void Awake()
    {
        statusBar = true;
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidStatusBar.dimmed = !statusBar;
            //当AndroidStatusBar.dimmed=false时，状态栏显示所有状态及通知图标
            //当AndroidStatusBar.dimmed=true时，状态栏仅电量和时间，不显示其他状态及通知

            ////显示状态栏，占用屏幕最上方的一部分像素
            //AndroidStatusBar.statusBarState = AndroidStatusBar.States.Visible;

            ////悬浮显示状态栏，不占用屏幕像素
            AndroidStatusBar.statusBarState = AndroidStatusBar.States.VisibleOverContent;

            ////透明悬浮显示状态栏，不占用屏幕像素
            //AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;

            ////隐藏状态栏
            //AndroidStatusBar.statusBarState = AndroidStatusBar.States.Hidden;

            //AndroidStatusBar.statusBarState = states;
        }
    }
    private void Start()
    {
        /*
        for(int i = 0;i<4;i++)
        {
            if(i==0)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.Visible;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }
            if (i == 1)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.VisibleOverContent;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }
            if (i == 2)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }
            if (i == 3)
            {
                button[i].onClick.AddListener(
                    delegate
                    {
                        AndroidStatusBar.statusBarState = AndroidStatusBar.States.Hidden;
                        //AndroidStatusBar.statusBarState = states;
                    });
            }



        }

    */
        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
        //AndroidStatusBar.statusBarState = states;


    }
}
