using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.UI;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

public class LqSettingWindow : SettingWindow
{
    /// <summary>
    /// 麻将经典牌背颜色蓝
    /// </summary>
    public UIToggle MahjongCardClassicColorBlue;
    /// <summary>
    /// 麻将经典牌背颜色黄
    /// </summary>
    public UIToggle MahjongCardClassicColorYellow;
    /// <summary>
    /// 麻将大牌牌背颜色绿
    /// </summary>
    public UIToggle MahjongCardBigCardColorGreen;
    /// <summary>
    /// 麻将大牌牌背颜色黄
    /// </summary>
    public UIToggle MahjongCardBigCardColorYellow;
    /// <summary>
    /// 麻将牌桌颜色绿
    /// </summary>
    public UIToggle MahjongTableColorGreen;
    /// <summary>
    /// 麻将牌桌颜色蓝
    /// </summary>
    public UIToggle MahjongTableColorBlue;
    /// <summary>
    /// 麻将经典布局
    /// </summary>
    public UIToggle MahjongClassicLayout; 
    /// <summary>
    /// 麻将大牌布局
    /// </summary>
    public UIToggle MahjongBigCardLayout;
    /// <summary>
    /// 麻将经典牌墙显示控制
    /// </summary>
    public UIToggle ShowMahjongClassicWallCtrl;
    /// <summary>
    /// 麻将大牌牌墙显示控制
    /// </summary>
    public UIToggle ShowMahjongBigCardWallCtrl;
    /// <summary>
    /// 听牌提示开关
    /// </summary>
    public UIToggle MahjongQueryCtrl;
    /// <summary>
    /// cpg箭头控制
    /// </summary>
    public UIToggle ShowCpgArrowCtrl;
    /// <summary>
    /// 排序动画控制
    /// </summary>
    public UIToggle MahjongSortTweenCtrl;

    protected override void OnFreshView()
    {
        base.OnFreshView();
        if (PlayerPrefs.HasKey("TableLayoutCtrl"))
        {
            var tableLayoutCtrl = PlayerPrefs.GetInt("TableLayoutCtrl");

            var mahjongCardColor = PlayerPrefs.GetInt("MahjongCardColor");
            if (tableLayoutCtrl == 0)
            {
                MahjongCardBigCardColorYellow.startsActive = mahjongCardColor == 0;
                MahjongCardBigCardColorGreen.startsActive = mahjongCardColor != 0;
            }
            else
            {
                MahjongCardClassicColorBlue.startsActive = mahjongCardColor == 1;
                MahjongCardClassicColorYellow.startsActive = mahjongCardColor != 1;
            }

            var mahjongTableColor = PlayerPrefs.GetInt("MahjongTableColor");
            MahjongTableColorGreen.startsActive = mahjongTableColor == 0;
            MahjongTableColorBlue.startsActive = mahjongTableColor != 0;

            var noShowMahjongWallCtrl = PlayerPrefs.GetInt("NoShowMahjongWallCtrl");
            ShowMahjongBigCardWallCtrl.startsActive = noShowMahjongWallCtrl == 0 && tableLayoutCtrl == 0;
            ShowMahjongClassicWallCtrl.startsActive = noShowMahjongWallCtrl == 0 && tableLayoutCtrl != 0;

            MahjongBigCardLayout.startsActive = tableLayoutCtrl == 0;
            MahjongClassicLayout.startsActive= tableLayoutCtrl != 0;
            var mahjongQueryCtrl = PlayerPrefs.GetInt("MahjongQueryCtrl");
            MahjongQueryCtrl.startsActive = mahjongQueryCtrl == 0;

            var showCpgArrowCtrl = PlayerPrefs.GetInt("ShowCpgArrowCtrl");
            ShowCpgArrowCtrl.startsActive = showCpgArrowCtrl == 0;

            var mahjongSortTweenCtrl = PlayerPrefs.GetInt("MahjongSortTweenCtrl");
            MahjongSortTweenCtrl.startsActive = mahjongSortTweenCtrl == 0;
        }
        else {
            PlayerPrefs.SetInt("TableLayoutCtrl",0);
            PlayerPrefs.SetInt("MahjongCardColor", 0);
            PlayerPrefs.SetInt("MahjongTableColor", 0);
            PlayerPrefs.SetInt("NoShowMahjongWallCtrl", 0);
            PlayerPrefs.SetInt("MahjongQueryCtrl", 0);
            PlayerPrefs.SetInt("ShowCpgArrowCtrl", 0);
            PlayerPrefs.SetInt("MahjongSortTweenCtrl", 0);
        }
    }

    public void OnClickBtn(UIToggle toggle,string field)
    {
        if (!toggle.value) return;
        PlayerPrefs.SetInt(field, int.Parse(toggle.name));
        if (field.Equals("TableLayoutCtrl"))
        {
            var showWall = -1;
            if (int.Parse(toggle.name) == 0)
            {
                showWall = ShowMahjongBigCardWallCtrl.value ? 1 : 0;
            }
            else {
                showWall = ShowMahjongClassicWallCtrl.value ? 1 : 0;
            }
            PlayerPrefs.SetInt("NoShowMahjongWallCtrl", showWall);
        }
    }

    public void OnSoundChange()
    {
        var musicMgr = Facade.Instance<MusicManager>();
        if (musicMgr.EffectVolume > 0)
        {
            EffectVolume.Value = 0;
        }
        else {
            EffectVolume.Value = 1;
        }
    }

    public void OnMusicChange()
    {
        var musicMgr = Facade.Instance<MusicManager>();
        if (musicMgr.MusicVolume > 0)
        {
            BackMusicVolume.Value = 0;
        }
        else
        {
            BackMusicVolume.Value = 1;
        }
    }
}
