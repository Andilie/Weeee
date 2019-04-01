using HtmlAgilityPack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Weeee
{
    /// <summary>
    /// 核心模块
    /// </summary>
    public class Core
    {
        public CookieContainer CookieContainer { get; set; }
        public string StrCookie { get; set; }

        public bool IsLogin = false;

        /// <summary>
        /// 选课菜单模型
        /// </summary>
        public static class SelectClassMenu
        {

            /// <summary>
            /// 专业列表
            /// </summary>
            public static SortedList SpecialtySortedList { get; set; }

            /// <summary>
            /// 学院列表
            /// </summary>
            public static SortedList College { get; set; }

            public static string LwPageSize { get; set; }

            /// <summary>
            /// 专业码
            /// </summary>
            public static string MajorCode { get; set; }


            /// <summary>
            /// （自己）专业
            /// </summary>
            public static string Specialty { get; set; }

            /// <summary>
            /// 年级
            /// </summary>
            public static string grade { get; set; }

            /// <summary>
            /// 正常/重修
            /// </summary>
            public static string Type { get; set; }

            /// <summary>
            /// （选课）专业
            /// </summary>
            public static string SelectMajor { get; set; }

            public static void WriteSpecialtyList()
            {
                if (SpecialtySortedList.Count > 0)
                {
                    foreach (DictionaryEntry item in SpecialtySortedList)
                    {
                        Console.WriteLine("专业码:{0}|专业名称,{1}", item.Key, item.Value);
                    }
                }


            }
            public static void WriteXueyuanList()
            {
                if (College.Count > 0)
                {
                    foreach (DictionaryEntry item in College)
                    {
                        Console.WriteLine("学院码:{0}|学院名称,{1}", item.Key, item.Value);
                    }
                }


            }
        }


        /// <summary>
        /// 选课模型
        /// </summary>
        public class SelectClassModel
        {
            public string LwPageSize { get; set; }

            /// <summary>
            /// 专业码
            /// </summary>
            public string MajorCode { get; set; }

            /// <summary>
            /// 选择院系
            /// </summary>
            public string Specialty { get; set; }

            /// <summary>
            /// 年级
            /// </summary>
            public string Grade { get; set; }

            /// <summary>
            /// 学院
            /// </summary>
            public string College { get; set; }

            /// <summary>
            /// 专业
            /// </summary>
            public string Major { get; set; }

            /// <summary>
            /// 课程
            /// </summary>
            public List<string> HasCourses { get; set; }

            /// <summary>
            /// 排除的课程
            /// </summary>
            public List<string> ExcludeCourse { get; set; }
            public SelectClassModel() { }

        }
        string login = "";
        string select = "";
        private List<string> classList;

        public string Url { get; set; }
        public virtual void Login(string username, string pwd, out string loaction)
        {

            string retStringHtml = null;
            string postString = "username=" + username + "&pwd=" + pwd + "XXXXXXX";
            try
            {
                byte[] postData1 = Encoding.GetEncoding("gb2312").GetBytes(postString);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(login);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.KeepAlive = true;
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/18.17763";
                httpWebRequest.Accept = "text/html, application/xhtml+xml, application/xml; q=0.9, */*; q=0.8";
                httpWebRequest.AllowAutoRedirect = false;
                httpWebRequest.ContentLength = postData1.Length;
                httpWebRequest.CookieContainer = new CookieContainer();
                CookieContainer = httpWebRequest.CookieContainer;
                using (Stream requestStream = httpWebRequest.GetRequestStream())
                {
                    requestStream.Write(postData1, 0, postData1.Length);
                    using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
                    {

                        if (response.StatusCode == HttpStatusCode.Found)
                        {
                            IsLogin = true;
                            response.Cookies = httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri);
                            CookieCollection cookie = response.Cookies;
                            StrCookie = httpWebRequest.CookieContainer.GetCookieHeader(httpWebRequest.RequestUri);
                            Stream responseStream = response.GetResponseStream();
                            if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))//解压
                            {
                                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                            }
                            using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))
                            {
                                retStringHtml = streamReader.ReadToEnd();
                                loaction = response.Headers["location"];
                                streamReader.Close();
                            }
                            responseStream.Close();
                            response.Dispose();//释放资源
                            response.Close();
                          
                        }
                        else
                        {
                            loaction = null;
                        }
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 加载选课菜单
        /// </summary>
        /// <param name="loaction"></param>
        /// <returns></returns>
        public string LoadSelectClassMenu(string loaction)
        {

            string content = null;
            if (IsLogin != false && CookieContainer.Count > 0 && loaction != string.Empty)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(select);
                request.Method = "GET";
                request.KeepAlive = true;
                request.Headers.Add("Cookie:" + StrCookie);
                request.CookieContainer = CookieContainer;
                request.AllowAutoRedirect = true;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        StrCookie = request.CookieContainer.GetCookieHeader(request.RequestUri);
                        Stream responseStream = response.GetResponseStream();
                        if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                        {
                            responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                        }
                        StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312"));
                        content = streamReader.ReadToEnd();
                        request.Abort();
                        streamReader.Close();
                        response.Dispose();//释放资源
                        response.Close();
                        var doc = new HtmlDocument();
                        doc.LoadHtml(content);
                        //加载选课参数：
                        HtmlNode form = doc.DocumentNode.SelectSingleNode(".//div[@name='XXXX']");

                        //获取专业
                        var specialty = form.SelectNodes("//div[@id='XXXX']/a");
                        SortedList sortedList = new SortedList();
                        bool oneLoad = true;
                        foreach (var item in specialty)
                        {
                            if (oneLoad)
                            {
                                oneLoad = false;
                                continue;
                            }
                            sortedList.Add(item.Attributes["value"].Value, item.InnerText);//item.NextSibling.InnerText /
                        }

                        //获取学院
                        var xueyuan = form.SelectNodes("//select[@name='XXX']/option");
                        SortedList sortedList2 = new SortedList();
                        bool fristLoad = true;
                        foreach (var item in xueyuan)
                        {
                            if (fristLoad)
                            {
                                fristLoad = false;
                                continue;
                            }
                            sortedList2.Add(item.Attributes["value"].Value, item.InnerText);
                        }

                        SelectClassMenu.LwPageSize = form.SelectSingleNode("//input[@name='XXX']").Attributes["value"].Value;
                        SelectClassMenu.SpecialtySortedList = sortedList;//(自己)专业列表                                               
                        SelectClassMenu.College = sortedList2;//学院列表

                        return "成功进入选课模块";
                    }
                    else
                    {
                        return response.StatusDescription;
                    }
                }
            }
            else
            {
                return "无法加载,请先登录";
            }
        }

        /// <summary>
        /// 加载课程
        /// </summary>
        /// <param name="selectClass"></param>
        public void LoadClassData(SelectClassModel selectClass)
        {

            string postData = "";
            byte[] postData1 = Encoding.GetEncoding("gb2312").GetBytes(postData);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("");
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.KeepAlive = true;
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.140 Safari/537.36 Edge/18.17763";
            httpWebRequest.Accept = "text/html, application/xhtml+xml, application/xml; q=0.9, */*; q=0.8";
            httpWebRequest.AllowAutoRedirect = false;
            //httpWebRequest.Headers["Accept-Language"] = "zh-CN,zh;q=0.";
            httpWebRequest.ContentLength = postData1.Length;
            httpWebRequest.CookieContainer = CookieContainer;
            string retString;
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(postData1, 0, postData1.Length);
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                response.Cookies = httpWebRequest.CookieContainer.GetCookies(httpWebRequest.RequestUri);
                //CookieCollection cookie = response.Cookies;
                StrCookie = httpWebRequest.CookieContainer.GetCookieHeader(httpWebRequest.RequestUri);
                Stream responseStream = response.GetResponseStream();
                if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"].ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("gb2312")))
                {
                    retString = streamReader.ReadToEnd();
                    streamReader.Dispose();
                    streamReader.Close();

                }
                responseStream.Dispose();
                response.Dispose();
                response.Close();

            }

            //获取表格内容
            var doc = new HtmlDocument();
            doc.LoadHtml(retString);
            HtmlNode node = doc.DocumentNode;
            var table = node.SelectSingleNode("//table");
            HtmlNodeCollection tr = table.SelectNodes(".//tr");
            //classList = new List<string>();
            //foreach (HtmlNode item in table.SelectNodes(".//input[@name='XXXXX']"))
            //{
            //    if (item.Attributes["value"].Value != null)
            //    {
            //        Console.WriteLine(item.Attributes["value"].Value);
            //        classList.Add(item.Attributes["value"].Value);
            //    }
            //}
            int cellSize = 16;//表格列数
            foreach (var row in tr)
            {
                foreach (var cell in row.SelectNodes("//td"))
                {
                    cellSize--;
                    if (cellSize > 0)
                    {
                        Console.WriteLine("cell: " + cell.InnerText);
                    }
                    else if (cellSize == 0)
                    {
                        cellSize = 16;
                        Console.WriteLine("-----row-----");
                    }

                }
            }

        }
    }
}
