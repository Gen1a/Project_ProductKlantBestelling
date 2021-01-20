using BusinessLayer.Models;
using BusinessLayer.Factories;
using BusinessLayer.Managers;
using KlantBestellingen.WPF.Languages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KlantBestellingen.WPF
{
    /// <summary>
    /// Interaction logic for Bestellingen.xaml
    /// </summary>
    public partial class OrderDetail : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private MainWindow _mainWindow;
        public MainWindow MainWindow {
            get => _mainWindow;
            set
            {
                if (_mainWindow == value)
                    return;
                _mainWindow = value;
            }
        }

        private Orders _ordersWindow;
        public Orders Orders
        {
            get => _ordersWindow;
            set
            {
                if (_ordersWindow == value)
                    return;
                _ordersWindow = value;
            }
        }

        // Current customer for the order
        private Klant _klant;
        public Klant Klant
        {
            get => _klant;
            set
            {
                if (_klant == value)
                {
                    return;
                }
                _klant = value;
                NotifyPropertyChanged("KlantNaam"); // If property changes, WPF will query KlantNaam again
                NotifyPropertyChanged("KlantAdres");
            }
        }

        public string KlantNaam => _klant != null ? Klant.Naam : "";
        public string KlantAdres => _klant != null ? Klant.Adres : "";


        private decimal _prijs = 0.0m;
        public decimal Prijs
        {
            get => _prijs;
            set
            {
                if (_prijs == value)
                {
                    return;
                }
                _prijs = value;
                NotifyPropertyChanged("Prijs");
            }
        }

        public decimal Totaalbedrag
        {
            get
            {
                decimal total = 0.0m;
                foreach (Product p in _bestellingProducten)
                {
                    total += p.Prijs;
                }
                return total;
            }
        }

        private bool _betaald = false;
        public bool Betaald
        {
            get => _betaald;

            set
            {
                if (_betaald == value)
                {
                    return;
                }
                _betaald = value;
                NotifyPropertyChanged("Betaald");
            }
        }

        private ObservableCollection<Product> _producten = new ObservableCollection<Product>(); // all products in database
        private ObservableCollection<Product> _bestellingProducten = new ObservableCollection<Product>();   // all products in 'shopping cart'

        private Bestelling _bestelling;
        public Bestelling Bestelling
        {
            get => _bestelling;
            set
            {
                // if _bestelling for OrderDetailWindow is the same AND _bestelling isn't null (= first time window is opened)
                if (_bestelling == value && _bestelling != null)
                {
                    return;
                }
                if (value == null)
                {
                    // Reset the order values of the orderdetail window
                    _bestellingProducten = new ObservableCollection<Product>();
                    DgProducts.ItemsSource = _bestellingProducten;  // possible via .Clear() ?
                    //_bestelling.BestellingId = 0;
                    _bestelling = null;
                    CbPrijs.IsChecked = false;
                    CbProducts.SelectedIndex = 0;
                    // We zeggen tegen XAML WPF: pas je aan aan nieuwe data
                    NotifyPropertyChanged("Bestelling");
                    NotifyPropertyChanged("Totaalbedrag");
                    return;
                }
                _bestelling = value;
                var orders = _bestelling.GeefProducten();
                _bestellingProducten = new ObservableCollection<Product>();
                foreach (KeyValuePair<Product, int> kvp in orders)
                {
                    for (int i = 0; i < kvp.Value; i++)
                    {
                        _bestellingProducten.Add(kvp.Key);
                    }
                }
                DgProducts.ItemsSource = _bestellingProducten;
                NotifyPropertyChanged("Bestelling");
                NotifyPropertyChanged("Totaalbedrag");
            }
        }

        public OrderDetail()
        {
            InitializeComponent();
            DataContext = this;
            _producten = new ObservableCollection<Product>(Controller.ProductManager.HaalOp());
            CbProducts.ItemsSource = _producten;
            CbProducts.SelectedIndex = 0;
            DgProducts.ItemsSource = _bestellingProducten;
        }

        private void BtnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CbProducts.SelectedIndex < 0)
                return;

            _bestellingProducten.Add(CbProducts.SelectedItem as Product);
            NotifyPropertyChanged("Totaalbedrag");
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            //if (CbProducts.SelectedIndex < 0) // not needed as Delete button is not visible without a product...
            //    return;

            _bestellingProducten.Remove(DgProducts.SelectedItem as Product);
            NotifyPropertyChanged("Totaalbedrag");
        }

        private void SlaBestellingOp_Click(object sender, RoutedEventArgs e)
        {
            if (_bestellingProducten.Count == 0) return;
            bool needsUpdate = false;
            int totaalProducten = 0;
            // Check if bestelling already exists
            if (Bestelling != null && Bestelling.BestellingId != 0)
            {
                needsUpdate = true;
            }

            Dictionary<Product, int> bestellingProducten = new Dictionary<Product, int>();
            decimal total = 0.0m;
            foreach (Product p in _bestellingProducten)
            {
                if (bestellingProducten.ContainsKey(p))
                {
                    bestellingProducten[p] += 1;
                }
                else
                {
                    bestellingProducten.Add(p, 1);
                }
                totaalProducten++;
                total += p.Prijs;
            }

            // Create new Bestelling object from the current orderDetail window
            if (needsUpdate)
            {
                _bestelling = BestellingFactory.MaakNieuweBestelling(DateTime.Now, Klant, bestellingProducten, _bestelling.BestellingId);
            }
            else
            {
                _bestelling = BestellingFactory.MaakNieuweBestelling(DateTime.Now, Klant, bestellingProducten);
            }
            _bestelling.Betaald = (bool)CbPrijs.IsChecked;
            _bestelling.PrijsBetaald = total;

            // Save or update Bestelling
            if (!needsUpdate)
                Controller.BestellingManager.VoegToe(_bestelling);
            else
            {
                Controller.BestellingManager.UpdateBestelling(_bestelling);
            }

            if (needsUpdate)
                MessageBox.Show(Translations.UpdateOrder, Translations.Confirmation, MessageBoxButton.OK);
            else
                MessageBox.Show(Translations.SaveOrder, Translations.Confirmation, MessageBoxButton.OK);

            // Refresh Datagrid and Progressbar
            MainWindow.Refresh();
            MainWindow.PbAantalProducten.Value = totaalProducten;
            Orders.RefreshBestellingen();
        }
        

        private void DgProducts_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                if (!(MessageBox.Show(Translations.DeleteConfirmation + "?", Translations.Confirmation, MessageBoxButton.YesNo) == MessageBoxResult.Yes))
                {
                    // Cancel delete and return
                    e.Handled = true;
                    return;
                }

                // We moeten een while gebruiken en telkens testen want met foreach treden problemen op omdat de verzameling intussen telkens wijzigt!
                while (grid.SelectedItems.Count > 0)
                {
                    var row = grid.SelectedItems[0];
                    _bestellingProducten.Remove(row as Product);
                }
            }
        }

        public void RefreshProducts()
        {
            _producten = new ObservableCollection<Product>(Controller.ProductManager.HaalOp());
            CbProducts.ItemsSource = _producten;
        }
    }
}
