using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using _32_1_Ibragimova_anile.NeuroNet;


namespace _32_1_Ibragimova_anile
{
    public partial class FormMain : Form
    {
        private double[] inputPixels;
        private Network network;

        public FormMain()
        {
            InitializeComponent();

            inputPixels = new double[15];
            network = new Network();
        }

        private void Changing_Color_Button(object sender, EventArgs e)
        {
            if (((Button)sender).BackColor == Color.White)
            {
                ((Button)sender).BackColor = Color.Black;
                inputPixels[((Button)sender).TabIndex] = 1d;
            }
            else
            {
                ((Button)sender).BackColor = Color.White;
                inputPixels[((Button)sender).TabIndex] = 0d;
            }
        }

        // Сохранить в файл ОБУЧАЮЩИЙ пример
        private void SaveTrainSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "train.txt";
            string tmpStr = NecessaryValue.Value.ToString();


            for (int i=0; i<inputPixels.Length; i++)
            {
                tmpStr += " " + inputPixels[i].ToString();
            }
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }

        // Сохранить в файл Тестовый пример
        private void SaveTestSample_Click(object sender, EventArgs e)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "test.txt";
            string tmpStr = NecessaryValue.Value.ToString();

            for (int i = 0; i < inputPixels.Length; i++)
            {
                tmpStr += " " + inputPixels[i].ToString();
            }
            tmpStr += "\n";

            File.AppendAllText(path, tmpStr);
        }


        private void buttonRecognise_Click(object sender, EventArgs e)
        {
            network.ForwardPass(inputPixels);
            Label_Output.Text = network.Fact.ToList().IndexOf(network.Fact.Max()).ToString();
            Label_Probability.Text = (100 * network.Fact.Max()).ToString("0.00") + " %";
        }
    }
}
