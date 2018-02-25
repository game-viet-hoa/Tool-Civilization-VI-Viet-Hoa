using Ionic.Zip;
using Microsoft.Samples;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Civilization_VI_Việt_Hóa
{
    public partial class Form1 : Form
    {
        RavenClient ravenClient = new RavenClient("https://66399bdc29864a0a8c57f529fd6e31cf:97d7a912bdf7428895d0cab1cf7128ce@sentry.io/288060");

        private string pathGame = "D:\\Game\\Civilization VI";
        private string pathMods = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\My Games\Sid Meier's Civilization VI\Mods\Civilization-VI-Viet-Hoa";
        private string pathVERSION;
        private string versionStepOne = "1.0.0.194";
        private Boolean stepTwo = false;
        private string versionGame = "unknow";
        private string versionVietHoa = "unknow";
        private string versionGameNew = "";
        private string versionVietHoaNew = "";
        private Boolean viethoa = true;
        private string strMaster = "master";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                CheckFolder();

                GetVersionGameAndTool();

                AddButtonPlay();

                UpdateTextVersion();

                CheckUpdateTool();

                CheckWrite();

                var version1 = new Version(versionStepOne);
                var version2 = new Version(versionGame);

                var result = version1.CompareTo(version2);
                if (result < 0) stepTwo = true;

                pathVERSION = stepTwo ? pathMods + "\\VERSION" : pathGame + "\\VERSION";

                if (stepTwo && File.Exists(pathGame + "\\VERSION"))
                    MessageBox.Show("Do một số thay đổi, xin vui lòng khôi phục tiếng Anh rồi tắt bật lại Tool để sử dụng tiếp.\n\nMenu: Việt hóa -> Khôi phục tiếng Anh");
                else
                    CheckVietHoa();
            }
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
            }

        }
        
        private void GetVersionGameAndTool()
        {
            // Lay verion Game
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe");
            versionGame = myFileVersionInfo.FileVersion;
            versionGame = versionGame.Replace(", ", ".");
            versionGame = versionGame.Substring(0, versionGame.IndexOf("(") - 1).Trim();
            if (versionGame.IndexOf(".") < 0)
                versionGame = "unknow";

            // Lay version Moi nhat
            try
            {
                var webClient2 = new WebClient();
                webClient2.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                versionGameNew = webClient2.DownloadString("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/master/VERSIONGAME");

                var version1 = new Version(versionGame);
                var version2 = new Version(versionGameNew);

                var result = version1.CompareTo(version2);
                if (result > 0)
                    versionGameNew = versionGame;
                
                if (versionGameNew == versionGame)
                    strMaster = "master";
                else
                    strMaster = versionGame;
                
                webClient2.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                versionVietHoaNew = webClient2.DownloadString("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/" + strMaster + "/VERSION");
            }
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
                MessageBox.Show("Không thể lấy thông tin phiên bản việt hóa mới nhất!\n\n" + exception.Message, "LỖI");
            }

            if (versionVietHoaNew.IndexOf(".") < 0)
                versionVietHoaNew = "";
            if (versionGameNew.IndexOf(".") < 0)
                versionGameNew = "";

            try
            {
                if (Directory.Exists(pathGame + "\\" + strMaster))
                    Directory.Delete(pathGame + "\\" + strMaster, true);
            }
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
                MessageBox.Show("Không thể xóa thư mục " + pathGame + "\\" + strMaster + "\n\n" + exception.Message, "LỖI");
            }
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
            catch { canWrite = false; }

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

        private bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                    .IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void CheckVietHoa()
        {
            var path = stepTwo ? pathMods + "\\VERSION" : pathGame + "\\VERSION";
            if (File.Exists(path))
            {
                string[] readText = File.ReadAllLines(path);
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

            label5.Visible = false;
        }

        private void UpdateTextVersion()
        {
            var path = stepTwo ? pathMods + "\\VERSION" : pathGame + "\\VERSION";

            // Lay verion Viet Hoa
            if (File.Exists(path + "\\VERSION"))
            {
                string[] readText = File.ReadAllLines(path + "\\VERSION");
                foreach (string s in readText)
                {
                    if (s.IndexOf(".") >= 0)
                    {
                        versionVietHoa = s.Trim();
                        break;
                    }
                }
            }

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
        }

        private void CheckFolder()
        {
            if (File.Exists(Application.StartupPath + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe"))
            {
                pathGame = Application.StartupPath + "\\";
            }
            else if (File.Exists(Application.StartupPath + "\\CivilizationVI.exe"))
            {
                pathGame = Application.StartupPath.Replace("\\Base\\Binaries\\Win64Steam", "\\");
            }
            else if (Application.StartupPath.IndexOf("\\Base") >= 0)
            {
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
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
                MessageBox.Show("Không thể lấy thông tin phiên bản Tool mới nhất!\n\n" + exception.Message, "LỖI");
            }
            
            if ((versionToolNew.IndexOf(".") > 0) && (versionTool != versionToolNew))
            {
                var contantNew = "";

                try
                {
                    var webClient = new WebClient();
                    webClient.Encoding = Encoding.UTF8;
                    webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                    contantNew = webClient.DownloadString("https://github.com/xkvnn/Civilization-VI-Viet-Hoa/releases/download/latest/Civilization.VI.Viet.Hoa.exe");
                }
                catch (Exception exception)
                {
                    ravenClient.Capture(new SentryEvent(exception));
                    MessageBox.Show("Không thể lấy thông tin phiên bản Tool mới nhất!\n\n" + exception.Message, "LỖI");
                }
                
                contantNew = contantNew.Substring(contantNew.IndexOf("table-list-cell commit-avatar-cell"));
                contantNew = contantNew.Substring(0, contantNew.IndexOf("commit-meta commit-author-section"));
                contantNew = contantNew.Substring(contantNew.IndexOf("title=") + 7);
                contantNew = contantNew.Substring(0, contantNew.IndexOf(">") - 1);

                DialogResult dialogResult = MessageBox.Show("Tool có phiên bản mới (" + versionToolNew + "):\n    " + contantNew + "\n\nBạn có muốn tải không?", "Tool Update", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                    System.Diagnostics.Process.Start("https://raw.githubusercontent.com/xkvnn/Civilization-VI-Viet-Hoa/master/Civilization%20VI%20Vi%E1%BB%87t%20H%C3%B3a.exe");

            }
        }

        private void sb_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe";
            processInfo.UseShellExecute = true;
            processInfo.WorkingDirectory = Path.GetDirectoryName(pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI.exe");

            try
            {
                Process proc = Process.Start(processInfo);
                System.Threading.Thread.Sleep(3000);
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
                MessageBox.Show("Không thể khởi động Game.\n\nINFO: " + exception.Message, "Lỗi");
            }
            
        }

        private void sb0_Click(object sender, EventArgs e)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI_DX12.exe";
            processInfo.UseShellExecute = true;
            processInfo.WorkingDirectory = Path.GetDirectoryName(pathGame + "\\Base\\Binaries\\Win64Steam\\CivilizationVI_DX12.exe");

            try
            {
                Process proc = Process.Start(processInfo);
                System.Threading.Thread.Sleep(3000);
                System.Windows.Forms.Application.Exit();
            }
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
                MessageBox.Show("Không thể khởi động Game.\n\nINFO: " + exception.Message, "Lỗi");
            }

        }

        private void xKVNNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.facebook.com/groups/Civilization6VietHoa/");
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hãy hỗ trợ tác giả để luôn có những bản cập nhật mới nhất.");
            System.Diagnostics.Process.Start("https://www.facebook.com/xkvnn");
        }

        private void startDownload()
        {
            Thread thread = new Thread(() =>
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("https://github.com/xkvnn/Civilization-VI-Viet-Hoa/archive/" + strMaster + ".zip"),
                    pathGame + "\\" + strMaster + ".zip");
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
                if (File.Exists(pathGame + "\\" + strMaster + ".zip"))
                {
                    ExtractFileToDirectory(pathGame + "\\" + strMaster + ".zip", pathGame + "\\" + strMaster);
                }
                if (viethoa == true)
                {
                    var SourcePath = pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Vietnamese";
                    if (Directory.Exists(SourcePath))
                    {
                        CopyAllFileInZip(SourcePath, viethoa);

                        File.Copy(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\VERSION", pathVERSION, true);
                        label5.Text = "Việt hóa thành công!";
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy phiên bản Game hoặc file tải về bị lỗi.");
                    }
                }
                else
                {
                    if (stepTwo)
                    {
                        try
                        {
                            if (Directory.Exists(pathMods))
                                Directory.Delete(pathMods, true);
                        }
                        catch (Exception exception)
                        {
                            ravenClient.Capture(new SentryEvent(exception));
                        }

                        label5.Visible = true;
                        label5.Text = "Khôi phục thành công!";
                        UpdateTextVersion();

                        return;
                    }

                    if (File.Exists(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip"))
                    {
                        ExtractFileToDirectory(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip", pathGame + "\\" + strMaster + "\\English");
                    }

                    var SourcePath = pathGame + "\\" + strMaster + "\\English";
                    if (Directory.Exists(SourcePath))
                    {
                        CopyAllFileInZip(SourcePath, viethoa);

                        File.Delete(pathVERSION);

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
                catch (Exception exception)
                {
                    ravenClient.Capture(new SentryEvent(exception));
                }
            });
        }
        public void ExtractFileToDirectory(string zipFileName, string outputDirectory)
        {
            try
            {

                ZipFile zip = ZipFile.Read(zipFileName);
                Directory.CreateDirectory(outputDirectory);
                foreach (ZipEntry e in zip)
                {
                    e.Extract(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
            catch (Exception exception)
            {
                ravenClient.Capture(new SentryEvent(exception));
                        
                DialogErrorUnzip dialogErrorUnzip = new DialogErrorUnzip(strMaster);
                DialogResult dr = dialogErrorUnzip.ShowDialog(this);

                try
                {
                    if (File.Exists(pathGame + "\\" + strMaster + ".zip"))
                        File.Delete(pathGame + "\\" + strMaster + ".zip");
                }
                catch (Exception exception2) { 
                    ravenClient.Capture(new SentryEvent(exception2));
                }
            }
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
                                CopyAllFileInZip(SourcePath, true);

                                File.Copy(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\VERSION", pathVERSION, true);

                                try
                                {
                                    if (Directory.Exists(pathGame + "\\" + strMaster))
                                        Directory.Delete(pathGame + "\\" + strMaster, true);
                                }
                                catch (Exception exception)
                                {
                                    ravenClient.Capture(new SentryEvent(exception));
                                }

                                label5.Visible = true;
                                label5.Text = "Việt hóa thành công!";
                                UpdateTextVersion();
                            }
                            else
                            {
                                try
                                {
                                    File.Delete(pathGame + "\\" + strMaster + ".zip");
                                    TaiVietHoa();
                                }
                                catch (Exception exception)
                                {
                                    ravenClient.Capture(new SentryEvent(exception));
                                    MessageBox.Show("Hãy xóa file " + strMaster + ".zip");
                                }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    ravenClient.Capture(new SentryEvent(exception));
                }
            } else
            {
                viethoa = true;
                label5.Visible = true;
                label5.Text = "Đang tải";
                startDownload();
            }
        }

        private void khôiPhụcTiếngAnhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stepTwo)
            {
                try
                {
                    if (Directory.Exists(pathMods))
                        Directory.Delete(pathMods, true);
                }
                catch (Exception exception)
                {
                    ravenClient.Capture(new SentryEvent(exception));
                }

                label5.Visible = true;
                label5.Text = "Khôi phục thành công!";
                UpdateTextVersion();

                return;
            }

            if (File.Exists(pathGame + "\\" + strMaster + ".zip"))
            {
                try
                {
                    ExtractFileToDirectory(pathGame + "\\" + strMaster + ".zip", pathGame + "\\" + strMaster);
                    if (File.Exists(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip"))
                    {
                        ExtractFileToDirectory(pathGame + "\\" + strMaster + "\\Civilization-VI-Viet-Hoa-" + strMaster + "\\Original\\" + versionGame + ".zip", pathGame + "\\" + strMaster + "\\English");
                        var SourcePath = pathGame + "\\" + strMaster + "\\English";
                        CopyAllFileInZip(SourcePath, false);

                        File.Delete(pathVERSION);

                        try
                        {
                            if (Directory.Exists(pathMods))
                                Directory.Delete(pathMods, true);
                        }
                        catch (Exception exception)
                        {
                            ravenClient.Capture(new SentryEvent(exception));
                            Console.WriteLine(exception.Message);
                        }

                        try
                        {
                            if (Directory.Exists(pathGame + "\\" + strMaster))
                                Directory.Delete(pathGame + "\\" + strMaster, true);
                        }
                        catch (Exception exception) {
                            ravenClient.Capture(new SentryEvent(exception));
                            Console.WriteLine(exception.Message);
                        }

                        label5.Visible = true;
                        label5.Text = "Khôi phục thành công!";
                        UpdateTextVersion();
                    }
                }
                catch (Exception exception){
                    ravenClient.Capture(new SentryEvent(exception));
                    Console.WriteLine(exception.Message);
                }
            } else
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

        private void CopyAllFileInZip(String SourcePath, Boolean vh)
        {
            var pathPaste = vh == true && stepTwo == true ? pathMods : pathGame;

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, pathPaste));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, pathPaste), true);
        }
    }
}
