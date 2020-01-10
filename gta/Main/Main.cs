using GTA.Main;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace gta
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            toolStripStatusLabel1.Text = "ver " + version;
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (pass.Text == "")
            {
                return;
            }

            StaticFunc.deCookie = AES.Builder().SetKey(pass.Text).AESDecrypt128(StaticFunc.cookie);
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
                    String[] result = Check_grade(StaticFunc.Split(playin[i].ToString(), "-")[0]);
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
                    if (!result[1].Contains(StaticFunc.Split(playin[i].ToString(), "-")[1]) && !result[1].Equals(""))
                    {
                        AddRichBox("변경 - " + playin[i].ToString() + " --> " + result[1] + "조로\n");
                        playin[i] = playin[i].ToString().Replace(StaticFunc.Split(playin[i].ToString(), "-")[1], result[1]);
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

            string[] rich_get = StaticFunc.Split(richTextBox2.Text, "\n");
            for (int i = 0; i < rich_get.Length; i++)
            {
                if (!rich_get[i].Equals(""))
                {
                    playin.Add(rich_get[i]);
                }
            }

            playin.Sort(new Compare.Comparer_level());
            int gourp_person = Convert.ToInt32(cnt.Text);
            for (int i = 0; i < playin.Count; i++)
            {
                play_group.Add(playin[i]);
                if (((i + 1) % gourp_person == 0 || i == playin.Count - 1) && i != 0)//조당 n명이 있거나 마지막 바퀴일때 대신 처음한번은 무시한다
                {
                    play_group.Sort(new Compare.Comparer_age());
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

        private void AddRichBox(string str)
        {
            Invoke(new Action(() =>
            {
                richTextBox1.Text += str;
            }));
        }

        public string[] Check_grade(string name)
        {
            try
            {
                int win_cnt = 0;
                string grade = "";
                Application.DoEvents();

                string strResult1 = StaticFunc.Request("https://cafe.naver.com/ArticleSearchList.nhn?search.clubid=14021316&search.searchBy=1&search.query=" + StaticFunc.Euckr(name), "euc-kr");

                if (!strResult1.Contains("<span class=\"head\">"))
                {
                    return new string[2] { "0", "" };
                }

                string[] detail = StaticFunc.Split(strResult1, "<span class=\"head\">");

                for (int cnt = 1; cnt < detail.Length; cnt++)
                {

                    string detail2 = StaticFunc.Split(detail[cnt], "</span>")[0];//cnt를 바꾸면 다음 댓글이 나옴

                    detail2 = StaticFunc.HtmlStrip(detail2);
                    detail2.Replace("\t", "").Replace(" ", "").Replace(" ", "");
                    detail2 = StaticFunc.Split(StaticFunc.Split(detail2, "[")[1], "]")[0];
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
            string strResult = StaticFunc.Request($"https://apis.naver.com/cafe-web/cafe-articleapi/cafes/14021316/articles/{ StaticFunc.Split(url.Text, "/")[4] }/comments/more/next?requestFrom=B&orderBy=asc&fromPopular=false&commentId={lastId}&refCommentId={lastRefId}", "utf-8");

            JObject jo = JObject.Parse(strResult);
            var items = jo.SelectToken("comments").SelectToken("items");
            if (items.Count() > 1)
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

    }
}

