using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using IvyGenerator.Properties;
using IvyGenerator.Utilities;
using IvyGenerator.View;

namespace IvyGenerator.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class CreateRuleSetViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the CreateRuleSetViewModel class.
        /// </summary>

        private string name = "";

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }
        RuleSet ruleSet = new RuleSet();

        public string Axiom
        {
            get { return ruleSet.Axiom; }
            set
            {
                ruleSet.Axiom = value;
                RaisePropertyChanged("Axiom");
            }
        }

        public BindingList<Rule> Rules
        {
            get
            {
                var rules = new BindingList<Rule>(ruleSet.Rules);
                return rules;
            }
            set
            {
                ruleSet.Rules = new List<Rule>(value);
                RaisePropertyChanged("Rules");
            }
        }

        private RelayCommand addRuleCommand;
        public RelayCommand AddRuleCommand
        {
            get
            {
                return addRuleCommand ??
                       (
                           addRuleCommand = new RelayCommand
                           (
                               () =>
                               {
                                   ruleSet.Rules.Add(new Rule());
                                   RaisePropertyChanged("Rules");
                               }
                           )
                       );
            }
        }

        private RelayCommand removeRuleCommand;
        public RelayCommand RemoveRuleCommand
        {
            get
            {
                return removeRuleCommand ??
                       (
                           removeRuleCommand = new RelayCommand
                           (
                               () =>
                               {
                                   ruleSet.Rules.RemoveAt(ruleSet.Rules.Count - 1);
                                   RaisePropertyChanged("Rules");
                               }
                           )
                       );
            }
        }

        private RelayCommand clearRulesCommand;
        public RelayCommand ClearRulesCommand
        {
            get
            {
                return clearRulesCommand ??
                       (
                           clearRulesCommand = new RelayCommand
                           (
                               () =>
                               {
                                   Rules.Clear();
                                   RaisePropertyChanged("Rules");
                               }
                           )
                       );
            }
        }

        private RelayCommand<CreateRuleSetWindow> confirmNewRuleCommand;
        public RelayCommand<CreateRuleSetWindow> ConfirmNewRuleCommand
        {
            get
            {
                return confirmNewRuleCommand ??
                       (
                           confirmNewRuleCommand = new RelayCommand<CreateRuleSetWindow>
                           (
                               (window) =>
                               {
                                   //Serialize L-System to object and save it in resources
                                   if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/"))
                                       Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/");
                                   using (var stream = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "Resources/" + Name + ".LRules"))
                                   {
                                       var formatter = new BinaryFormatter();
                                       formatter.Serialize(stream, ruleSet);
                                   }
                                   //close window
                                   window.Close();
                               }
                           )
                       );
            }
        }

        private RelayCommand<CreateRuleSetWindow> cancelRuleCommand;
        public RelayCommand<CreateRuleSetWindow> CancelRuleCommand
        {
            get
            {
                return cancelRuleCommand ??
                       (
                           cancelRuleCommand = new RelayCommand<CreateRuleSetWindow>
                           (
                               (window) =>
                               {
                                    //Close window
                                    window.Close();
                               }
                           )
                       );
            }
        }

        public CreateRuleSetViewModel()
        {
        }
    }
}