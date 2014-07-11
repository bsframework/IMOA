using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMOAWinClient
{
    public partial class Form_BX : Form
    {
        public Form_BX()
        {
            InitializeComponent();
            Uri uri = new Uri("https://office.inspur.com/eportal/ui?pageId=334521&fromUrl=aHR0cHM6Ly9vZmZpY2UuaW5zcHVyLmNvbS9lcG9ydGFsL3VpLztqc2Vzc2lvbmlkPTE1MDI0NDAyQjEyREY5RUQyNjRBRTA4MEQ3NDUxMTI2JnBhZ2VJZD1udWxs");
            webBrowser1.Navigate(uri);
        }
        bool islog = false;
        private void Form_BX_Load(object sender, EventArgs e)
        {

            webBrowser1.DocumentCompleted += webBrowser1_DocumentCompleted;


        }

        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
           if(islog==false)
           {
               webBrowser1.Document.GetElementsByTagName("input")[18].InnerText = "aaa";
               webBrowser1.Document.GetElementsByTagName("input")[17].InnerText = "aaaa";
               HtmlElement formLogin = webBrowser1.Document.Forms["adminLoginform"];
               formLogin.InvokeMember("submit");
               Thread.Sleep(5000);
               webBrowser1.Navigate(new Uri("http://portal.inspur.com:9080/inspurportal/jsp/lcp/portal/workbench/index.jsp"));
           }
           islog = true;
           foreach (HtmlElement archor in this.webBrowser1.Document.Links)
           {
               archor.SetAttribute("target", "_self");
           }

           //将所有的FORM的提交目标，指向本窗体
           foreach (HtmlElement form in this.webBrowser1.Document.Forms)
           {
               form.SetAttribute("target", "_self");
           }


        }
        private void herfclick(string url)
        {

            for (int i = 0; i < webBrowser1.Document.All.Count; i++)
            {

                if (webBrowser1.Document.All[i].TagName == "A" && webBrowser1.Document.All[i].GetAttribute("href").ToString().Trim() == url)
                {

                    webBrowser1.Document.All[i].InvokeMember("click");//引发”CLICK”事件

                    break;

                }

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
