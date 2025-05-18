using System;
using UnityEngine;

namespace Assets.Skins.SkinResource.skin_0019.Scripts.Tea.Page.TeaTotalRecordContainer
{
    [Serializable]
    public class LabelStyle
    {
        public Color NormalColor = Color.white;
        public bool ApplyGradient;
        public Color GradientBottom = Color.white;
        public Color GradientTop = Color.white;
        public UILabel.Effect EffectStyle = UILabel.Effect.None;
        public Color EffectColor = Color.white;
        public Vector2 EffectDistance = new Vector2(2, 2);
    }

    public enum CardType
    {
        //普通牌型
        sanpai = 0,
        yidui = 1,
        liangdui = 2,
        santiao = 3,
        shunzi = 4,
        tonghua = 5,
        hulu = 6,
        tiezhi = 7,
        tonghuashun = 8,
        wutong = 9,

        none = 51,

        //特殊牌型
        sanshunzi = 100,
        santonghua = 110,
        liuduiban = 120,
        wuduisan = 130,
        sitiaosan = 140,
        couyise = 150,
        quanxiao = 160,
        quanda = 170,
        sanzhadan = 180,
        santonghuashun = 190,
        shierhuang = 200,
        shisanshui = 210,
        tonghuashisanshui = 220,
    }
}