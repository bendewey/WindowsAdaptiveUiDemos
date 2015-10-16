using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GameOfThronesMap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public List<Castle> Castles { get; set; }

        private bool? _showBattles;
        public bool? ShowBattles
        {
            get { return _showBattles; }
            set { _showBattles = value; OnPropertyChanged("ShowBattles"); }
        }

        private bool? _showRelationships;
        public bool? ShowRelationships
        {
            get { return _showRelationships; }
            set { _showRelationships = value; OnPropertyChanged("ShowRelationships"); }
        }

        public MainPage()
        {
            ShowBattles = false;
            ShowRelationships = false;
            Castles = new List<Castle> {
                new Castle()
                {
                    Name = "Winterfell",
                    Margin = new Thickness() { Top = 140, Left = 120 },
                    Image = "/Assets/winterfell.jpg",
                    Lord = "Stark"
                },
                new Castle()
                {
                    Name = "Kings Landing",
                    Margin = new Thickness() { Top = 340, Left = 160 },
                    Image = "/Assets/kingslanding.jpg",
                    Lord = "Baratheon"
                }
            };

            this.InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Castle
    {
        public string Image { get; set; }
        public string Lord { get; internal set; }
        public Thickness Margin { get; set; }
        public string Name { get; set; }
    }
    
}
