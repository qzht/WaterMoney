using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace WaterMoney
{
    public class Common
    {
        #region - 10转16进制（0x） -
        private uint sixTotendesc(string ten)
        {
            try
            {
                //uint retu = 0 ;
                string shiliu = Convert.ToInt32(ten).ToString("X");
                //string xs = "0x";
                string xs = "";
                string result = Regex.Replace(shiliu, @".{2}", "$0 ");//一串字符串每隔两个用空格分开
                string[] nres = result.Split(' ');
                for (int i = nres.Length - 1; i >= 0; i--)
                {
                    xs += nres[i].ToString();
                }

                xs = xs.PadRight(8, '0');
                UInt32 retu = Convert.ToUInt32(xs, 16);
                return retu;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        #endregion
    }
}
