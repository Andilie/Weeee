using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Weeee
{
    class Program
    {
        enum Menu
        {
            加载选课列表,
            加载学院列表,
            加载专业列表,
            选课,
            登出

        }
        static void Main(string[] args)
        {


            Core core = new Core();
            bool isOK = Login(core);
            while (!isOK)
            {
                isOK = Login(core);
            }
            if (isOK)
            {
                Console.WriteLine("正在加载选课模块...");
                Console.WriteLine(core.LoadSelectClassMenu(""));
                Console.WriteLine("-------------专业列表-------------");
                Core.SelectClassMenu.WriteSpecialtyList();
                Console.WriteLine("---------专业列表加载完毕---------\n");
                Console.WriteLine("-------------学院列表-------------");
                Core.SelectClassMenu.WriteXueyuanList();
                Console.WriteLine("---------学院列表加载完毕---------\n");
                Core.SelectClassModel selectClass = new Core.SelectClassModel();
                LoadClass(core, selectClass);

            }


            Console.ReadKey();
        }

        private static void LoadClass(Core core, Core.SelectClassModel selectClass)
        {
            Console.Write("请输入自己所在的专业码:");
            selectClass.Specialty = Console.ReadLine();
            Console.Write("请输入所在年级:");
            selectClass.Grade = Console.ReadLine();
            Console.Write("请输入所要选的学院码:");
            selectClass.College = Console.ReadLine();
            Console.Write("请输入所要选的专业码:");
            selectClass.MajorCode = Console.ReadLine();
            Console.Write("输入完毕！");
            core.LoadClassData(selectClass);
        }

        static bool Login(Core core)
        {
            Console.Write("用户名/学号：");
            string userName = Console.ReadLine();
            Console.Write("密码：");
            string pwd = Console.ReadLine();
            string location = null;
            //core.
            core.Login(userName, pwd, out location);
            if (!core.IsLogin)
            {
                Console.WriteLine("登录失败");
                return false;
            }
            else
            {
                Console.WriteLine("登录成功");
                return true;
            }
        }

    }
}
