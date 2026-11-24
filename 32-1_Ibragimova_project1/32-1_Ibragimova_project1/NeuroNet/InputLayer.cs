using System;
using System.IO;
using System.Windows.Forms;

namespace _32_1_Ibragimova_project1.NeuroNet
{
    class InputLayer
    {

        //  Поля
        private double[,] trainset;//100 изображений в обучающей выборке
        private double[,] testset;//10 изображений в тестовой выборке

        //Свойства
        public double [,] Trainset { get => trainset; }
        public double [,] Testset { get => testset; }

        public InputLayer(NetworkMode nm)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;  //директория, в которой экзэшник находится
            string[] tmpArrStr; //временный массив строк
            string[] tmpStr; //временный (вспомогательный)массив элементов в строке

            switch (nm)
            {
                case NetworkMode.Train:
                    tmpArrStr = File.ReadAllLines(path + "train.txt");
                    trainset = new double[tmpArrStr.Length, 16];

                    for (int i = 0; i < tmpArrStr.Length; i++)
                    {
                        tmpStr = tmpArrStr[i].Split(' ');

                        for (int j = 0; j < 16; j++)
                        {
                            trainset[i, j] = double.Parse(tmpStr[j]);
                        }
                    }
                    break;
                case NetworkMode.Test:
                    tmpArrStr = File.ReadAllLines(path + "test.txt");
                    Shuffle(tmpArrStr);
                    testset = new double[tmpArrStr.Length, 16];

                    for (int i = 0; i < tmpArrStr.Length; i++)
                    {
                        tmpStr = tmpArrStr[i].Split(' ');

                        for (int j = 0; j < 16; j++)

                        {
                            testset[i, j] = double.Parse(tmpStr[j]);
                        }
                    }
                    break;
            }
        }


        private static readonly Random rng = new Random();
        public void Shuffle(string[] tmpArrStr)
        {   // Вариант с перемешиванием сразу при получении из файла (сейчас не используется)
            if (tmpArrStr == null) throw new ArgumentNullException(nameof(tmpArrStr));

            for (int i = tmpArrStr.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                string temp = tmpArrStr[i];
                tmpArrStr[i] = tmpArrStr[j];
                tmpArrStr[j] = temp;
            }
        }
        public void Shuffle(double[,] tmpArr)
        {   // Вариант с перемешиванием при запуске функции тренировки
            if (tmpArr == null) throw new ArgumentNullException(nameof(tmpArr));
            int row_length = tmpArr.GetLength(0);
            int col_length = tmpArr.GetLength(1);
            for (int i = row_length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                if (j == i) continue;
                for (int l = 0; l < col_length; l++)
                {
                    double temp = tmpArr[i, l];
                    tmpArr[i, l] = tmpArr[j, l];
                    tmpArr[j, l] = temp;
                }
            }
        }
    }
}
