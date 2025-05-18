/**
 * @Description: TODO
 * @author wangqiao
 * @Date 2017年4月12日 下午9:55:29
 */
package com.yxixia.jpmj.wxapi;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Random;
import org.json.JSONException;
import org.json.JSONObject;
import com.tencent.mm.sdk.modelmsg.SendAuth;
import com.tencent.mm.sdk.modelmsg.SendMessageToWX;
import com.tencent.mm.sdk.modelmsg.WXImageObject;
import com.tencent.mm.sdk.modelmsg.WXMediaMessage;
import com.tencent.mm.sdk.modelmsg.WXMusicObject;
import com.tencent.mm.sdk.modelmsg.WXTextObject;
import com.tencent.mm.sdk.modelmsg.WXVideoObject;
import com.tencent.mm.sdk.modelmsg.WXWebpageObject;
import com.tencent.mm.sdk.modelpay.PayReq;
import com.tencent.mm.sdk.openapi.IWXAPI;
import com.tencent.mm.sdk.openapi.WXAPIFactory;
import com.unity3d.player.UnityPlayer;
import com.youxia.hall.MainActivity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.util.Log;

/**
 * @Description: 微信sdk主控制类 **
 * @author wangqiao
 * @Date 2017年4月12日 下午9:55:29
 */
public class WXMain {

	/**
	 * 微信api
	 */
	protected static IWXAPI api;

	/**
	 * 缩略图大小
	 */
	private static final int THUMB_SIZE = 150;

	/**
	 * @Description: 将appid注册到微信
	 * @param @param appId
	 * @return void
	 * @throws
	 */
	public static void InitWechat(String appId){
		//		Log.i("Unity", "微信InitWechat");
		if(appId == null || appId == ""){
			Log.i("Unity", "appId不能为空!");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,appId不能为空!");
			return;
		}

		try{
			api = WXAPIFactory.createWXAPI(MainActivity.Instance, appId, false);
		}catch(Exception e){
			Log.i("Unity", "微信注册报错!");
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,微信注册报错:" + e.getMessage());
			return;
		}

		if (!api.registerApp(appId)) {
			Log.i("Unity", "appId 注册失败!");
		}

	}

	/**
	 * @Description: 返回是否已安装微信
	 * @param @return 是否
	 * @return boolean
	 * @throws
	 */
	public static boolean IsInstalledWechat(){

		try{
			return api.isWXAppInstalled();
		}catch(Exception e){
			Log.i("Unity", "检测安装微信报错!");
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,检测安装微信报错:" + e.getMessage());
			return false;
		}
	}

	/**
	 * @Description: 检查微信是否符合最小api限制
	 * @param @return 是否
	 * @return boolean
	 * @throws
	 */
	public static boolean IsCheckWechatApiLevel(){
		try{
			return api.isWXAppSupportAPI();
		}catch(Exception e){
			Log.i("Unity", "检测微信api报错!");
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,检测微信api报错:" + e.getMessage());
			return false;
		}
	}
	
	/**
	 * @Description: 微信付款
	 * @param 
	 * @return void
	 * @throws
	 */
	public static void WXPayment(String appid,String partnerid,String prepayid,String packageV,String nonce,String time,String sign){
		if(appid == null || appid == ""){
			Log.i("Unity", "appid为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!appid");
			return;
		}
		if(partnerid == null || partnerid == ""){
			Log.i("Unity", "partnerid为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!partnerid");
			return;
		}
		if(prepayid == null || prepayid == ""){
			Log.i("Unity", "prepayid为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!prepayid");
			return;
		}
		if(packageV == null || packageV == ""){
			Log.i("Unity", "packageV为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!packageV");
			return;
		}
		if(nonce == null || nonce == ""){
			Log.i("Unity", "nonce为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!nonce");
			return;
		}
		if(time == null || time == ""){
			Log.i("Unity", "time为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!time");
			return;
		}
		if(sign == null || sign == ""){
			Log.i("Unity", "sign为空");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付失败:参数为空!sign");
			return;
		}
		try{
			PayReq request = new PayReq();
			request.appId = appid;//微信appid
			request.partnerId = partnerid;//商户号
			request.prepayId = prepayid;//支付交易id
			request.packageValue = packageV;//自定义扩展
			request.nonceStr = nonce;//随机字符串
			request.timeStamp = time;//时间戳
			request.sign = sign;//md5签名
			api.sendReq(request);
		}catch(Exception e){
			Log.i("Unity", "微信支付报错!");
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,支付错误:" + e.getMessage());
		}
	}

