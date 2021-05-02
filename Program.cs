using System;
using CommandLine;
using CommandLine.Text;
using System.IO;
using System.Collections.Generic;

namespace winFileFind
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<command>(args)
                .WithParsed(opt => {opt.Execute();})
                .WithNotParsed(er => {/*nothing*/});
        }
        
        class command
        {
            public SortedList<string,FileInfo> FileList = new SortedList<string,FileInfo>();
            public void Execute()
            {
                var dir = new DirectoryInfo(this.inDirStr);
                getFileInfo(dir);
                var writeList = new List<string>();
                foreach (var f in FileList)
                {
                    if (checkCondition(f.Value))
                    {
                        writeList.Add(command.GetStringLineFromFileinfo(f.Value));
                        Console.WriteLine(command.GetStringLineFromFileinfo(f.Value));
                    }
                }
                File.AppendAllLines(this.outFileStr,writeList.ToArray());
            }
            private bool checkCondition(FileInfo file)
            {
                bool result = true;
                if (this.updateDate != null)
                {
                   if (DateTime.Compare(file.LastWriteTime,this.updateDateTh) > 0)
                   {
                       result = false;
                   } 
                }
                if (this.accessDate != null)
                {
                   if (DateTime.Compare(file.LastWriteTime,this.accessDateTh) > 0)
                   {
                       result = false;
                   } 
                }
                return result;
            }
            public void getFileInfo(DirectoryInfo dir)
            {
                foreach (var file in dir.GetFiles())
                {
                    FileList.Add(file.FullName,file);   
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var d in dir.GetDirectories())
                    {
                        getFileInfo(d);
                    }
                }
                
            }
            public static string GetStringLineFromFileinfo(FileInfo file)
            {
                string[] str = {file.FullName, file.Name,file.Length.ToString(),file.LastWriteTime.ToString(),file.LastAccessTime.ToString()};
                return string.Join(",",str);
            }

            [Option('i', "indir", Required = true)]
            public string inDirStr { get; set; }
            [Option('o', "output", Required = true)]
            public string outFileStr { get; set; }
            [Option('u', "update", Required = false)]
            public string updateDate { get; set; }

            public DateTime updateDateTh {get{return DateTime.ParseExact(this.updateDate, "yyyyMMdd", null);}}

            [Option('a', "access", Required = false)]
            public string accessDate { get; set; }
            public DateTime accessDateTh {get{return DateTime.ParseExact(this.accessDate, "yyyyMMdd", null);}}

            [Option('l', "limit", Required = false)]
            public DateTime limitNum { get; set; }
        }
    }   
}
