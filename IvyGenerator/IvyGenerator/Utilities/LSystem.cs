﻿using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace IvyGenerator.Utilities
{
    [Serializable]
    public class RuleSet
    {
        private string axiom;

        public string Axiom
        {
            get { return axiom; }
            set { axiom = value; }
        }

        public List<Rule> Rules = new List<Rule>();
    }

    [Serializable]
    public class Rule 
    {
        private char predecessor = 'A';

        public char Predecessor
        {
            get { return predecessor; }
            set
            {
                predecessor = value;
                //RaisePropertyChanged("Predecessor");
            }
        }
        private string successor = "FF+[+F-F-F]-[-F+F+F]";

        public string Successor
        {
            get { return successor; }
            set
            {
                successor = value;
                //RaisePropertyChanged("Successor");
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

        public LSystem(RuleSet rs)
        {
            Axiom = rs.Axiom;
            current = Axiom;

            foreach(var r in rs.Rules)
                ruleSet.Add(r);
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
