using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace WaterMoney
{
    public static class Card
    {
        #region - 用户结构体 -
        /// <summary>
        /// 用户结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]   
        public struct RYUserInfo
        {
           public uint nCardNo;//卡号
           public Int32 nBalance;//余额
           public Int32 nDaySum;//日累计
           public Int32 nSum;//总累计
           public uint tLast;
           public byte nU2;
           public uint nPass;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
           public char[] aPersonID;//工号
           public byte nIDType;//身份
           public Int32 nU3;
           public Int32 nU4;
           public uint tStart;
           public uint tLast2;
           public uint tOpen;
           public Int32 nU5;
           public Int32 nU6;
           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
           public char[] aU1;
           public byte nNation;//民族
           public Int32 nDept;
           [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)] 
           public char[] aName;//姓名unicode
           public byte nState;//状态
           public byte nU7;
           public byte nU8;
           public byte nU9;
        }
        #endregion

        #region - PFileRYUserInfo -
        /// <summary>
        /// PFileRYUserInfo
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FileRYUserInfo
        {
            public uint nID;
            [MarshalAs(UnmanagedType.Struct)]
            public RYUserInfo ui;
        }
        #endregion

        #region - RYAdminInfo管理员结构体 -
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RYAdminInfo
        {
            public uint nU1;
            public uint tTime;//操作时间
            public Int32 nU2;
            public byte nFlag1;//=0
            public byte nFlag2;//=byte_735278
            public byte nFlag3;//=0D 修改信息   04   修改余额   01  消费
            public byte nFlag4;//=byte_735279
            public byte nFlag5;//=-1
            public byte nStation;//站号
            public byte nMess;//食堂
            public byte nTerm;//分组
            public byte nMeal;//饭
            public byte nTerminal;//机器

        }
        #endregion

        #region PPRYLogInfo
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]   
        public struct RYLogInfo
        {
            public uint nNext;
            public uint nPre;
            public uint nLogID;
            public uint nTime;
            public ushort nU1;
            public byte nFlag1;
            public byte nFlag2;
            public byte nOpFlag;
            public byte nAdminID;
            public byte nFlag5;
            public byte nStation;
            public ushort nU2;
            public byte nMeal;
            public byte nTerminal;
            public int n2;
            public int n3;
            public int nChange;
            public int nAmount;
            public int nBalance;
            public ushort nID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public char[] aU2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public char[] aName;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)] 
            public char[] aDept;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] 
            public char[] aIDType;
            public uint nU3;
        }
        #endregion

        #region RY
        /// <summary>
        /// 初始化函数, 只须调用一次
        /// </summary>
        /// <param name="nType">程序类型 0=如意, 1=金龙, 2=单机</param>
        /// <returns>服务器软件进程句柄，后面的各个函数需要用到此句柄</returns>
        [DllImport("RYControl.dll")]
        public static extern IntPtr InitRYProc(int nType);//初始化, CallingConvention = CallingConvention.Cdecl

        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="hProcess">服务器软件进程句柄</param>
        /// <param name="nCardNo">卡号</param>
        /// <param name="nType">判断帐户正常状态</param>
        /// <returns></returns>
        [DllImport("RYControl.dll")]
        public static extern int GetBalanceByCardNo(IntPtr hProcess, uint nCardNo, int nType);//获取用户余额
        

        /// <summary>
        /// 修改余额
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="nCardNO"></param>
        /// <param name="nMoney"></param>
        /// <returns></returns>
        [DllImport("RYControl.dll")]
        public static extern int ChangeBalanceByCardNo(IntPtr hProcess, uint nCardNO, int nMoney);

        /// <summary>
        /// 获取全部流水
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="nDate"></param>
        /// <param name="nDay"></param>
        /// <param name="PPRYLogInfo"></param>
        /// <returns></returns>
        [DllImport("RYControl.dll")]
        //public static extern uint GetAllLog(IntPtr hProcess, uint nDate, uint nDay, ref RYLogInfo ppData);
        public static extern uint GetAllLog(IntPtr hProcess, uint nDate, uint nDay, ref uint ppdata);//m1

        //public static extern uint GetAllLog(IntPtr hProcess, uint nDate, uint nDay, byte[] ppdata);//m2

        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <param name="hProcess"></param>
        /// <param name="pFileUserInfo"></param>
        /// <returns></returns>
        [DllImport("RYControl.dll")]
        //public static extern int GetAllUser(IntPtr hProcess, ref FileRYUserInfo pFileUserInfo);
        public static extern int GetAllUser(IntPtr hProcess, IntPtr fiIntPtr);

        /// <summary>
        /// 获取用户数量
        /// </summary>
        /// <param name="hProcess">hProcess 服务器软件进程句柄</param>
        /// <returns>用户数量</returns>
        [DllImport("RYControl.dll")]
        public static extern int GetUserCount(IntPtr hProcess);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="hProcess">hProcess 服务器软件进程句柄</param>
        /// <param name="nCardNo">卡号</param>
        /// <param name="?"></param>
        /// <returns></returns>
        [DllImport("RYControl.dll")]
        public static extern int GetUserInfoByCardNo(IntPtr hProcess, uint nCardNo, ref RYUserInfo ryUserInfo);

        /// <summary>
        /// 刷卡
        /// </summary>
        /// <param name="hProcess">服务器软件进程句柄</param>
        /// <param name="nCardNo">卡号</param>
        /// <param name="nMess">食堂</param>
        /// <param name="nTerm">小组</param>
        /// <param name="nMeal">餐次</param>
        /// <param name="nTerminal">售饭机号</param>
        /// <param name="nMoney">消费金额</param>
        /// <returns>1 成功,小于1 失败</returns>
        [DllImport("RYControl")]
        public static extern int SwipeCardByCardNo(IntPtr hProcess,uint nCardNo,int nMess,int nTerm,int nMeal,int nTerminal,int nMoney,int nType,int nDay);
        
        #endregion
    }
}
