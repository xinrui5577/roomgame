package com.yxixia.jpmj.wxapi;
import com.tencent.mm.sdk.constants.ConstantsAPI;
import com.tencent.mm.sdk.modelbase.BaseReq;
import com.tencent.mm.sdk.modelbase.BaseResp;
import com.tencent.mm.sdk.modelmsg.SendAuth;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.unity3d.player.UnityPlayer;
import com.youxia.hall.MainActivity;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

/**
 * @Description: 微信api接入Activity**
 * @author wangqiao
 * @Date 2016年12月9日 上午9:52:02
 */
public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
	
//	private static IWXAPI api;
	//微信验证用,自己定义发过去,原样发回,作比较
	protected static String weChatState;
	
//	/**
//	 * 缩略图大小
//	 */
//	private static final int THUMB_SIZE = 150;
	
	@Override
	protected void onCreate(Bundle savedInstanceState){
//		Log.i("Unity", "微信onCreate");
		super.onCreate(savedInstanceState);
		/*******接收微信授权*******/
		Intent intent = this.getIntent();
		if(intent != null){
			WXMain.api.handleIntent(intent, this);
			finish();
		}
	}
	
	@Override
	protected void onRestart() {
//		Log.i("Unity", "微信onRestart");
		super.onRestart();
	}
	
	//将appid注册到微信
	public static void InitWechat(String appId){
		WXMain.InitWechat(appId);
//		Log.i("Unity", "微信InitWechat");
//		api = WXAPIFactory.createWXAPI(MainActivity.Instance, appId, false);
//		
//		if (!api.registerApp(appId)) {
//			Log.i("Unity", "appId 注册失败!");
//		}
		
	}
	
	//申请微信授权
	public static void LoginWechat(){
		WXMain.LoginWechat();
//		SendAuth.Req req = new SendAuth.Req();
//		weChatState = "youxia_wechat_auth" + new Random().nextInt(300);
//		req.transaction = "auth";
//	    req.scope = "snsapi_userinfo";
//	    req.state = weChatState;
//	    api.sendReq(req);
	}
	
	//返回是否已安装微信
	public static boolean IsInstalledWechat(){
		return WXMain.IsInstalledWechat();
//		return api.isWXAppInstalled();
	}
	
	//检查微信是否符合最小api限制
	public static boolean IsCheckWechatApiLevel(){
		return WXMain.IsCheckWechatApiLevel();
//		return api.isWXAppSupportAPI();
	}
	
	/**
	 * @Description: unity调用微信分享
	 * @param json数据
	 * @return void
	 * @throws
	 */
	public static void ShareContent(String json){
		
		WXMain.ShareContent(json);
		
//		try {
//			
//			JSONObject jsonObj = new JSONObject(json);
//			switch (jsonObj.getInt("shareType")) {
//			case 0:
//				String content = jsonObj.getString("content");
//				if(content == null || content == ""){
//					Log.i("Unity", "请输入分享文字!");
//				}
//				ShareText(jsonObj);
//				break;
//			case 1:
//				String imageUrl = jsonObj.getString("imageUrl");
//				if(imageUrl == null || imageUrl == ""){
//					Log.i("Unity", "请输入分享图片地址!");
//				}
//				ShareImage(jsonObj);
//				break;
//			case 2:
//				String musicUrl = jsonObj.getString("musicUrl");
//				if(musicUrl == null || musicUrl == ""){
//					Log.i("Unity", "请输入分享音乐地址!");
//				}
//				ShareMusic(jsonObj);
//				break;
//			case 3:
//				String videoUrl = jsonObj.getString("videoUrl");
//				if(videoUrl == null || videoUrl == ""){
//					Log.i("Unity", "请输入分享视频地址!");
//				}
//				ShareVideo(jsonObj);
//				break;
//			case 4:
//				String Url = jsonObj.getString("url");
//				if(Url == null || Url == ""){
//					Log.i("Unity", "请输入分享网页地址!");
//				}
//				ShareWebsite(jsonObj);
//				break;
//
//			default:
//				Log.i("Unity", "没有定义的分享类型!");
//				break;
//			}
//			
//		} catch (JSONException e) {
//			e.printStackTrace();
//		}
	}
	
	/**
	 * @Description: 分享文字类型
	 */
