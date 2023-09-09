using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTerminal
{
    internal class s_ls
    {
        public void start(string[] ss)
        {
            if (ss.Length == 2)
            {
                if (ss[1] == "-f")
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                    if (directoryInfo.Exists)
                    {
                        Console.WriteLine($"Текущая директория: {directoryInfo.FullName}:");
                        foreach (var item in directoryInfo.GetFileSystemInfos())
                        {
                            string itemType = (item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "|DIR" : "-FILE";
                            Console.WriteLine($"{itemType,-12}{item.FullName}; C: {item.CreationTime}; LW: {item.LastWriteTime}; LA: {item.LastAccessTime};");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Директория {ss[1]} не существует.");
                    }

                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ss[1]);

                    if (directoryInfo.Exists)
                    {
                        Console.WriteLine($"Текущая директория: {directoryInfo.FullName}:");
                        foreach (var item in directoryInfo.GetFileSystemInfos())
                        {
                            string itemType = (item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "|DIR" : "-FILE";
                            Console.WriteLine($"{itemType,-12}{item.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Директория {ss[1]} не существует.");
                    }
                }
            }
            else if (ss.Length >= 3)
            {
                if (ss[2] == "-f")
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ss[1]);

                    if (directoryInfo.Exists)
                    {
                        Console.WriteLine($"Текущая директория: {directoryInfo.FullName}:");
                        foreach (var item in directoryInfo.GetFileSystemInfos())
                        {
                            string itemType = (item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "|DIR" : "-FILE";
                            Console.WriteLine($"{itemType,-12}{item.FullName}; C: {item.CreationTime}; LW: {item.LastWriteTime}; LA: {item.LastAccessTime};");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Директория {ss[1]} не существует.");
                    }
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ss[1]);

                    if (directoryInfo.Exists)
                    {
                        Console.WriteLine($"Текущая директория: {directoryInfo.FullName}:");
                        foreach (var item in directoryInfo.GetFileSystemInfos())
                        {
                            string itemType = (item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "|DIR" : "-FILE";
                            Console.WriteLine($"{itemType,-12}{item.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Директория {ss[1]} не существует.");
                    }
                }
            }
            else
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

                if (directoryInfo.Exists)
                {
                    Console.WriteLine($"Текущая директория: {directoryInfo.FullName}:");
                    foreach (var item in directoryInfo.GetFileSystemInfos())
                    {
                        string itemType = (item.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? "|DIR" : "-FILE";
                        Console.WriteLine($"{itemType,-12}{item.Name}");
                    }
                }
                else
                {
                    Console.WriteLine($"Директория {ss[1]} не существует.");
                }
            }
        }
    }
}
