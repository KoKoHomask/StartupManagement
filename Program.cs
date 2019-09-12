using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
namespace StartupManagement
{
    class Program
    {
        static List<StartupModel> lst = new List<StartupModel>();
        static void Main(string[] args)
        {
            RegistryKey rk = Registry.CurrentUser;
            PrintKeys(rk, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            rk = Registry.LocalMachine;
            PrintKeys(rk, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            PrintKeys(rk, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run");
            Console.WriteLine("Input Del + ID to drop startup from list");
            try
            {
                string cmd = Console.ReadLine().ToLower();
                if (cmd.IndexOf("del") == 0)
                {
                    cmd = cmd.Replace("del", "");
                    cmd = cmd.Replace(" ", "");
                    var index = int.Parse(cmd);
                    var item= lst.Where(x => x.ID == index).FirstOrDefault();
                    item.RKEY.DeleteValue(item.ProcessName, true);
                }
            }catch(Exception ex) { Console.WriteLine(ex.ToString()); }
            Console.WriteLine("Successfule drop item from list");
            Console.ReadKey();
        }
        static void PrintKeys(RegistryKey rkey,string Path)
        {
            var view = rkey.View;
            rkey= rkey.OpenSubKey(Path,true);
            String[] names = rkey.GetValueNames();
            Console.WriteLine(rkey.Name);
            Console.WriteLine("ID\tName\tPath");
            Console.WriteLine("-----------------------------------------------");
            foreach (String s in names)
            {
                var kind = rkey.GetValueKind(s);
                if (kind == RegistryValueKind.String)
                {
                    var path = rkey.GetValue(s);
                    Console.Write((lst.Count + 1) + "\t");
                    Console.Write(s);
                    Console.WriteLine('\t' + path.ToString());
                    lst.Add(new StartupModel
                    {
                        ID = lst.Count + 1,
                        Path = path.ToString(),
                        ProcessName = s,
                        RKEY = rkey
                    });
                }
            }
            Console.Write("\r\n");
        }
    }
}
