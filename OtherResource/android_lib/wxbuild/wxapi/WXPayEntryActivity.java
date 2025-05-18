/**
 * @Description: TODO
 * @author wangqiao
 * @Date 2017年4月12日 下午6:05:13
 */
package com.yxixia.jpmj.wxapi;

import com.tencent.mm.sdk.constants.ConstantsAPI;
import com.tencent.mm.sdk.modelbase.BaseReq;
import com.tencent.mm.sdk.modelbase.BaseResp;
import com.tencent.mm.sdk.openapi.IWXAPIEventHandler;
import com.unity3d.player.UnityPlayer;
import com.youxia.hall.MainActivity;
import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.MenuItem;
import android.widget.Toast;

/**
 * @Description: 微信支付Activity **
 * @author wangqiao
 * @Date 2017年4月12日 下午6:05:13
 */
public class WXPayEntryActivity extends Activity implements IWXAPIEventHandler {

	@Override  
	protected void onCreate(Bundle savedInstanceState) {  
		super.onCreate(savedInstanceState);  
		/*******接收微信授权*******/
		Intent intent = this.getIntent();
		if(intent != null){
			WXMain.api.handleIntent(intent, this);
//			finish();
		}
	}  

	@Override  
	public boolean onOptionsItemSelected(MenuItem item) {  
		if (item.getItemId() == android.R.id.home) {  
			this.finish();  
		}
		return super.onOptionsItemSelected(item);  
	}  

	@Override  
	protected void onNewIntent(Intent intent) {  
		super.onNewIntent(intent);  
		setIntent(intent);  
		WXMain.api.handleIntent(intent, this);  
//		finish();
	}  

	/* (non-Javadoc)
	 * @see com.tencent.mm.sdk.openapi.IWXAPIEventHandler#onReq(com.tencent.mm.sdk.modelbase.BaseReq)
	 */
	@Override
	public void onReq(BaseReq arg0) {

	}

	/* (non-Javadoc)
	 * @see com.tencent.mm.sdk.openapi.IWXAPIEventHandler#onResp(com.tencent.mm.sdk.modelbase.BaseResp)
	 */
	@Override
	public void onResp(BaseResp baseResp) {
		if (baseResp.getType() == ConstantsAPI.COMMAND_PAY_BY_WX) {  
			String returnInfo = "";
			if (baseResp.errCode == 0) {  
				Toast.makeText(MainActivity.Instance, "支付成功", 2).show();
				returnInfo += "paySuccess," + baseResp.errCode;
			} else if (baseResp.errCode == -1) {  
				Toast.makeText(MainActivity.Instance, "支付错误", 2).show();
				returnInfo += "payError," + baseResp.errStr;
			} else if (baseResp.errCode == -2) {  
				Toast.makeText(MainActivity.Instance, "取消支付", 2).show();
				returnInfo += "payCancel," + baseResp.errStr;
			}  
			
			UnityPlayer.UnitySendMessage(MainActivity.Instance.ReceiveGameObject, MainActivity.Instance.ReceiveMethod, returnInfo);
			this.finish();  
		} else {  
			
		}  
	}

}
