using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace IvyGenerator.Utilities
{
    public class LSystem
    {
        private class Rule
        {
            private char predecessor;
            public char Predecessor { get { return predecessor; } }
            private string successor;
            public string Successor { get { return successor; } }

            public Rule(char predecessor, string successor)
            {
                this.predecessor = predecessor;
                this.successor = successor;
            }
        }

        private string Axiom = "A";
        private string Alphabet = "AB";
        private string current = "";
        public string Current { get { return current; } }

        private List<Rule> ruleSet = new List<Rule>();

        public LSystem()
        {
            ruleSet.Add(new Rule('A', "AB"));
            ruleSet.Add(new Rule('B', "A"));
            current = Axiom;
        }

        public void Generate()
        {
            StringBuilder next = new StringBuilder();

            foreach (char c in current)
            {
                foreach (Rule r in ruleSet)
                {
                    if (c == r.Predecessor)
                    {
                        next.Append(r.Successor);
                        break;
                    }
                }
            }

            current = next.ToString();
        }
    }
}
