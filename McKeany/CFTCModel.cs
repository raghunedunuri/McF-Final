using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McKeany
{
    public class CFTCRootSymbol
    {
        public string ExchangeKey { get; set; }
        public string ExchangeSymbol { get; set; }
        public string EsignalSymbol { get; set; }
    }

    public class ComboItem
    {
        public string Name;
        public int Value;
        public ComboItem(string name, int value)
        {
            Name = name; Value = value;
        }
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }
    }
}
