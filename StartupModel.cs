using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace StartupManagement
{
    class StartupModel
    {
        public RegistryKey RKEY { get; set; }
        public int ID { get; set; }
        public string ProcessName { get; set; }
        public string Path { get; set; }
    }
}
