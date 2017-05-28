using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;


namespace IvyGenerator.Utilities
{
    public class Rule : ObservableObject
    {
        private char predecessor = 'A';

        public char Predecessor
        {
            get { return predecessor; }
            set
            {
                predecessor = value;
                RaisePropertyChanged("Predecessor");
            }
        }
        private string successor = "FF+[+F-F-F]-[-F+F+F]";

        public string Successor
        {
            get { return successor; }
            set
            {
                successor = value;
                RaisePropertyChanged("Successor");
            }
        }

        public Rule() { }

        public Rule(char predecessor, string successor)
        {
            this.predecessor = predecessor;
            this.successor = successor;
        }
    }

    public class LSystem
    {
        
        private string Axiom = "fA";
        private string current = "";
        public string Current { get { return current; } }

        private List<Rule> ruleSet = new List<Rule>();

        public LSystem()
        {
            //ruleSet.Add(new Rule('F', "FF+[+F-F-F]-[-F+F+F]"));
            //ruleSet.Add(new Rule('F', "FF+[+F-F-F]-[-F+F+F]"));
            //ruleSet.Add(new Rule('F', "F+F-F+F-"));
            //ruleSet.Add(new Rule('A', "f[++Al][--Al]>>>A"));
            //ruleSet.Add(new Rule('A', "^fB>>B>>>>>B"));
            //ruleSet.Add(new Rule('B', "[^^f>>>>>>A]"));

            //A =^ fB >> B >>>>> B
            //B =[^^ f >>>>>> A]

            current = Axiom;
        }

        public void LoadRuleSet()
        {
            
        }

        public void SaveRuleSet()
        {
            
        }

        public void Generate()
        {
            StringBuilder next = new StringBuilder();
            bool replaced = false;

            foreach (char c in current)
            {
                replaced = false;
                foreach (Rule r in ruleSet)
                {
                    if (c == r.Predecessor)
                    {
                        next.Append(r.Successor);
                        replaced = true;
                        break;
                    }
                }

                if(!replaced)
                    next.Append(c);
            }

            current = next.ToString();
        }
    }
}
