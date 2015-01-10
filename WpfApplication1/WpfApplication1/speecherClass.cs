using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Recognition;

namespace WpfApplication1
{
    public class Speecher : SpeechRecognizer
    {
        public Dictionary<string, FuncPtr> _funcTab;

        public Speecher(ref Dictionary<string, FuncPtr> funcTab,
                        EventHandler<SpeechRecognizedEventArgs> speechEvent)
        {
            Choices color;
            GrammarBuilder gb;

            color = new Choices();
            color.Add(new string[] { "play", "stop", "pause", "plainécran", "avancerapide", "suivant", "précédent" });
            gb = new GrammarBuilder();
            gb.Append(color);
            this.LoadGrammar(new Grammar(gb));
            this._funcTab = funcTab;
            this.SpeechRecognized += speechEvent;
        }

        public void Invoke(String stringTrigger)
        {
            this._funcTab[stringTrigger].Invoke();
        }
    }
}
