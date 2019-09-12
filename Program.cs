using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.Security.Principal;

namespace StartupManagement
{
    class Program
    {
        static List<StartupModel> lst = new List<StartupModel>();
        public static bool IsAdministrator()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        static void Main(string[] args)
        {
            bool isAdmin = IsAdministrator();
            RegistryKey rk = Registry.CurrentUser;
            PrintKeys(rk, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", isAdmin);
            rk = Registry.LocalMachine;
            PrintKeys(rk, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", isAdmin);
            PrintKeys(rk, @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", isAdmin);
            if(isAdmin)
            {
                Console.WriteLine("Input Del + ID to drop startup from list");
                try
                {
                    string cmd = Console.ReadLine().ToLower();
                    if (cmd.IndexOf("del") == 0)
                    {
                        cmd = cmd.Replace("del", "");
                        cmd = cmd.Replace(" ", "");
                        var index = int.Parse(cmd);
                        var item = lst.Where(x => x.ID == index).FirstOrDefault();
                        item.RKEY.DeleteValue(item.ProcessName, true);
                    }
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                Console.WriteLine("Successfule drop item from list");
            }
            else
            {
                Console.WriteLine("Need Admin to del startup items");
            }
            Console.ReadKey();
        }
        static void PrintKeys(RegistryKey rkey,string Path,bool writable)
        {
            var view = rkey.View;
            rkey= rkey.OpenSubKey(Path, writable);
            String[] names = rkey.GetValueNames();
            Console.WriteLine(rkey.Name);
            Console.WriteLine("ID\tName\tPath");
            Console.WriteLine("-----------------------------------------------");
            foreach (String s in names)
            {
                var kind = rkey.GetValueKind(s);
                if (kind == RegistryValueKind.String||kind==RegistryValueKind.ExpandString||kind==RegistryValueKind.MultiString)
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