//	public static void ShareText(JSONObject jsonO){
//		try {
//			String content = jsonO.getString("content");
//			WXTextObject textObj = new WXTextObject(content);
//			WXMediaMessage msg = new WXMediaMessage(textObj);
//			msg.description = content;
//			
//			SendMessageToWX.Req req = new SendMessageToWX.Req();
//			req.message = msg;
//			req.transaction = jsonO.getString("reqId");
//			req.scene = jsonO.getInt("wechatSence");
//			
//			api.sendReq(req);
//			
//		} catch (JSONException e) {
//			e.printStackTrace();
//		}
//	}
	
	/**
	 * @Description: 分享图片类型
	 */
//	public static void ShareImage(JSONObject jsonO){
//		try {
//			String content = jsonO.getString("content");
//			String imageUrl = jsonO.getString("imageUrl");
//			
//			Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
//			Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
//			
//			WXImageObject imageObj = new WXImageObject(bmp);
//			WXMediaMessage msg = new WXMediaMessage(imageObj);
//			msg.setThumbImage(thumbBmp);
//			msg.title = jsonO.getString("title");
//			msg.description = content;
//			bmp.recycle();
//			thumbBmp.recycle();
//			
//			SendMessageToWX.Req req = new SendMessageToWX.Req();
//			req.message = msg;
//			req.transaction = jsonO.getString("reqId");
//			req.scene = jsonO.getInt("wechatSence");
//			
//			api.sendReq(req);
//			
//		} catch (JSONException e) {
//			e.printStackTrace();
//		} catch (MalformedURLException e) {
//			e.printStackTrace();
//		} catch (IOException e) {
//			e.printStackTrace();
//		}
//	}
	
	/**
	 * @Description: 分享音频类型
	 */
//	public static void ShareMusic(JSONObject jsonO){
//		try {
//			String content = jsonO.getString("content");
//			String musicUrl = jsonO.getString("musicUrl");
//			String imageUrl = jsonO.getString("imageUrl");
//			String url = jsonO.getString("url");
//			WXMusicObject musicObj = new WXMusicObject();
//			musicObj.musicUrl = url;
//			musicObj.musicDataUrl = musicUrl;
//			WXMediaMessage msg = new WXMediaMessage(musicObj);
//			msg.description = content;
//			msg.title = jsonO.getString("title");
//			
//			if(imageUrl != null && imageUrl != ""){
//				Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
//				Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
//				msg.setThumbImage(thumbBmp);
//				bmp.recycle();
//				thumbBmp.recycle();
//			}
//			
//			SendMessageToWX.Req req = new SendMessageToWX.Req();
//			req.message = msg;
//			req.transaction = jsonO.getString("reqId");
//			req.scene = jsonO.getInt("wechatSence");
//			
//			api.sendReq(req);
//			
//		} catch (JSONException e) {
//			e.printStackTrace();
//		} catch (MalformedURLException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//	}
	
	/**
	 * @Description: 分享视频类型
	 */
//	public static void ShareVideo(JSONObject jsonO){
//		try {
//			String content = jsonO.getString("content");
//			String videoUrl = jsonO.getString("videoUrl");
//			String imageUrl = jsonO.getString("imageUrl");
//			WXVideoObject videoObj = new WXVideoObject();
//			videoObj.videoUrl = videoUrl;
//			WXMediaMessage msg = new WXMediaMessage(videoObj);
//			msg.description = content;
//			msg.title = jsonO.getString("title");
//			
//			if(imageUrl != null && imageUrl != ""){
//				Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
//				Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
//				msg.setThumbImage(thumbBmp);
//				bmp.recycle();
//				thumbBmp.recycle();
//			}
//			
//			SendMessageToWX.Req req = new SendMessageToWX.Req();
//			req.message = msg;
//			req.transaction = jsonO.getString("reqId");
//			req.scene = jsonO.getInt("wechatSence");
//			
//			api.sendReq(req);
//			
//		} catch (JSONException e) {
//			e.printStackTrace();
//		} catch (MalformedURLException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//	}
	
	/**
	 * @Description: 分享网址
	 */
