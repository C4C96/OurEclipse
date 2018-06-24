using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Env
    {
        private Hashtable table;
        public Env prev;

        public Env(Env p)
        {
            table = new Hashtable();
            prev = p;
        }

        public void add(string var, string type)
        {
            List<string> val = new List<string>();
            val.Add(type);
            val.Add("");
            table.Add(var, val);
        }

        public bool HasItem(string var)
        {
            return table.Contains(var);
        }

        public List<string> get(string s)
        {
            Env e = this;
            while (e != null)
            {
                List<string> found = (List<string>)e.table[s];
                if (found != null)
                    return found;

                e = e.prev;
            }
            return null;
        }
    }
}
