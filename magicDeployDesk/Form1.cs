using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace magicDeployDesk
{
    public partial class Form1 : Form
    {
        string archivoConfig = "";
        string archivoConfigTemplate = "";
        Process p;
        List<configDeploy> cfgDep;
        string deploys = "[" +
                "{'nombre':'API 1','configuraciones':[{'nombre':'Nombre','valor':'pepe'},{'nombre':'Apellido','valor':'popo'},{'nombre':'Edad','valor':'40'}]}," +
                "{'nombre':'API 2','configuraciones':[{'nombre':'Nombre','valor':'pepe2'},{'nombre':'Apellido','valor':'popo2'},{'nombre':'Edad','valor':'402'}]}" +
                "]";

        FileSystemWatcher fsw;
        private int indiceLstBoxAnterior = 0;
        private string[] GUIDsConfig;

        delegate void ActualizacionDeDatos();

        public Form1()
        {
            InitializeComponent();
        }

        private void panelPropiedadDefault_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            archivoConfig = AppDomain.CurrentDomain.BaseDirectory + "\\" + "deploysConfig.json";
            archivoConfigTemplate = AppDomain.CurrentDomain.BaseDirectory + "\\" + "deploysConfigTemplate.json";

            cargarConfig();

            CheckForIllegalCrossThreadCalls = false;
            fsw = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "deploysConfig.json");
            fsw.EnableRaisingEvents = true;

            fsw.Changed += Fsw_Changed;

            
        }


        private void cargarConfig()
        {
            lst_Deploy.Items.Clear();

            StreamReader rd;
            if (!File.Exists(archivoConfig))
            {
                rd = new StreamReader(archivoConfigTemplate);

                StreamWriter wr = new StreamWriter(archivoConfig, false);
                wr.Write("[" + rd.ReadToEnd() + "]");
                wr.Close();
                wr.Dispose();


            }
            rd = new StreamReader(archivoConfig);
            deploys = rd.ReadToEnd();

            
            

            rd.Close();
            rd.Dispose();
            armarPanelDePropiedades();
        }

        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            cambioEnArchivoJSON();
        }
        private void cambioEnArchivoJSON()
        {
            if (this.InvokeRequired)
            {
                ActualizacionDeDatos P = new ActualizacionDeDatos(cambioEnArchivoJSON);
                this.Invoke(P);
            }
            else
            {
                cargarConfig();
            }
        }
        private void guardarConfig()
        {
            fsw.EnableRaisingEvents = false;
            if (File.Exists(archivoConfig)) File.Delete(archivoConfig);
            StreamWriter wr = new StreamWriter(archivoConfig, false);
            wr.Write(deploys);
           
            wr.Close();
            wr.Dispose();
            fsw.EnableRaisingEvents = true;
            foreach (var item in panelListaPropiedades.Controls)
            {
                campoValor cmp = (campoValor)item;
                cmp.valorGuardado();
            }
        }
        private void armarPanelDePropiedades()
        {
            deploys= deploys.Replace("'", "\"");
            cfgDep= System.Text.Json.JsonSerializer.Deserialize<List<configDeploy>>(deploys);

            GUIDsConfig = new string[cfgDep.Count];
            int n = 0;
            foreach (var item in cfgDep)
            {                
                lst_Deploy.Items.Add(item.nombre + $"({item.horaEjecucionProgramada.TimeOfDay.ToString()})");
                GUIDsConfig[n] = item.guidProceso;
                n++;
            }

            lst_Deploy.SelectedIndex = 0;
            indiceLstBoxAnterior = lst_Deploy.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            guardar(lst_Deploy.SelectedIndex);
        }

        private void guardar(int indiceLstBox)
        {
            int deployIdx = indiceLstBox;
            configDeploy cDep = cfgDep[deployIdx];

            foreach (var item in cDep.configuraciones)
            {
                var ctrl = panelListaPropiedades.Controls.Find($"campoValor_{item.nombre}", true).FirstOrDefault();
                if (ctrl != null)
                {
                    item.valor = ctrl.Controls["panelPropiedadDefault"].Controls["txt_valorCampo"].Text;
                    //ctrl.Controls["panelPropiedadDefault"].Controls["txt_valorCampo"].Text = "hola";
                }
            }

            deploys = System.Text.Json.JsonSerializer.Serialize(cfgDep);
            guardarConfig();
        }
        private bool cambiosGuardados()
        {
            foreach (var item in panelListaPropiedades.Controls)
            {
                campoValor cmp = (campoValor)item;
                if (!cmp.Name.Contains("HASH"))
                {
                    if (cmp.valorModificado)
                    {
                        return false;
                    }
                }
                
            }
            return true;
           
        }
        private void lst_Deploy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!cambiosGuardados())
            {
                var dialogo = MessageBox.Show(this, "Hay cambios sin guardar, ¿Desea guardarlos?.", "Configuracion del Deploy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogo == DialogResult.Yes)
                {
                    guardar(indiceLstBoxAnterior);
                }
            }

            panelListaPropiedades.Controls.Clear();
            panelListaPropiedades.RowCount = cfgDep[lst_Deploy.SelectedIndex].configuraciones.Count;
            int n = 0;
            foreach (var item in cfgDep[lst_Deploy.SelectedIndex].configuraciones)
            {
                panelListaPropiedades.Controls.Add(new campoValor(item, item.nombre == "HASH Utlimo commit"), 0, n);

                n++;
            }
            indiceLstBoxAnterior = lst_Deploy.SelectedIndex;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            cargarConfig();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            lst_Deploy.Enabled = false;
            progressBar2.Value = 0;
            progressBar2.Maximum = lst_Deploy.CheckedItems.Count;

            

            foreach (var item in lst_Deploy.CheckedItems)
            {

                int idx=lst_Deploy.Items.IndexOf(item);
                
                configDeploy dep;
                dep = (from cf in cfgDep where item.ToString() == cf.nombre + $"({cf.horaEjecucionProgramada.TimeOfDay.ToString()})" select cf).FirstOrDefault();
                if (dep!=null)
                {
                    procesarDeploy(dep, idx);
                }
                
            }
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            lst_Deploy.Enabled = true;
        }

        private void habilitarSSL()
        {
            p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "git";
            p.StartInfo.Arguments = $"config --global http.sslVerify true";
            p.Start();
        }
        private void deshabilitarSSL()
        {
            p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "git";
            p.StartInfo.Arguments = $"config --global http.sslVerify false";
            p.Start();
        }
        private void procesarDeploy(configDeploy config,int indiceLstBox)
        {
            //lst_Deploy.SelectedIndex = indiceLstBox;
            progressBar2.PerformStep();

            rtbx_log.Clear();
            progressBar2.Refresh();
            fsw.EnableRaisingEvents = false;
            progressBar1.Value = 0;

            string output = "";

            rtbx_log.AppendText($"{DateTime.Now} >>> Iniciando Deploy de: {config.nombre}\n");rtbx_log.ScrollToCaret();
            
            campoConfig cv = (from v in config.configuraciones where v.nombre == "Branch para Deploy" select v).FirstOrDefault()
                ?? new campoConfig() {nombre= "Branch para Deploy",valor="!" };
            string BRANCH = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "URL Repositorio GitLab" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "URL Repositorio GitLab", valor = "!" };
            string REPO = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Usuario GitLab" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Usuario GitLab", valor = "!" };
            string USER = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Password GitLab" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Password GitLab", valor = "!" };
            string PASS = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "HASH Utlimo commit" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "HASH Utlimo commit", valor = "!" };
            string hashCommit = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Carpeta de BackUp" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Carpeta de BackUp", valor = "!" };
            string RUTABACKUP = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Carpeta de destino del Deploy" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Carpeta de destino del Deploy", valor = "!" };
            string DESTINODEPLOY = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Archivo .BAT de proceso de Publish" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Archivo .BAT de proceso de Publish", valor = "!" };
            string BATPUBLICACION = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Carpeta de destino de resultado de proceso Publish" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Carpeta de destino de resultado de proceso Publish", valor = "!" };
            string RUTAPUBLISH = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Archivos a ignorar" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Archivos a ignorar", valor = "!" };
            string ARCHIVOSIGNORAR = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Application Site IIS" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Application Site IIS", valor = "!" };
            string APPSITEIIS = cv.valor;

            cv = (from v in config.configuraciones where v.nombre == "Carpeta de CheckOut" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Carpeta de CheckOut", valor = "!" };
            string CARPETACHECKOUT = cv.valor;
            
            cv = (from v in config.configuraciones where v.nombre == "Sub Carpetas a ignorar" select v).FirstOrDefault()
                ?? new campoConfig() { nombre = "Sub Carpetas a ignorar", valor = "!" };
            string CARPETASIGNORAR = cv.valor;

            if (BRANCH!="!" && REPO != "!" && USER != "!" && PASS != "!" && RUTABACKUP != "!" && DESTINODEPLOY != "!" && BATPUBLICACION != "!"
                && RUTAPUBLISH != "!" && ARCHIVOSIGNORAR != "!" && APPSITEIIS != "!" && CARPETACHECKOUT != "!" && CARPETASIGNORAR != "!")
            {


                deshabilitarSSL();
                progressBar1.Value += 10;rtbx_log.Refresh();

                REPO = REPO.Replace("https://", $"https://{USER}:{PASS}@");

                rtbx_log.AppendText($"{DateTime.Now} >>> Obteniendo ultimo commit...\n");rtbx_log.ScrollToCaret();
                p = new Process();
                // Redirect the output stream of the child process.
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "git";
                p.StartInfo.Arguments = $" ls-remote {REPO} refs/heads/{BRANCH}";
                p.Start();
                output = p.StandardOutput.ReadToEnd();
                p.Close();
                p.Dispose();

                progressBar1.Value += 10; rtbx_log.Refresh();

                string[] outStr = output.Split('\t');
                if (outStr.Length == 2)
                {
                    rtbx_log.AppendText($"{DateTime.Now} >>> Ultimo COMMIT sobre rama {BRANCH}: {outStr[0]}\n");rtbx_log.ScrollToCaret();

                    if (hashCommit != outStr[0])
                    {
                        hashCommit = outStr[0];
                        var dnfo = Directory.CreateDirectory(CARPETACHECKOUT + $"\\{formatearFecha(DateTime.Now)}_{BRANCH}_{outStr[0]}");
                        Directory.SetCurrentDirectory(dnfo.FullName);

                        string nombreCarpetaCheckout = dnfo.FullName+"\\"+REPO.Substring(REPO.LastIndexOf("/"), REPO.Length - REPO.LastIndexOf("/")).Replace("/", "").Replace(".git", "");

                        rtbx_log.AppendText($"{DateTime.Now} >>> Realizando CHECKOUT de la rama {BRANCH} en la carpeta temporal {dnfo.FullName}\n");rtbx_log.ScrollToCaret();

                        p = new Process();
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.FileName = "git";
                        p.StartInfo.Arguments = $"clone --single-branch --branch {BRANCH} {REPO}";//git checkout -b <branch>
                        p.Start();
                        output = p.StandardOutput.ReadToEnd();
                        rtbx_log.AppendText(output+"\n");rtbx_log.ScrollToCaret();
                        p.Close();
                        p.Dispose();

                        progressBar1.Value += 10; rtbx_log.Refresh();

                        habilitarSSL();

                        progressBar1.Value += 10; rtbx_log.Refresh();

                        Directory.SetCurrentDirectory(Directory.GetCurrentDirectory());
                        

                        if (backUpSistemaActual(DESTINODEPLOY, RUTABACKUP))
                        {

                            progressBar1.Value += 10; rtbx_log.Refresh();

                            if (crearPublish(nombreCarpetaCheckout,BATPUBLICACION, RUTAPUBLISH))
                            {
                                progressBar1.Value += 10; rtbx_log.Refresh();

                                if (detenerIIS(APPSITEIIS))
                                {
                                    progressBar1.Value += 10; rtbx_log.Refresh();

                                    if (copiarPublicacion(RUTAPUBLISH, DESTINODEPLOY, ARCHIVOSIGNORAR, CARPETASIGNORAR,true))
                                    {
                                        progressBar1.Value += 10; rtbx_log.Refresh();
                                        dnfo = null;
                                        
                                        eliminarTemporales(RUTAPUBLISH, nombreCarpetaCheckout);
                                        progressBar1.Value += 10; rtbx_log.Refresh();
                                    }
                                }
                                
                            }
                            iniciarIIS(APPSITEIIS);
                            progressBar1.Value += 10; rtbx_log.Refresh();
                        }


                        cv = (from v in config.configuraciones where v.nombre == "HASH Utlimo commit" select v).FirstOrDefault();
                        cv.valor = hashCommit;

                        cfgDep.Find(x => x.nombre == config.nombre).configuraciones.Find(y => y.nombre == "HASH Utlimo commit").valor = hashCommit;

                        deploys = System.Text.Json.JsonSerializer.Serialize(cfgDep);
                        guardarConfig();


                    }
                    else
                    {
                        rtbx_log.AppendText($"{DateTime.Now} >>> El ultimo COMMIT sobre rama {BRANCH} ya fue procesado.\n");rtbx_log.ScrollToCaret();
                    }
                }



                habilitarSSL();
                progressBar1.Value += 10; rtbx_log.Refresh();

            }

            rtbx_log.SaveFile($"{formatearFecha(DateTime.Now)}_logPublicacion_{config.nombre}.txt");
            fsw.EnableRaisingEvents = true;
            progressBar1.Value =0; rtbx_log.Refresh();
        }

        private void eliminarTemporales(string rutaPublish,string rutaCheckout)
        {
            try
            {
                rtbx_log.AppendText($"{DateTime.Now} >>> Eliminando carpeta temporal {rutaPublish}\n");rtbx_log.ScrollToCaret();
                Directory.Delete(rutaPublish, true);
                rtbx_log.AppendText($"{DateTime.Now} >>> Eliminando carpeta temporal {rutaCheckout}\n");rtbx_log.ScrollToCaret();

                Directory.Delete(rutaCheckout, true);
            }
            catch (Exception)
            {
            }
           
        }

        private bool detenerIIS(string sitio)
        {
            rtbx_log.AppendText($"{DateTime.Now} >>> Deteniendo IIS Sitio: {sitio}\n");rtbx_log.ScrollToCaret();
            var serverManager = new ServerManager();
            var appPool = serverManager.ApplicationPools.FirstOrDefault(ap => ap.Name.Equals(sitio));
            if (appPool!=null)
            {
                appPool.Stop();

                while (appPool.State != ObjectState.Stopped) ;

                rtbx_log.AppendText($"{DateTime.Now} >>> Sitio {sitio} detenido.\n"); rtbx_log.ScrollToCaret();
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool iniciarIIS(string sitio)
        {
            rtbx_log.AppendText($"{DateTime.Now} >>> Iniciando IIS Sitio: {sitio}\n");rtbx_log.ScrollToCaret();
            var serverManager = new ServerManager();
            var appPool = serverManager.ApplicationPools.FirstOrDefault(ap => ap.Name.Equals(sitio));
            if (appPool != null)
            {
                appPool.Start();
                while (appPool.State != ObjectState.Started) ;
                //C:\Windows\System32\runas.exe /savecred /user:pablo_viroulaud@yahoo.com.ar C:\RepisitorioGitHUB\magicDeploy\magicDeploy\bin\Release\net5.0\magicDeploy.exe
                rtbx_log.AppendText($"{DateTime.Now} >>> Sitio {sitio} iniciado.\n"); rtbx_log.ScrollToCaret();
                return true;
            }
            else
            {
                return false;
            }
            

        }
        private bool copiarPublicacion(string ruta,string rutaDestino,string archivosIgnorar,string carpetasIgnorar,bool eliminarContenido)
        {
            if (eliminarContenido)
            {
                rtbx_log.AppendText($"{DateTime.Now} >>> Eliminando contenido de la carpeta {rutaDestino}. Archivos que no se eliminan: {archivosIgnorar}. Carpetas que no se eliminan: {carpetasIgnorar}.\n");rtbx_log.ScrollToCaret();

                if (Directory.Exists(rutaDestino))
                {
                    DirectoryDelete(rutaDestino, archivosIgnorar, carpetasIgnorar);
                }
                
            }
            if (!Directory.Exists(rutaDestino)) Directory.CreateDirectory(rutaDestino);

            rtbx_log.AppendText($"{DateTime.Now} >>> Iniciando la copia de la publicacion...\n");rtbx_log.ScrollToCaret();
            //robocopy C:\Users\pviroulaud\Documents\1_IntegracionContinua\publish1 C:\inetpub\API /XF "nocopy.txt" "nocopy2.txt"
            DirectoryCopy(ruta, rutaDestino, true, archivosIgnorar, carpetasIgnorar);
            rtbx_log.AppendText($"{DateTime.Now} >>> Copia de publicacion finalizada ({rutaDestino}).\n");rtbx_log.ScrollToCaret();
            return true;
            
        }

        private bool crearPublish(string carpetaCheckout,string archivoBAT,string rutaDestino)
        {
            

            if (!Directory.Exists(rutaDestino)) Directory.CreateDirectory(rutaDestino);


            rtbx_log.AppendText($"{DateTime.Now} >>> Iniciando publicacion. Ejecutando {archivoBAT}\n");rtbx_log.ScrollToCaret();

            p = new Process();
            
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = carpetaCheckout +"\\"+ archivoBAT;
            p.StartInfo.Arguments = $"{rutaDestino} {carpetaCheckout}";
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            rtbx_log.AppendText(output + "\n");rtbx_log.ScrollToCaret();
            p.Close();
            p.Dispose();
           
            rtbx_log.AppendText($"{DateTime.Now} >>> Publicacion finalizada. Publicada en {rutaDestino}\n");rtbx_log.ScrollToCaret();
            return true;

            /*
             * nombre.bat param1
             * %1=param1
             
@echo off
setlocal 
cls

SET "carpetaDePublicacion=%1"

echo Carpeta de publicacion: %carpetaDePublicacion%

echo * Limpiando carpeta de publicacion
rd /S /Q %carpetaDePublicacion%\Firmas

echo * Creando carpeta de publicacion
md %carpetaDePublicacion%\Firmas

echo Publicando FIRMAS:

echo * Publicando API Documento
rd /S /Q %carpetaDePublicacion%\Firmas\DocumentosAPI
dotnet publish -o %carpetaDePublicacion%\Firmas\DocumentosAPI -c Release ContractManager\Documento.API\Documento.API.csproj

echo * Publicando API Notificaciones
rd /S /Q %carpetaDePublicacion%\Firmas\NotificacionesAPI
dotnet publish -o %carpetaDePublicacion%\Firmas\NotificacionesAPI -c Release ContractManager\Notificaciones.API\Notificaciones.API.csproj


echo * Publicando API Gestion
rd /S /Q %carpetaDePublicacion%\Firmas\GestionAPI
dotnet publish -o %carpetaDePublicacion%\Firmas\GestionAPI -c Release ContractManager\Gestion.API\Gestion.API.csproj 
             
           */
        }


        private bool backUpSistemaActual(string ruta,string rutaDestino)
        {
            rtbx_log.AppendText($"{DateTime.Now} >>> Creando BackUp del sistema ({ruta})\n");rtbx_log.ScrollToCaret();
            if (!Directory.Exists(rutaDestino)) Directory.CreateDirectory(rutaDestino);
            if (Directory.Exists(ruta))
            {
                ZipFile.CreateFromDirectory(ruta, rutaDestino + $"\\{formatearFecha(DateTime.Now)}_BKP.zip", CompressionLevel.Optimal, true);
            }
            

            rtbx_log.AppendText($"{DateTime.Now} >>> BackUp del sistema guardado en {rutaDestino}\n");rtbx_log.ScrollToCaret();
            return true;
        }
        private string formatearFecha(DateTime fecha)
        {
            string ret = fecha.Year.ToString();
            if (fecha.Month < 10) ret += 0;
            ret += fecha.Month.ToString();
            if (fecha.Day < 10) ret += 0;
            ret += fecha.Day.ToString();
            ret += "_";
            if (fecha.Hour < 10) ret += 0;
            ret += fecha.Hour.ToString();
            if (fecha.Minute < 10) ret += 0;
            ret += fecha.Minute.ToString();
            if (fecha.Second < 10) ret += 0;
            ret += fecha.Second.ToString();
            return ret;
        }
        private void DirectoryDelete(string DirName, string ignoreFiles, string ignoreSubDirectories)
        {
            DirectoryContentDelete(DirName, ignoreFiles, ignoreSubDirectories);

            if (Directory.Exists(DirName))
            {
                DirectoryInfo dir = new DirectoryInfo(DirName);
                if (dir.EnumerateFiles().Count() == 0 && dir.EnumerateDirectories().Count() == 0)
                {
                    Directory.Delete(dir.FullName);
                }
            }
            
        }
        private void DirectoryContentDelete(string sourceDirName, string ignoreFiles, string ignoreSubDirectories)
        {
            string[] ignoredFiles = ignoreFiles.Split("|", StringSplitOptions.RemoveEmptyEntries);
            string[] ignoredDirectories = ignoreSubDirectories.Split("|", StringSplitOptions.RemoveEmptyEntries);
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!ignoredFiles.Contains(file.Name))
                {
                    File.Delete(file.FullName);                    
                }

            }

            foreach (DirectoryInfo subdir in dirs)
            {
                if (!ignoredDirectories.Contains(subdir.Name))
                {
                    DirectoryContentDelete(subdir.FullName, ignoreFiles, ignoreSubDirectories);
                    if (subdir.EnumerateFiles().Count()==0 && subdir.EnumerateDirectories().Count()==0)
                    {
                        Directory.Delete(subdir.FullName);
                    }
                }

            }
            
        }


        private void DirectoryCopy(string sourceDirName, string destDirName,
                                    bool copySubDirs,string ignoreFiles,string ignoreSubDirectories)
        {
            string[] ignoredFiles = ignoreFiles.Split("|", StringSplitOptions.RemoveEmptyEntries);
            string[] ignoredDirectories = ignoreSubDirectories.Split("|", StringSplitOptions.RemoveEmptyEntries);
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                if (!ignoredFiles.Contains(file.Name))
                {
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, false);
                }
               
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    if (!ignoredDirectories.Contains(subdir.Name))
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs, ignoreFiles,ignoreSubDirectories);
                    }
                   
                }
            }
        }

        private void rtbx_log_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            var dialogo = MessageBox.Show(this, "Si presiona NO la aplicación quedará ejecutandose en segundo plano.", "¿Cerrar MagicDeploy?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogo == DialogResult.Yes)
            {
                return;
            }
            else
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    this.Visible = false;
                    notifyIcon1.ShowBalloonTip(1000, "MagicDeploy", "La aplicacion se esta ejecutando en segundo plano", ToolTipIcon.Info);
                }
            }
            //In case windows is trying to shut down, don't hold the process up
            

            

            //if (this.DialogResult == DialogResult.Cancel)
            //{
            //    // Assume that X has been clicked and act accordingly.
            //    // Confirm user wants to close
            //    switch (MessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //    {
            //        //Stay on this form
            //        case DialogResult.No:
            //            e.Cancel = true;
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
        }

        private void nuevaTareaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamReader rd = new StreamReader(archivoConfigTemplate);
            string ncfg = rd.ReadToEnd();
            rd.Close();
            rd.Dispose();

            configDeploy nuevoCf = System.Text.Json.JsonSerializer.Deserialize<configDeploy>(ncfg);
            nuevoCf.guidProceso = Guid.NewGuid().ToString();
            cfgDep.Add(nuevoCf);

            deploys = System.Text.Json.JsonSerializer.Serialize(cfgDep);
            guardarConfig();

            cargarConfig();
        }

        private void editarTareaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lst_Deploy.SelectedIndex >= 0)
            {
                configDeploy editCf = cfgDep.Find(x => x.guidProceso == GUIDsConfig[lst_Deploy.SelectedIndex]);
                string nombreProceso = editCf.nombre;
                string horaEjecucion = editCf.horaEjecucionProgramada.ToString();
                
            }
        }

        private void eliminarToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            cfgDep.Remove(cfgDep.Find(x => x.guidProceso == GUIDsConfig[lst_Deploy.SelectedIndex]));
            deploys = System.Text.Json.JsonSerializer.Serialize(cfgDep);
            guardarConfig();

            cargarConfig();
        }
    }
}
