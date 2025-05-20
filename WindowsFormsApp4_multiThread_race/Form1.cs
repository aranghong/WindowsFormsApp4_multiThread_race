using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp4_multiThread_race
{
    public partial class Form1 : Form
    {
        List<Thread> carThreads = new List<Thread>();
        DateTime startTime;
        object lockObj = new object();
        int finishedCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            finishedCount = 0;

            ProgressBar[] bars = { progressBar1, progressBar2, progressBar3, progressBar4, progressBar5 };
            Label[] labels = { label1, label2, label3, label4, label5 };
            Label[] labels_end = { label6, label7, label8, label9, label10 };

            string[] carNames = { "Car A", "Car B", "Car C", "Car D", "Car E" };

            // 초기화
            for (int i = 0; i < 5; i++)
            {
                bars[i].Value = 0;
                labels[i].Text = $"{carNames[i]}";
            }

            startTime = DateTime.Now;

            // 스레드 시작
            for (int i = 0; i < 5; i++)
            {
                int index = i; // 클로저 문제 방지
                Thread t = new Thread(() => RunCar(index, carNames[index], bars[index], labels_end[index]));
                t.IsBackground = true;
                carThreads.Add(t);
                t.Start();
            }
        }
        private void RunCar(int index, string carName, ProgressBar bar, Label label)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            int distance = 0;

            while (distance < 100)
            {
                int step = rand.Next(1, 6); // 1~5씩 증가
                distance = Math.Min(distance + step, 100);
                Thread.Sleep(rand.Next(100, 300)); // 0.1~0.3초 간격

                this.Invoke((MethodInvoker)(() =>
                {
                    bar.Value = distance;
                    label.Text = $"진행 중... {distance}%";
                }));
            }

            TimeSpan timeTaken = DateTime.Now - startTime;

            this.Invoke((MethodInvoker)(() =>
            {
                label.Text = $"{carName} 도착! 시간: {timeTaken.TotalSeconds:F2}초";
                finishedCount++;
                textBox1.Text += $"{carName} 도착!\r\n";

                if (finishedCount == 5)
                {
                    textBox1.Text += "\r\n---------------\r\n";
                    textBox1.Text += "모든 차량 도착! \r\n레이스 종료";
                }
            }));

            
        }
    }
}
