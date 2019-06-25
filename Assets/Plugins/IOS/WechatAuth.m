//
//  WechatAuth.m
//  Unity-iPhone
//
//  Created by 游侠 on 16/11/7.
//
//

#import "WechatAuth.h"
#import "WechatAuthSDK.h"
#import "WXApi.h"
#import "WXApiObject.h"

@implementation WechatAuth

static NSString* ReceiveGmaeObject;
static NSString* ReceiveMethod;

+(instancetype)sharedManager {
    static dispatch_once_t onceToken;
    static WechatAuth *instance;
    dispatch_once(&onceToken, ^{
        instance = [[WechatAuth alloc] init];
    });
    return instance;
}

void InitUnityInfo(const char* gameObject,const char* method){
    NSLog(@"进入 == InitUnityInfo");
    NSLog(@"%s",gameObject);
    NSLog(@"%s",method);
    ReceiveGmaeObject = [[NSString alloc] initWithUTF8String:gameObject];
    ReceiveMethod = [[NSString alloc] initWithUTF8String:method];
    //[RoomInfoListener InitRoomListener:ReceiveGmaeObject second:ReceiveMethod];
    //[RoomInfoListener TrySendUrlInfo];
}

void InitWechat(const char* appId){
    NSLog(@"进入 == InitWechat");
    NSString * id = [[NSString alloc] initWithUTF8String:appId];
    [WXApi registerApp:id];
}

void LoginWechat(){
    NSLog(@"进入 == LoginWechat");
    SendAuthReq* req = [[SendAuthReq alloc] init];
    
    req.scope = @"snsapi_userinfo";
    req.state = @"youxia_wechat_auth";
    [WXApi sendReq:req];
}

bool IsInstalledWechat(){
    NSLog(@"进入 == IsInstalledWechat");
    return [WXApi isWXAppInstalled];
}

bool IsChcekWechatApiLevel(){
    NSLog(@"进入 == IsChcekWechatApiLevel");
    return [WXApi isWXAppSupportApi];
}

