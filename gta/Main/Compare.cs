using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GTA.Main
{
    class Compare
    {
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

    }
}
