using static System.Math;

namespace _32_1_Ibragimova_anile.NeuroNet
{
    class Neuron
    {
        // поля
        private NeuronType type; // тип нейрона
        private double[] weights;
        private double[] inputs;
        private double output;
        private double derivative;

        // константы для функции активации
        private double a = 0.01d;

        // свойства
        private double[] Weights { get => weights; set => weights = value; }
        private double[] Inputs { get => inputs; set => inputs = value; }
        public double[] Output { get => output; }
        public double[] Derivative { get => derivative; }

        public Neuron(double[] memoryWeights, NeuronType typeNeuron)
        {
            type = typeNeuron;
            weights = memoryWeights;
        }
        public void Activator(double[] i)
        {
            inputs = i;
            double sum = weights[0];

            for (int j=0; j<inputs.Length;j++)
            {
                sum += inputs[j] * weights[j + 1];
            }

            switch (type)
            {
                case NeuronType.Hidden:
                    output = Logistic(sum);
                    derivative = Loistic_Derivativator(sum);


                case NeuronType.Output:
                    output = Exp(sum);
                    break;
            }
        }

        private double Logistic(double sum)
    }
}
