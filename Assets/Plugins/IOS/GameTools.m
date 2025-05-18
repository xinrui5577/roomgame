//
//  GameTools.m
//  Unity-iPhone
//
//  Created by 安德 on 2017/9/21.
//

#import "GameTools.h"
#include "WechatAuth.h"
@implementation GameTools
static NSString* SaveRoomInfo;
static NSString* ReciveObject;
static NSString* ReciveMethod;

+(void)TrySendUrlInfo
{
    NSLog(@"调用尝试发送参数");
    if ([GameTools IsNotNullOrEmpty:ReciveObject]&&[GameTools IsNotNullOrEmpty:ReciveMethod])
    {
        if ([GameTools IsNotNullOrEmpty:SaveRoomInfo])
        {
            NSString* sendInfo=@"RoomInfo,";
            sendInfo=[sendInfo stringByAppendingString:SaveRoomInfo];
            NSLog(@"向unity发送房间相关信息%@",sendInfo);
            UnitySendMessage([ReciveObject UTF8String], [ReciveMethod UTF8String], [sendInfo UTF8String]);
            SaveRoomInfo=@"";
        }
    }
}
+(bool)IsNotNullOrEmpty:(NSString *) str
{
    return str!=NULL&&![str isEqual:@""];
}

void InitToolsListener(const char* objName,const char* methodName)
{
		NSLog(@"进入 == InitRoomListener");
    NSLog(@"%s",objName);
    NSLog(@"%s",methodName);
    
    ReciveObject=[[NSString alloc] initWithUTF8String:objName];
    ReciveMethod=[[NSString alloc] initWithUTF8String:methodName];
    [GameTools TrySendUrlInfo];
}

+(void)SaveUrlInfo:(NSString*) url
{
    SaveRoomInfo=url;
}

void SaveClipText(const char* content)
{
    UIPasteboard* pasteboard = [UIPasteboard generalPasteboard];
    [pasteboard setString:[[NSString alloc] initWithUTF8String:content]];
}

const char* GetClipText()
{
    UIPasteboard* pasteboard = [UIPasteboard generalPasteboard];
    if (pasteboard==NULL) {
        return strdup("");
    }
    const char * saveInfo=[pasteboard.string UTF8String];
    if (saveInfo==NULL) {
        return strdup("");
    }
    return strdup(saveInfo);
}

const char* GetSystemInfo()
{
    NSDictionary *systemAttributes = [[NSFileManager defaultManager]attributesOfFileSystemForPath:NSHomeDirectory()error:nil];
    NSString *diskTotalSize = [systemAttributes objectForKey:NSFileSystemSize];
    NSString *diskFreeSize = [systemAttributes objectForKey:NSFileSystemFreeSize];
    NSMutableDictionary *infoDic=[NSMutableDictionary dictionary];
    bool state=false;
    [infoDic setObject:@(state) forKey: @"IsRemovable" ];
    [infoDic setObject:diskFreeSize forKey: @"FreeBlock" ];
    [infoDic setObject:@"磁盘" forKey: @"Desc" ];
    [infoDic setObject:diskTotalSize forKey: @"TotalBlock" ];
    [infoDic setObject:NSHomeDirectory() forKey: @"Path" ];
    for (NSString *string in infoDic) {
        NSLog(@"key is:%@,value is:%@",string,[infoDic objectForKey:string]);
    }
    
    NSArray *array=[NSArray arrayWithObject:infoDic];
    NSMutableDictionary *systemInfoDic=[NSMutableDictionary dictionary];
    [systemInfoDic setObject:array forKey: @"StorageInfos" ];
    NSData *data = [NSJSONSerialization dataWithJSONObject:systemInfoDic options: NSJSONWritingPrettyPrinted error:NULL];
    NSString *jsonStr = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    NSLog(@"jsonStr is :%@",jsonStr);
    const char * saveInfo=[jsonStr UTF8String];
    return strdup([jsonStr UTF8String]);
}

void UpdateFileState(const char* path)
{
    if (path==NULL)
    {
      return;
    }
	NSString *strReadAddr = [[NSString alloc] initWithUTF8String:path];
    UIImage *img = [UIImage imageWithContentsOfFile:strReadAddr];  
    NSLog(@"%@", [NSString stringWithFormat:@"w:%f, h:%f", img.size.width, img.size.height]);
    NSObject *instance = [NSObject alloc];
    UIImageWriteToSavedPhotosAlbum(img, instance,nil, nil);
}
@end

