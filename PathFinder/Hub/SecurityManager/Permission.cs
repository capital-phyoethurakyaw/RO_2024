using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
namespace RouteOptimizer.SecurityManager
{
    public class Permission
    {
        public void AddAllUserRight()
        {
            string rootDirectory = @"C:\Program Files\Wyzrs\RouteOptimizer\"; 
            DirectoryInfo directoryInfo = new DirectoryInfo(rootDirectory); 
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl(); 
            FileSystemAccessRule accessRule = new FileSystemAccessRule(
                "TrustedInstaller",                            // Specify the identity for which to add the permission
                FileSystemRights.FullControl,       // Specify the permission to grant
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, // Specify inheritance flags
                PropagationFlags.None,              // Specify propagation flags
                AccessControlType.Allow              // Specify the type of access control
            );

            directorySecurity.AddAccessRule(accessRule);

            // Set the modified DirectorySecurity object back to the directory
            directoryInfo.SetAccessControl(directorySecurity);
        }
        public void AddRightsByPS(string path = null)
        {
            string Path = path == null ? Entity.StaticCache.path.Replace("DS", "") : path; // "C:\\Program Files\\Wyzrs\\RouteOptimizer\\";
            var cmd =
                 "$folderPath=\"" + Path + "\"" + Environment.NewLine +
                 "$acl=Get-Acl $folderPath" + Environment.NewLine +
                 "$rule = New-Object System.Security.AccessControl.FileSystemAccessRule(" + Environment.NewLine +
                 "\"Everyone\",                               # Specify the identity for which to grant access" + Environment.NewLine +
                 "\"FullControl\",                            # Specify the access rights to grant" + Environment.NewLine +
                 "\"ContainerInherit,ObjectInherit\",         # Specify inheritance flags" + Environment.NewLine +
                 "\"None\",                                   # Specify propagation flags" + Environment.NewLine +
                 "\"Allow\"                                   # Specify the type of access control" + Environment.NewLine +
                 ")" + Environment.NewLine +
                 "$acl.SetAccessRule($rule)" + Environment.NewLine +
                 "Set-Acl -Path $folderPath -AclObject $acl" + Environment.NewLine;


            var temppath = @"C:\OptimizerTemp";
            var temppermission = temppath + "\\" + "permission.ps1";
            var tempadmin = temppath + "\\" + "runAdmin.ps1";
            try
            {
                using (PowerShell PowerShellInst = PowerShell.Create())
                {
                    PowerShell ps = PowerShell.Create();
                    if (!Directory.Exists(temppath))
                        Directory.CreateDirectory(temppath);
                    if (!File.Exists(temppermission))
                        using (StreamWriter writer = new StreamWriter(temppermission))
                        {
                            writer.Write(cmd);
                            writer.Flush();
                            writer.Close();
                        }
                    string admincmd = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -windowstyle hidden -NoProfile -ExecutionPolicy Bypass -Command ""& {Start-Process C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -windowstyle hidden -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File """ + temppermission + "\"' -Verb RunAs}\"";  // @"D:\_a\runAdmin.ps1";
                    if (!File.Exists(tempadmin))
                        using (StreamWriter writer = new StreamWriter(tempadmin))
                        {
                            writer.Write(admincmd);
                            writer.Flush();
                            writer.Close();
                        }
                    ps.AddScript(File.ReadAllText(tempadmin));

                    ps.Invoke();
                    try
                    {
                        ps.Invoke();
                        ps.Invoke();
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteLog(ex);

                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog.WriteLog(ex);

            }
            try
            {
                Directory.Delete(temppath, true);
            }
            catch
            {

            }
            if (path == null)
                RemoveAllCSV();
        }
        public void RemoveAllCSV()
        {
            var fall = Directory.GetFiles(Entity.StaticCache.path.Replace("\\DS", ""));
            if (fall.Count() == 0) return;
            foreach (var f in fall)
            {
                try
                {
                    if (Path.GetExtension(f).ToString().ToLower().Contains("csv"))
                        File.Delete(f);
                }
                catch (Exception ex)
                {

                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c del \"{f}\"",
                            Verb = "runas"
                        };

                        Process.Start(psi);
                    }
                    catch (Exception exx)
                    {
                       // DebugLog.WriteLog(ex);

                        //  Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }

            }
        }
    }
 
}
