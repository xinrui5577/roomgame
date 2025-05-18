using YxFramwork.Common;
using Assets.Scripts.Game.toubao;
using YxFramwork.View;
using YxFramwork.Framework;
using System;
using YxFramwork.Enums;
using UnityEngine;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Common.Model;
using YxFramwork.Common.Adapters;

public class TouBaoSettingWindow : YxWindow
{
    /// <summary>
    /// 音效滑块
    /// </summary>
    [Tooltip("音效滑块")]
    public YxBaseSliderAdapter EffectVolume;
    /// <summary>
    /// 音乐滑块
    /// </summary>
    [Tooltip("音乐滑块")]
    public YxBaseSliderAdapter BackMusicVolume;
    /// <summary>
    /// 版本文本
    /// </summary>
    [Tooltip("版本文本")]
    public YxBaseLabelAdapter VersionLabel;
    /// <summary>
    /// 音乐滑块组
    /// </summary>
    [Tooltip("音乐滑块")]
    public YxBaseToggleAdapter[] MusiceToggles;
    /// <summary>
    /// 玩家信息
    /// </summary>
    public YxBasePlayer PlayerInfo;

    protected override void OnAwake()
    {
        var count = MusiceToggles.Length;
        var typeName = PlayerPrefs.GetString("MusiceType", count > 0 ? MusiceToggles[0].name : "");
        for (var i = 0; i < count; i++)
        {
            var toggle = MusiceToggles[i];
            if (toggle.name != typeName)
            {
                toggle.StartsActive = false;
                continue;
            }
            toggle.StartsActive = true;
            toggle.Value = true;
        }
    }

    protected override void OnStart()
    {
        var musicMgr = Facade.Instance<MusicManager>();
        if (EffectVolume != null) { EffectVolume.Value = musicMgr.EffectVolume; }
        if (BackMusicVolume != null) { BackMusicVolume.Value = musicMgr.MusicVolume; }
        if (VersionLabel != null) { VersionLabel.Text(Application.version); }
        if (PlayerInfo != null) { PlayerInfo.Info = UserInfoModel.Instance.UserInfo; }
    }

    public void OnUpdateMusicVolume(float volume)
    {
        Facade.Instance<MusicManager>().MusicVolume = volume;
    }

    public void OnUpdateSoundVolume(float volume)
    {
        Facade.Instance<MusicManager>().EffectVolume = volume;
    }

    /// <summary>
    /// 切换账号
    /// </summary>
    //public void OnChangeAccount()
    //{
    //    HallMainController.Instance.ChangeAccount();
    //    Close();
    //}

    public void OnQuitGame()
    {
        Hide();
        if (App.GetGameManager<TouBaoGameManager>().CanQuit)
        {
            YxMessageBox.Show("确定要退出大厅吗?", "", (box, btnName) =>
            {
                if (btnName == YxMessageBox.BtnLeft)
                {
                    App.QuitGame();
                }
            }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }
        else
        {
            YxMessageBox.Show("游戏中不能返回大厅");
        }
    }

    public void OnChangeMusiceType()
    {
        var toggle = UIToggle.current;
        if (toggle == null) return;
        if (!toggle.value) return;
        var typeName = toggle.name;
        var oldName = PlayerPrefs.GetString("MusiceType");
        if (typeName == oldName) return;
        PlayerPrefs.SetString("MusiceType", typeName);
    }

    public override YxEUIType UIType
    {
        get { return YxEUIType.Default; }
    }
}