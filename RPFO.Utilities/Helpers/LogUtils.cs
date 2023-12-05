using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RPFO.Utilities.Helpers
{
    public class LogUtils
    {
        //public static void Writelog(string headding, string LogData)
        //{
        //    var LogPath = AppVal.Appsettings.LogPath;
        //    var LogFileName = LogPath + "\\SyncLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        //    if (!Directory.Exists(LogPath))
        //    {
        //        Directory.CreateDirectory(LogPath);
        //    }
        //    if (!File.Exists(@LogFileName))
        //    {
        //        using (var fs = File.Create(@LogFileName))
        //        {
        //            var info = new UTF8Encoding(true).GetBytes("File Create." + Environment.NewLine);

        //            fs.Write(info, 0, info.Length);
        //        }
        //    }
        //    try
        //    {
        //        if (LogData.Trim() != string.Empty)
        //        {
        //            var line = "[" + DateTime.Now.ToString("yyyy/MM/dd ") + DateTime.Now.ToString("- HH:mm:ss") + "] : " + headding.ToUpper() + LogData;
        //            if (headding == "######")
        //            {
        //                var oldTxt_null = File.ReadAllText(@LogFileName);
        //                TextWriter tw_null = new StreamWriter(@LogFileName);
        //                tw_null.WriteLine(oldTxt_null + string.Empty);
        //                tw_null.WriteLine(string.Empty);
        //                tw_null.Close();
        //            }

        //            var oldTxt = File.ReadAllText(@LogFileName);
        //            TextWriter tw = new StreamWriter(@LogFileName);
        //            tw.WriteLine(oldTxt + line);
        //            tw.Close();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        //public static string[] ReadLog()
        //{
        //    var LogPath = AppVal.Appsettings.LogPath;
        //    var LogFileName = LogPath + "\\SyncLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
        //    var lines = (string[])null;
        //    if (File.Exists(@LogFileName))
        //    {
        //        lines = System.IO.File.ReadAllLines(@LogFileName);
        //    }
        //    return lines;
        //}

        /////// <summary>
        /////// Ghi data của Eton ra file
        /////// </summary>
        /////// <param name="eventName">Tên của sự kiện cần ghi. Sẽ đặt thành tên Folder.</param>
        /////// <param name="queueName">Tên của Queue, sẽ đặt thành file name.</param>
        /////// <param name="text">Nội dung Json cần ghi.</param>
        ////public static void WriteLogEton(string eventName, string queueName, string text)
        ////{
        ////    try
        ////    {
        ////        WriteLogEton(AppConstants.EtonLogDirectory, eventName, queueName, text);
        ////        //string fileName = $"{queueName}_{DateTime.Now.ToString("yyyyMMdd")}_{DateTime.Now.ToString("HHmmss")}_{DateTime.Now.ToString("fff")}.txt";
        ////        //string directory = Path.Combine(AppConstants.EtonLogDirectory, eventName);
        ////        //if (!Directory.Exists(directory))
        ////        //{
        ////        //    Directory.CreateDirectory(directory);
        ////        //}
        ////        //File.WriteAllText(directory + "\\" + fileName, text, Encoding.UTF8);
        ////    }
        ////    catch
        ////    {
        ////    }
        ////}

        /// <summary>
        /// Ghi data ra file
        /// </summary>
        /// <param name="baseDirectory">Đường dẫn của Folder.</param>
        /// <param name="eventName">Tên của sự kiện cần ghi. Sẽ đặt thành tên Folder.</param>
        /// <param name="queueName">Tên của Queue, sẽ đặt thành file name.</param>
        /// <param name="text">Nội dung Json cần ghi.</param>
        public static void WriteLogData(string baseDirectory, string eventName, string queueName, string text)
        {
            try
            {
                string fileName = $"{queueName}_{DateTime.Now:HHmmss}_{DateTime.Now:fff}.txt";
                string directory = Path.Combine(baseDirectory, eventName, DateTime.Now.ToString("yyyyMMdd"));
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(directory + "\\" + fileName, text, Encoding.UTF8);
            }
            catch
            {
            }
        }
    }
}
