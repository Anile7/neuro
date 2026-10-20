using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _32_1_Ibragimova_anile.NeuroNet
{
    class Network
    {
        private InputLayer input_layer = null;
        private HiddenLayer hidden_layer1 = new HiddenLayer(70, 15, NeuronType.Hidden, nameof(hidden_layer1));
        private HiddenLayer hidden_layer2 = new HiddenLayer(38, 70, NeuronType.Hidden, nameof(hidden_layer2));
        private HiddenLayer output_layer = new OutputLayer(10, 38, NeuronType.Output, nameof(output_layer));

        private double[] fact = new double[10];
        private double[] e_error_avr;

        public double[] Fact { get => fact; }

        public double[] E_Error_Avr 
        {

        }
    }
}
