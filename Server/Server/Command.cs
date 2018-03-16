using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Command
    {
        /// <summary>
        /// Identifiziert Datenpaket
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// Restliche mitgesendete Daten
        /// </summary>
        public string[] Arguments { get; private set; }

        public Command(string identifier, params string[] arguments)
        {
            Identifier = identifier;
            Arguments = arguments;
        }

        /// <summary>
        /// Wandelt das Command Objekt in einen String um
        /// </summary>
        public override string ToString()
        {
            string str = Identifier;
            foreach(string arg in Arguments)
            {
                str += "[-!-]" + arg;
            }
            return str;
        }

        /// <summary>
        /// Wandelt den String in ein Command Objekt um
        /// </summary>
        public static Command ToCommand(string str)
        {
            string[] splitted = str.Split(new string[] { "[-!-]" }, StringSplitOptions.None);
            string identifier = splitted[0];
            string[] args = new string[splitted.Length - 1];
            for(int i = 1; i < splitted.Length; i++)
            {
                args[i - 1] = splitted[i];
            }
            return new Command(identifier, args);
        }
    }
}
