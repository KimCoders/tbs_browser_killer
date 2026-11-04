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
            MessageBox.Show("tbs_browser.exe를 선택 하세요!");

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "tbs_browser.exe 선택";
                ofd.Filter = "실행 파일 (*.exe)|*.exe";
                ofd.FileName = "tbs_browser.exe";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string exePath = ofd.FileName;
                    BlockProgram(exePath, "Block_TBS_Out", true);
                    BlockProgram(exePath, "Block_TBS_In", false);
                    MessageBox.Show("방화벽 차단 규칙이 설정되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BlockProgram(string programPath, string ruleName, bool outbound)
        {
            string direction = outbound ? "out" : "in";
            string arguments = $"advfirewall firewall add rule name=\"{ruleName}\" dir={direction} action=block program=\"{programPath}\" enable=yes";

            ProcessStartInfo psi = new ProcessStartInfo("netsh", arguments)
            {
                Verb = "runas", // 관리자 권한
                CreateNoWindow = true,
                UseShellExecute = true
            };

            try
            {
                Process.Start(psi)?.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
