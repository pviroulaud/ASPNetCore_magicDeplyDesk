using System;
using System.Diagnostics;
using System.IO;

namespace magicDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            //antes del proceso -> git config --global http.sslVerify false

            //despues del proceso -> git config --global http.sslVerify true


            //git ls-remote https://<user>:<pass>@gitlab-clusterdev01.addoc.com/investigaciondesarrollo/pruebasgit.git refs/heads/QA

            string gitLabUser = "PViroulaud";
            string gitLagPass = "2letrasy6Numeros";
            string BRANCH = "QA";

            string repo = $"https://{gitLabUser}:{gitLagPass}@gitlab-clusterdev01.addoc.com/investigaciondesarrollo/pruebasgit.git";
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "git";
            p.StartInfo.Arguments = $" ls-remote {repo} refs/heads/{BRANCH}";
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();

            string[] outStr= output.Split('\t');
            if (outStr.Length==2)
            {
                Console.WriteLine("Ultimo COMMIT sobre rama QA: " + outStr[0]);

                var dnfo = Directory.CreateDirectory(Directory.GetCurrentDirectory() + $"\\{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_{BRANCH}_{outStr[0]}");
                Directory.SetCurrentDirectory(dnfo.FullName);

                Console.WriteLine("CHECKOUT de la rama QA en " + dnfo.FullName);
                p = new Process();               
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "git";
                p.StartInfo.Arguments = $"clone --single-branch --branch {BRANCH} {repo}";//git checkout -b <branch>
                p.Start();

            }


            

            p.WaitForExit();


        }
    }
}
