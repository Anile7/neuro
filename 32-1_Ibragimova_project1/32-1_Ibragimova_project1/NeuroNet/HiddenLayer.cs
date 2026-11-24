using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _32_1_Ibragimova_project1.NeuroNet
{
    class HiddenLayer : Layer
    {
        // Конструктор скрытого слоя вызывает конструктор родителя и всё (не нужен дополнительный функционал)
        public HiddenLayer(int non, int nopn, NeuronType nt, string type) : base(non, nopn, nt, type) { }
        // Прямой проход: все нейроны текущего слоя "активированы", значит имеют выходы (отклики), мы их записываем в массив и передает на следующий слой
        public override void Recognize(Network net, Layer nextLayer)
        {   // Получает на вход всю нейросеть (не нужна, передано null), а также следующий слой
            double[] hidden_out = new double[numofneurons];
            for (int i = 0; i < numofneurons; i++)
            {
                hidden_out[i] = neurons[i].Output;
            }
            nextLayer.Data = hidden_out;    //передача выходного сигнала на вход след.слоя
        }
        // Обратный проход
        public override double[] BackwardPass(double[] gr_sums)
        {   // Получаем на вход массив ошибок текущего слоя (становится градиентом при умножении на соответствующие производные)
            // Массив ошибок предыдущего слоя, необходимый для вычисления градиента (возврат функции)
            double[] err_sum = new double[numofprevneurons];
            // Мой блок - отдельно вычислям градиент, а не умножаем каждый раз ошибку текущего нейрона на производную
            for (int i = 0; i < numofneurons; i++)
            {
                gr_sums[i] *= neurons[i].Derivative;
            }
            // Вычисление ошибок для предыдущего слоя
            for (int j = 0; j < numofprevneurons; j++)
            {   // Для каждого нейрона из предыдущего слоя считаем его ошибку
                double sum = 0.0;   // Ошибка (сумма) j+1 нейрона предыдущего слоя (прибавляем 1, т.к. вначале идет порог в массиве весов)
                // Для каждого нейрона текущего слоя берем произведение его градиента и веса j+1 нейрона предыдущего слоя
                for (int k = 0; k < numofneurons; k++)  
                    sum += neurons[k].Weights[j + 1] * gr_sums[k];
                err_sum[j] = sum;
            }
            // После получения ошибок для предыдущего слоя меняем веса для каждого нейрона текущего слоя
            for (int i = 0; i < numofneurons; i++)
                for (int n = 0; n < numofprevneurons + 1; n++)
                {
                    double deltaw;
                    if (n == 0) //Порог: поправка = инерция * предыдущее обучение + скорость обучения * градиент
                        deltaw = momentum * lastdeltaweights[i, 0] + learningrate * gr_sums[i];
                    else        // Веса: поправка =  инерция * предыдущее обучение + скорость обучения * входной сигнал (отклик) * градиент 
                        deltaw = momentum * lastdeltaweights[i, n] +
                            learningrate * neurons[i].Inputs[n - 1] * gr_sums[i]; // Индексы inputs с -1, т.к. тут нет порога
                    //
                    lastdeltaweights[i, n] = deltaw; // Сохраняем текущее значение поправки 
                    neurons[i].Weights[n] += deltaw; // Корректируем веса (оценки) текущего нейрона
                }
            //
            return err_sum;
        }
    }
}
