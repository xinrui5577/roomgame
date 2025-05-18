namespace Assets.Scripts.Game.jpmj.MahjongScripts.Public
{
    public enum UIEventId
    {
        GameStatus = 10000,
        UserJionIn,
        UserOutLine,
        UserOutRoom,
        UserReady,
        UserGlodChange,
        OperationMenu,
        ChangeCurrUser,
        Banker,
        DingqueShow,
        SignWBT,
        MahjongCnt,
        Result,
        CgChoose,
        ChooseTing,
        Cpg,
        Hu,
        FanPai,
        GameStart,
        HandUp,
        CurrRound,
        UserTalk,
        OnGetSoundKey,
        DownLoadVoice,
        IsGameOver,
        GameOverShowOneRoundBtn,

        ShowSettintPnl = 11000,
        ShowChatPnl,
        ShowTalkPnl,
        ShowUserDetail,
        ShowTotalResult,
        ShowUserSpeak,
        StopUserSpeak,
        RoomInfo,
        QdjtRoomInfo,
        QdjtUpdateRound,

        ShowJiaPiaoBtn,
        ShowJiaPiaoEffect,
        HideJiaPiaoEffect,
        ShowDiaoYuIcon,

        ShowChooseXJFD,

        ShowQueryHulistPnl,
        HideQueryHulistPnl,
        ShowYoujinBtnByType,
        HideGuoBtn,
        ShowHuanBaoEffect,
        //泉州麻将，当双游 or 三游的时候，出牌时显示特效
        ShowThrowEffectOnYoujin,

        ShowWangdiaoBtnByType,
        //最后一轮提示
        ShowFinalRoundWarning,

        ShowChoosHuPai,
        HideChoosHuPai,
        //接收GPS信息
        ReceiveGPSInfo,
        //显示GPS信息
        ShowGPSInfo,
        GetIpAndCountry,

        ShowStartTingBtnMenu,
        ShowJiamaPnl,
        ShowJiamaResult,
        GetHuList,
        HideChooseLiangPai,
        PlayEffectLiangPai,
        LiangPaiTip,
        ReadyBtActive,

        //使用艺术字加动画表现加减分
        UsersGlodChangeWithWordart,
        //绝杠
        ShowJuegangBtn,
        //显示断门按钮
        ShowDuanmenBtn,
        HideDuanmenBtn,
        ShowSwitchCardsBtn,
        HideSwitchCardsBtn,       
        FgBtnCtrl,
        LgBtnCtrl,
        FgConfirmgBtnCtrl,
        ShowDbsmjEffectByEnum,
        DisableTrusteeship,
        SetPrepareBtnState,
        ShowYwLaizi,
        //最后一局
        OnLastGame,
        HideInviteFriendPnl,

        OnRestartGame,
        OnHandupHideBtn,

        ShowPaoPnl,
        ShowPaoEffect,
        RejoinShowPaoPnl,

        OnSwitchShen,
        SelectXiapaoScore,
        ClearBupaoScore,
        OnHelpPutCard,
        QifeiBtnCtrl,
        OnShowKlsmjEffect,
        QifeiPnlCtrl,        
        //显示解散房间按钮
        OnShowDismissPnl,
        SwitchCardsBtnCtrl,
        RedisplayButtons, 
        ClearButtonStateCache,

        //初始化Gameui界面
        OnContinueGame,
        //红中赖子杠按钮
        ShowHzlzgBtn,
        OnCanLzgChoose,
        BuzhangEffect,
    }

    public enum GameEventId
    {
        GameStatus = 20000,
        SendCard,
        GetInCard,
        OutPuCard,
        Cpg,
        Hu,
        Ting,
        Banker,
        SaiziPoint,
        ChangeCurrUser,
        FlagMahjong,
        CleareFlagMahjong,
        WallMahjongFinish,
        Result,
        FanPai,
        RoomInfo,
        BuZhang,
        ChooseTing,
        ChooseNiuTing,
        LiangNiuPai,
        ChooseTingCancel,
        CancelTingIcon,
        ChooseXJFD,
        SendXJFD,
        OnXJFD,
        CancelXJFD,
        SendRuleXJFD,
        RomoveCardXJFD,
        FangDa,
        SendDesList,
        GetBao,
        TingList,
        YouJinList,
        QueryHulist,
        RemoveOneWallCard,
        XuanZeTiShi,
        MJToNormal,
        AddHuIcon,
        ChangeVoice,
        ChangeTableColor,

        XZStartSwitch,
        XZStartRotation,
        XZStartSelColor,
        XZSelColorOver,
        XZSomeOneHuPai,
        SendLiangPai,
        CancelLiangPai,
        ChooseLiangPai,

        OnDelayOperator,
        OnJueGang,
        SwitchConfirm,
        InitTable,

        //当倒序摸牌时的
        RevWallMahjongFinish,
      
        OnShowGangDi,       
        OnSwitchFengGang,    
        ResetFengGangState,   
        OnConfirmFenggangClick,
        //托管
        OnTrusteeship,
        OnLiGang,        
        //分张
        OnFenZhang,  
        OnConllectionGangCards, 
        
        OnDingshen,  
        OnQifei,
        JyStartSwitch,
        OnConfirmSwitchClick,
        OnShowQfPlayerCards,

        //初始化Gametable界面
        OnContinueGame,
        OnGangHzlz,
        OnRefreshHandCard,    
    }

    public enum NetEventId
    {
        OnOutMahjong = 30000,
        OnTingPai,
        OnNiuTingPai,
        OnChiClick,
        OnPengClick,
        OnGangClick,
        OnHuClick,
        OnTingClick,
        OnGuoClick,
        OnLiangClick,
        OnNiuClick,
        ChooseNiu,
        OnUserReady,
        OnGameRestart,
        OnGameContinue,
        OnDismissRoom,
        OnUserTalk,
        OnChangeRoom,
        OnSendLiangPai,

        OnUserDetail,
        OnTotalResult,

        OnVoiceUpload,

        GetNeedCard,
        PauseResponse,
        OnJiaPiaoClick,

        ReJionShowBao,

        OnSendXJFD,
        OnXJFDClick,
        OnZiDongDaPai,

        OnQueryHuLish,

        OnSwitchConfirmClick,
        OnWanCardsClick,
        OnBingCardsClick,
        OnTiaoCardsClick,
        OnContinue,

        OnStartTing,
        SendGpsInfo,
        OnJiamaByType,

        OnJueGang,
        OnConfirmFengGangClick,
        OnLiGangClick,

        OnPaoFenClick,
        OnHuanshenClick,     
        
        OnQifeiClick,
        //继续游戏，创建一个新房间
        OnCreateNewGame,
        //加入一条网络信息
        OnPushNetRespons,
    }

    public enum ShareEventID
    {
        OnWeiChatShareTableInfo = 40000,
        OnWeiChatShareGameResult,
    }

    public enum TableDataEventId
    {
        OnWeiChatShareTableInfo = 50000,
        //托管
        OnTrusteeshipClick,
        //初始化游戏状态
        OnRestartGame,
        //手动准备
        OnPrepareClick,
        //发送房间是状态
        OnSendLeaveRoomState,
    }
}