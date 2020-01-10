using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta
{
    public partial class Form1 : Form
    {
        static readonly String cookie = Request("https://raw.githubusercontent.com/jungh0/Topspin-tennis-match-table/master/cook.txt", "utf-8");

        public string[] Split(string a, string b)
        {
            return a.Split(new string[] { b }, StringSplitOptions.None);
        }

        public string Between(string a, string b, string c)
        {
            string contents = Split(a, b)[1];
            contents = Split(contents, c)[0];
            return contents;
        }

        public static string Request(string url, string encode)
        {
            HttpWebRequest request5 = (HttpWebRequest)WebRequest.Create(url);
            request5.Method = "Get";
            request5.Referer = "http://cafe.naver.com/topspintennis";
            request5.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";

            request5.ContentType = "text/plain";
            request5.Headers[HttpRequestHeader.Cookie] = cookie;
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

        public string[] Check_grade(string name)
        {
            try
            {
                int win_cnt = 0;
                string grade = "";
                System.Windows.Forms.Application.DoEvents();

                //string strResult1 = request("https://m.cafe.naver.com/ArticleSearchList.nhn?search.query=" + name + "&search.menuid=&search.searchBy=1&search.sortBy=date&search.clubid=14021316&search.option=0&search.defaultValue=");
                string strResult1 = Request("https://cafe.naver.com/ArticleSearchList.nhn?search.clubid=14021316&search.searchBy=1&search.query=" + Euckr(name), "euc-kr");
                //string strResult1 = request("https://cafe.naver.com/topspintennis?iframe_url=/ArticleSearchList.nhn%3Fsearch.clubid=14021316%26search.searchdate=all%26search.searchBy=0%26search.query=" + name + "%26search.defaultValue=1%26search.includeAll=%26search.exclude=%26search.include=%26search.exact=%26search.sortBy=date%26userDisplay=15%26search.media=0%26search.option=0");

                if (!strResult1.Contains("<span class=\"head\">"))
                {
                    return new string[2] { "0", "" };
                }

                /*
                string page = between(strResult1, "<ul class=\"list_tit\">", "</ul>");
                if (!page.Contains("<h3>"))
                {
                    return new string[2] { "0", "" };
                }*/

                string[] detail = Split(strResult1, "<span class=\"head\">");

                for (int cnt = 1; cnt < detail.Length; cnt++)
                {

                    string detail2 = Split(detail[cnt], "</span>")[0];//cnt를 바꾸면 다음 댓글이 나옴

                    detail2 = HtmlStrip(detail2);
                    detail2.Replace("\t", "").Replace(" ", "").Replace(" ", "");
                    detail2 = Split(Split(detail2, "[")[1], "]")[0];
                    //MessageBox.Show(detail2);
                    //richTextBox1.Text = richTextBox1.Text + detail2 + "\n";
                    if (detail2.Contains("마일리지우승"))
                    {
                        win_cnt++;
                    }
                    else
                    {
                        bool isAlphaBet = Regex.IsMatch(detail2[0].ToString(), "[a-z]", RegexOptions.IgnoreCase);
                        if (isAlphaBet)
                        {
                            grade = detail2[0].ToString().ToUpper();
                            //richTextBox1.Text = richTextBox1.Text + win_cnt.ToString() + "\n";
                            //richTextBox1.Text = richTextBox1.Text + grade + "\n";
                            return new string[2] { win_cnt.ToString(), grade };//여기가 정상종료
                        }
                    }

                    /*
                    //조를 결정할때 2가지 경우의 수가 있음
                    //1.우승 시 조 입니다
                    if (detail2.Contains("조 입니다") && !detail2.Contains("마일리지우승"))
                    {
                        int location = detail2.IndexOf("조 입니다");
                        grade = detail2[location - 1].ToString().ToUpper();
                        //richTextBox1.Text = richTextBox1.Text + win_cnt.ToString() + "\n";
                        //richTextBox1.Text = richTextBox1.Text + grade + "\n";
                        return new string[2] { win_cnt.ToString(), grade };//여기가 정상종료
                    }
                    if (detail2.Contains("조입니다") && !detail2.Contains("마일리지우승"))
                    {
                        int location = detail2.IndexOf("조입니다");
                        grade = detail2[location - 1].ToString().ToUpper();
                        //richTextBox1.Text = richTextBox1.Text + win_cnt.ToString() + "\n";
                        //richTextBox1.Text = richTextBox1.Text + grade + "\n";
                        return new string[2] { win_cnt.ToString(), grade };//여기가 정상종료
                    }
                    //2. 피드백 시 에서 조
                    if (detail2.Contains("에서"))
                    {
                        int location = detail2.IndexOf("에서");
                        grade = detail2[location + 3].ToString().ToUpper();
                        //richTextBox1.Text = richTextBox1.Text + win_cnt.ToString() + "\n";
                        //richTextBox1.Text = richTextBox1.Text + grade + "\n";
                        return new string[2] { win_cnt.ToString(), grade };//여기가 정상종료
                    }*/

                }
                if (win_cnt > 0)
                {
                    return new string[2] { win_cnt.ToString(), "" };
                }
                return new string[2] { "0", "" };
            }
            catch
            {
                return new string[2] { "0", "" };
            }

        }

        public Form1()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            toolStripStatusLabel1.Text = "ver " + version;
        }

        public class Comparer_age : IComparer
        {
            readonly Comparer _comparer = new Comparer(System.Globalization.CultureInfo.CurrentCulture);

            public int Compare(object x, object y)
            {
                return _comparer.Compare(Convert.ToInt32(Regex.Replace(x.ToString(), @"\D", "")), Convert.ToInt32(Regex.Replace(y.ToString(), @"\D", "")));
            }
        }

        public class Comparer_level : IComparer
        {
            readonly Comparer _comparer = new Comparer(System.Globalization.CultureInfo.CurrentCulture);

            public int Compare(object x, object y)
            {
                return _comparer.Compare((Split(x.ToString(), "-")[1]), (Split(y.ToString(), "-")[1]));
            }

            public string[] Split(string a, string b)
            {
                return a.Split(new string[] { b }, StringSplitOptions.None);
            }
        }

        private void AddData(ref ArrayList playin, string writer, string contents)
        {
            char cate = contents.ToUpper().ToCharArray()[0];
            if (cate <= 'Z' && cate >= 'A')//댓글 내용중에 앞에 첫글자가 알파벳으로 시작
            {
                int is_in = 0;
                string name_cate = writer + "-" + cate.ToString();

                for (int i = 0; i < playin.Count; i++)
                {
                    if (playin[i].ToString().Contains(writer))
                    {
                        is_in = 1;
                        richTextBox1.Text = richTextBox1.Text + "수정 - " + contents + "-" + writer + "\n";
                        playin.RemoveAt(i);
                        playin.Add(name_cate);
                        break;
                    }
                }
                if (is_in == 0)
                {
                    if (!writer.Contains("박코치"))
                    {
                        richTextBox1.Text = richTextBox1.Text + "추가 - " + contents + "-" + writer + "\n";
                        playin.Add(name_cate);
                    }
                }

            }
            if (contents.Contains("취소") || contents.Contains("불참"))
            {
                for (int i = 0; i < playin.Count; i++)
                {
                    if (playin[i].ToString().Contains(writer))
                    {
                        richTextBox1.Text = richTextBox1.Text + "제거 - " + contents + "-" + writer + "\n";
                        playin.RemoveAt(i);
                    }
                }
            }
            if (contents.Contains("불가"))
            {
                if (writer.Contains("박코치"))
                {
                    richTextBox1.Text = richTextBox1.Text + "제거 - " + contents + "-" + writer + "\n";
                    playin.RemoveAt(playin.Count - 1);
                }
            }
        }

        private void Recursive(ref ArrayList playin, ref string lastId, ref string lastRefId)
        {
            string strResult = Request($"https://apis.naver.com/cafe-web/cafe-articleapi/cafes/14021316/articles/{ Split(url.Text, "/")[4] }/comments/more/next?requestFrom=B&orderBy=asc&fromPopular=false&commentId={lastId}&refCommentId={lastRefId}", "utf-8");

            JObject jo = JObject.Parse(strResult);
            var items = jo.SelectToken("comments").SelectToken("items");
            if(items.Count() > 1)
            {
                foreach (var item in items)
                {
                    var content = item.SelectToken("content").ToString();
                    var id = item.SelectToken("id").ToString();
                    var refId = item.SelectToken("refId").ToString();
                    var nick = item.SelectToken("writer").SelectToken("nick").ToString();
                    lastId = id;
                    lastRefId = refId;

                    AddData(ref playin, nick, content);
                }
                Recursive(ref playin, ref lastId, ref lastRefId);
            }
        }

        private void AddRichBox(string str)
        {
            Invoke(new Action(() =>
            {
                richTextBox1.Text += str;
            }));
        }

        private void Start_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                string lastId = "";
                string lastRefId = "";
                ArrayList playin = new ArrayList();//참가하는 모든 사람

                Recursive(ref playin, ref lastId, ref lastRefId);

                AddRichBox("------------------------\n");
                toolStripProgressBar1.Maximum = playin.Count;
                for (int i = 0; i < playin.Count; i++)
                {
                    toolStripProgressBar1.Value = i + 1;
                    String[] result = Check_grade(Split(playin[i].ToString(), "-")[0]);
                    playin[i] = playin[i] + "-" + result[0];

                    if (result[1].Contains("-"))
                    {
                        AddRichBox("확인 - " + playin[i].ToString() + " --> " + result[1] + "에서 Z로\n");
                        result[1] = "Z";
                    }

                    if (result[1].Contains("+"))
                    {
                        AddRichBox("확인 - " + playin[i].ToString() + " --> " + result[1] + "에서 A로\n");
                        result[1] = "A";
                    }

                    if (result[1].Equals(""))
                    {
                        AddRichBox("확인 - " + playin[i].ToString() + " --> " + result[1] + "검색불가\n");
                    }
                    if (!result[1].Contains(Split(playin[i].ToString(), "-")[1]) && !result[1].Equals(""))
                    {
                        AddRichBox("변경 - " + playin[i].ToString() + " --> " + result[1] + "조로\n");
                        playin[i] = playin[i].ToString().Replace(Split(playin[i].ToString(), "-")[1], result[1]);
                    }
                }

                for (int i = 0; i < playin.Count; i++)
                {
                    richTextBox2.Text = richTextBox2.Text + playin[i] + "\n";
                    richTextBox2.Text = richTextBox2.Text.Replace("\n\n", "");
                }

                MessageBox.Show("완료");

            }).Start();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            String csv = "";
            ArrayList playin = new ArrayList();//참가하는 모든 사람
            ArrayList play_group = new ArrayList();//임시 조

            string[] rich_get = Split(richTextBox2.Text, "\n");
            for (int i = 0; i < rich_get.Length; i++)
            {
                if (!rich_get[i].Equals(""))
                {
                    playin.Add(rich_get[i]);
                }
            }

            playin.Sort(new Comparer_level());
            int gourp_person = Convert.ToInt32(cnt.Text);
            for (int i = 0; i < playin.Count; i++)
            {
                play_group.Add(playin[i]);
                if (((i + 1) % gourp_person == 0 || i == playin.Count - 1) && i != 0)//조당 n명이 있거나 마지막 바퀴일때 대신 처음한번은 무시한다
                {
                    play_group.Sort(new Comparer_age());
                    for (int ii = 0; ii < play_group.Count; ii++)
                    {
                        csv = csv + play_group[ii] + ",";
                    }
                    csv += "\n";
                    play_group = new ArrayList();
                }
            }

            //richTextBox1.Text = richTextBox1.Text + csv;

            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\아카데미조_" + System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".csv", csv, Encoding.Default);
            MessageBox.Show("완료");
        }

    }
}

