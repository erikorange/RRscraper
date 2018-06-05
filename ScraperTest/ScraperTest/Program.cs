using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace ScraperTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // GCRCN all talkgroups
            // table class rrtable w1p
            // all td , or, all tr with a td
            string theUrl = @"https://www.radioreference.com/apps/db/?sid=7337&opt=all_tg#tgs";
            string xPath = @"//table[@class='rrtable w1p']/tr/td[@class='td0 t' or @class='td1 t']";
            string filePath = @"C:\users\erik\documents";

            int idx = 0;

            var web = new HtmlWeb();
            var doc = web.Load(theUrl);

            List<string> rawData = new List<string>();
            List<TalkGroup> TGs = new List<TalkGroup>();

            idx = 0;
            foreach (HtmlNode n in doc.DocumentNode.SelectNodes(xPath))
            {
                rawData.Add(n.InnerHtml);
                idx++;

                if (idx == 7)
                {
                    TalkGroup tg = CreateTG(rawData);
                    TGs.Add(tg);
                    rawData.Clear();
                    idx = 0;
                }
            }

            SaveTGsToFile(TGs, filePath);

            Console.WriteLine("Done.");
            Console.ReadKey();
        }

        private static void SaveTGsToFile(List<TalkGroup> tgs, string filePath)
        {
            string fileName = "talkgroups.txt";

            using (StreamWriter sw = new StreamWriter(filePath + @"\" + fileName))
            {
                foreach (TalkGroup t in tgs)
                {
                    sw.WriteLine(t.TalkGroupIdDec + "," + t.AlphaTag);
                }
            }
        }

        private static TalkGroup CreateTG(List<string> rawData)
        {
            // TODO remove &NBSP
            TalkGroup tg = new TalkGroup();
            tg.TalkGroupIdDec = rawData[0].Trim();
            tg.TalkGroupIdHex = rawData[1].Trim();
            tg.Mode = rawData[2];
            tg.AlphaTag = rawData[3];
            tg.Description = RemoveNBSP(rawData[4]);
            tg.Tag = RemoveNBSP(rawData[5]);
            tg.Group = RemoveNBSP(rawData[6]);
            return tg;
        }

        private static string RemoveNBSP(string data)
        {
            return data.Split('&')[0];
        }
    }
}
