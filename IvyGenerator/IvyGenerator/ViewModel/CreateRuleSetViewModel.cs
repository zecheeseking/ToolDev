using System.Collections.Generic;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
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

        private BindingList<Rule> rules = new BindingList<Rule>();
        public BindingList<Rule> Rules
        {
            get { return rules; }
            set
            {
                rules = value;
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
                                   Rules.Add(new Rule());
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
                                   Rules.RemoveAt(rules.Count - 1);
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

        public CreateRuleSetViewModel()
        {
        }
    }
}