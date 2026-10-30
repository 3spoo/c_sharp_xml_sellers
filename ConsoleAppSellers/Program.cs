using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleAppSellers
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string FILE = "venditori.xml";
            XmlDocument xd = new XmlDocument();
            xd.Load(FILE);

            // E.1
            List<Seller> sellers = getSellersBestByYear(xd);
            foreach (Seller seller in sellers)
            {
                Console.WriteLine(seller.Name + ", " + seller.Sname + ", " + seller.BestYear);
            }

            // E.2
            createSellersPivot(xd, "venditori.dat");
        }

        public class Seller
        {
            private string _name;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _sname;

            public string Sname
            {
                get { return _sname; }
                set { _sname = value; }
            }

            private int _best_year;

            public int BestYear
            {
                get { return _best_year; }
                set { _best_year = value; }
            }

            public Seller(string n, string s, int y)
            {
                _name = n;
                _sname = s;
                _best_year = y;
            }
        }

        public class Seller2020
        {
            private static readonly Dictionary<string, string> AREAS = new Dictionary<string, string>
            {
                { "NO", "NORD-OVEST" },
                { "NE", "NORD-EST" },
                { "C", "CENTRO" },
                { "S", "SUD" },
                { "I", "ISOLE" },
                { "EURO", "EUROPA" },
                { "AME", "AMERICHE" },
                { "AS", "ASIA" }
            };

            private string _name;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _sname;

            public string Sname
            {
                get { return _sname; }
                set { _sname = value; }
            }

            private int _2020;

            public int Budget
            {
                get { return _2020; }
                set { _2020 = value; }
            }

            private string _area;

            public string Area
            {
                get { return _area; }
                set { _area = value; }
            }


            public Seller2020(string n, string s, string a, int budget)
            {
                _name = n;
                _sname = s;
                _area = AREAS[a];
                _2020 = budget;
            }
        }

        public static List<Seller> getSellersBestByYear(XmlDocument xd)
        {
            List<Seller> data = new List<Seller>();

            foreach(XmlNode node in xd.DocumentElement.ChildNodes)
            {
                string[] sname_name = node.SelectSingleNode("anagrafica").InnerText.Split(' ');
                int money = Convert.ToInt16(node.SelectSingleNode("fatturato_2019_k_euro").InnerText) > Convert.ToInt16(node.SelectSingleNode("budget_2020_k_euro").InnerText) ?
                    2019 : 2020;

                data.Add(new Seller(sname_name[1], sname_name[0], money));
            }

            return data;
        }

        public static void createSellersPivot(XmlDocument xd, string oput)
        {
            string oput_declaration = "cognome_venditore;nome_venditore;area;fatturato_2020";
            List<Seller2020> data = new List<Seller2020>();

            foreach (XmlNode node in xd.DocumentElement.ChildNodes)
            {
                string[] sname_name = node.SelectSingleNode("anagrafica").InnerText.Split(' ');
                string area = node.SelectSingleNode("area").InnerText;
                int b2020 = Convert.ToInt32(node.SelectSingleNode("budget_2020_k_euro").InnerText) * 1000;

                data.Add(new Seller2020(sname_name[1], sname_name[0], area, b2020));
            }

            using (StreamWriter sw = new StreamWriter(oput))
            {
                sw.WriteLine(oput_declaration);
                foreach (var seller in data)
                {
                    sw.WriteLine(seller.Sname + ";" + seller.Name + ";" + seller.Area + ";" + seller.Budget);
                }
            }
        }
    }
}
