using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// 解码调整
    /// </summary>
    public class HF_Decoder
    {
        //一定不能出现大于等于0xDF的数,否则输出解码会超过12位(10进制)
        public enum MsgCode
        {
            None = 0,
            TouBiBiLi = 0xA0,
            ChangDiLeiXing = 0xA1,
            DaMaTianShu = 0xA2,
            XiTongShiJian = 0xA3,
            XianShiDaMaXinXi = 0xA4
        }
        public static MsgCode GetMsgPlainType(byte[] msgPlain)
        {
            if (msgPlain[0] == (byte)MsgCode.TouBiBiLi)
                return MsgCode.TouBiBiLi;

            if (msgPlain[0] == (byte)MsgCode.ChangDiLeiXing)
                return MsgCode.ChangDiLeiXing;
            if (msgPlain[0] == (byte)MsgCode.DaMaTianShu)
                return MsgCode.DaMaTianShu;
            if (msgPlain[0] == (byte)MsgCode.XiTongShiJian)
                return MsgCode.XiTongShiJian;
            if (msgPlain[0] == (byte)MsgCode.XianShiDaMaXinXi)
                return MsgCode.XianShiDaMaXinXi;
            return MsgCode.None;
        }

        /// <summary>
        /// 投币比例 ,转换为消息
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] TouBiBiLi_ToMsgPlain(int num, uint tableNum, uint tagNum)
        {

            byte[] numBytes = BitConverter.GetBytes((ushort)num);
            byte[] msg = new byte[] { ((byte)MsgCode.TouBiBiLi), numBytes[0], numBytes[1], (byte)tagNum, (byte)(tableNum ^ tagNum) };//补位

            return msg;
        }

        /// <summary>
        /// 消息->投币比例
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static int TouBiBiLi_FromMsgPlain(byte[] plain, uint tableNum, uint tagNum, out bool isVerifySucess)
        {
            if (plain[3] == (byte)tagNum && plain[4] == (byte)(tableNum ^ tagNum))
                isVerifySucess = true;
            else
                isVerifySucess = false;

            return (int)(BitConverter.ToUInt16(new byte[] { plain[1], plain[2] }, 0));
        }

    

        /// <summary>
        /// 场地类型 -> 消息
        /// </summary>
        /// <param name="type">0:小,1:中,2:大</param>
        /// <returns></returns>
        public static byte[] ChangeDiLeiXing_ToMsgPlain(int type, uint tableNum, uint tagNum)
        {

            byte[] msg = new byte[] { ((byte)MsgCode.ChangDiLeiXing), (byte)type, (byte)tableNum, (byte)tagNum, (byte)(tableNum ^ tagNum) };//补位

            return msg;
        }

        /// <summary>
        /// 消息->场地类型
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static int ChangeDiLeiXing_FromMsgPlain(byte[] plain, uint tableNum, uint tagNum, out bool isVerifySucess)
        {
            if (plain[2]== (byte)tableNum && plain[3] == (byte)tagNum && plain[4] == (byte)(tableNum ^ tagNum))
                isVerifySucess = true;
            else
                isVerifySucess = false;

            return plain[1];
        }
     

        /// <summary>
        /// 打码天数 -> 消息
        /// </summary>
        /// <param name="type">1~13(+1)</param>
        /// <returns></returns>
        public static byte[] DaMaTianShu_ToMsgPlain(int day, uint tableNum, uint tagNum)
        {

            byte[] msg = new byte[] { ((byte)MsgCode.DaMaTianShu), (byte)day, (byte)tableNum, (byte)tagNum, (byte)(tableNum ^ tagNum) };//补位

            return msg;
        }

        /// <summary>
        /// 消息->打码天数
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static int DaMaTianShu_FromMsgPlain(byte[] plain, uint tableNum, uint tagNum, out bool isVerifySucess)
        {
            if (plain[2] == (byte)tableNum && plain[3] == (byte)tagNum && plain[4] == (byte)(tableNum ^ tagNum))
                isVerifySucess = true;
            else
                isVerifySucess = false;

            return plain[1];
        }


        /// <summary>
        /// 显示打码信息 -> 消息
        /// </summary>
        /// <param name="type">0:false,1:true</param>
        /// <returns></returns>
        public static byte[] XianShiDaMaXinXi_ToMsgPlain(bool isView, uint tableNum, uint tagNum)
        {

            byte[] msg = new byte[] { ((byte)MsgCode.XianShiDaMaXinXi), (byte)(isView ? 1 : 0), (byte)tableNum, (byte)tagNum, (byte)(tableNum ^ tagNum) };//补位

            return msg;
        }

        /// <summary>
        /// 消息->显示打码信息
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public static bool XianShiDaMaXinXi_FromMsgPlain(byte[] plain, uint tableNum, uint tagNum, out bool isVerifySucess)
        {
            if (plain[2] == (byte)tableNum && plain[3] == (byte)tagNum && plain[4] == (byte)(tableNum ^ tagNum))
                isVerifySucess = true;
            else
                isVerifySucess = false;

            return plain[1] == 1 ? true : false;
        }


        /// <summary>
        /// 系统时间 -> 消息
        /// </summary>
        /// <param name="type">0:false,1:true</param>
        /// <returns></returns>
        public static byte[] XiTongShiJian_ToMsgPlain(uint year, uint month, uint day, uint hour, uint minit, uint tableNum, uint tagNum)
        {
            //7,4,5,5,6
            uint data = (year << 25) | (month << 21) | (day << 16) | (hour << 11) | (minit << 5);
            byte[] dataBytes = BitConverter.GetBytes(data);
            byte[] msg = new byte[] { ((byte)MsgCode.XiTongShiJian), dataBytes[0], dataBytes[1], dataBytes[2], dataBytes[3] };

            return msg;
        }

        /// <summary>
        /// 消息->系统时间
        /// </summary>
        /// <param name="plain"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minit"></param>
        public static void XiTongShiJian_FromMsgPlain(byte[] plain, out uint year, out uint month, out uint day, out uint hour, out uint minit)
        {
            byte[] dataBytes = new byte[] { plain[1], plain[2], plain[3], plain[4] };
            uint data = BitConverter.ToUInt32(dataBytes, 0);
            year = data >> 25;
            month = data << 7 >> 28;
            day = data << 11 >> 27;
            hour = data << 16 >> 27;
            minit = data << 21 >> 26;

        }


        /// <summary>
        /// 用 机台号 和 解码特征码 加密 明文
        /// </summary>
        /// <param name="plain"></param>
        /// <param name="tableNum"></param>
        /// <param name="tagNum"></param>
        /// <returns></returns>
        public static byte[] Encrypt_Msg(byte[] data, uint tableNum, uint tagNum)
        {
            byte[] keyBytes = BitConverter.GetBytes(tableNum ^ tagNum);
            byte keyByte = 0;
            for (int i = 0; i != keyBytes.Length; ++i)
                keyByte ^= keyBytes[i];


            for (int j = 0; j != 2; ++j)
                for (int i = 1; i != data.Length; ++i)
                {
                    data[i] ^= keyByte;
                    keyByte = data[i];
                }

            return data;
        }

        /// <summary>
        /// 解密信息 (需要机台号+特征码)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tableNum"></param>
        /// <param name="tagNum"></param>
        /// <returns></returns>
        public static byte[] Decrypt_Msg(byte[] data, uint tableNum, uint tagNum)
        {
            byte[] keyBytes = BitConverter.GetBytes(tableNum ^ tagNum);
            byte keyByte = 0;
            for (int i = 0; i != keyBytes.Length; ++i)
                keyByte ^= keyBytes[i];

            byte nextData;
            //int j = 0;
            for (int j = 0; j != 2; ++j)
                for (int i = data.Length - 1; i != 0; --i)
                {
                    if (j == 0 && i == 1)//第一趟最后使用data最后一个数据
                        nextData = data[data.Length - 1];
                    else if (j == 1 && i == 1)//第二趟最好需要用到keybyte
                        nextData = keyByte;
                    else
                        nextData = data[i - 1];
                    //data[i] ^= keyByte;
                    data[i] ^= nextData;
                    //nextData = i == 0 ? data[data.Length - 1] : data[i - 2];

                }

            return data;
        }

        /// <summary>
        /// 消息 -> 10进制码
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static ulong MsgToDecimalCode(byte[] msg)
        {
            byte[] ba = new byte[8];
            for (int i = 0; i != msg.Length; ++i)
                ba[i] = msg[msg.Length - i - 1];

            return BitConverter.ToUInt64(ba, 0);
        }

        /// <summary>
        /// 10进制码->消息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static byte[] MsgFromDecimalCode(ulong code)
        {
            byte[] ba = BitConverter.GetBytes(code);
            byte[] outVal = new byte[5];
            //Array.Copy(ba, outVal, 5);
            for (int i = 0; i != 5; ++i)
                outVal[i] = ba[4 - i];
            return outVal;
        }

        /// <summary>
        /// 生成特征码
        /// </summary>
        /// <param name="tableIdx">台号</param>
        /// <param name="lineIdx">线号</param>
        /// <param name="formulaCode">公式码</param>
        /// <param name="gameIdx">游戏索引</param>
        /// <remarks>
        /// 注意:
        ///     1.为兼容以前版本,当判断 (formulaCode==0xffffffff && gameIdx==0xffffffff)时,则忽略formulaCode,gameIdx两个参数
        /// 不足:
        ///     1.不一定有8位,需要外部补完
        /// </remarks>
        /// <returns></returns>
        public static uint GenerateTagCode(uint tableIdx, uint lineIdx, uint formulaCode, uint gameIdx)
        {
            uint cipher0;
            if (formulaCode == 0xffffffff && gameIdx == 0xffffffff)//为兼容以前版本
            {
                cipher0 = (tableIdx ^ lineIdx) & 0x3FFFFFF;//屏蔽两位
            }
            else
            {
                cipher0 = (tableIdx ^ lineIdx ^ formulaCode ^ gameIdx) & 0x3FFFFFF;//屏蔽两位
            }


            uint cipher = (cipher0 ^ ((uint)new Random().Next(0, 7143) * 9394));//保证生成数范围足够大(10进制八位)

            //if (cipher > 99999999)
            //    return GenerateTagCode(tableIdx, lineIdx);

            return cipher;
        }


        /// <summary>
        /// 验证特征码是否正确
        /// </summary>
        /// <param name="tagCode">待验证特征码</param>
        /// <param name="tableIdx">台号</param>
        /// <param name="lineIdx">线号</param>
        /// <param name="formulaCode">公式码</param>
        /// <param name="gameIdx">游戏索引</param>
        /// <remarks>
        ///  注意:
        ///     1.为兼容以前版本,当判断 (formulaCode==0xffffffff && gameIdx==0xffffffff)时,则忽略formulaCode,gameIdx两个参数
        /// </remarks>
        /// <returns></returns>
        public static bool IsValidTagCode(uint tagCode, uint tableIdx, uint lineIdx, uint formulaCode, uint gameIdx)
        {
            uint rndNum;
            if (formulaCode == 0xffffffff && gameIdx == 0xffffffff)//为兼容以前版本
            {
                rndNum = tagCode ^ ((tableIdx ^ lineIdx) & 0x3FFFFFF);
            }
            else
            {
                rndNum = tagCode ^ ((tableIdx ^ lineIdx ^ formulaCode ^ gameIdx) & 0x3FFFFFF);
            }

            if ((rndNum) % 9394 == 0)
                return true;
            else
                return false;
        }
    }


    /// <summary>
    /// 打码报账 相关
    /// </summary>
    class HF_CodePrint
    {
        public static uint EncryptUint4Bit(uint ui)
        {
            uint outVal = ui;
            for (int i = 0; i != 8; ++i)
                outVal = (outVal >> 4) ^ ui;
            return outVal & 0xF;
        }
 
        /// <summary>
        /// 游戏数据 -> 报账码(可逆)
        /// </summary>
        /// <param name="gainTotal"></param>
        /// <param name="gainCurrent"></param>
        /// <param name="tableIdx"></param>
        /// <param name="numPrint"></param>
        /// <param name="codeComfirm"></param>
        /// <param name="gainAdjustIdx">
        /// 6bit数据
        /// (0-20)盈利调整索引
        /// "保持现状","放水一千","放水二千","放水三千","放水四千","放水五千","放水八千","放水一万","放水两万","放水五万","放水十万" ,"抽水一千","抽水二千","抽水三千","抽水四千","抽水五千","抽水八千","抽水一万","抽水两万","抽水五万","抽水十万" </param>
        /// (30-36)加难设置六档难度
        /// 30:不加难 ， 31-36：六档难度
        /// <remarks>
        /// 注意：
        /// 1.输出uint只有26bit有效(打码只有8位十进制数),高6bit必须为0
        /// 2.【弊端】输入抽放水和难度设置不能同时打
        /// </remarks>
        /// <returns></returns>
        public static uint DataToPrintCode(int gainTotal, int gainCurrent, uint tableIdx, uint numPrint, uint codeComfirm, uint gainAdjustIdx)
        {
            if (gainAdjustIdx > 63)//如果输入值大于63，则会在VerifyPrintCode中返回错误的gainAdjustIdx，导致错误操作
                return 0;

            uint outVal = 0;
            outVal |= EncryptUint4Bit((uint)gainTotal) << 16;
            outVal |= EncryptUint4Bit((uint)gainCurrent) << 12;
            outVal |= EncryptUint4Bit(tableIdx) << 8;
            outVal |= EncryptUint4Bit(numPrint) << 4;
            outVal |= EncryptUint4Bit(codeComfirm);
            outVal |= gainAdjustIdx << 20; 
        

            //混淆
            outVal = (outVal << 13) & 0x3FFFFFF | (outVal >> 13); //13位前后互换

            uint mask = 0x2000000;
            uint vec = 0;
            uint[] masks = new uint[26];
            uint[] deMasks = new uint[26];
            for (int i = 0; i != 26; ++i)
            {
                masks[i] = mask >> (i + 1);
                deMasks[i] = masks[i] ^ 0xffffffff;
            }

            for (int j = 0; j != 2; ++j)
                for (int i = 0; i != 26; ++i)
                {
                    uint a1 = (vec ^ outVal) & masks[i];
                    uint a2 = outVal & deMasks[i];
                    vec = a1 >> 1;
                    outVal = a1 | a2;
                }

            return outVal;
        }


   
        /// <summary>
        /// 验证报账码,并返回gainAdjustIdx
        /// </summary>
        /// <param name="printCodeEnc"></param>
        /// <param name="gainTotal"></param>
        /// <param name="gainCurrent"></param>
        /// <param name="tableIdx"></param>
        /// <param name="numPrint"></param>
        /// <param name="codeComfirm"></param>
        /// <param name="gainAdjustIdx">抽放水系数(0-20)</param>
        /// <param name="gainRatioMulti">游戏加难设置(1-6)</param>
        /// <returns>printCode正确</returns>
        public static bool VerifyPrintCode(uint printCodeEnc, int gainTotal, int gainCurrent, uint tableIdx, uint numPrint, uint codeComfirm
                                           , ref uint gainAdjustIdx
                                           , ref uint gainRatioMulti)
        {
            //解密printCode
            uint mask = 0x2000000;
            uint vec = 0;
            uint[] masks = new uint[26];
            uint[] deMasks = new uint[26];
            for (int i = 0; i != 26; ++i)
            {
                masks[i] = mask >> (i + 1);
                deMasks[i] = masks[i] ^ 0xffffffff;
            }

            uint decryptPrintCode = printCodeEnc;
            int bitStart = 25;
            for (int j = 1; j != -1; --j)
            {
                for (int i = bitStart; i != 0; --i)
                {
                    vec = decryptPrintCode & masks[i - 1];//取得最后袷

                    uint a1 = ((vec >> 1) ^ decryptPrintCode) & masks[i];
                    uint a2 = decryptPrintCode & deMasks[i];
                    decryptPrintCode = a1 | a2;
                }
                //处理最后一个
                vec = j != 0 ? decryptPrintCode & masks[bitStart] : 0;
                uint b1 = ((vec >> 1) ^ decryptPrintCode) & masks[0];
                uint b2 = decryptPrintCode & deMasks[0];
                decryptPrintCode = b1 | b2;
            }
            decryptPrintCode = (decryptPrintCode << 13) & 0x3FFFFFF | (decryptPrintCode >> 13); //13位前后互换
            uint outVal = decryptPrintCode >> 20;
            if (outVal >= 0 && outVal <= 20)
                gainAdjustIdx = outVal;
            else if (outVal >= 30 && outVal <= 36)//根据函数DataToPrintCode中的值规定
                gainRatioMulti = outVal % 30;

            uint calcPrintCode = 0;
            calcPrintCode |= EncryptUint4Bit((uint)gainTotal) << 16;
            calcPrintCode |= EncryptUint4Bit((uint)gainCurrent) << 12;
            calcPrintCode |= EncryptUint4Bit(tableIdx) << 8;
            calcPrintCode |= EncryptUint4Bit(numPrint) << 4;
            calcPrintCode |= EncryptUint4Bit(codeComfirm);

            return (decryptPrintCode & 0xFFFFF) == calcPrintCode;
        }

        /// <summary>
        ///[作废] 游戏数据 -> 报账码
        /// </summary>
        /// <param name="gainTotal"></param>
        /// <param name="gainCurrent"></param>
        /// <param name="tableIdx"></param>
        /// <param name="numPrint"></param>
        /// <param name="codeComfirm"></param>
        /// <returns></returns>
        //public static uint DataToPrintCode(int gainTotal,int gainCurrent, uint tableIdx,uint numPrint, uint codeComfirm)
        //{
        //    List<byte> dataCollect = new List<byte>();
        //    byte[] byteBuf = BitConverter.GetBytes(gainTotal);
        //    foreach (byte b in byteBuf)
        //        dataCollect.Add(b);

        //    byteBuf = BitConverter.GetBytes(gainCurrent);
        //    foreach (byte b in byteBuf)
        //        dataCollect.Add(b);
        //    byteBuf = BitConverter.GetBytes(tableIdx);
        //    foreach (byte b in byteBuf)
        //        dataCollect.Add(b);

        //    byteBuf = BitConverter.GetBytes(numPrint);
        //    foreach (byte b in byteBuf)
        //        dataCollect.Add(b);

        //    byteBuf = BitConverter.GetBytes(codeComfirm);
        //    foreach (byte b in byteBuf)
        //        dataCollect.Add(b);

        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] md5data = md5.ComputeHash(dataCollect.ToArray());//计算data字节数组的哈希值 
        //    md5.Clear();

        //    for (int i = 0; i != 4; ++i)
        //        md5data[i+12] = (byte)(md5data[i] ^ md5data[i + 4] ^ md5data[i + 8] ^ md5data[i + 12]);

        //    uint outCode = BitConverter.ToUInt32(md5data, 12);//取最后12位

        //    outCode %= 100000000;//得出8位码
        //    return outCode;
        //}

        /// <summary>
        /// 生成四位验证码
        /// </summary>
        /// <param name="gainTotal">总盈利数</param>
        /// <param name="gainCurrent">当期盈利数</param>
        /// <param name="lineIdx">线号</param>
        /// <param name="tableIdx">台号</param>
        /// <param name="numPrint">打码次数</param>
        /// <param name="formulaCode">公式码</param>
        /// <param name="gameIdx">游戏索引</param>
        /// <remarks>
        /// 注意:
        ///     1.为兼容以前版本,当判断 (formulaCode==0xffffffff && gameIdx==0xffffffff)时,则忽略formulaCode,gameIdx两个参数
        /// </remarks>
        /// <returns></returns>
        public static uint GenerateComfirmCode(int gainTotal, int gainCurrent, int lineIdx, uint tableIdx, uint numPrint, uint formulaCode, uint gameIdx)
        {
            int[] code;
            if (formulaCode == 0xffffffff && gameIdx == 0xffffffff)//为兼容以前版本
            {
                code = new int[] { gainTotal, gainCurrent, lineIdx, (int)tableIdx, (int)numPrint };
            }
            else
            {
                code = new int[] { gainTotal, gainCurrent, lineIdx, (int)tableIdx, (int)numPrint, (int)formulaCode, (int)gameIdx };
            }


            List<byte> dataCollect = new List<byte>();

            foreach (int c in code)
            {
                byte[] byteBuf = BitConverter.GetBytes(c);//将int值转为byte数组(1 int = 4 byte)
                foreach (byte b in byteBuf)
                    dataCollect.Add(b);
            }

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] md5data = md5.ComputeHash(dataCollect.ToArray());//计算data字节数组的哈希值 
            md5.Clear();

            for (int i = 0; i != 4; ++i)
                md5data[i + 12] = (byte)(md5data[i] ^ md5data[i + 4] ^ md5data[i + 8] ^ md5data[i + 12]);

            uint outCode = BitConverter.ToUInt32(md5data, 12);//取得最后12位 
            outCode %= 10000;
            return outCode;
        }

    }
}