	/**
	 * @Description: 申请微信授权
	 * @param 
	 * @return void
	 * @throws
	 */
	public static void LoginWechat(){
		try{
			SendAuth.Req req = new SendAuth.Req();
			WXEntryActivity.weChatState = "youxia_wechat_auth" + new Random().nextInt(300);
			req.transaction = "auth";
			req.scope = "snsapi_userinfo";
			req.state = WXEntryActivity.weChatState;
			api.sendReq(req);
		}catch(Exception e){
			Log.i("Unity", "微信授权报错!");
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,微信授权报错:" + e.getMessage());
		}
	}

	/**
	 * @Description: unity调用微信分享
	 * @param json数据
	 * @return void
	 * @throws
	 */
	public static void ShareContent(String json){

		try {
			
			Log.i("Unity","Json == " +  json);
			JSONObject jsonObj = new JSONObject(json);
			switch (jsonObj.getInt("shareType")) {
			case 0:
				String content = jsonObj.getString("content");
				if(content == null || content == ""){
					Log.i("Unity", "请输入分享文字!");
				}
				ShareText(jsonObj);
				break;
			case 1:
				String imageUrl = jsonObj.getString("imageUrl");
				if(imageUrl == null || imageUrl == ""){
					Log.i("Unity", "请输入分享图片地址!");
				}
				ShareImage(jsonObj);
				break;
			case 2:
				String musicUrl = jsonObj.getString("musicUrl");
				if(musicUrl == null || musicUrl == ""){
					Log.i("Unity", "请输入分享音乐地址!");
				}
				ShareMusic(jsonObj);
				break;
			case 3:
				String videoUrl = jsonObj.getString("videoUrl");
				if(videoUrl == null || videoUrl == ""){
					Log.i("Unity", "请输入分享视频地址!");
				}
				ShareVideo(jsonObj);
				break;
			case 4:
				String Url = jsonObj.getString("url");
				if(Url == null || Url == ""){
					Log.i("Unity", "请输入分享网页地址!");
				}
				ShareWebsite(jsonObj);
				break;

			default:
				Log.i("Unity", "没有定义的分享类型!");
				UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,没有定义的分享类型!");
				break;
			}

		} catch (JSONException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		}
	}

	/**
	 * @Description: 分享文字类型
	 */
	public static void ShareText(JSONObject jsonO){
		try {
			String content = jsonO.getString("content");
			WXTextObject textObj = new WXTextObject(content);
			WXMediaMessage msg = new WXMediaMessage(textObj);
			msg.description = content;

			SendMessageToWX.Req req = new SendMessageToWX.Req();
			req.message = msg;
			req.transaction = jsonO.getString("reqId");
			int mScene = 0;
			if(jsonO.has("sence")){
				mScene = jsonO.getInt("sence");
			}
			if(jsonO.has("wechatSence")){
				mScene = jsonO.getInt("wechatSence");
			}
			
			req.scene = mScene;

			api.sendReq(req);

		} catch (JSONException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		}
	}

