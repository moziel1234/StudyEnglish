using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudyEnglish
{
    public partial class FullLettersBoard : Form
    {
        public FullLettersBoard()
        {
            InitializeComponent();
        }
        int endGameNum = 20;

        string letterForTest = "";
        int successes = 0;
        int successesCont = 0;
        int faileures = 0;

        public static string RandomLetter()
        {
            Random r = new Random();
            int rInt = r.Next(1, 26);
            return ((char)(rInt + 64)).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            letterForTest = RandomLetter();
        }

        private void EnterImages(object sender, EventArgs e)
        {
            // ((PictureBox)sender).BorderStyle = BorderStyle.FixedSingle;
        }

        private void LeaveImage(object sender, EventArgs e)
        {
           // ((PictureBox)sender).BorderStyle = BorderStyle.None;
        }

        private void pictureBoxEar_Click(object sender, EventArgs e)
        {
            if (radioButtonTest.Checked)
                MakeLetterSound(letterForTest);
        }

        public static byte[] StreamToBytes(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

        private void LetterClick(object sender, EventArgs e)
        {
            string letter = ((Char)(Convert.ToInt32((((PictureBox)sender).Name).Replace("pictureBox", "")) + 64)).ToString();
            if (radioButtonStudy.Checked)
                MakeLetterSound(letter);
            else
            {
                if (letter == letterForTest)
                {
                    successes += 1;
                    successesCont += 1;
                    labelSuccessNum.Text = successes.ToString();
                    labelSucessNumCont.Text = successesCont.ToString();
                    if (successesCont == endGameNum)
                        new FinalForm().ShowDialog();
                    letterForTest = RandomLetter();
                    MakeLetterSound(letterForTest);
                    // add V animation
                }
                else
                {
                    faileures += 1;
                    successesCont = 0;
                    labelFailuresNum.Text = faileures.ToString();
                    labelSucessNumCont.Text = successesCont.ToString();
                    runXAnimation((PictureBox)sender);
                }
            }
        }

        private void runXAnimation(PictureBox p)
        {
            using (Graphics g = p.CreateGraphics())
            {
                Pen pen = new Pen(Color.Red, 10);
                g.DrawLine(pen, 0, 0, p.Width, p.Height);
                g.DrawLine(pen, p.Width, 0, 0, p.Height);
            }
            Thread.Sleep(500);
            p.Refresh();

        }

        private static void MakeLetterSound(string letter)
        {
            UnmanagedMemoryStream sound = (UnmanagedMemoryStream)Properties.Resources.ResourceManager.GetStream(letter);
            MemoryStream ms = new MemoryStream(StreamToBytes(sound));
            WaveStream ws = new WaveFileReader(ms);
            WaveOutEvent output = new WaveOutEvent();

            // output.PlaybackStopped += new EventHandler<StoppedEventArgs>(Media_Ended);
            output.Init(ws);
            output.Play();
        }

        private void radioButtonTest_CheckedChanged(object sender, EventArgs e)
        {
            successes = 0;
            successesCont = 0;
            faileures = 0;

            labelFailuresNum.Text = "0";
            labelSuccessNum.Text = "0";
            labelSucessNumCont.Text = "0";

            letterForTest = RandomLetter();
            MakeLetterSound(letterForTest);
        }
    }
}
