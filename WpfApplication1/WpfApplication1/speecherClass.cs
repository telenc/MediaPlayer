using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace WpfApplication1
{
    class Speecher : SpeechRecognizer
    {
        private Dictionary<string, FuncPtr> funcTab;

        public Speecher(ref Dictionary<string, FuncPtr> funcTab,
                        EventHandler<SpeechRecognizedEventArgs> e)
        {
            Choices color;
            GrammarBuilder gb;

            this.SpeechRecognized += e;
            color = new Choices();
            color.Add(new string[] { "play", "stop", "pause", "plainécran", "avancerapide", "suivant", "précédent" });
            gb = new GrammarBuilder();
            gb.Append(color);
            this.LoadGrammar(new Grammar(gb));
        }

        public void addRecognizedHandlerEvent(EventHandler<SpeechRecognizedEventArgs> e)
        {
            this.SpeechRecognized += e;
        }

        public void Invoke(String stringTrigger)
        {
            this.funcTab[stringTrigger].Invoke();
        }
    }
}
