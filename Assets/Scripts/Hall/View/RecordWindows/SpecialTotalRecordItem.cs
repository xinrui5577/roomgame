using UnityEngine;
using Assets.Scripts.Hall.View.RecordWindows;
using Assets.Scripts.Common.Utils;
using System.Collections.Generic;
using System;

public class SpecialTotalRecordItem : TotalRecordItem
{
    public UISprite GameNameBg;
    public  List<DifferentGameShowData> DifferentGameShowDatas;

    public override void ShowItem(string roundNum, string selfId)
    {
        _curItemData.ShowRoundNum = roundNum;
        SetItemInfo(_curItemData);
        SetHeads(_curItemData.HeadDatas);
        GameName.TrySetComponentValue(_curItemData.GameName);
        //if (BgSprite)
        //{
        //    float moveY = HeadGrid.cellHeight * ((HeadGrid.GetChildList().Count - 1) / HeadGrid.maxPerLine);
        //    BgSprite.height = (int)moveY + BaseHeight;
        //}
        DifferentGameDeal();
        //交错背景的控制显示
        SetInterlacedBg(roundNum);
    }

    protected override void SetItemInfo(TotalRecordItemData itemData)
    {
        if (Time != null) Time.text = itemData.Time;
        SelfSpecialColorldata();
        if (UpdateTime != null)
        {
            var updateTime = itemData.UpdateTime;
            if (string.IsNullOrEmpty(updateTime))
            {
                updateTime = itemData.Time;
            }
            UpdateTime.text = updateTime;
        }
        RoomId.text =string.Format("{0}房", itemData.ShowRoomId);
        RoundNum.text = string.Format(RoundFormat, Id);
    }

    private void  DifferentGameDeal()
    {
        foreach (var differentGameShowData in DifferentGameShowDatas)
        {
            if (_curItemData.GameKey.Equals(differentGameShowData.GameKey))
            {
                if (BgSprite)
                {
                    GameNameBg.spriteName = differentGameShowData.GameNameBg;
                    BgSprite.height = differentGameShowData.BgHeight;
                    RoomId.color = differentGameShowData.LableColor;
                }
            }
        }
        
    }
}
[Serializable]
public class DifferentGameShowData
{
    public string GameKey;
    public string GameNameBg;
    public int BgHeight;
    public Color LableColor;
}