	/**
	 * @Description: 分享图片类型
	 */
	public static void ShareImage(JSONObject jsonO){
		try {
			String content = jsonO.getString("content");
			String imageUrl = jsonO.getString("imageUrl");

			Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
			Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);

			WXImageObject imageObj = new WXImageObject(bmp);
			WXMediaMessage msg = new WXMediaMessage(imageObj);
			msg.setThumbImage(thumbBmp);
			msg.title = jsonO.getString("title");
			msg.description = content;
			bmp.recycle();
			thumbBmp.recycle();

			SendMessageToWX.Req req = new SendMessageToWX.Req();
			req.message = msg;
			req.transaction = jsonO.getString("reqId");
			int mScene = 0;
			if(jsonO.has("sence")){
				mScene = jsonO.getInt("sence");
			}
			if(jsonO.has("wechatSence")){
				mScene = jsonO.getInt("wechatSence");
			}
			
			req.scene = mScene;

			api.sendReq(req);

		} catch (JSONException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (MalformedURLException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (IOException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		}
	}

	/**
	 * @Description: 分享音频类型
	 */
	public static void ShareMusic(JSONObject jsonO){
		try {
			String content = jsonO.getString("content");
			String musicUrl = jsonO.getString("musicUrl");
			String imageUrl = jsonO.getString("imageUrl");
			String url = jsonO.getString("url");
			WXMusicObject musicObj = new WXMusicObject();
			musicObj.musicUrl = url;
			musicObj.musicDataUrl = musicUrl;
			WXMediaMessage msg = new WXMediaMessage(musicObj);
			msg.description = content;
			msg.title = jsonO.getString("title");

			if(imageUrl != null && imageUrl != ""){
				Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
				Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
				msg.setThumbImage(thumbBmp);
				bmp.recycle();
				thumbBmp.recycle();
			}

			SendMessageToWX.Req req = new SendMessageToWX.Req();
			req.message = msg;
			req.transaction = jsonO.getString("reqId");
			int mScene = 0;
			if(jsonO.has("sence")){
				mScene = jsonO.getInt("sence");
			}
			if(jsonO.has("wechatSence")){
				mScene = jsonO.getInt("wechatSence");
			}
			
			req.scene = mScene;

			api.sendReq(req);

		} catch (JSONException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (MalformedURLException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (IOException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		}
	}

	/**
	 * @Description: 分享视频类型
	 */
	public static void ShareVideo(JSONObject jsonO){
		try {
			String content = jsonO.getString("content");
			String videoUrl = jsonO.getString("videoUrl");
			String imageUrl = jsonO.getString("imageUrl");
			WXVideoObject videoObj = new WXVideoObject();
			videoObj.videoUrl = videoUrl;
			WXMediaMessage msg = new WXMediaMessage(videoObj);
			msg.description = content;
			msg.title = jsonO.getString("title");

			if(imageUrl != null && imageUrl != ""){
				Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
				Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
				msg.setThumbImage(thumbBmp);
				bmp.recycle();
				thumbBmp.recycle();
			}

			SendMessageToWX.Req req = new SendMessageToWX.Req();
			req.message = msg;
			req.transaction = jsonO.getString("reqId");
			int mScene = 0;
			if(jsonO.has("sence")){
				mScene = jsonO.getInt("sence");
			}
			if(jsonO.has("wechatSence")){
				mScene = jsonO.getInt("wechatSence");
			}
			
			req.scene = mScene;

			api.sendReq(req);

		} catch (JSONException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (MalformedURLException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (IOException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		}
	}

	/**
	 * @Description: 分享网址
	 */
	public static void ShareWebsite(JSONObject jsonO){
		try {
			String content = jsonO.getString("content");
			String imageUrl = jsonO.getString("imageUrl");
			String url = jsonO.getString("url");
			WXWebpageObject webPage = new WXWebpageObject();
			webPage.webpageUrl = url;
			WXMediaMessage msg = new WXMediaMessage(webPage);
			msg.description = content;
			msg.title = jsonO.getString("title");

			if(imageUrl != null && imageUrl != ""){
				Bitmap bmp = BitmapFactory.decodeStream(new URL(imageUrl).openStream());
				Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, THUMB_SIZE, THUMB_SIZE, true);
				msg.setThumbImage(thumbBmp);
				bmp.recycle();
				thumbBmp.recycle();
			}

			SendMessageToWX.Req req = new SendMessageToWX.Req();
			req.message = msg;
			req.transaction = jsonO.getString("reqId");
			int mScene = 0;
			if(jsonO.has("sence")){
				mScene = jsonO.getInt("sence");
			}
			if(jsonO.has("wechatSence")){
				mScene = jsonO.getInt("wechatSence");
			}
			
			req.scene = mScene;

			api.sendReq(req);

		} catch (JSONException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (MalformedURLException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		} catch (IOException e) {
			e.printStackTrace();
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, "error,分享错误:" + e.getMessage());
		}
	}
}
