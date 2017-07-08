using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using IvyGenerator.Properties;
using IvyGenerator.Utilities;
using IvyGenerator.ViewModel;

namespace IvyGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            var formatter = new BinaryFormatter();
            bool initializedTree = false;
            foreach (DictionaryEntry resource in resourceSet)
            {
                var byteStream = resource.Value as byte[];
                if(byteStream == null) continue;
                using(var ms = new MemoryStream(byteStream))
                {
                    var ruleSet = (RuleSet) formatter.Deserialize(ms);
                    MenuItem item = new MenuItem();
                    item.Header = "Create " + resource.Key + "...";
                    item.Click += (sender, e) =>
                    {
                        (PrimaryWindow.DataContext as MainViewModel).DoCommand.Execute(item);
                        (PrimaryWindow.DataContext as MainViewModel).Tree.SetRuleSet(ruleSet);
                    };

                    if (!initializedTree)
                    {
                        (PrimaryWindow.DataContext as MainViewModel).Tree.SetRuleSet(ruleSet);
                    }
                    initializedTree = true;
                    AddMenu.Items.Add(item);
                }
            }
        }
    }
}
