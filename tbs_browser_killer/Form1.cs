using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace tbs_browser_killer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ReLoad()
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("실행중인 tbs, 메모리사용량");
            Process[] processList = Process.GetProcessesByName("tbs_browser");

            foreach (var list in processList)
            {
                long mem = list.WorkingSet64 / (1024 * 1024);
                listBox1.Items.Add(list.ProcessName + ", " + mem.ToString() + " MB");
            }

            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.실시간체크박스)
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }

            if (Properties.Settings.Default.런처체크박스)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

            if (Properties.Settings.Default.클라체크박스)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }




            ReLoad();
            if (listBox1.Items.Count == 1)
            {
                label1.Text = "총 " + (listBox1.Items.Count - 1).ToString() + "개 실행중..";
            }
            else
            {
                label1.Text = "총 " + (listBox1.Items.Count - 1).ToString() + "개 실행중..";
            }
        }

        private void ProcessFindAndKill()
        {
            Process[] processList = Process.GetProcessesByName("tbs_browser");

            for (int i = processList.Length - 1; i >= 0; i--)
            {
                processList[i].Kill();
                processList[i].Close();
            }

            if(checkBox1.Checked)
            {
                Process[] processList2 = Process.GetProcessesByName("nikke_launcher");

                for (int i = processList2.Length - 1; i >= 0; i--)
                {
                    processList2[i].Kill();
                    processList2[i].Close();
                }
            }

            if(checkBox2.Checked)
            {
                Process[] processList3 = Process.GetProcessesByName("nikke");

                for (int i = processList3.Length - 1; i >= 0; i--)
                {
                    processList3[i].Kill();
                    processList3[i].Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessFindAndKill();
            Delay(1000);
            ReLoad();

            if (listBox1.Items.Count == 1)
            {
                label1.Text = "총 " + (listBox1.Items.Count - 1).ToString() + "개 실행중..";
            }
            else
            {
                label1.Text = "총 " + (listBox1.Items.Count - 1).ToString() + "개 실행중..";
            }
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        private void 자동차단설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> paths = FindBrowserPaths();
            if (paths.Count == 0)
            {
                MessageBox.Show("설치 경로를 자동으로 찾지 못했습니다. tbs_browser.exe를 직접 선택해 주세요.",
                    "자동 검색 실패", MessageBoxButtons.OK, MessageBoxIcon.Information);
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "tbs_browser.exe 선택";
                    ofd.Filter = "tbs_browser.exe|tbs_browser.exe";
                    ofd.FileName = "tbs_browser.exe";
                    if (ofd.ShowDialog() != DialogResult.OK) return;
                    if (!string.Equals(Path.GetFileName(ofd.FileName), "tbs_browser.exe", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("tbs_browser.exe 파일을 선택해 주세요.");
                        return;
                    }
                    paths.Add(ofd.FileName);
                }
            }

            var failures = new List<string>();
            foreach (string path in paths)
            {
                foreach (bool outbound in new[] { true, false })
                {
                    try { BlockProgram(path, outbound); }
                    catch (Exception ex)
                    {
                        failures.Add(path + " (" + (outbound ? "아웃바운드" : "인바운드") + "): " + ex.Message);
                    }
                }
            }
            if (failures.Count == 0)
                MessageBox.Show("방화벽 차단 규칙이 설정되었습니다.\n\n" + string.Join("\n", paths),
                    "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("일부 또는 모든 규칙을 설정하지 못했습니다.\n\n" + string.Join("\n", failures),
                    "방화벽 설정 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static List<string> FindBrowserPaths()
        {
            var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            const string uninstall = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            foreach (RegistryHive hive in new[] { RegistryHive.CurrentUser, RegistryHive.LocalMachine })
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using (RegistryKey root = RegistryKey.OpenBaseKey(hive, view))
                    using (RegistryKey entries = root.OpenSubKey(uninstall))
                    {
                        if (entries == null) continue;
                        foreach (string name in entries.GetSubKeyNames())
                        {
                            try
                            {
                                using (RegistryKey entry = entries.OpenSubKey(name))
                                {
                                    if (entry == null) continue;
                                    string displayName = entry.GetValue("DisplayName") as string ?? "";
                                    if (name.IndexOf("nikke", StringComparison.OrdinalIgnoreCase) < 0 &&
                                        displayName.IndexOf("nikke", StringComparison.OrdinalIgnoreCase) < 0) continue;
                                    AddBrowserPath(paths, entry.GetValue("GameInstallPath") as string);
                                    AddBrowserPath(paths, entry.GetValue("InstallLocation") as string);
                                }
                            }
                            catch (Exception ex) when (ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is IOException) { }
                        }
                    }
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is IOException || ex is ArgumentException) { }
            }
            string drive = Path.GetPathRoot(Environment.SystemDirectory);
            AddBrowserPath(paths, Path.Combine(drive, "NIKKE"));
            AddBrowserPath(paths, Path.Combine(drive, "NIKKE_GOOGLE"));
            return paths.OrderBy(p => p, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static void AddBrowserPath(HashSet<string> paths, string installPath)
        {
            if (string.IsNullOrWhiteSpace(installPath)) return;
            try
            {
                string root = Environment.ExpandEnvironmentVariables(installPath.Trim().Trim('"'));
                foreach (string relative in new[] { @"Launcher\intl_service\tbs_browser.exe", @"intl_service\tbs_browser.exe" })
                {
                    string candidate = Path.GetFullPath(Path.Combine(root, relative));
                    if (File.Exists(candidate)) paths.Add(candidate);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException || ex is System.Security.SecurityException) { }
        }

        private static void BlockProgram(string programPath, bool outbound)
        {
            string hash;
            using (SHA256 sha = SHA256.Create())
                hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(programPath.ToUpperInvariant()))).Replace("-", "").Substring(0, 16);
            string name = "TBS_Browser_Block_" + hash + (outbound ? "_Out" : "_In");
            // 이 프로그램이 만든 동일 경로의 규칙만 교체하여 중복 생성을 방지한다.
            RunNetsh("advfirewall firewall delete rule name=\"" + name + "\"", false);
            RunNetsh("advfirewall firewall add rule name=\"" + name + "\" dir=" + (outbound ? "out" : "in") +
                " action=block program=\"" + programPath + "\" enable=yes", true);
        }

        private static void RunNetsh(string arguments, bool requireSuccess)
        {
            using (Process process = Process.Start(new ProcessStartInfo("netsh.exe", arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }))
            {
                if (process == null) throw new InvalidOperationException("netsh를 실행할 수 없습니다.");
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (requireSuccess && process.ExitCode != 0)
                    throw new InvalidOperationException((error + " " + output).Trim() + " (종료 코드: " + process.ExitCode + ")");
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
                timer1.Enabled = true;
            else
                timer1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReLoad();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.실시간체크박스 = checkBox3.Checked ? true : false;
            Properties.Settings.Default.런처체크박스 = checkBox1.Checked ? true : false;
            Properties.Settings.Default.클라체크박스 = checkBox2.Checked ? true : false;

            Properties.Settings.Default.Save();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("관리자 권한으로 실행 해야 설정 가능합니다.");

            PriorityForm form = new PriorityForm();
            form.ShowDialog();

            if(form.SelectedValue == "High")
            {
                Process[] processList = Process.GetProcessesByName("nikke");

                if (processList.Length == 0)
                {
                    MessageBox.Show("nikke.exe를 찾을 수 없습니다.");
                    return;
                }

                foreach (Process proc in processList)
                {
                    try
                    {
                        proc.PriorityClass = ProcessPriorityClass.High;
                        MessageBox.Show("우선 순위 \"높음\" 설정 완료.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("오류 발생 : " + ex.Message);
                    }
                }
            }
            else if (form.SelectedValue == "RealTime")
            {
                Process[] processList = Process.GetProcessesByName("nikke");

                if (processList.Length == 0)
                {
                    MessageBox.Show("nikke.exe를 찾을 수 없습니다.");
                    return;
                }

                foreach (Process proc in processList)
                {
                    try
                    {
                        proc.PriorityClass = ProcessPriorityClass.RealTime;
                        MessageBox.Show("우선 순위 \"실시간\" 설정 완료.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("오류 발생 : " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("우선 순위 설정 취소");
            }

        }

        private void 수동설정방법ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Stream stream = typeof(Form1).Assembly.GetManifestResourceStream("tbs_browser_killer.ManualGuide.png"))
            {
                if (stream == null)
                {
                    MessageBox.Show("수동 설정 안내 이미지를 불러올 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                using (Image image = Image.FromStream(stream))
                using (Form guide = new Form())
                using (PictureBox picture = new PictureBox())
                {
                    guide.Text = "방화벽 수동 설정 방법";
                    guide.StartPosition = FormStartPosition.CenterParent;
                    guide.ClientSize = new Size(900, 900);
                    guide.MinimumSize = new Size(500, 500);
                    picture.Dock = DockStyle.Fill;
                    picture.SizeMode = PictureBoxSizeMode.Zoom;
                    picture.Image = image;
                    guide.Controls.Add(picture);
                    guide.ShowDialog(this);
                }
            }
        }
    }
}
