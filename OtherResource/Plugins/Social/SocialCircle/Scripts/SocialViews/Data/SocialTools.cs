using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Common.Utils;
using BestHTTP.JSON;
using com.yx.chatsystem;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data
{
    public static class SocialTools
    {
        #region Keys
        /// <summary>
        /// key id
        /// </summary>
        public const string KeyId = "id";
        /// <summary>
        /// key config
        /// </summary>
        public const string KeyConfig = "config";
        /// <summary>
        /// key  im id (群信息中使用)
        /// </summary>
        public const string KeyOwnerId = "owner_id";
        /// <summary>
        /// key 玩家信息中的user id
        /// </summary>
        public const string KeyUserInfoUserId = "user_id";
        /// <summary>
        /// key 游戏 user id
        /// </summary>
        public const string KeyOtherId = "other_id";
        /// <summary>
        /// key im_id(im通讯id)
        /// </summary>
        public const string KeyImId = "im_id";
        /// <summary>
        /// key 拉黑imId
        /// </summary>
        public const string KeyBlackId = "black_id";
        /// <summary>
        /// key data(通常表示对应请求的数据主体)
        /// </summary>
        public const string KeyData = "data";
        /// <summary>
        /// key 分享信息到亲友圈
        /// </summary>
        public const string KeyShareToSocial= "shareToSocial";

        /// <summary>
        /// key hall
        /// </summary>
        public const string KeyGameInHall = "hall";
        /// <summary>
        /// key game
        /// </summary>
        public const string KeyGameInGaming = "game";
        /// <summary>
        /// key 亲友圈初始化标记
        /// </summary>
        public const string KeyEntryNum= "entry_num";
        /// <summary>
        /// key 来源
        /// </summary>
        public const string KeySrc = "src";
        /// <summary>
        /// key 回调
        /// </summary>
        public const string KeyCallBack = "callBack";
        /// <summary>
        /// key 不检测发送到本地
        /// </summary>
        public const string KeyNoCheckSend = "NoCheckSend";
        /// <summary>
        /// key action(表示请求的对应事件)
        /// </summary>
        public const string KeyAction = "action";
        /// <summary>
        /// key code(表示请求的消息状态码：0为成功，其它为失败)
        /// </summary>
        public const string KeyCode = "code";
        /// <summary>
        /// key message(表示请求的提示信息，通常存放错误数据 )
        /// </summary>
        public const string KeyMessage = "msg";
        /// <summary>
        /// key gid(表示群组ID)
        /// </summary>
        public const string KeyGroupId = "gid";
        /// <summary>
        /// key 人物信息的群id
        /// </summary>
        public const string KeyUseInfoGroupId = "group_id";
        /// <summary>
        /// key 成员集合
        /// </summary>
        public const string KeyMembers = "members";
        /// <summary>
        /// key 成员信息
        /// </summary>
        public const string KeyMemberInfo = "memberInfo";
        /// <summary>
        /// key ids(id集合)
        /// </summary>
        public const string KeyIds = "ids"; 
        /// <summary>
        /// key user id 集合
        /// </summary>
        public const string KeyOtherIds = "other_ids"; 
        /// <summary>
        /// key user ids(user id集合)
        /// </summary>
        public const string KeyUserIds = "userids"; 
        /// <summary>
        /// key 陌生人列表数据
        /// </summary>
        public const string KeyStrangef = "strangef";
        /// <summary>
        /// key 亲友的列表数据（包括黑名单）
        /// </summary>
        public const string KeyFirends = "friends";
        /// <summary>
        /// 是否为最后一次请求
        /// </summary>
        public const string KeyIsLastRequest= "isLastRequest";
        /// <summary>
        /// Key 默认key 返回（服务器返回字段）
        /// </summary>
        public const string KeyDefKey= "defkey";
        /// <summary>
        /// Key 聊天消息时间
        /// </summary>
        public const string KeyTalkItemTime= "timestamp";
        /// <summary>
        /// Key 相册数据
        /// </summary>
        public const string KeyPhotoData = "album";
        /// <summary>
        /// Key 相册上传次数
        /// </summary>
        public const string KeyPhotoUploadNum = "upload_num";
        /// <summary>
        /// Key 亲友圈相册名称
        /// </summary>
        public const string KeyPhotoAlbumName = "亲友圈";
        /// <summary>
        /// 牌桌ID（房间号）
        /// </summary>
        public const string KeyRoomId = "rndId";
        /// <summary>
        /// 排序类型
        /// </summary>
        public const string KeySortType = "sortt";

        /// <summary>
        /// Key 来源Id
        /// </summary>
        public const string KeySourceId = "source_id";
        /// <summary>
        /// Key 来源名称
        /// </summary>
        public const string KeySourceName = "source";
        /// <summary>
        /// Key 消息Id
        /// </summary>
        public const string KeyBoxId = "box_id";
        /// <summary>
        /// Key 回复
        /// </summary>
        public const string KeyReply = "reply";
        /// <summary>
        /// key datanum(请求数据集合上限值)
        /// </summary>
        public const string KeyDataNum = "datanum";
        /// <summary>
        /// key 数量
        /// </summary>
        public const string KeyNum = "num";
        /// <summary>
        /// key 群列表
        /// </summary>
        public const string KeyGroupList = "grouplist";
        /// <summary>
        /// key 黑名单列表
        /// </summary>
        public const string KeyBlackList = "blacklist";
        /// <summary>
        /// key 排行名次
        /// </summary>
        public const string KeyRankNum = "ranking";
        /// <summary>
        /// key 列表
        /// </summary>
        public const string KeyList = "list";
        /// <summary>
        /// key 分页数据总数量
        /// </summary>
        public const string KeyTotalSize = "ttsize";
        /// <summary>
        /// key 分页页码
        /// </summary>
        public const string KeyPage = "p";
        /// <summary>
        /// key 评价信息
        /// </summary>
        public const string KeyComments = "comments";
        /// <summary>
        /// key 是否为单条消息
        /// </summary>
        public const string KeySingleItem = "Single";
        /// <summary>
        /// key 请求来源
        /// </summary>
        public const string KeyFromAction = "fromAction";
        /// <summary>
        /// key 礼物点
        /// </summary>
        public const string KeyGiftPoint = "item8_q";
        /// <summary>
        /// key 申请状态
        /// </summary>
        public const string KeyHandles = "handle";
        /// <summary>
        /// key 消息接收者Id
        /// </summary>
        public const string KeyMessageReceiveId = "receive_id";
        /// <summary>
        /// key 未读状态
        /// </summary>
        public const string KeyMessageUnRead= "HaveUnRead";
        /// <summary>
        /// key 消息发送人Id
        /// </summary>
        public const string KeyMessageSendId = "send_id";
        /// <summary>
        /// key 消息内容
        /// </summary>
        public const string KeyMessageContent = "content";
        /// <summary>
        /// key 消息类型（群：group 好友：friend）
        /// </summary>
        public const string KeyMessageType = "type";

        /// <summary>
        /// key 通用类型
        /// </summary>
        public const string KeyType = "type";

        /// <summary>
        /// key 通用名称
        /// </summary>
        public const string KeyName = "name";
        /// <summary>
        /// key 推送消息类型-1：删除 0：更新 1.新增
        /// </summary>
        public const string KeyUpdateType = "type";
        /// <summary>
        /// key 拉黑列表类型 0：自己的拉黑列表 1.被拉黑列表
        /// </summary>
        public const string KeyBlackListType = "type";
        /// <summary>
        /// key 离线喇叭消息类型
        /// </summary>
        public const string KeyHornType = "type";
        /// <summary>
        /// key 消息n内容类型（0:文字1:表情2:图片:3:语音）
        /// </summary>
        public const string KeyMessageContentType = "content_type";
        /// <summary>
        /// key 群名称
        /// </summary>
        public const string KeyGroupName= "groupname";
        /// <summary>
        /// key 昵称
        /// </summary>
        public const string KeyNickName = "nick_name";
        /// <summary>
        /// Key 当前玩家点赞状态
        /// </summary>
        public const string KeyAgreeStatus = "self_like";
        /// <summary>
        /// Key点赞数量
        /// </summary>
        public const string KeyAgreeNum = "likes";
        /// <summary>
        /// Key点赞状态
        /// </summary>
        public const string KeyAgree = "like";
        /// <summary>
        /// key 评论 id
        /// </summary>
        public const string KeyCommentId = "comment_id";
        /// <summary>
        /// Key是否为本人消息
        /// </summary>
        public const string KeyIsSelf = "isSelf";
        /// <summary>
        /// Key 显示消息时间
        /// </summary>
        public const string KeyShowMessageTime= "showMessageTime";
        /// <summary>
        /// Key喇叭消息格式化字段
        /// </summary>
        public const string KeyHornFormat = "str";
        /// <summary>
        /// Key喇叭消息数据主体
        /// </summary>
        public const string KeyHornData = "hrons";
        /// <summary>
        /// 当前玩家昵称显示
        /// </summary>
        public const string KeySelfNickShow = "我";
        /// <summary>
        /// 保存音乐值
        /// </summary>
        public const string KeySaveMusicValue = "SaveMusicValue";
        /// <summary>
        /// Key语音上传接口
        /// </summary>
        public const string KeyVoiceUpLoadAction = "index/upload/upload_voice";
        /// <summary>
        /// Key图片上传接口
        /// </summary>
        public const string KeyImageUpLoadAction = "index/upload/upload_image";

        #region Action Keys
        /// <summary>
        /// key 本地消息前缀
        /// </summary>
        public const string KeyLocalActionPrefix = "Local_";
        /// <summary>
        /// key login(登录事件)
        /// </summary>
        public const string KeyActionLogin = "login";
        /// <summary>
        /// key login(获得最新的群排序)
        /// </summary>
        public const string KeyActionGetGroupList= "index/indexapi/get_grouplist";
        /// <summary>
        /// key 亲友列表更新
        /// </summary>
        public const string KeyActionGroupListUpdate = "refresh_gl";
        /// <summary>
        /// key 接收到礼物
        /// </summary>
        public const string KeyActionBeSendGift = "update_bp";
        /// <summary>
        /// key 获取群信息(获取群信息)
        /// </summary>
        public const string KeyActionGetGroupInfo = "index/indexapi/get_group_info";
        /// <summary>
        /// key 获取玩家信息
        /// </summary>
        public const string KeyActionGetUserInfo= "index/indexapi/get_user_info"; 
        /// <summary>
        /// key 获取群成员信息(集合)
        /// </summary>
        public const string KeyActionGetGroupMemberInfo = "index/indexapi/get_group_members";
        /// <summary>
        /// key 发送消息事件
        /// </summary>
        public const string KeyActionSendMessage = "index/indexapi/sendmessage";
        /// <summary>
        /// key 聊天消息
        /// </summary>
        public const string KeyTalkMessage= "message";
        /// <summary>
        /// 聊天对象更改
        /// </summary>
        public const string KeyTalkTargetChange = "talkTargetChange";
        /// <summary>
        /// key 获取评价列表
        /// </summary>
        public const string KeyActionCommentsList = "index/indexapi/get_user_comment";
        /// <summary>
        /// key 获取黑名单列表
        /// </summary>
        public const string KeyActionBlackList = "index/indexapi/get_blacklist";
        /// <summary>
        /// key 设置黑名单人物
        /// </summary>
        public const string KeyActionSetBlack= "index/indexapi/into_blacklist";
        /// <summary>
        /// key 还原黑名单人物
        /// </summary>
        public const string KeyActionRestoreBlack = "index/indexapi/restore_blacklist";
        /// <summary>
        /// key 黑名单变化推送
        /// </summary>
        public const string KeyActionBlackPush = "black";
        /// <summary>
        /// key 获取礼物配置
        /// </summary>
        public const string KeyActionGiftSetting = "gift.getGiftSetting";
        /// <summary>
        /// key 送礼接口
        /// </summary>
        public const string KeyActionSendGift= "index/gift/send_gifts";
        /// <summary>
        /// key 喇叭消息列表
        /// </summary>
        public const string KeyActionHornList = "HornList";
        /// <summary>
        /// key 喇叭离线消息
        /// </summary>
        public const string KeyActionOutLineHornList= "index/horn/get_offline";
        /// <summary>
        /// key 喇叭消息推送
        /// </summary>
        public const string KeyActionHornUpdate = "horn";
        /// <summary>
        /// key 房间列表
        /// </summary>
        public const string KeyActionRoomList = "index/indexapi/get_room_list";
        /// <summary>
        /// key 房间列表信息
        /// </summary>
        public const string KeyActionRoomInfos = "index/indexapi/get_room_info";
        /// <summary>
        /// key 房间信息变化推送
        /// </summary>
        public const string KeyActionRoomUpdate = "room";
        /// <summary>
        /// key 房间列表刷新
        /// </summary>
        public const string KeyActionRoomListUpdate = "refresh_rooml";
        /// <summary>
        /// key 排行列表
        /// </summary>
        public const string KeyActionRankInfos = "RankInfos";
        /// <summary>
        /// key 推荐好友列表
        /// </summary>
        public const string KeyActionAddList = "AddList";
        /// <summary>
        /// key 好友申请列表
        /// </summary>
        public const string KeyActionFriendRequestList = "index/indexapi/get_ask_friend_list";
        /// <summary>
        /// key 好友申请信息集合
        /// </summary>
        public const string KeyActionFriendRequestInfos = "index/indexapi/get_msgbox_info";
        /// <summary>
        /// key 添加进入次数
        /// </summary>
        public const string KeyActionEntryAdd= "index/indexapi/add_entry";
        /// <summary>
        /// key 添加好友
        /// </summary>
        public const string KeyActionAddFriend = "index/indexapi/ask_friend";
        /// <summary>
        /// key 群进入监听
        /// </summary>
        public const string KeyActionObserverIn = "observer_in";
        /// <summary>
        /// key 群退出监听
        /// </summary>
        public const string KeyActionObserverOut = "observer_out";
        /// <summary>
        /// key 群在线人数变化监听
        /// </summary>
        public const string KeyActionMemberNumChange = "ob_change";
        /// <summary>
        /// Key 好友申请信息推送（update）
        /// </summary>
        public const string KeyActionFriendRequestUpdate= "friend";
        /// <summary>
        /// key 排行名次变化
        /// </summary>
        public const string KeyRankNumChange = "RankNumChange";
        /// <summary>
        /// key 请求数量变化
        /// </summary>
        public const string KeyRequestNumChange= "RequestNumChange";
        /// <summary>
        /// key 被拉黑提示
        /// </summary>
        public const string KeyNoticeBlockedInfo = "哇哦，你已经被拉黑了!";
        /// <summary>
        /// key 输入ID为空
        /// </summary>
        public const string KeyNoticeInputIdEmpty = "ID不能为空!";
        /// <summary>
        /// key 分享人存在于黑名单内
        /// </summary>
        public const string KeyNoticeShareUserInBlackList = "对方存在于你的黑名单内，不能分享!";
        /// <summary>
        /// 相册下载提示
        /// </summary>
        public const string KeyNoticePhotoDownload = "已成功下载到本地!";
        /// <summary>
        /// 评论输入提示
        /// </summary>
        public const string KeyNoticeCommitInputError = "请输入正确的评论内容!";
        /// <summary>
        /// 亲友圈首次进入欢迎词
        /// </summary>
        public const string KeyNoticeSocialFirstContent = "欢迎{0}首次进入玩嘛亲友圈功能!";
        /// <summary>
        /// Item Id不存在
        /// </summary>
        public const string KeyNoticeItemIdNotExist = "当前字典中不存在对应ItemID：";
        /// <summary>
        /// Only Id 是空的
        /// </summary>
        public const string KeyNoticeOnlyIdEmpty = "OnlyId 为空!";
        /// <summary>
        /// 拉黑自己相关提示
        /// </summary>
        public const string KeyNoticeSetSelfIntoBlack = "不能拉黑自己!";
        /// <summary>
        /// 多选空提示
        /// </summary>
        public const string KeyNoticeSelectEmpty = "请选择后添加!";
        /// <summary>
        /// 申请发送成功
        /// </summary>
        public const string KeyNoticeRequestSendSuccess = "嘿，申请发送成功啦！";
        /// <summary>
        /// 推送数据异常
        /// </summary>
        public const string KeyNoticePushDataError= "未知操作类型action:{0},type{1}!";
        /// <summary>
        /// 推送数据异常
        /// </summary>
        public const string KeyNoticeRankOutOfRange = "99+";
        /// <summary>
        /// 礼物不足提示
        /// </summary>
        public const string KeyNoticeGiftNotEnough = "礼物点不足，点击确定去【充值】页面进行兑换!";

        /// <summary>
        /// 不允许给自己送礼提示
        /// </summary>
        public const string KeyNoticeCouldNotSendToSelf= "不能给自己送礼!";
        /// <summary>
        /// 礼物数量小于0
        /// </summary>
        public const string KeyNoticeGiftNumLessThanZero = "赠送礼物数量不能小于0!";
        /// <summary>
        /// 未选择礼物相关提示
        /// </summary>
        public const string KeyNoticeSendGiftEmpty = "请选择礼物后再赠送!";
        /// <summary>
        /// 礼物赠送成功
        /// </summary>
        public const string KeyNoticeGiftSendSuccess = "赠送成功!";
        /// <summary>
        /// 评论成功提示
        /// </summary>
        public const string KeyNoticeSendCommentSuccess = "评论成功啦！快去玩牌吧！";
        /// <summary>
        /// 评论成功提示
        /// </summary>
        public const string KeyNoticeSendAgreeSuccess = "点赞成功!";
        /// <summary>
        /// 推送数据异常
        /// </summary>
        public const string KeyNoticeUpLoadImageFailed = "上传图片失败:";
        /// <summary>
        /// 拉黑存在于黑名单中的人
        /// </summary>
        public const string KeyNoticeAlreadyExistInBlackList = "该玩家已存在与黑名单中，不可再次拉黑!";
        /// <summary>
        /// 分享成功提示
        /// </summary>
        public const string KeyNoticeShareSuccess="分享成功啦！";
        #endregion

        #endregion

        #region function
        /// <summary>
        /// 获取本地消息Action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseAction"></param>
        /// <returns></returns>
        public static string GetLocalAction<T>(T baseAction)
        {
            return String.Format("{0}{1}", KeyLocalActionPrefix, baseAction);
        }

        /// <summary>
        /// 获取list(string)数据
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryGetStringListWithKey(this Dictionary<string, object> dic, out List<string> value, string key)
        {
            var getState = false;
            if (dic.ContainsKey(key))
            {
                var list = (List<object>)dic[key];
                if (list != null)
                {
                    value = list.ConvertAll(item => item.ToString());
                    getState = true;
                }
                else
                {
                    value = new List<string>();
                }
            }
            else
            {
                value = new List<string>();
            }
            return getState;
        }

        /// <summary>
        /// 根据列表数据筛选出需要的亲友数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static Dictionary<string, Dictionary<string, object>> SelectDicDataFromList(List<string> list, Dictionary<string, Dictionary<string, object>> dic)
        {
            var returnDic = new Dictionary<string, Dictionary<string, object>>();
            foreach (var item in dic)
            {
                if (list.Contains(item.Key))
                {
                    returnDic[item.Key] = item.Value;
                }
            }
            return returnDic;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="uploadUrl"></param>
        /// <param name="data"></param>
        /// <param name="finishCall"></param>
        /// <param name="failCall"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerator Uploading(string uploadUrl, byte[] data, YxVoiceChatSystem.OnUploadFinish finishCall = null, YxVoiceChatSystem.OnUploadFinish failCall = null, string key = "f")
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData(key, data, ".jpg");
            WWW www = new WWW(uploadUrl, form);
            yield return www;
            if (www.error != null)
            {
                YxDebug.Log("文件上传失败【{0}】{1}", "SocialTools", null, uploadUrl, www.error);
                if (failCall!= null)
                    failCall(www.error);
            }
            else
            {
                if (finishCall != null)
                {
                    YxDebug.Log("文件上传成功", "VoiceChatSystem", null, uploadUrl);
                    finishCall(www.text);
                }
            }
            www.Dispose();
        }

        /// <summary>
        /// 处理语音数据
        /// </summary>
        /// <param name="length"></param>
        /// <param name="url"></param>
        /// <param name="splitFlag">分割符</param>
        /// <returns></returns>
        public static string EncodeVoiceData(int length, string url, string splitFlag = ";")
        {
            return length + splitFlag + url;
        }

        /// <summary>
        /// 解析语音数据
        /// </summary>
        /// <param name="sourceData">数据源</param>
        /// <param name="length">解析后的语音长度</param>
        /// <param name="url">解析后的语音地址</param>
        /// <param name="splitFlag">分割标识</param>
        /// <returns></returns>
        public static void DecodeVoiceData(string sourceData,out int length,out string url, char splitFlag = ';')
        {
            if (!string.IsNullOrEmpty(sourceData))
            {
                string[] arr = sourceData.Split(splitFlag);
                if (arr.Length >= 2)
                {
                    int.TryParse(arr[0], out length);
                    url = arr[1];
                    return;
                }
            }
            length = 0;
            url = string.Empty;
        }

        /// <summary>
        /// 加载本地图片
        /// </summary>
        /// <param name="url">本地路径</param>
        /// <param name="callBack"></param>
        /// <param name="localHead"></param>
        /// <returns></returns>
        public static IEnumerator LoadLocalImage(string url, Action<Texture2D> callBack = null, string localHead = "file:///")
        {
            string filepath = localHead + url;
            WWW www = new WWW(filepath);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                if (callBack != null)
                {
                    callBack(null);
                }
            }
            else
            {
                if (callBack != null)
                {
                    callBack(www.texture);
                }
            }
        }

        /// <summary>
        /// 获取语音或图片地址
        /// </summary>
        /// <param name="reciveMessage"></param>
        /// <returns></returns>
        public static string GetUpLoadUrl(string reciveMessage)
        {
            var dic = Json.Decode(reciveMessage) as Dictionary<string, object>;
            if (dic != null)
            {
                var infoDic = dic[KeyData] as Dictionary<string, object>;
                if (infoDic != null)
                {
                    return infoDic[KeySrc].ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 解析默认数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ParseDefKeyDic(this Dictionary<string, object> data)
        {
            Dictionary<string, object> defDic;
            data.TryGetValueWitheKey(out defDic,KeyDefKey);
            return defDic;
        }


        /// <summary>
        /// 设置默认数据
        /// </summary>
        /// <param name="mainDic"></param>
        /// <param name="defDic"></param>
        /// <returns></returns>
        public static Dictionary<string, object> SetDefKeyDic(this Dictionary<string, object> mainDic, Dictionary<string, object> defDic)
        {
            if (mainDic==null)
            {
                mainDic=new Dictionary<string, object>();
            }
            mainDic[KeyDefKey] = defDic;
            return mainDic;
        }

        /// <summary>
        /// 获得当前的系统时间。默认开始时间为格林威治时间1970.1.1.0，即北京时间1970.1.1.8
        /// unix 为秒数转换
        /// js为毫秒
        /// </summary>
        /// <param name="svt"></param>
        /// <returns></returns>
        public static DateTime GetSvtTime(long svt)
        {
            DateTime s = new DateTime(1970, 1, 1, 8, 0, 0);
            s = s.AddSeconds(svt);
            return s;
        }
        /// <summary>
        /// 星期信息格式
        /// </summary>
        private static List<string> _weekInfo=new List<string>()
        {
            "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六", 
        };
        /// <summary>
        /// 获取消息显示时间格式
        /// </summary>
        /// <param name="lastTime"></param>
        /// <param name="nowTime"></param>
        /// <param name="deltaTime"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetMessageTimeShow(long lastTime,long nowTime,long deltaTime=3600,string format="{0} {1} {2}")
        {
            var returnTime = string.Empty;
            if (nowTime - lastTime >= deltaTime)
            {
                var talkTime = GetSvtTime(nowTime);
                if (DateTime.Now.Day!= talkTime.Day)
                {
                returnTime += _weekInfo[(int)talkTime.DayOfWeek];
                }
                returnTime = string.Format(format, returnTime, talkTime.Hour>=12?"下午":"上午",talkTime.ToString("hh:mm:ss"));
            }
            return returnTime;
        }

        /// <summary>
        /// 获取本地图片数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="tex"></param>
        /// <returns></returns>
        public static byte[] GetLocalImageData(string url, Texture2D tex)
        {
            if (tex==null||string.IsNullOrEmpty(url))
            {
                return new byte[0];
            }
            return Path.GetExtension(url).ToLower() == ".png"
                ? tex.EncodeToPNG()
                : tex.EncodeToJPG();
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        /// <param name="tex"></param>
        /// <param name="url"></param>
        public static void ShowImage(this YxBaseTextureAdapter tex,string url)
        {
            if (tex)
            {
                if (!string.IsNullOrEmpty(url) && (url.StartsWith("http://") || url.StartsWith("https://")))
                {
                    int hashCode = url.GetHashCode();
                    if (hashCode != 0 && tex.TextureHashCode == hashCode)
                        return;
                    tex.TextureHashCode = hashCode;
                    AsyncImage.Instance.GetAsyncImage(url, tex.SetTextureWithCheck);
                }
            }
        }

        #endregion
    }
}
