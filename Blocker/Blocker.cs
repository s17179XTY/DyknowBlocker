using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using Blocker;

namespace Blocker
{
    public partial class Blocker1 : Form
    {
        private readonly Blocker2 logForm;

        public Blocker1()
        {
            InitializeComponent();
            logForm = new Blocker2(this);
        }

        private static bool pause = false;
        private bool isContextMenuOpen = false;
        private static bool IsServerSocket = false;

        private const int INITIAL_DELAY = 5;
        private const int MAX_DELAY = 10;
        private static int backoffDelay = INITIAL_DELAY;
        private const int MONITOR_INTERVAL = 10;

        private void Exit_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
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
                    //fileNames.Add(fileName);
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
            return fileNames;
        }

        private void Log_Load(object sender, EventArgs e)
        {
            if (IsServerSocket == false)
            {
                try
                {
                    TcpListener serverSocket = new TcpListener(IPAddress.Any, 22222);
                    serverSocket.Start();
                    IsServerSocket = true;

                    if (!serverSocket.Server.IsBound)
                    {
                        Console.Error.WriteLine("Another instance of the application is already running. Exiting...");
                        Environment.Exit(0);
                    }
                }
                catch (IOException)
                {
                    Console.Error.WriteLine("Unable to start the application. Exiting...");
                    Environment.Exit(0);
                }
            }

            Task.Run(() => BlockApplications());

        }

        private void BlockApplications()
        {
            if (!pause)
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
                                string logMessage = $"Blocked and terminated process '{processName}'";
                                Console.WriteLine(logMessage);
                                logForm.LogBox.BeginInvoke((MethodInvoker)(() => logForm.LogBox.AppendText(logMessage + Environment.NewLine)));
                                backoffDelay = INITIAL_DELAY;
                            }
                            catch (Exception ex)
                            {
                                string errorMessage = $"Error blocking process '{processName}': {ex.Message}";
                                Console.WriteLine(errorMessage);
                                //logForm.LogBox.BeginInvoke((MethodInvoker)(() => logForm.LogBox.AppendText(errorMessage + Environment.NewLine)));
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
            else
            {
                try
                {
                    logForm.LogBox.BeginInvoke((MethodInvoker)(() => logForm.LogBox.Text = ""));
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

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            pause = !pause;
            ButtonClick.Text = pause ? "Continue" : "Pause";
        }

        private void OpenLog_Click(object sender, EventArgs e)
        {
            this.Hide();
            logForm.Show();
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

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
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

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
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

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            isContextMenuOpen = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                dragStart = e.Location;
                Capture = true;
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPos = PointToScreen(e.Location);
                Location = new Point(currentPos.X - dragStart.X, currentPos.Y - dragStart.Y);
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Capture = false;
            }
        }

        private void contextMenuStrip1_MouseHover(object sender, EventArgs e)
        {
            
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pause = !pause;
            pauseToolStripMenuItem.Text = pause ? "Continue" : "Pause";
            isContextMenuOpen = false;
        }

        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logForm.Show();
            notifyIcon1.Visible = false;
        }
    }

}
