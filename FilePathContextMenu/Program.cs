/*
Copyright (c) 2014 Alion Systems LLC
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions 
are met:

1. Redistributions of source code must retain the above copyright 
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright 
   notice, this list of conditions and the following disclaimer in the 
   documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED 
TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS 
BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
POSSIBILITY OF SUCH DAMAGE. 
*/

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace FilePathContextMenu {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            const string install = "-install";
            const string uninstall = "-uninstall";
            const string do_uninstall = "-do_uninstall";
            const string name = "-name";
            const string path = "-path";

            if (args.Length == 2) {
                String str = args[1];
                if (args[0] == name) {
                    int last_bs = str.LastIndexOf("\\");
                    str = str.Substring(last_bs + 1);
                }
                else if (args[0] == path) {
                }
                else {
                    Environment.Exit(0);
                }
                Clipboard.SetData(DataFormats.Text, (Object)str);
                Environment.Exit(0);
            }

            if (args.Length == 0 || (args.Length == 1 && args[0] == uninstall)) {
                string currentProcess = Assembly.GetEntryAssembly().Location;
                ProcessStartInfo startInfo = new ProcessStartInfo(currentProcess,
                    (args.Length == 0) ? install : do_uninstall);
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
            }
            else if (args.Length == 1) {
                if (args[0] == install) {
                    RegistryKey key = Registry.LocalMachine;
                    RegistryKey key1 = key.OpenSubKey("Software\\Classes\\*\\Shell", true);
                    using (RegistryKey
                        cfp = key1.CreateSubKey("Copy File Name\\Command")) {
                        cfp.SetValue(null, "\"" + Application.ExecutablePath + "\" \"" + name + "\" \"%1\"");
                    }
                    using (RegistryKey
                        cfp = key1.CreateSubKey("Copy File Path\\Command")) {
                        cfp.SetValue(null, "\"" + Application.ExecutablePath + "\" \"" + path + "\" \"%1\"");
                    }
                    key1 = key.OpenSubKey("Software\\Classes\\Directory\\Shell", true);
                    using (RegistryKey
                        cfp = key1.CreateSubKey("Copy Folder Name\\Command")) {
                        cfp.SetValue(null, "\"" + Application.ExecutablePath + "\" \"" + name + "\" \"%1\"");
                    }
                    using (RegistryKey
                        cfp = key1.CreateSubKey("Copy Folder Path\\Command")) {
                        cfp.SetValue(null, "\"" + Application.ExecutablePath + "\" \"" + path + "\" \"%1\"");
                    }
                }
                else if (args[0] == do_uninstall) {
                    RegistryKey key = Registry.LocalMachine;
                    RegistryKey key1 = key.OpenSubKey("Software\\Classes\\*\\Shell", true);
                    key1.DeleteSubKeyTree("Copy File Name", false);
                    key1.DeleteSubKeyTree("Copy File Path", false);
                    key1 = key.OpenSubKey("Software\\Classes\\Directory\\Shell", true);
                    key1.DeleteSubKeyTree("Copy Folder Name", false);
                    key1.DeleteSubKeyTree("Copy Folder Path", false);
                }
            }
        }
    }
}
