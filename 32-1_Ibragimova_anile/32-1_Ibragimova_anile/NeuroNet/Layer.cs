using System;
using System.IO;
using System.Windows.Forms;

namespace _32_1_Ibragimova_anile.NeuroNet
{
    abstract class Layer
    {
        // Поля
        // модификаторы protected стоят для внутрииерархического использования
        protected string name_Layer;    // наименование слоя
        string pathDirWeights;          // путь к каталогу, где находится
        string pathFileWeights;         // путь к файлу синаптических 
        protected int numofneurons;     // число нейронов текущего слоя
        protected int numofprevneurons; // число нейронов предыдущего слоя
        protected const double learningrate = 0.05;  // скорость обучения
        protected const double momentum = 0.5;       // момент инерции
        protected double[,] lastdeltaweights;        // веса предыдущего 
        protected Neuron[] neurons;

        // Свойства
        public double[] Data   // передача входных данных на нейроны
        {
            set
            {
                for (int i=0;i<numofneurons;i++)
                {
                    neurons[i].Activator(value);
                }
            }
        }

        // Конструктор
        protected Layer(int non, int nopn, NeuronType nt, string nm_Layer)
        {

            numofneurons = non;
            numofprevneurons = nopn;
            neurons = new Neuron[non];
            name_Layer = nm_Layer;
            pathDirWeights = AppDomain.CurrentDomain.BaseDirectory + "memory\\";
            pathFileWeights = pathDirWeights + name_Layer + "_memory.csv";

            lastdeltaweights = new double[non, nopn + 1];
            double[,] Weights;

            if (File.Exists(pathFileWeights))
                Weights = WeightInitialize(MemoryMode.GET, pathFileWeights);
            else
            {
                Directory.CreateDirectory(pathDirWeights);
                Weights = WeightInitialize(MemoryMode.INIT, pathFileWeights);
            }

            for (int i=0; i<non; i++)     // цикл формирования нейронов слоя и заполнения ими массива нейронов 
            {
                double[] tmp_weights = new double[nopn + 1];
                for (int j=0;j<nopn+1;j++)
                {
                    tmp_weights[j] = Weights[i, j];
                }
                neurons[i] = new Neuron(tmp_weights, nt);
            }
        }

        // Метод работы с массивом синаптических весов слоя
        public double[,] WeightInitialize(MemoryMode mm, string path)
        {
            char[] delim = new char[] { ';', ' ' };
            string tmpStr;
            string[] tmpStrWeights;
            double[,] weights = new double[numofneurons, numofprevneurons + 1];

            switch (mm)
            {
                case MemoryMode.GET:
                    tmpStrWeights = File.ReadAllLines(path);
                    string[] memory_elemnt;
                    for (int i=0;i<numofneurons;i++)
                    {
                        memory_elemnt = tmpStrWeights[i].Split(delim);
                        for (int j=0;j<numofprevneurons+1;j++)
                        {
                            weights[i,j] = double.Parse(memory_elemnt[j].Replace(',', '.'),
                                System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    break;
                case MemoryMode.SET:
                    tmpStrWeights = new string[numofneurons];

                    if (!File.Exists(path))
                    {
                        MessageBox.Show("!!! В Н И М А Н И Е !!!\nФайл " + name_Layer + "_memory_csv синаптических весов\nНЕ НАЙДЕН!!!" +
                            "\nПосле нажатия кнопки ОК автоматически создастся отсутствующий файл, " +
                            "нейросеть вернётся к исходному состоянию <<новорождённости>>, и потребуется обучать её ЗАНОВО.",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    for (int i=0;i<numofneurons;i++)
                    {
                        tmpStr = neurons[i].Weights[0].ToString();
                        for (int j=0;j<numofprevneurons+1;j++)
                        {
                            tmpStr += delim[0] + neurons[i].Weights[j].ToString();
                        }
                        tmpStrWeights[i] = tmpStr;
                    }
                    File.WriteAllLines(path, tmpStrWeights);

                    break;
                case MemoryMode.INIT:



                    MessageBox.Show("!!! В Н И М А Н И Е !!!\nФайл "+name_Layer + "_memory_csv"+" синаптических весов\nНЕ НАЙДЕН!!!"+
                        "\nПосле нажатия ОК автоматически проинициализируются веса и создастся отсутствующий файл, "+
                        "нейросеть вернётся к исходному состоянию <<новорождённости>>, и потребуется обучать её ЗАНОВО.", 
                        "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    Random random = new Random();

                    double tmpRation;
                    double tmpShift;
                    double[] tmpArr = new double[numofneurons + 1];
                    tmpStrWeights = new string[numofneurons];

                    for (int i=0;i<numofneurons;i++)
                    {
                        for (int j = 0; j < numofprevneurons + 1; j++)
                            tmpArr[j] = 0.02 * random.NextDouble() - 0.01;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
