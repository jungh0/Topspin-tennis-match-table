using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTA.Main
{
    class StaticFunc
    {
        public static readonly String cookie = StaticFunc.Request("https://raw.githubusercontent.com/jungh0/Topspin-tennis-match-table/master/cook.txt", "utf-8");
        public static String deCookie = "";

        public static string Request(string url, string encode)
        {
            HttpWebRequest request5 = (HttpWebRequest)WebRequest.Create(url);
            request5.Method = "Get";
            request5.Referer = "http://cafe.naver.com/topspintennis";
            request5.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            request5.ContentType = "text/plain";
            request5.Headers[HttpRequestHeader.Cookie] = deCookie;
            request5.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request5.Headers.Add("X-Requested-With", "XMLHttpRequest");

            HttpWebResponse response = (HttpWebResponse)request5.GetResponse();
            Stream stReadData = response.GetResponseStream();
            StreamReader srReadData = new StreamReader(stReadData, Encoding.GetEncoding(encode));

            string strResult = srReadData.ReadToEnd();
            srReadData.Dispose();

            response.Close();
            //MessageBox.Show(strResult);
            return strResult;
        }

        public static string[] Split(string a, string b)
        {
            return a.Split(new string[] { b }, StringSplitOptions.None);
        }

        public static string HtmlStrip(string input)
        {
            input = Regex.Replace(input, "<style>(.|\n)*?</style>", string.Empty);
            input = Regex.Replace(input, @"<xml>(.|\n)*?</xml>", string.Empty); // remove all <xml></xml> tags and anything inbetween.  
            return Regex.Replace(input, @"<(.|\n)*?>", string.Empty); // remove any tags but not there content "<p>bob<span> johnson</span></p>" becomes "bob johnson"
        }

        public static string Euckr(string str)
        {
            Encoding euckr = Encoding.GetEncoding(51949);
            byte[] tmp = euckr.GetBytes(str);

            string res = "";

            foreach (byte b in tmp)
            {
                res += "%";
                res += string.Format("{0:X}", b);
            }

            return res;
        }

    }
}
