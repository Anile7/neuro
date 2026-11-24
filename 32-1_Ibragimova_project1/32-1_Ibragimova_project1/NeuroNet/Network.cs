using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _32_1_Ibragimova_project1.NeuroNet
{
    class Network
    {
        // все слои сети
        private InputLayer input_layer = null;
        private HiddenLayer hidden_layer1 = new HiddenLayer(70, 15, NeuronType.Hidden, nameof(hidden_layer1));
        private HiddenLayer hidden_layer2 = new HiddenLayer(37, 70, NeuronType.Hidden, nameof(hidden_layer2));
        private OutputLayer output_layer = new OutputLayer(10, 37, NeuronType.Output, nameof(output_layer));

        private double[] fact = new double[10];  // массив фактического выхода
        private double[] e_error_avr;            // среднее значение энергии ошибки эпохи обучения

        // свойства
        public double[] Fact { get => fact; }    // массив фактического выхода сети

        // среднее значение энергии ошибки эпохи обучения
        public double[] E_Error_Avr 
        {
            get => e_error_avr;
            set=> e_error_avr=value;
        }

        public Network() { }

        public void ForwardPass(double[] netInput)
        {
            hidden_layer1.Data=netInput;
            hidden_layer1.Recognize(null,hidden_layer2);
            hidden_layer2.Recognize(null,output_layer);

            output_layer.Recognize(this, null);

        }

        public void Train(Network net)
        {
            input_layer = new InputLayer(NetworkMode.Train);

            int epoches = 100;
            double tmpSumError;
            double[] errors;
            double[] tmp_gsums1;
            double[] tmp_gsums2;

            e_error_avr = new double[epoches];

            for (int k=0;k<epoches; k++)
            {
                e_error_avr[k] = 0;
                input_layer.Shuffle(input_layer.Trainset);

                for (int i=0; i<input_layer.Trainset.GetLength(0);i++)
                {
                    double[] tmpTrain = new double[15];
                    for (int j=0;j<tmpTrain.Length;j++)
                    {
                        tmpTrain[j] = input_layer.Trainset[i, j+1];
                    }

                    ForwardPass(tmpTrain);

                    tmpSumError = 0;
                    errors = new double[fact.Length];
                    for (int x=0;x<errors.Length;x++)
                    {
                        if (x == input_layer.Trainset[i, 0]) errors[x] = 1.0 - fact[x];
                        else errors[x] = -fact[x];

                        tmpSumError += errors[x] * errors[x] / 2;
                    }

                    e_error_avr[k] += tmpSumError / errors.Length;

                    tmp_gsums2 = output_layer.BackwardPass(errors);
                    tmp_gsums1 = hidden_layer2.BackwardPass(tmp_gsums2);
                    hidden_layer1.BackwardPass(tmp_gsums1);
                }

                e_error_avr[k] /= input_layer.Trainset.GetLength(0);
            }

            input_layer = null;
/*
            hidden_layer1.WeightInitialize(MemoryMode.SET, memoryPath+ "hidden_layer1_memory.csv");
            hidden_layer2.WeightInitialize(MemoryMode.SET, memoryPath + "hidden_layer2_memory.csv");
            output_layer.WeightInitialize(MemoryMode.SET, memoryPath + "output_layer_memory.csv");
*/
            hidden_layer1.WeightInitialize(MemoryMode.SET, "memory\\" + nameof(hidden_layer1) + "_memory.csv");
            hidden_layer2.WeightInitialize(MemoryMode.SET, "memory\\" + nameof(hidden_layer2) + "_memory.csv");
            output_layer.WeightInitialize(MemoryMode.SET, "memory\\" + nameof(output_layer) + "_memory.csv");
        }



    }


}