//	public static void ShareWebsite(JSONObject jsonO){
//		try {
//			String content = jsonO.getString("content");
//			String imageUrl = jsonO.getString("imageUrl");
//			String url = jsonO.getString("url");
//			WXWebpageObject webPage = new WXWebpageObject();
//			webPage.webpageUrl = url;
//			WXMediaMessage msg = new WXMediaMessage(webPage);
//			msg.description = content;
//			msg.title = jsonO.getString("title");
//			
//			if(imageUrl != null && imageUrl != ""){
//				Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
//				Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
//				msg.setThumbImage(thumbBmp);
//				bmp.recycle();
//				thumbBmp.recycle();
//			}
//			
//			SendMessageToWX.Req req = new SendMessageToWX.Req();
//			req.message = msg;
//			req.transaction = jsonO.getString("reqId");
//			req.scene = jsonO.getInt("wechatSence");
//			
//			api.sendReq(req);
//			
//		} catch (JSONException e) {
//			e.printStackTrace();
//		} catch (MalformedURLException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (IOException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}
//	}
	
	@Override
	protected void onNewIntent(Intent intent) {
//		Log.i("Unity", "微信onNewIntent");
		super.onNewIntent(intent);
		/*******接收微信授权*******/
		setIntent(intent);
        WXMain.api.handleIntent(intent, this);
        finish();
	}

	@Override
	public void onReq(BaseReq req) {
		Log.i("Unity", "微信onReq openId = " + req.openId);
		
		switch (req.getType()) {
		case ConstantsAPI.COMMAND_GETMESSAGE_FROM_WX:
			break;
		case ConstantsAPI.COMMAND_SHOWMESSAGE_FROM_WX:
			break;
		default:
			break;
		}
	}

	@Override
	public void onResp(BaseResp resp) {
		String returnInfo = "";
		if(!resp.transaction.equals("auth")){
			//transaction不是纯数字则返回
			try {
				Integer.parseInt(resp.transaction);
			} catch (Exception e) {
				return;
			}
			
			//发送分享
			switch (resp.errCode) {
			case BaseResp.ErrCode.ERR_OK:
				returnInfo = "sendSuccess," + "," + resp.transaction;
				Toast.makeText(MainActivity.Instance, "分享成功", 2).show();
				break;
			case BaseResp.ErrCode.ERR_USER_CANCEL:
				returnInfo = "sendCancel," + resp.errStr + "," + resp.transaction;
				Toast.makeText(MainActivity.Instance, "取消分享", 2).show();
				break;
			case BaseResp.ErrCode.ERR_SENT_FAILED:
				returnInfo = "sendFailed," + resp.errStr + "," + resp.transaction;
				Toast.makeText(MainActivity.Instance, "分享失败" + resp.errStr, 2).show();
				break;
			default:
				returnInfo = "sendFailed," + resp.errStr + "," + resp.transaction;
				Toast.makeText(MainActivity.Instance, "分享失败" + resp.errStr, 2).show();
				break;
			}
		}else{
			//授权
			SendAuth.Resp respinfo = (SendAuth.Resp)resp;
			//state不一致即return
			if(!respinfo.state.equals(weChatState)) return;
			
			switch (respinfo.errCode) {
			case BaseResp.ErrCode.ERR_OK:
				returnInfo = "wechatLogin," + respinfo.code;
				Toast.makeText(MainActivity.Instance, "授权成功", 2).show();
				break;
			case BaseResp.ErrCode.ERR_USER_CANCEL:
				returnInfo = "userCancel," + respinfo.errStr;
				Toast.makeText(MainActivity.Instance, "取消授权", 2).show();
				break;
			case BaseResp.ErrCode.ERR_AUTH_DENIED:
				returnInfo = "authFailed," + respinfo.errStr;
				Toast.makeText(MainActivity.Instance, "授权失败", 2).show();
				break;
			default:
				returnInfo = "authFailed," + respinfo.errStr;
				Toast.makeText(MainActivity.Instance, "授权失败", 2).show();
				break;
			}
		}
		UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, returnInfo); 
	}

		

}
