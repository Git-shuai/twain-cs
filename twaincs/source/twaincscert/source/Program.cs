///////////////////////////////////////////////////////////////////////////////////////
//
//  twaincscert.Program
//
//  Our entry point.
//
///////////////////////////////////////////////////////////////////////////////////////
//  Author          Date            Comment
//  M.McLaughlin    13-Jan-2020     Initial Release
///////////////////////////////////////////////////////////////////////////////////////
//  Copyright (C) 2020-2021 Kodak Alaris Inc.
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
//  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////////////

// Helpers...
using System;
using System.Threading;
using System.Windows.Forms;
using TWAINWorkingGroup;

namespace twaincscert
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="a_aszArgs">interesting arguments</param>
        [STAThread]
        static void Main(string[] a_aszArgs)
        {
            string szExecutableName;
            string szWriteFolder;

            // 加载配置文件和参数，以便在代码的任何地方都可以访问它们
            if (!Config.Load(System.Reflection.Assembly.GetEntryAssembly().Location, a_aszArgs, "appdata.txt"))
            {
                Console.Out.WriteLine("启动错误。请尝试卸载并重新安装此软件。");
                Environment.Exit(1);
            }

            // 设置数据文件夹
            szWriteFolder = Config.Get("writeFolder", "");
            szExecutableName = Config.Get("executableName", "");

            // 开启日志记录
            Log.Open(szExecutableName, szWriteFolder, 1);
            Log.SetLevel((int)Config.Get("logLevel", 0));
            Log.Info(szExecutableName + " 日志开始...");

            // Windows 需要一个窗口，我们需要在不同的线程中运行控制台和窗口以获得完全控制
            if (TWAIN.GetPlatform() == TWAIN.Platform.WINDOWS)
            {
                // 初始化窗体
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                FormMain formmain = new FormMain();

                // 启动终端窗口，我们在一个线程中执行此操作，以便可以同时拥有控制台和窗体
                // 当线程完成时，我们可以退出应用程序
                Terminal terminal = new twaincscert.Terminal(formmain);
                Thread threadTerminal = new Thread(
                    new ThreadStart(
                        delegate ()
                        {
                            terminal.Run();
                            Environment.Exit(0);
                        }
                    )
                );
                threadTerminal.Start();

                // 运行窗体，以上的退出将终止它
                formmain.SetTerminal(terminal);
                Application.Run(formmain);
            }
            // Linux 和 Mac 的处理方式更简单
            else
            {
                Terminal terminal = new twaincscert.Terminal(null);
                terminal.Run();
            }

            // 完成所有操作
            Log.Info(szExecutableName + " 日志结束...");
            Log.Close();
            Environment.Exit(0);
        }
    }
}
