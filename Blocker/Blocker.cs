using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using System.Net.Http;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Linq.Expressions;
using System.Web;

namespace Blocker
{
    public partial class Blocker1 : Form
    {
        private readonly Blocker2 logForm;

        private readonly Bar bar;

        public string Send_log2 { get; set; }

        public NotifyIcon NotifyIconV
        {
            get { return notifyIcon1; }
        }

        public const string Send_log = "0";

        public bool CheckRK()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return (rk.GetValueNames().Contains("DyknowBlocker"));
        }

        private async void CheckVersion()
        {
            string versionUrl = "https://raw.githubusercontent.com/s17179XTY/DyknowBlocker/refs/heads/master/version_check";
            string fileUrl = "https://1010.filemail.com/api/file/get?filekey=DG8sz7szDhWZ7NsqanSMlT2Osvx_Cxc_cwZKnkL1WIE66rxDldqBBaJkPA&skipreg=true&pk_vid=886c528a1ec61cc4172947109956cdf4";

            string localVersion = "1.1.5";
            string FilePath = @"C:\Users\s17179\source\repos\DyknowBlocker\Blocker\bin\Debug\README.md";

            using (HttpClient client = new HttpClient())
            {
                string remoteVersion = await client.GetStringAsync(versionUrl);
                remoteVersion = remoteVersion.Trim();
                try
                {
                    if (remoteVersion != localVersion)
                    {
                        MessageBox.Show($"New version detected: {remoteVersion}");
                        bar.Show();
                        this.Visible = false;
                        try
                        {
                            using (HttpResponseMessage response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                            {
                                response.EnsureSuccessStatusCode();

                                long totalBytes = response.Content.Headers.ContentLength ?? -1L;
                                bar.progressBar.Value = 0;
                                bar.progressBar.Maximum = 100;

                                if (totalBytes == -1L)
                                {
                                    MessageBox.Show("The file size is unknown, this might cause progress reporting issues");
                                }

                                try
                                {
                                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(), fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                                    {
                                        byte[] buffer = new byte[8192];
                                        long totalBytesRead = 0;
                                        int bytesRead;

                                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                        {
                                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                                            totalBytesRead += bytesRead;

                                            if (totalBytes != -1)
                                            {
                                                double progress = (double)totalBytesRead / totalBytes * 100;
                                                bar.progressBar.Value = (int)progress;
                                                MessageBox.Show($"Progress: {(int)progress}% ({totalBytesRead} of {totalBytes} bytes");
                                            }
                                        }
                                    }
                                    MessageBox.Show("File downloaded successfully", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bar.Visible = false;
                                    this.Visible = true;
                                    Application.Exit();
                                }
                                catch (HttpRequestException ex)
                                {
                                    MessageBox.Show($"Error fetching version or file: {ex.Message}");
                                    bar.Visible = false;
                                    this.Visible = true;
                                }
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            MessageBox.Show($"Error fetching version or file: {e.Message}");
                            bar.Visible = false;
                            this.Visible = true;
                        }
                        catch (IOException e)
                        {
                            MessageBox.Show($"Error handling file: {e.Message}");
                            bar.Visible = false;
                            this.Visible = true;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show($"Error handling file: {e.Message}");
                            bar.Visible = false;
                            this.Visible = true;
                        }
                    }
                    else
                    {
                        BlockApplications();
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Error fetching version or file: {ex.Message}");
                    bar.Visible = false;
                    this.Visible = true;
                }
            }
        }

        public Blocker1()
        {
            InitializeComponent();
            logForm = new Blocker2(this);
            bar = new Bar();
            if (CheckRK() == true)
            {
                button1.Text = "Start on boot : true";
            }
            else
            {
                button1.Text = "Start on boot : false";
            }
            Task.Run(() => BlockApplications());
            //CheckVersion();
        }



        private bool isContextMenuOpen = false;
        private const int INITIAL_DELAY = 5;
        private const int MAX_DELAY = 10;
        private static int backoffDelay = INITIAL_DELAY;
        private const int MONITOR_INTERVAL = 10;

        private void Exit_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            
        }

        private static List<string> GetBlacklist()
        {
            List<string> fileNames = new List<string>();
            try
            {
                var paths = Directory.GetFiles("C:\\Program Files\\DyKnow\\", "*.exe", SearchOption.AllDirectories);
                foreach (var path in paths)
                {
                    string fileName = Path.GetFileName(path);
                    fileName = Path.GetFileNameWithoutExtension(fileName);
                    if (!fileName.Equals("DyKnow", StringComparison.OrdinalIgnoreCase))
                    {
                        fileNames.Add(fileName);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            if (fileNames.Count == 0)
            {
                Thread.Sleep(1000);
                return null;
            }
            return fileNames;
        }

        private void Log_Load(object sender, EventArgs e)
        {
            
        }

        private void OpenLog_Click(object sender, EventArgs e)
        {
            this.Hide();
            Send_log2 = "1";
            logForm.Show();
        }

        private void BlockApplications()
        {
            if (GetBlacklist() != null)
            {
                while (true)
                {

                    List<string> blacklist = GetBlacklist();

                    foreach (var processName in blacklist)
                    {
                        Process[] processes = Process.GetProcessesByName(processName);

                        foreach (var process in processes)
                        {
                            try
                            {
                                Thread.Sleep(backoffDelay);
                                process.Kill();
                                if (Send_log2 == "1")
                                {
                                    string logMessage = $"Blocked and terminated process '{processName}'";
                                    logForm.LogBox.BeginInvoke((MethodInvoker)(() => logForm.LogBox.AppendText(logMessage + Environment.NewLine)));
                                }
                                else
                                {
                                    logForm.LogBox.Clear();
                                }
                                backoffDelay = INITIAL_DELAY;
                            }
                            catch (Exception ex)
                            {
                                if (Send_log2 == "1")
                                {
                                    string errorMessage = $"Error blocking process '{processName}': {ex.Message}";
                                    logForm.LogBox.BeginInvoke((MethodInvoker)(() => logForm.LogBox.AppendText(errorMessage + Environment.NewLine)));
                                }
                            }
                        }
                        try
                        {
                            Thread.Sleep(MONITOR_INTERVAL);
                            if (backoffDelay < MAX_DELAY)
                            {
                                backoffDelay *= 2;
                            }
                        }
                        catch (ThreadInterruptedException e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    //Thread.Sleep(500);
                }
            }
            else if (GetBlacklist() == null)
            {
                string NullMessage = $"Error finding files";
                Console.WriteLine(NullMessage);
                logForm.LogBox.BeginInvoke((MethodInvoker)(() => logForm.LogBox.AppendText(NullMessage + Environment.NewLine)));
            }
            else
            {
                try
                {
                    logForm.LogBox.Clear();
                    Thread.Sleep(200);
                }
                catch (ThreadInterruptedException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                GC.Collect();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Minimize1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private bool isDragging = false;
        private Point dragStart;

        private void Blocker1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                Capture = true; // Capture the mouse
            }
        }

        private void Blocker1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - dragStart.X, currentPos.Y - dragStart.Y);
            }
        }

        private void Blocker1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false; // Release the mouse capture
            }
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e is MouseEventArgs mouseEventArgs && mouseEventArgs.Button == MouseButtons.Right)
            {
                if (isContextMenuOpen)
                {
                    contextMenuStrip1.Close();
                    isContextMenuOpen = false;
                }
                else
                {
                    contextMenuStrip1.Show(Cursor.Position);
                    isContextMenuOpen = true;
                }
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            isContextMenuOpen = false;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                Capture = true;
            }
        }

        private void Panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - dragStart.X, currentPos.Y - dragStart.Y);
            }
        }

        private void Panel2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false;
            }
        }

        private void ContextMenuStrip1_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void LogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logForm.Show();
        }

        private void Button1_Click_1(object sender, EventArgs e)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (CheckRK() != true)
            {
                button1.Text = "Start on boot : true";
                rk.SetValue("DyknowBlocker", Application.ExecutablePath.ToString());
            }
            else if(CheckRK() == true)
            {
                rk.DeleteValue("DyknowBlocker", false);
                button1.Text = "Start on boot : false";
            }
        }
    }

}
