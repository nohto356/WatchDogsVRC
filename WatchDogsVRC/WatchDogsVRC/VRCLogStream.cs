using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optional;

namespace NotoIto.App.WatchDogsVRC
{
    public class VRCLogStream
    {
        public event EventHandler<EventArgs> LogDataAvailable;
        public readonly string FolderPath;

        private DateTime LastUpdateTime;
        private string LastFileName = "";
        private int LastLine = -1;
        private System.Threading.Thread thread;
        public VRCLogStream()
        {
            LastUpdateTime = DateTime.Now;
            FolderPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat\";
            LogDataAvailable += DataReceived;
            thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(
                    () =>
                    {
                        while (true)
                        {
                            GetLogText(FolderPath).MatchSome(
                                (strList) =>
                                {
                                    foreach (var str in strList)
                                        LogDataAvailable?.Invoke(str, EventArgs.Empty);
                                }
                                );
                            System.Threading.Thread.Sleep(1000);
                        }
                    })
                );
            thread.Start();
        }

        private void DataReceived(object sender, EventArgs e)
        {
            string logMessage = sender as string;
            var (logType, messageSender, messageType, messages) = VRCLogParser.ParseText(logMessage);
            foreach (var message in messages)
                Console.WriteLine(message);
        }

        private Option<string[]> GetLogText(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "output_log_*M.txt");
            List<string> texts = new List<string>();
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                if(fi.LastWriteTime > LastUpdateTime)
                {
                    LastUpdateTime = fi.LastWriteTime;
                    var latestFileName = file;
                    if(LastFileName != latestFileName)
                    {
                        LastFileName = latestFileName;
                        LastLine = 0;
                    }
                    using(FileStream fs = new FileStream(LastFileName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite))
                    using (StreamReader sr = new StreamReader(fs,Encoding.UTF8))
                    {
                        int i;
                        for (i = 0; sr.Peek() != -1; i++)
                        {
                            var text = sr.ReadLine();
                            if (i >= LastLine && text != "")
                            {
                                texts.Add(text);
                            }
                        }
                        LastLine = i;
                    }
                }
            }
            if (texts.Count != 0)
                return Option.Some(texts.ToArray());
            else
                return Option.None<string[]>();
        }
    }
}
