using static System.Math;

namespace _32_1_Ibragimova_project1.NeuroNet
{
    class Neuron
    {
        private NeuronType type;
        private double[] weights;
        private double[] inputs;
        private double output;
        private double derivative;

        private double a = 0.01d;

        public double[] Weights { get => weights; set => weights = value; }
        public double[] Inputs { get => inputs; set => inputs = value; }
        public double Output { get => output;}
        public double Derivative { get => derivative;}

        public Neuron(double[] memoryWeights, NeuronType typeNeuron)
        {
            type = typeNeuron;
            weights = memoryWeights;
        }

        public void Activator(double[] i)
        {
            inputs = i;
            double sum = weights[0];

            for (int j = 0; j < inputs.Length; j++)
            {
                sum += inputs[j] * weights[j+1];
            }

            switch (type)
            {
                case NeuronType.Hidden:
                    output = hyperFunc(sum); //метод в соответствии с инд. заданием
                    derivative = hyperFun_Derivativator(sum);
                    break;

                case NeuronType.Output:
                    output = Exp(sum);
                    break;

            }
        }

        private double hyperFunc(double sum)
        {
            return 1.0 / (1.0 + Exp(-sum));
        }

        private double hyperFun_Derivativator(double sum)
        {
            double s = hyperFunc(sum);
            return s * (1 - s);
        }


    }
}
