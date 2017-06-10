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
                        (PrimaryWindow.DataContext as MainViewModel).Tree.SetRuleSet(ruleSet);
                    };

                    AddMenu.Items.Add(item);
                }
            }

            //var files = Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "/Resources/");
            //foreach (var file in files)
            //{
            //    using (var fs = new MemoryStream(Properties.Resources.Basic))
            //    {
            //        string s = file.Substring(file.LastIndexOf('/') + 1, file.LastIndexOf('.') - file.LastIndexOf('/') - 1);

            //        var formatter = new BinaryFormatter();
            //        var ruleSet = (RuleSet)formatter.Deserialize(fs);

            //        MenuItem item = new MenuItem();
            //        item.Header = "Create " + s + "...";
            //        item.Click += (sender, e) =>
            //        {
            //            (PrimaryWindow.DataContext as MainViewModel).Tree.SetRuleSet(ruleSet);
            //        };

            //        AddMenu.Items.Add(item);
            //    }
            //}
        }
    }
}
