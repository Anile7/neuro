using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace _32_1_Ibragimova_project1.NeuroNet
{
    abstract class Layer
    {
        //Поля
        protected string name_Layer;                // Наименование слоя, используется для составления пути сохранения файлов
        string pathDirWeights;                      // Путь к папке, где будут храниться веса
        string pathFileWeights;                     // Путь с именем файла хранения синапт.весов
        protected int numofneurons;                 // Число нейронов текущего слоя
        protected int numofprevneurons;             // Число нейронов пред.слоя
        protected const double learningrate = 0.05; // Скорость обучения - константа
        protected const double momentum = 0.5;      // Момент инерции - константа, связана с lastdeltaweights
        protected double[,] lastdeltaweights;       // На сколько изменились веса в предыдущей итерации, размер = кол-во нейронов в этом слое * (кол-во нейронов в предслое + 1)
        protected Neuron[] neurons;                 // Массив нейронов текущего слоя, размер = кол-во нейронов в этом слое
        //Свойства
        public double[] Data
        {   // "Активирует" каждый нейрон в слое - заставляет их обработать выходы предыдущего слоя
            set
            {
                for (int i = 0; i < numofneurons; i++)
                    neurons[i].Activator(value);
            }
        }
        //Конструтор
        public Layer(int non, int nopn, NeuronType nt,string nm_Layer)
        {   // Layer(количество нейронов в слое, количество нейронов в пред.слое, тип нейронов, имя слоя)
            numofneurons = non;
            numofprevneurons = nopn;
            name_Layer = nm_Layer;
            // Инициализируем массив нейронов (выделяем память)
            neurons = new Neuron[non];
            // Получаем путь к файлу синаптических весов
            pathDirWeights=AppDomain.CurrentDomain.BaseDirectory + "memory\\";
            pathFileWeights = pathDirWeights + name_Layer + "_memory.csv";
            // Инициализируем массив изменений синаптических весов (выделяем память)
            lastdeltaweights = new double[non, nopn + 1];
            // Матрица для считывания синаптических весов из файла при запуске
            double[,] Weights;
            // Считываем синаптические веса в матрицу
            if(File.Exists(pathFileWeights))
            {   // Файл есть - считываем готовые веса
                Weights = WeightInitialize(MemoryMode.GET, pathFileWeights);
            }
            else
            {   // Файла нет - инициализируем случайные веса, сохраняем в файл и переменную
                Directory.CreateDirectory(pathDirWeights);
                Weights = WeightInitialize(MemoryMode.INIT, pathFileWeights);
            }
            // Инициализируем все нейроны слоя
            for (int i = 0; i < non; i++) 
            {
                double[] tmp_weight = new double[nopn + 1];
                // Считываем из матрицы порог нейрона и веса (оценки) нейронов предыдущего слоя
                for (int j = 0; j < nopn + 1; j++) 
                {
                    tmp_weight[j] = Weights[i, j];
                }
                neurons[i] = new Neuron(tmp_weight, nt);
            }
        }
        //Методы
        public double[,] WeightInitialize(MemoryMode mm, string path)
        {   // Функция работает с файлами весов: считывает с них (и инициализирует, если их нет), сохраняет в них результаты
            char[] delim = new char[] { ';', ' ' }; // Список разделителей для файлов (csv, txt)
            string tmpStr;          // Переменная для работы со строками
            string[] tmpStrWeights; // Массив для хранения строк из файла для парсинга (GET) или для записи в файл (SET и INIT)
            // Матрица синаптических весов текущего слоя (заполняется в GET и INIT), результат работы функции
            double[,] weights = new double[numofneurons, numofprevneurons + 1];
            // Выполняем нужную часть в зависимости от режима
            switch (mm)
            {
                case MemoryMode.GET:    // Файл уже существует, получаем из него синаптические веса
                    tmpStrWeights = File.ReadAllLines(path);
                    string[] memory_element;
                    for (int i = 0; i < numofneurons; i++)
                    {   // Массив хранит non строк nopn+1 весов
                        memory_element = tmpStrWeights[i].Split(delim);
                        for (int j = 0; j < numofprevneurons + 1; j++) 
                        {
                            weights[i, j] = double.Parse(memory_element[j].Replace(',', '.'),
                                System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    break;
                case MemoryMode.SET:    // Заполняем веса в файле обновленными значениями (если файла нет, то создаем его)
                    tmpStrWeights = new string[numofneurons];

                    if(!File.Exists(path))
                    {
                        MessageBox.Show("!!!ВНИМАНИЕ!!!\nФайл " + name_Layer + "_memory.csv синаптических весов" +
                            "\nНЕ НАЙДЕН!!!\nПосле нажатия кнопки ОК автоматически создается отсутствующий файл и все значения",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    for (int i = 0; i < numofneurons; i++)
                    {   // Создаем (склеиваем) веса (оценки) текущего нейрона в строку для записи в файл
                        tmpStr = neurons[i].Weights[0].ToString();
                        for (int j = 1; j < numofprevneurons + 1; j++)
                            tmpStr += delim[0] + neurons[i].Weights[j].ToString();
                        tmpStrWeights[i] = tmpStr; 
                    }
                    File.WriteAllLines(path, tmpStrWeights);
                    break;
                case MemoryMode.INIT:   // Происходит, если при запуске файл отсутствует, тогда он создается в этой ветке
                    MessageBox.Show("!!!ВНИМАНИЕ!!!\nФайл " + name_Layer + "_memory.csv синаптических весов\nНЕ НАЙДЕН!!!" +
                            "\nПосле нажатия кнопки ОК автоматически инициализируются веса и создастся отсутствующий файл, "+
                            "нейросеть вернется к исходному состоянию <<<новорожденности>>>, и потребуется обучать её ЗАНОВО",
                            "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Random random = new Random();
                    double[] tmpArr = new double[numofprevneurons + 1];
                    tmpStrWeights = new string[numofneurons];
                    //Инициализация исходных значений
                    double tmpRatio;    // Верхний предел случайных значений при генерации = [0, tmpRatio)
                    double tmpShift;    // На сколько надо будет сместить все веса, чтобы в среднем было 0 (считается для каждого нейрона отдельно)
                    tmpRatio = Math.Sqrt(3.0 / Math.Sqrt(numofprevneurons));
                    for (int i = 0; i < numofneurons; i++)
                    {   // Для каждого нейрона инициализируем случайный порог и веса (оценки)
                        double sum = 0.0;
                        for (int j = 0; j < numofprevneurons + 1; j++)
                        {
                            tmpArr[j] = random.NextDouble() * tmpRatio; // [0, 1) * tmpRatio = [0, tmpRatio)
                            sum += tmpArr[j]; // Считаем сумму сгенерированных весов для их последующего изменения
                        }
                        tmpShift = sum / (numofprevneurons + 1);    // Считаем, насколько нужно изменить каждое число, чтобы в среднем было 0
                        // Смещаем отдельно 0 вес, чтобы записать в строку без разделителя
                        tmpArr[0] -= tmpShift;
                        weights[i, 0] = tmpArr[0];
                        tmpStr = tmpArr[0].ToString();
                        for (int j = 1; j < numofprevneurons + 1; j++)
                        {   // Смещаем значения, сохраняем их в матрицу весов и строку для записи в файл
                            tmpArr[j] -= tmpShift;
                            weights[i, j]=tmpArr[j];
                            tmpStr += delim[0] + tmpArr[j].ToString();
                        }
                        tmpStrWeights[i] = tmpStr;  // Сохраняем веса текущего нейрона в массив, сохраняемый в файл
                        //tmpStrWeights[i] = string.Join(";", tmpArr.Select(w => w.ToString(System.Globalization.CultureInfo.InvariantCulture)));
                    }
                    File.WriteAllLines(path, tmpStrWeights);
                    break;
            }
            return weights;
        }
        // Абстрактные методы для наследования
        abstract public void Recognize(Network net, Layer nextLayer);   // Передать выходы на следующий слой (прямой проход)
        abstract public double[] BackwardPass(double[] stuff);          // Получает ошибки текущего слоя подсчитанные на следующем слое, считает ошибки для предыдущего слоя (результат функции) после чего меняет веса текущего слоя
    }
}
