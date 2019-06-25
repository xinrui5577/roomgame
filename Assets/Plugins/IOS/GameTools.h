//
//  GameTools.h
//  Unity-iPhone
//
//  Created by 安德 on 2017/9/21.
//
#import <Foundation/Foundation.h>
@interface GameTools:NSObject
{
    NSString * saveRoomInfo;
}

+(void)TrySendUrlInfo;
+(bool)IsNotNullOrEmpty:(NSString *) str;
void InitToolsListener(const char* objName,const char* methodName);
+(void)SaveUrlInfo:(NSString *) url;
void SaveClipText(const char* content);
void UpdateFileState(const char* path);
const char* GetClipText();
const char* GetSystemInfo();
@end
