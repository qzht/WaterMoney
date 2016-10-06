using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using XK.NBear.DB;
using System.Threading;

namespace WaterMoney
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private IDO db = DatabaseFactory.CreateOperation("SYSTEM");

        public delegate void ShowMsg(string msg);
        public void ShowMsgFun(string msg)
        {
            lblmsg.Text = msg+"-时间间隔:"+nudmiao.Value;
        }
        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            //0=如意, 1=金龙, 2=单机
            cbbType.Items.Add(new ListItem("0", "如意"));
            cbbType.Items.Add(new ListItem("1", "金龙"));
            cbbType.Items.Add(new ListItem("2", "单机"));
            cbbType.SelectedIndex = 1;
            string path = AppDomain.CurrentDomain.BaseDirectory + "versin.bin";
            if (File.Exists(path))
            {
                // bindData();
            }
            else
            {
                SetDatabase setdb = new SetDatabase();
                setdb.ShowDialog();
            }

            IniClass ini = new IniClass(AppDomain.CurrentDomain.BaseDirectory + "Config.inf");
            string timer = ini.IniReadValue("设置", "时间间隔");
            nudmiao.Value =decimal.Parse(timer);
            if (th != null)
            {
                th.Abort();
            }
            th = new Thread(new ThreadStart(CZ));
            th.IsBackground = true;
            th.Start();
        }

        Thread th = null;
        /// <summary>
        /// 开启
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (th != null)
            {
                th.Abort();
            }
            th = new Thread(new ThreadStart(CZ));
            th.IsBackground = true;
            th.Start();
        }

        public void CZ()
        {
            while (true)
            {
                this.Invoke(new ShowMsg(ShowMsgFun), new object[] { "开启线程" });
                int type = 0;
                type = int.Parse(((ListItem)(cbbType.SelectedItem)).Key);

                p = Card.InitRYProc(type);
                if (p != IntPtr.Zero)
                {
                    this.Invoke(new ShowMsg(ShowMsgFun), new object[] { "初始化成功" });
                }
                else
                {
                    this.Invoke(new ShowMsg(ShowMsgFun), new object[] { "初始化失败" });
                    Thread.Sleep(int.Parse(nudmiao.Value.ToString("#0")));
                    continue;
                }

                DataTable Temp = db.ExecuteDataTable(" select * from A_incases");
                if (Temp != null)
                {
                    this.Invoke(new ShowMsg(ShowMsgFun), new object[] { "获取待处理数据条数：" + Temp.Rows.Count });
                }
                int i = 0;
                foreach (DataRow dr in Temp.Select())
                {
                    string id = dr["id"].ToString();
                    string CarNum = dr["cardid"].ToString();
                    string transindex = dr["transindex"].ToString();
                    decimal money = decimal.Parse(dr["cmoney"].ToString()) * 10000;
                    if (CarNum.Length != 8)
                    {
                        MessageBox.Show("卡号异常");
                        continue;
                    }
                    uint CarNum2 = Convert.ToUInt32(CarNum, 16);
                    int result = Card.ChangeBalanceByCardNo(p, CarNum2, int.Parse(money.ToString("#0")));
                    string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (result > 0)
                    {
                        //string sql = " insert into A_inlog ([caseid],[cardid],[billid],[uname],[curdate],[device],[devicetime],[cmoney],[charge],[bankcaseid],[transtype],[transresult],[bankmsg],[bankid],[transindex],[banktime] ,[state],[tag],[memo]) "
                        //           + "  select [caseid],[cardid],[billid],[uname],[curdate],[device],[devicetime],[cmoney],[charge],[bankcaseid],[transtype],'1','充值成功',[bankid],[transindex],'" + time + "',[state],[tag],[memo]"
                        //           + "  from A_incases where id='" + id + "'";
                        string sql = " update A_inlog set [transresult]=1,[bankmsg]='充值成功', banktime='"+time+"' where transindex='" + transindex + "'  ";
                        db.NonQuery(sql);
                        db.NonQuery("delete from A_incases where id='" + id + "'");
                    }
                    else
                    {
                        //string sql = " insert into A_inlog ([caseid],[cardid],[billid],[uname],[curdate],[device],[devicetime],[cmoney],[charge],[bankcaseid],[transtype],[transresult],[bankmsg],[bankid],[transindex],[banktime] ,[state],[tag],[memo])  "
                        //  + "  select [caseid],[cardid],[billid],[uname],[curdate],[device],[devicetime],[cmoney],[charge],[bankcaseid],[transtype],'-1','充值失败',[bankid],[transindex],'" + time + "',[state],[tag],[memo]"
                        //  + "  from A_incases where id='" + id + "'";
                        string sql = " update A_inlog set [transresult]=-1,[bankmsg]='充值失败', banktime='" + time + "' where transindex='" + transindex + "'  ";
                        db.NonQuery(sql);
                        db.NonQuery("delete from A_incases where id='" + id + "'");
                    }
                    i++;
                    this.Invoke(new ShowMsg(ShowMsgFun), new object[] { "处理数据情况：" + Temp.Rows.Count + "/" + i });
                }
                Thread.Sleep(int.Parse(nudmiao.Value.ToString("#0")));
            }
        }


        IntPtr p = new IntPtr(0);

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("是否退出系统", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != res)
            {
                e.Cancel = true;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (th != null)
            {
                th.Abort();
            }
        }

        private void nudmiao_ValueChanged(object sender, EventArgs e)
        {
            nudmiao.Value = (int)nudmiao.Value;
            IniClass ini = new IniClass(AppDomain.CurrentDomain.BaseDirectory + "Config.inf");
             ini.IniWriteValue("设置", "时间间隔",nudmiao.Value.ToString());
        }
    }
}
