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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gta
{
    public partial class Form1 : Form
    {
        static String cookie = "NNB=VI5HOPPX2QKFY; npic=pq1XV21sNI7Hams6QxFFUtRx5H13K2cIx6XvWMOf7OftWXNTYtbv2oVn6hyQ1FopCA==; nid_inf=350662032; NID_AUT=Cv3dgFg8cAg2JX0Y1lALqDNCDTtbcHwE/oj0qjFMwpV8/+Q2EEEgc2rQdmshgFvE; NID_JKL=LE6HVYU+uMbqY0fNDYVpGGQqeNz+T2PV98Uho7BNAqY=; NID_SES=AAABhJaOjp7qikprUZK3PjZVea6RQdYcEv/loGANv1d6vIlVsMgoKNwNuBY1px4nCrkIOD93zFB0D36D8Jikv0rf6emrK4WoE6KCrssClTnibfiblVnxqpxDQ7eRbVuxQDOXseTs1Nym84Of+FPXYYRdtwgV8CwjCavh+RuisOj7ZwWcDXi/oz0IJbAM/NOYQfd+iuSOnmzIsTuLDikpFBoayhcldsOdyQR+OpEApUlTdinFC3Upk/av7AyCsMDOtB09Ck0VPrPQE56dWVbYcpjYfyw7npvlNeUo3Wh9lIIAkQ1Apvii8zhqzpGj65e+g7j9E7qiQLBEFLB0dDo40WPUrnpNW1ntKD4X75nUFootXci+9gp3S2X0WBFcBHxp5uUUXLJ7zDO1qrszDT9xsAw8ebhICeintnKZk6YNAf4tZmHj22OiV6jbcCT4Es/2I65EGpHLLenhg0wdeLucJzImEzvzgDdRKb4IAzL3CL12EASB5WhM7GCIUuNYWPeaJaaGjGgoRfApfGTnA4W7bkVYjb4=; ncu=8cab5b7069244afa89680838749cc09f0f069f53211ab5; nci4=c8f81b30206805b0ed0344535eb2d3b51647a0450a4fa578835d538b1411c2c402e22efa407af9b6bb5501db1bbd53b57a401ee36e443e76d519c20ca5ecb194868480fcf5f8e98bff8c88737b5e66535f52524a4a65684f5667555c50775c6a255746614475554d40674f7d3545481034054d303f1a3d08472827022512232e21082f1c5156576f696b6f6e101c3b1a2b6e1778737313637e1f6c7373; ncmc4=8cbc5f74642c41f4a94700171ae38be5471fe605525ebe629f2a19e97665b671b0768a7d9ebb242e7ed2ce61d755f23ae7fdd8ef232a62526e8d0de16c5b4d; personaconmain|asdflkjh0101=EAE11B32508CF3CE8BF477F4AEAB68A851A2577398D1B4F267C9095D7D1CBA26; personacon|asdflkjh0101=4C677C8778A588342D3DB972012E464709B8549A2FDF0C1C3C22AC32B23A9C2CCB1E306FC7959A3E; ncvc2=e0c7371c05482696e5046454748ff08d2f7e8262307e8242a0635c89331ff5348a; JSESSIONID=EB2D953A3AECC80674B3A49BC8D6A8CE";

        public string[] split(string a, string b)
        {
            return a.Split(new string[] { b }, StringSplitOptions.None);
        }

        public string between(string a,string b, string c)
        {
            string contents = split(a, b)[1];
            contents = split(contents, c)[0];
            return contents;
        }

        public static string request(string url)
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
            StreamReader srReadData = new StreamReader(stReadData, Encoding.GetEncoding("utf-8"));

            string strResult = srReadData.ReadToEnd();
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

        public string[] check_grade(string name)
        {
            try
            {
                int win_cnt = 0;
                string grade = "";
                System.Windows.Forms.Application.DoEvents();
                string strResult1 = request("https://m.cafe.naver.com/ArticleSearchList.nhn?search.query=" + name + "&search.menuid=&search.searchBy=1&search.sortBy=date&search.clubid=14021316&search.option=0&search.defaultValue=");
                if (!strResult1.Contains("<ul class=\"list_tit\">"))
                {
                    return new string[2] { "0", "" };
                }
                string page = between(strResult1, "<ul class=\"list_tit\">", "</ul>");

                if (!page.Contains("<h3>"))
                {
                    return new string[2] { "0", "" };
                }
                
                string[] detail = split(page, "<h3>");
                for (int cnt = 1; cnt < detail.Length; cnt++)
                {
                    string detail2 = split(detail[cnt], "</h3>")[0];//cnt를 바꾸면 다음 댓글이 나옴
                    detail2 = HtmlStrip(detail2);

                    //richTextBox1.Text = richTextBox1.Text + detail2 + "\n";

                    //조를 결정할때 2가지 경우의 수가 있음
                    //1.우승 시 조 입니다
                    if (detail2.Contains("조 입니다"))
                    {
                        int location = detail2.IndexOf("조 입니다");
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
                    }

                    if (detail2.Contains("우승"))
                    {
                        win_cnt++;
                    }
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
            Comparer _comparer = new Comparer(System.Globalization.CultureInfo.CurrentCulture);

            public int Compare(object x, object y)
            {
                // Convert string comparisons to int
 
                return _comparer.Compare(Convert.ToInt32(Regex.Replace(x.ToString(), @"\D", "")), Convert.ToInt32(Regex.Replace(y.ToString(), @"\D", "")));
            }
        }

        public class Comparer_level : IComparer
        {
            Comparer _comparer = new Comparer(System.Globalization.CultureInfo.CurrentCulture);

            public int Compare(object x, object y)
            {
                // Convert string comparisons to int

                return _comparer.Compare((split(x.ToString(), "-")[1]), (split(y.ToString(), "-")[1]));
            }

            public string[] split(string a, string b)
            {
                return a.Split(new string[] { b }, StringSplitOptions.None);
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            ArrayList playin = new ArrayList();//참가하는 모든 사람

            string strResult = request("https://m.cafe.naver.com/ArticleRead.nhn?clubid=14021316&articleid=" + split(url.Text, "/")[4] + "&page=1");
            string strResult1 = request("http://m.cafe.naver.com/CommentView.nhn?search.clubid=14021316&search.page=" + "1" + "&search.articleid=" + split(url.Text, "/")[4]);
            string page = between(strResult1, "<ul class=\"u_cbox_list\">", "</ul>");
            if(Convert.ToInt32(between(strResult, "<em class=\"u_cnt\">", "</em>")) > 100)
            {
                string strResult2 = request("http://m.cafe.naver.com/CommentView.nhn?search.clubid=14021316&search.page=" + "2" + "&search.articleid=" + split(url.Text, "/")[4]);
                page = page + between(strResult2, "<ul class=\"u_cbox_list\">", "</ul>");
            }
            page = page.Replace(" ", "");

            string[] detail = split(page, "<divclass=\"u_cbox_comment_box\">");
            for (int cnt = 1; cnt < detail.Length; cnt++)
            {
                try
                {
                    string detail2 = detail[cnt];//<-숫자를 바꾸면 다음 댓글이 나옴
                                                 //MessageBox.Show(detail2);
                    string contents = between(detail2, "<spanclass=\"u_cbox_contents\">", "</span>");//댓글 내용
                    contents = contents.Replace("\r", "");

                    //MessageBox.Show(contents);

                    string writer = between(detail2, "<spanclass=\"u_cbox_info_main\">", "</span>");//댓글 작성자
                    writer = HtmlStrip(writer);
                    writer = writer.Replace("\r\n", "").Replace("작성자", "").Replace("New", "");
                    //MessageBox.Show(contents);

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
                        if(is_in == 0)
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
                            playin.RemoveAt(playin.Count-1);
                        }
                    }
                }
                catch
                {

                }
                
            }
            richTextBox1.Text = richTextBox1.Text + "------------------------\n";

            for (int i = 0; i < playin.Count; i++)
            {
                String[] result = check_grade(split(playin[i].ToString(), "-")[0]);
                playin[i] = playin[i] + "-" + result[0];

                if (result[1].Contains("-"))
                {
                    richTextBox1.Text = richTextBox1.Text + "확인 - " + playin[i].ToString() + " --> " + result[1] + "에서 Z로\n";
                    result[1] = "Z";
                }

                if (result[1].Contains("+"))
                {
                    richTextBox1.Text = richTextBox1.Text + "확인 - " + playin[i].ToString() + " --> " + result[1] + "에서 A로\n";
                    result[1] = "A";
                }

                if (result[1].Equals(""))
                {
                    richTextBox1.Text = richTextBox1.Text + "확인 - " + playin[i].ToString() + " --> " + result[1] + "검색불가\n";
                }
                if (!result[1].Contains(split(playin[i].ToString(), "-")[1]) && !result[1].Equals(""))
                {
                    richTextBox1.Text = richTextBox1.Text + "변경 - " + playin[i].ToString() + " --> " + result[1] + "조로\n";
                    playin[i] = playin[i].ToString().Replace(split(playin[i].ToString(), "-")[1], result[1]);
                }
            }

            for (int i = 0; i < playin.Count; i++)
            {
                richTextBox2.Text = richTextBox2.Text + playin[i] + "\n";
                richTextBox2.Text = richTextBox2.Text.Replace("\n\n", "");
            }

            MessageBox.Show("완료");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String csv = "";
            ArrayList playin = new ArrayList();//참가하는 모든 사람
            ArrayList play_group = new ArrayList();//임시 조

            string[] rich_get = split(richTextBox2.Text,"\n");
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
                    csv = csv + "\n";
                    play_group = new ArrayList();
                }
            }

            //richTextBox1.Text = richTextBox1.Text + csv;

            System.IO.File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\아카데미조_" + System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".csv", csv, Encoding.Default);
            MessageBox.Show("완료");
        }
    }
}

