using Ionic.Zip;
using Microsoft.Samples;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace The_Long_Dark_Việt_Hóa
{
    public partial class Form1 : Form
    {
        private string pathGame = "E:\\Game\\Sid Meiers Civilization VI";
        private string versionGame = "unknow";
        private string versionVietHoa = "unknow";
        private string versionGameNew = "";
        private string versionVietHoaNew = "";
        private Boolean viethoa = true;
        private string strMaster = "master";
        Boolean canFind = false;
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckFolder();

            // Lay verion Game
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe");
            versionGame = myFileVersionInfo.FileVersion;
            versionGame = versionGame.Replace(", ", ".");
            versionGame = versionGame.Substring(0, versionGame.IndexOf("(") - 1).Trim();
            if (versionGame.IndexOf(".") < 0)
                versionGame = "unknow";

            // Lay verion Viet Hoa
            if (File.Exists(pathGame + "\\VERSION"))
            {
                string[] readText = File.ReadAllLines(pathGame + "\\VERSION");
                foreach (string s in readText)
                {
                    if (s.IndexOf(".") >= 0)
                    {
                        versionVietHoa = s.Trim();
                        break;
                    }
                }
            }

            // Lay version Moi nhat
            try
            {
                var webClient2 = new WebClient();
                webClient2.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                versionGameNew = webClient2.DownloadString("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/master/VERSIONGAME");
                if (versionGameNew == versionGame)
                    strMaster = "master";
                else
                    strMaster = versionGame;
                webClient2.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                versionVietHoaNew = webClient2.DownloadString("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/"+ strMaster + "/VERSION");                                                   
            }
            catch { MessageBox.Show("Lỗi lấy thông tin phiên bản việt hóa mới nhất!"); }
            if (versionVietHoaNew.IndexOf(".") < 0)
                versionVietHoaNew = "";
            if (versionGameNew.IndexOf(".") < 0)
                versionGameNew = "";
            try
            {
                if (Directory.Exists(pathGame + "\\" + strMaster))
                    Directory.Delete(pathGame + "\\" + strMaster, true);
            }
            catch { }

            AddButtonPlay();
            CheckUpdateTool();
            CheckVietHoa();
            CheckWrite();
        }

        private void CheckWrite()
        {
            Boolean canWrite = true;
            try
            {
                System.IO.File.WriteAllText(pathGame + "\\fileTest.txt", "test");
                if (File.Exists(pathGame + "\\fileTest.txt"))
                    File.Delete(pathGame + "\\fileTest.txt");
            }
            catch { canWrite = false;}

            if ((!canWrite) && (IsAdministrator() == false))
            {
                ProcessStartInfo proc = new ProcessStartInfo();
                proc.UseShellExecute = true;
                proc.WorkingDirectory = Environment.CurrentDirectory;
                proc.FileName = Application.ExecutablePath;
                proc.Verb = "runas";
                try
                {
                    Process.Start(proc);
                }
                catch
                {
                    // The user refused the elevation.
                    // Do nothing and return directly ...
                    return;
                }
                Application.Exit();  // Quit itself
            }
            
        }
        public bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }
        private void CheckVietHoa()
        {
            if (File.Exists(pathGame + "\\VERSION"))
            {
                string[] readText = File.ReadAllLines(pathGame + "\\VERSION");
                foreach (string s in readText)
                {
                    if (s.IndexOf(".") >= 0)
                    {
                        var strTemp = s.Trim();
                        if (versionVietHoaNew != strTemp)
                        {
                            DialogResult dialogResult = MessageBox.Show("Việt hóa có cập nhật mới. Có muốn cập nhật luôn không?", "Việt Hóa", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                TaiVietHoa();
                            }
                        }
                    }
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Game chưa được việt hóa. Có muốn việt hóa luôn không?", "Việt Hóa", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    TaiVietHoa();
                }
            }
        }

        private void AddButtonPlay()
        {
            SplitButton sb = new SplitButton();
            sb.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            sb.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            sb.Location = new System.Drawing.Point(146, 48);
            sb.Size = new System.Drawing.Size(124, 47);
            sb.UseVisualStyleBackColor = true;
            sb.Text = "PLAY";
            sb.ShowSplit = true;
            sb.TabStop = false;
            sb.ContextMenuStrip = new ContextMenuStrip();
            sb.ContextMenuStrip.Items.Add("DirectX 12");
            this.Controls.Add(sb);
            sb.Click += new System.EventHandler(this.sb_Click);
            sb.ContextMenuStrip.Items[0].Click += new System.EventHandler(this.sb0_Click);

            if ((versionGameNew == versionGame) && (versionVietHoaNew == versionVietHoa))
            {
                label2.Text = versionGame + " (Đã cập nhật)";
                label4.Text = versionVietHoa + " (Đã cập nhật)";
            }
            else
            {
                var strlb2 = "";
                var strlb4 = "";
                if (versionGame.IndexOf(".") < 0 && versionGameNew.IndexOf(".") < 0)
                { strlb2 = "Không rõ"; }
                else if (versionGame.IndexOf(".") >= 0 && versionGameNew.IndexOf(".") < 0)
                {
                    strlb2 = versionGame + " (Hiện tại)";
                }
                else if (versionGame.IndexOf(".") < 0 && versionGameNew.IndexOf(".") >= 0)
                {
                    strlb2 = versionGameNew + " (Mới nhất)";
                }
                else if (versionGameNew == versionGame)
                {
                    strlb2 = versionGame + " (Đã cập nhật)";
                }
                else
                {
                    strlb2 = versionGame + " (Hiện tại) - " + versionGameNew + " (Mới nhất)";
                }

                if (versionVietHoa.IndexOf(".") < 0 && versionVietHoaNew.IndexOf(".") < 0)
                { strlb4 = "Không rõ"; }
                else if (versionVietHoa.IndexOf(".") >= 0 && versionVietHoaNew.IndexOf(".") < 0)
                {
                    strlb4 = versionVietHoa + " (Hiện tại)";
                }
                else if (versionVietHoa.IndexOf(".") < 0 && versionVietHoaNew.IndexOf(".") >= 0)
                {
                    strlb4 = versionVietHoaNew + " (Mới nhất)";
                }
                else if (versionVietHoa == versionVietHoaNew)
                {
                    strlb4 = versionVietHoa + " (Đã cập nhật)";
                }
                else
                {
                    strlb4 = versionVietHoa + " (Hiện tại) - " + versionVietHoaNew + " (Mới nhất)";
                }
                label2.Text = strlb2;
                label4.Text = strlb4;
            }

            label5.Visible = false;
        }

        private void CheckFolder()
        {       
            if (File.Exists(Application.StartupPath + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe"))
            {
                canFind = true;
                pathGame = Application.StartupPath + "\\";
            }else if (File.Exists(Application.StartupPath + "\\CivilizationVI.exe"))
            {
                canFind = true;
                pathGame = Application.StartupPath.Replace("\\Base\\Binaries\\Win64Steam","\\");
            }
            else if (Application.StartupPath.IndexOf("\\Base")>=0){
                pathGame = Application.StartupPath.Substring(0, Application.StartupPath.IndexOf("\\Base"));
            }
            else
            {
                MessageBox.Show("Hãy để File này vào trong thư mục Game!");
                System.Windows.Forms.Application.Exit();
            }
            
        }

        private void CheckUpdateTool()
        {
            // Kiem tra Version Tool
            FileVersionInfo myFileVersionInfo2 = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
            var versionTool = myFileVersionInfo2.FileVersion;
            this.Text = "Civilization VI Việt Hóa v" + versionTool;
            var versionToolNew = "";
            try
            {
                var webClient = new WebClient();
                webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                versionToolNew = webClient.DownloadString("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/master/VERSIONTOOL");
            }
            catch { MessageBox.Show("Lỗi lấy thông tin phiên bản Tool mới nhất!"); }
            if ((versionToolNew.IndexOf(".") > 0) && (versionTool != versionToolNew))
            {
                var contantNew = "";
                var webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                contantNew = webClient.DownloadString("https://github.com/xkvnn/Civilization-VI-Viet-Hoa/commits/master/Civilization%20VI%20Vi%E1%BB%87t%20H%C3%B3a.exe");

                contantNew = contantNew.Substring(contantNew.IndexOf("table-list-cell commit-avatar-cell"));

                contantNew = contantNew.Substring(0, contantNew.IndexOf("commit-meta commit-author-section"));
                contantNew = contantNew.Substring(contantNew.IndexOf("title=") + 7);
                contantNew = contantNew.Substring(0, contantNew.IndexOf(">") - 1);

                DialogResult dialogResult = MessageBox.Show("Tool có phiên bản mới (" + versionToolNew + "):\n    " + contantNew + "\n\nBạn có muốn tải không?", "Tool Update", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/master/Civilization%20VI%20Vi%E1%BB%87t%20H%C3%B3a.exe");

                }
            }
        }

        private void sb_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe";
            processInfo.ErrorDialog = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.WorkingDirectory = Path.GetDirectoryName(pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe");
            Process proc = Process.Start(processInfo);
            System.Threading.Thread.Sleep(1000);
            System.Windows.Forms.Application.Exit();
        }
        private void sb0_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI_DX12.exe";
            processInfo.ErrorDialog = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.WorkingDirectory = Path.GetDirectoryName(pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI_DX12.exe");
            Process proc = Process.Start(processInfo);
            System.Threading.Thread.Sleep(1000);
            System.Windows.Forms.Application.Exit();       
        }

        private void xKVNNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/groups/1245814878810301/");
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hỗ trợ tác giả mua Game Steam để luôn có những bản cập nhật mới nhất.");
            System.Diagnostics.Process.Start("https://www.facebook.com/xkvnn");
        }

        private void startDownload()
        {
            Thread thread = new Thread(() =>
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("https://github.com/xkvnn/Civilization-VI-Viet-Hoa/archive/"+ strMaster + ".zip"),
                    pathGame + "\\"+strMaster+".zip");
            });
            thread.Start();
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                label5.Text = "Đã tải " + e.BytesReceived / 1024 + " kB";
            });
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                if (File.Exists(pathGame + "\\"+strMaster+".zip"))
                {
                    ExtractFileToDirectory(pathGame + "\\" + strMaster + ".zip", pathGame + "\\" + strMaster);
                }
                if (viethoa == true)
                {
                    if (Directory.Exists(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Vietnamese"))
                    {
                        var SourcePath = pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Vietnamese";
                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(SourcePath, pathGame));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(SourcePath, pathGame), true);
                        File.Copy(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\VERSION", pathGame + "\\VERSION", true);
                        label5.Text = "Việt hóa thành công!";
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy phiên bản Game hoặc file tải về bị lỗi.");
                    }
                }
                else
                {
                    if (File.Exists(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip"))
                    {
                        ExtractFileToDirectory(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip", pathGame + "\\" + strMaster + "\\English");
                    }

                    if (Directory.Exists(pathGame + "\\" + strMaster + "\\English"))
                    {
                        var SourcePath = pathGame + "\\" + strMaster + "\\English";
                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(SourcePath, pathGame));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(SourcePath, pathGame), true);

                        File.Delete(pathGame + "\\VERSION");
                        label5.Text = "Khôi phục thành công!";
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy phiên bản Game hoặc file tải về bị lỗi.");
                    }
                }
                try
                {
                    if (Directory.Exists(pathGame + "\\" + strMaster))
                        Directory.Delete(pathGame + "\\" + strMaster, true);
                }
                catch { }
            });
        }
        public void ExtractFileToDirectory(string zipFileName, string outputDirectory)
        {
            try {
                ZipFile zip = ZipFile.Read(zipFileName);
                Directory.CreateDirectory(outputDirectory);
                foreach (ZipEntry e in zip)
                {
                    // check if you want to extract e or not

                    e.Extract(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch { MessageBox.Show("Lỗi giải nén file!"); }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.fshare.vn/folder/FTMDP5D5CXG5");
        }

        private void tảiViệtHóaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaiVietHoa();
        }
        private void TaiVietHoa()
        {
            Boolean updateXong = false;
            if (File.Exists(pathGame + "\\" + strMaster + ".zip"))
            {
                try
                {
                    ExtractFileToDirectory(pathGame + "\\" + strMaster + ".zip", pathGame + "\\" + strMaster);

                    string[] readText = File.ReadAllLines(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\VERSION");
                    foreach (string s in readText)
                    {
                        if (s.IndexOf(".") >= 0)
                        {
                            var strTemp = s.Trim();
                            if (versionVietHoaNew == strTemp)
                            {
                                var SourcePath = pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Vietnamese";
                                //Now Create all of the directories
                                foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                                    SearchOption.AllDirectories))
                                    Directory.CreateDirectory(dirPath.Replace(SourcePath, pathGame));

                                //Copy all the files & Replaces any files with the same name
                                foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                                    SearchOption.AllDirectories))
                                    File.Copy(newPath, newPath.Replace(SourcePath, pathGame), true);

                                File.Copy(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\VERSION", pathGame + "\\VERSION", true);

                                try
                                {
                                    if (Directory.Exists(pathGame + "\\" + strMaster))
                                        Directory.Delete(pathGame + "\\" + strMaster, true);
                                }
                                catch { }

                                updateXong = true;
                                label5.Visible = true;
                                label5.Text = "Việt hóa thành công!";
                            }
                        }
                    }
                }
                catch { }
            }

            if (!updateXong)
            {
                viethoa = true;
                label5.Visible = true;
                label5.Text = "Đang tải";
                startDownload();
            }
        }
        private void khôiPhụcTiếngAnhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Boolean updateXong = false;
            if (File.Exists(pathGame + "\\" + strMaster + ".zip"))
            {
                try
                {
                    ExtractFileToDirectory(pathGame + "\\" + strMaster + ".zip", pathGame + "\\" + strMaster);
                    if (File.Exists(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip"))
                    {
                        ExtractFileToDirectory(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip", pathGame + "\\" + strMaster + "\\English");
                        var SourcePath = pathGame + "\\" + strMaster + "\\English";
                        //Now Create all of the directories
                        foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                            SearchOption.AllDirectories))
                            Directory.CreateDirectory(dirPath.Replace(SourcePath, pathGame));

                        //Copy all the files & Replaces any files with the same name
                        foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                            SearchOption.AllDirectories))
                            File.Copy(newPath, newPath.Replace(SourcePath, pathGame), true);

                        File.Delete(pathGame + "\\VERSION");

                        try
                        {
                            if (Directory.Exists(pathGame + "\\" + strMaster))
                                Directory.Delete(pathGame + "\\" + strMaster, true);
                        }
                        catch { }

                        updateXong = true;
                        label5.Visible = true;
                        label5.Text = "Khôi phục thành công!";
                    }
                }
                catch { }
            }

            if (!updateXong)
            {
                DialogResult dialogResult = MessageBox.Show("Chắc không?", "Khôi phục tiếng Anh", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    viethoa = false;
                    label5.Visible = true;
                    label5.Text = "Đang tải";
                    startDownload();

                }
            }
        }
    }
}