//微信支付方法
void WXPayment(const char* appid,const char* partnerid,const char* prepayid,const char* packageV,const char* nonce,const char* time,const char* sign){
    if(appid == nil || appid == NULL){
        NSLog(@"appid为空");
        NSString *errorStr = @"error,支付失败：参数为空！appid";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    if(partnerid == nil || partnerid == NULL){
        NSLog(@"partnerid为空");
        NSString *errorStr = @"error,支付失败：参数为空！partnerid";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    if(prepayid == nil || prepayid == NULL){
        NSLog(@"prepayid为空");
        NSString *errorStr = @"error,支付失败：参数为空！prepayid";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    if(packageV == nil || packageV == NULL){
        NSLog(@"packageV为空");
        NSString *errorStr = @"error,支付失败：参数为空！packageV";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    if(nonce == nil || nonce == NULL){
        NSLog(@"nonce为空");
        NSString *errorStr = @"error,支付失败：参数为空！nonce";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    if(time == nil || time == NULL){
        NSLog(@"time为空");
        NSString *errorStr = @"error,支付失败：参数为空！time";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    if(sign == nil || sign == NULL){
        NSLog(@"sign为空");
        NSString *errorStr = @"error,支付失败：参数为空！sign";
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [errorStr UTF8String]);
        return;
    }
    
    PayReq *request = [[PayReq alloc] init];
    
    request.partnerId = [[NSString alloc]initWithUTF8String: partnerid];
    request.prepayId= [[NSString alloc]initWithUTF8String: prepayid];
    request.package = [[NSString alloc]initWithUTF8String: packageV];
    request.nonceStr= [[NSString alloc]initWithUTF8String: nonce];
    request.timeStamp = time;
    request.sign= [[NSString alloc]initWithUTF8String: sign];
    
    [WXApi sendReq:request];
}

//微信分享方法
void ShareContent(const char* json){
    NSError *error = nil;
    NSString *jsonStr = [[NSString alloc] initWithUTF8String:json];
    NSData *data = [jsonStr dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *shareInfo = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:&error];
    
    switch ([shareInfo[@"shareType"] intValue]) {
        case 0:
        {
            NSString *content = shareInfo[@"content"];
            if (content == nil || content == NULL) {
                NSLog(@"请输入分享文字！");
            }
            ShareText(shareInfo);
        }
            break;
        case 1:
        {
            NSString *imageUrl = shareInfo[@"imageUrl"];
            if (imageUrl == nil || imageUrl == NULL) {
                NSLog(@"请输入分享图片地址！");
            }
            ShareImage(shareInfo);
        }
            break;
        case 2:
        {
            NSString *musicUrl = shareInfo[@"musicUrl"];
            if (musicUrl == nil || musicUrl == NULL) {
                NSLog(@"请输入分享音乐地址！");
            }
            ShareMusic(shareInfo);
        }
            break;
        case 3:
        {
            NSString *videoUrl = shareInfo[@"videoUrl"];
            if (videoUrl == nil || videoUrl == NULL) {
                NSLog(@"请输入分享视频地址！");
            }
            ShareVideo(shareInfo);
        }
            break;
        case 4:
        {
            NSString *url = shareInfo[@"url"];
            if (url == nil || url == NULL) {
                NSLog(@"请输入分享网页地址！");
            }
            ShareWebsite(shareInfo);
        }
            break;
        default:
            NSLog(@"没有定义的分享类型！");
            break;
    }
}
//分享文字
void ShareText(NSDictionary *info){
    NSString *content = info[@"content"];
    
    int scene = 0;
    if([[info allKeys] containsObject:@"sence"]){
        scene = [info[@"sence"] intValue];
    }
    if([[info allKeys] containsObject:@"wechatSence"]){
        scene = [info[@"wechatSence"] intValue];
    }
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.text = content;
    req.bText = true;
    req.scene =scene;
    [WXApi sendReq:req];
}
//分享图片
void ShareImage(NSDictionary *info){
    NSString *imageUrl = info[@"imageUrl"];
    NSString *title = info[@"title"];
    NSString *content = info[@"content"];
    int scene = 0;
    if([[info allKeys] containsObject:@"sence"]){
        scene = [info[@"sence"] intValue];
    }
    if([[info allKeys] containsObject:@"wechatSence"]){
        scene = [info[@"wechatSence"] intValue];
    }
    bool imageLocal = [info[@"imageLocal"] boolValue];
    NSData *imgData;
    if (imageLocal) {
        imgData = [NSData dataWithContentsOfFile:imageUrl];
    }else{
        imgData = [NSData dataWithContentsOfURL:[NSURL URLWithString:imageUrl]];
    }
    
    UIImage *thumbImage = [[UIImage alloc] initWithData:imgData];
    
    CGSize newSize = CGSizeMake(192, 108);
    UIGraphicsBeginImageContext(newSize);
    [thumbImage drawInRect:CGRectMake(0,0,newSize.width,newSize.height)];
    UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    WXImageObject *imageO = [WXImageObject object];
    imageO.imageData = imgData;
    
    WXMediaMessage *msg = [WXMediaMessage message];
    msg.mediaObject = imageO;
    msg.title = title;
    msg.description = content;
    [msg setThumbImage:newImage];
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = msg;
    req.scene = scene;
    [WXApi sendReq:req];
}
//分享音频
void ShareMusic(NSDictionary *info){
    NSString *imageUrl = info[@"imageUrl"];
    NSString *musicUrl = info[@"musicUrl"];
    NSString *title = info[@"title"];
    NSString *content = info[@"content"];
    NSString *url = info[@"url"];
    int scene = 0;
    if([[info allKeys] containsObject:@"sence"]){
        scene = [info[@"sence"] intValue];
    }
    if([[info allKeys] containsObject:@"wechatSence"]){
        scene = [info[@"wechatSence"] intValue];
    }
    bool imageLocal = [info[@"imageLocal"] boolValue];
    NSData *imgData;
    if (imageLocal) {
        imgData = [NSData dataWithContentsOfFile:imageUrl];
    }else{
        imgData = [NSData dataWithContentsOfURL:[NSURL URLWithString:imageUrl]];
    }
    
    UIImage *thumbImage = [[UIImage alloc] initWithData:imgData];
    
    CGSize newSize = CGSizeMake(192, 108);
    UIGraphicsBeginImageContext(newSize);
    [thumbImage drawInRect:CGRectMake(0,0,newSize.width,newSize.height)];
    UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    WXMusicObject *musicO = [WXMusicObject object];
    musicO.musicUrl = url;
    musicO.musicLowBandUrl = musicO.musicUrl;
    musicO.musicDataUrl = musicUrl;
    musicO.musicLowBandDataUrl = musicO.musicDataUrl;
    
    WXMediaMessage *msg = [WXMediaMessage message];
    msg.mediaObject = musicO;
    msg.title = title;
    msg.description = content;
    [msg setThumbImage:newImage];
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = msg;
    req.scene = scene;
    
    [WXApi sendReq:req];
}
//分享视频
void ShareVideo(NSDictionary *info){
    NSString *imageUrl = info[@"imageUrl"];
    NSString *videoUrl = info[@"videoUrl"];
    NSString *title = info[@"title"];
    NSString *content = info[@"content"];
    NSString *url = info[@"url"];
    int scene = 0;
    if([[info allKeys] containsObject:@"sence"]){
        scene = [info[@"sence"] intValue];
    }
    if([[info allKeys] containsObject:@"wechatSence"]){
        scene = [info[@"wechatSence"] intValue];
    }
    bool imageLocal = [info[@"imageLocal"] boolValue];
    NSData *imgData;
    if (imageLocal) {
        imgData = [NSData dataWithContentsOfFile:imageUrl];
    }else{
        imgData = [NSData dataWithContentsOfURL:[NSURL URLWithString:imageUrl]];
    }
    
    UIImage *thumbImage = [[UIImage alloc] initWithData:imgData];
    
    CGSize newSize = CGSizeMake(192, 108);
    UIGraphicsBeginImageContext(newSize);
    [thumbImage drawInRect:CGRectMake(0,0,newSize.width,newSize.height)];
    UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    WXVideoObject *videoO = [WXVideoObject object];
    videoO.videoUrl = videoUrl;
    videoO.videoLowBandUrl = videoUrl;
    
    WXMediaMessage *msg = [WXMediaMessage message];
    msg.mediaObject =videoO;
    msg.title = title;
    msg.description = content;
    [msg setThumbImage:newImage];
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = msg;
    req.scene = scene;
    
    [WXApi sendReq:req];
}
//分享网址
void ShareWebsite(NSDictionary *info){
    NSString *imageUrl = info[@"imageUrl"];
    NSString *title = info[@"title"];
    NSString *content = info[@"content"];
    NSString *url = info[@"url"];
    int scene = 0;
    if([[info allKeys] containsObject:@"sence"]){
        scene = [info[@"sence"] intValue];
    }
    if([[info allKeys] containsObject:@"wechatSence"]){
        scene = [info[@"wechatSence"] intValue];
    }
    bool imageLocal = [info[@"imageLocal"] boolValue];
    NSData *imgData;
    if (imageLocal) {
        imgData = [NSData dataWithContentsOfFile:imageUrl];
    }else{
        imgData = [NSData dataWithContentsOfURL:[NSURL URLWithString:imageUrl]];
    }
    
    UIImage *thumbImage = [[UIImage alloc] initWithData:imgData];
    
    CGSize newSize = CGSizeMake(192, 108);
    UIGraphicsBeginImageContext(newSize);
    [thumbImage drawInRect:CGRectMake(0,0,newSize.width,newSize.height)];
    UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    WXWebpageObject *webpageO = [WXWebpageObject object];
    webpageO.webpageUrl = url;
    
    WXMediaMessage *msg = [WXMediaMessage message];
    msg.mediaObject = webpageO;
    msg.title = title;
    msg.description = content;
    [msg setThumbImage:newImage];
    
    SendMessageToWXReq *req = [[SendMessageToWXReq alloc] init];
    req.bText = NO;
    req.message = msg;
    req.scene = scene;
    
    [WXApi sendReq:req];
}

- (void)onResp:(BaseResp *)resp {
    if ([resp isKindOfClass:[SendMessageToWXResp class]]) {
        NSLog(@"进入 == SendMessageToWXResp");
        SendMessageToWXResp *messageResp = (SendMessageToWXResp *)resp;
        NSString *receive = @"send";
        switch (messageResp.errCode) {
            case WechatAuth_Err_Ok:
                receive = [receive stringByAppendingString:@"Success,,"];
                break;
            case WechatAuth_Err_Cancel:
                receive = [receive stringByAppendingString:@"Cancel,"];
                if (resp.errStr == nil) {
                    receive = [receive stringByAppendingString:@","];
                }else{
                    receive = [receive stringByAppendingString:resp.errStr];
                    receive = [receive stringByAppendingString:@","];
                }
                break;
            case WechatAuth_Err_NormalErr:
                receive = [receive stringByAppendingString:@"Failed,"];
                if (resp.errStr == nil) {
                    receive = [receive stringByAppendingString:@","];
                }else{
                    receive = [receive stringByAppendingString:resp.errStr];
                    receive = [receive stringByAppendingString:@","];
                }                break;
            default:
                receive = [receive stringByAppendingString:@"Failed,"];
                if (resp.errStr == nil) {
                    receive = [receive stringByAppendingString:@","];
                }else{
                    receive = [receive stringByAppendingString:resp.errStr];
                    receive = [receive stringByAppendingString:@","];
                }
                break;
        }
        
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [receive UTF8String]);
    } else if ([resp isKindOfClass:[SendAuthResp class]]) {
        NSLog(@"进入 == SendAuthResp");
        SendAuthResp *authResp = (SendAuthResp *)resp;
        
        NSString* receive;
        switch (authResp.errCode) {
            case WechatAuth_Err_Ok:
                receive = @"wechatLogin,";
                receive = [receive stringByAppendingString:authResp.code];
                break;
            case WechatAuth_Err_Cancel:
                receive = @"userCancel,";
                receive = [receive stringByAppendingString:authResp.errStr];
                break;
            case WechatAuth_Err_NormalErr:
                receive = @"authFailed,";
                receive = [receive stringByAppendingString:authResp.errStr];
                break;
            default:
                receive = @"authFailed,";
                receive = [receive stringByAppendingString:authResp.errStr];
                break;
        }
        NSLog(@"对象名 == ");
        NSLog(@"%@",ReceiveGmaeObject);
        NSLog(@"%@",ReceiveMethod);
        
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [receive UTF8String]);
        
    } else if ([resp isKindOfClass:[AddCardToWXCardPackageResp class]]) {
        NSLog(@"进入 == AddCardToWXCardPackageResp");
        AddCardToWXCardPackageResp *addCardResp = (AddCardToWXCardPackageResp *)resp;
    } else if ([resp isKindOfClass:[WXChooseCardResp class]]) {		
        NSLog(@"进入 == WXChooseCardResp");
        WXChooseCardResp *chooseCardResp = (WXChooseCardResp *)resp;
    } else if ([resp isKindOfClass:[PayResp class]]){
        NSLog(@"进入 == PayResp");
        PayResp *payResp = (PayResp*)resp;
        NSString *receive;
        switch (payResp.errCode) {
                //支付成功
            case WXSuccess:
                receive = @"paySuccess,";
                break;
                //用户取消支付
            case WXErrCodeUserCancel:
                receive = @"payCancel,";
                break;
                //支付失败
            case WXErrCodeCommon:
                receive = @"payError,";
                break;
                //支付失败
            default:
                receive = @"payError,";
                break;
        }
        
        UnitySendMessage([ReceiveGmaeObject UTF8String], [ReceiveMethod UTF8String], [receive UTF8String]);
    }
}

- (void)onReq:(BaseReq *)req {
    if ([req isKindOfClass:[GetMessageFromWXReq class]]) {
        NSLog(@"进入 == GetMessageFromWXReq");
        GetMessageFromWXReq *getMessageReq = (GetMessageFromWXReq *)req;
    } else if ([req isKindOfClass:[ShowMessageFromWXReq class]]) {
        NSLog(@"进入 == ShowMessageFromWXReq");
        ShowMessageFromWXReq *showMessageReq = (ShowMessageFromWXReq *)req;
    } else if ([req isKindOfClass:[LaunchFromWXReq class]]) {
        NSLog(@"进入 == LaunchFromWXReq");
        LaunchFromWXReq *launchReq = (LaunchFromWXReq *)req;
    }
}

@end
