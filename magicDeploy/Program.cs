using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;


namespace magicDeploy
{
    class Program
    {
        static void Main(string[] args)
        {

            var serverManager = new ServerManager();
            var appPool = serverManager.ApplicationPools.FirstOrDefault(ap => ap.Name.Equals("API_FechaHora"));
            appPool.Stop();

            //appPool.Start();

            //C:\Windows\System32\runas.exe /savecred /user:pablo_viroulaud@yahoo.com.ar C:\RepisitorioGitHUB\magicDeploy\magicDeploy\bin\Release\net5.0\magicDeploy.exe


            //var pass = new SecureString();
            //pass.AppendChar('A');
            //pass.AppendChar('p');
            //pass.AppendChar('p');
            //pass.AppendChar('_');
            //pass.AppendChar('4');
            //pass.AppendChar('3');
            //pass.AppendChar('2');
            //pass.AppendChar('0');
            //pass.AppendChar('*');
            //pass.AppendChar('3');
            ////Process.Start(ruta, "appweb", pass, "adeaweb");

            //Console.WriteLine("------");
            //Console.ReadLine();
            //Process p = new Process();
            //var psi = new ProcessStartInfo
            //{
            //    Verb="runas",
            //    FileName = "c:/windows/system32/inetsrv/appcmd.exe",
            //    //Arguments= " stop apppool /apppool.name:" + '"' + "CM_API_Localizacion" + '"',
            //    UserName = @"adeaweb\appweb",
            //    //Domain = "adeaweb",
            //    Password = pass,
            //    UseShellExecute = false,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true
            //};
            //p.StartInfo = psi;
            //p.Start();
            //p.WaitForExit();
            //string output = p.StandardOutput.ReadToEnd();
            //Console.WriteLine("------");
            //Console.WriteLine(output);

            Console.ReadLine();



            //antes del proceso -> git config --global http.sslVerify false

            //despues del proceso -> git config --global http.sslVerify true


            ////git ls-remote https://<user>:<pass>@gitlab-clusterdev01.addoc.com/investigaciondesarrollo/pruebasgit.git refs/heads/QA

            //string gitLabUser = "PViroulaud";
            //string gitLagPass = "2letrasy6Numeros";
            //string BRANCH = "QA";

            //string repo = $"https://{gitLabUser}:{gitLagPass}@gitlab-clusterdev01.addoc.com/investigaciondesarrollo/pruebasgit.git";
            //// Start the child process.
            //Process p = new Process();
            //// Redirect the output stream of the child process.
            //p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardOutput = true;
            //p.StartInfo.FileName = "git";
            //p.StartInfo.Arguments = $" ls-remote {repo} refs/heads/{BRANCH}";
            //p.Start();
            //// Do not wait for the child process to exit before
            //// reading to the end of its redirected stream.
            //// C
            //// Read the output stream first and then wait.
            //string output = p.StandardOutput.ReadToEnd();

            //string[] outStr= output.Split('\t');
            //if (outStr.Length==2)
            //{
            //    Console.WriteLine("Ultimo COMMIT sobre rama QA: " + outStr[0]);

            //    var dnfo = Directory.CreateDirectory(Directory.GetCurrentDirectory() + $"\\{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_{BRANCH}_{outStr[0]}");
            //    Directory.SetCurrentDirectory(dnfo.FullName);

            //    Console.WriteLine("CHECKOUT de la rama QA en " + dnfo.FullName);
            //    p = new Process();               
            //    p.StartInfo.UseShellExecute = false;
            //    p.StartInfo.RedirectStandardOutput = true;
            //    p.StartInfo.FileName = "git";
            //    p.StartInfo.Arguments = $"clone --single-branch --branch {BRANCH} {repo}";//git checkout -b <branch>
            //    p.Start();

            //}




            //p.WaitForExit();


        }
    }
}
