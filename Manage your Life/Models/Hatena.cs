﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Xml;

namespace Manage_your_Life.Models
{
    class Hatena
    {

        /// <summary>
        /// cnameタグのリスト
        /// </summary>
        static public string[] cnameList =  {
                "book", "music", "movie", "web", "elec", "animal", "anime", 
                "food", "sports", "game", "comic", "hatena", "club", "science",
                "art", "geography", "idol", "society", "tv"
        };

        string url = "http://d.hatena.ne.jp/xmlrpc";
        string baseXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
                    + "<methodCall>\n"
                    + "  <methodName>hatena.setKeywordLink</methodName>\n"
                    + "  <params>\n"
                    + "    <param>\n"
                    + "      <value>\n"
                    + "        <struct>\n"
                    + "          <member>\n"
                    + "            <name>body</name>\n"
                    + "            <value><string>@body</string></value>\n"
                    + "          </member>\n"
                    + "          <member>\n"
                    + "            <name>mode</name>\n"
                    + "            <value><string>lite</string></value>\n"
                    + "          </member>\n"
                    + "          <member>\n"
                    + "            <name>score</name>\n"
                    + "            <value><int>@score</int></value>\n"
                    + "          </member>\n"
                    + "        </struct>\n"
                    + "      </value>\n"
                    + "    </param>\n"
                    + "  </params>\n"
                    + "</methodCall>\n";

        string xPath = "/methodResponse/params/param/value/struct/member"
                     + "/value/array/data/value/struct";

        //カテゴリNGワード
        StringCollection ngWords;

        int score;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="score"></param>
        public Hatena(int score)
        {
            this.score = score;

            ngWords = Properties.Settings.Default.categoryNGWord;
        }



        /// <summary>
        /// 与えられた文字列をHatenaAPIを使用してカテゴライズする
        /// </summary>
        /// <param name="windowTitle">APIの解析に使用する文字列</param>
        /// <returns>抽出された単語とカテゴリーのDictionary</returns>
        internal Dictionary<string, string> Categorizing(string windowTitle)
        {
            XmlDocument xml = null;
            RetryHelper.Retry(() => 
                {
                    xml = AccessToAPI(windowTitle);
                },
                ex => 
                {
                    throw new Exception("はてなAPIとの通信に失敗したため、カテゴライズが出来ません。\nネットワーク接続を確認して下さい。", ex);
                },
                ex => ex is Exception, 5
            );

            return GetXmlData(xml);
        }



        /// <summary>
        /// XMLを解析して中身をディクショナリで返す
        /// </summary>
        /// <param name="xml">解析させたいXML</param>
        /// <returns>"word", "cname"なディクショナリ</returns>
        private Dictionary<string, string> GetXmlData(XmlDocument xml)
        {
            //<"word", "cname">なディクショナリ
            Dictionary<string, string> dic = new Dictionary<string, string>();

            //xPathに一致するノード
            XmlNodeList node = xml.SelectNodes(xPath);

            //XMLにキーワードがない状態
            if (node.Count == 0) return null;

            //キーワードの数だけディクショナリ作成
            for (int i = 0; i < node.Count; i++)
            {
                //wordを取得
                XmlElement element = (XmlElement)node[i].ChildNodes.Item(0);
                string word = element.GetElementsByTagName("value")[0].InnerText;

                //cnameを取得
                element = (XmlElement)node[i].ChildNodes.Item(3);
                string cname = element.GetElementsByTagName("value")[0].InnerText;

                if (cname == "") continue;
                if (ngWords.Contains(word)) continue;

                //wordが既に含まれていればディクショナリに追加
                //同じwordで複数のcnameをもつ場合がある
                if (!dic.ContainsKey(word))
                {
                    dic.Add(word, cname);
                }
            }

            return dic;
        }


        /// <summary>
        /// はてなキーワード自動リンクAPIを叩いてXMLを貰ってくる
        /// </summary>
        /// <param name="body">キーワードを抽出させたい任意のテキスト
        /// (ウィンドウタイトル)</param>
        /// <returns>API叩いて帰ってきたXML</returns>
        private XmlDocument AccessToAPI(string body)
        {
            XmlDocument xml = new XmlDocument();

            //ひな形のxmlで必要部分を置換
            baseXml = baseXml.Replace("@body", body);
            baseXml = baseXml.Replace("@score", score.ToString());

            //xmlをUTF8でbyte[]にエンコード
            byte[] bytes = Encoding.UTF8.GetBytes(baseXml);

            //Requestの作成
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "text/xml; charset=UTF-8;";
            req.ContentLength = bytes.Length;

            //xmlの送信
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }


            //xmlの受信
            using (WebResponse res = req.GetResponse())
            {
                using (Stream stream = res.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        xml.LoadXml(sr.ReadToEnd());
                    }
                }
            }

            return xml;
        }

    }
}
