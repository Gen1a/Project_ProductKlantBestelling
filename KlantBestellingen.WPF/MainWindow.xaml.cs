using BusinessLayer.Managers;
using BusinessLayer.Models;
using KlantBestellingen.WPF.Languages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace KlantBestellingen.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Customers _customerWindow = new Customers();
        private Products _productsWindow = new Products();
        private OrderDetail _orderDetailWindow = new OrderDetail();
        private Orders _ordersWindow = new Orders();
        private string placeholder = Translations.Culture.Name == "en-US" ? "Enter customer name..." : "Geef klantnaam in...";

        public MainWindow()
        {
            InitializeComponent();
            // Set input placeholder
            tbKlant.Text = placeholder;
            tbKlant.GotFocus += RemovePlaceholder;
            tbKlant.LostFocus += AddPlaceholder;
            // Subscribe to Closing event
            Closing += MainWindow_Closing;
            _customerWindow.Closing += _Window_Closing;
            _productsWindow.Closing += _Window_Closing;
            _orderDetailWindow.Closing += _Window_Closing;
            _ordersWindow.Closing += _Window_Closing;
            // Hide statusbar information
            _orderDetailWindow.Orders = _ordersWindow;
            _productsWindow.OrderDetailWindow = _orderDetailWindow;
            SetStatusbarVisibility(true);
            DisableButton(BtnNewOrder);
        }

        public OrderDetail OrderDetail
        {
            get => _orderDetailWindow;
            set
            {
                if (_orderDetailWindow == value)
                    return;
                _orderDetailWindow = value;
            }
        }

        private void RemovePlaceholder(object sender, EventArgs e)
        {
            if (tbKlant.Text == placeholder)
            {
                tbKlant.Text = "";
            }
        }

        public void AddPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbKlant.Text))
                tbKlant.Text = placeholder;
        }

        /// <summary>
        /// Interferes the closing of a window. Hides it in the background instead of completely closing it (more efficient).
        /// This can be used for windows which only have 1 single 'view'.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Make sure the event gets handled on the UI-thread
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object o)
            {
                ((Window)sender).Hide();
                return null;
            }, null);
            // Cancel the initial closing event
            e.Cancel = true;
        }

        /// <summary>
        /// Close the application when the Main window gets closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Opens the 'Customers' window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Klanten_Click(object sender, RoutedEventArgs e)
        {
            if (_customerWindow != null)
                _customerWindow.Show();
        }

        /// <summary>
        /// Opens the 'Products' window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Producten_Click(object sender, RoutedEventArgs e)
        {
            if (_productsWindow != null)
                _productsWindow.Show();
        }

        /// <summary>
        /// Opens the 'Orders' window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Bestellingen_Click(object sender, RoutedEventArgs e)
        {
            if (_ordersWindow != null)
            {
                _ordersWindow.MainWindow = this;
                _ordersWindow.Show();
            }
        }

        /// <summary>
        /// Closes the application when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Sluiten_Click(object sender, RoutedEventArgs e)
        {
            if (!(MessageBox.Show(Translations.CloseAppConfirmation, Translations.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes))
            {
                // Cancel delete and return
                e.Handled = true;
                return;
            }
            System.Windows.Application.Current.Shutdown();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            // Indien geen waarde in textbox ingevuld
            if (string.IsNullOrEmpty(tbKlant.Text))
            {
                cbKlanten.ItemsSource = null;
                return;
            }
            var klanten = Controller.KlantManager.HaalOp(k => k.Naam.ToLower().Contains(tbKlant.Text.ToLower()));
            cbKlanten.ItemsSource = klanten;

            // Indien er effectief klanten zijn, selecteer eerste klant in Combobox
            if (klanten.Count > 0)
            {
                cbKlanten.SelectedIndex = 0;    // zero-based
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Indien klant selectie in combobox wijzigt, refresh datagrid
            SetStatusbarVisibility(true);
            Refresh();
        }

        public void Refresh()
        {
            // Indien klant geselecteerd
            if (cbKlanten.SelectedItem != null)
            {
                Klant k = (Klant)cbKlanten.SelectedItem;
                var bestellingen = Controller.BestellingManager.HaalOp(b => b.Klant.KlantId == k.KlantId);
                dgOrderSelection.ItemsSource = bestellingen;
                EnableButton(BtnNewOrder);
            }
            else
            {
                dgOrderSelection.ItemsSource = null;
                DisableButton(BtnNewOrder);
            }
        }

        private void MaakBestelling_Click(object sender, RoutedEventArgs e)
        {
            if (cbKlanten.SelectedIndex >= 0)    // of cbKlanten.SelectedItem != null
            {
                _orderDetailWindow.Klant = (Klant)cbKlanten.SelectedItem;
                _orderDetailWindow.Bestelling = null;
                _orderDetailWindow.MainWindow = this;
                _orderDetailWindow.Show();
            }
        }

        private void Row_Bestelling_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if actual row was double clicked (and not empty space)
            var row = ItemsControl.ContainerFromElement(dgOrderSelection, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null)
            {
                e.Handled = true;
                return;
            }
            else
            {
                Bestelling b = (Bestelling)row.Item;
                Klant k = (Klant)cbKlanten.SelectedItem;
                _orderDetailWindow.Klant = k;
                _orderDetailWindow.Bestelling = b;
                _orderDetailWindow.Betaald = b.Betaald;
                _orderDetailWindow.MainWindow = this;
                _orderDetailWindow.Show();
            }
        }

        private void Row_Bestelling_SingleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgOrderSelection.SelectedItem == null)
            {
                e.Handled = true;
                return;
            }
            else
            {
                try
                {
                    SetStatusbarVisibility(false);
                    Bestelling b = (Bestelling)dgOrderSelection.SelectedItem;
                    DbProduct_BestellingManager pbm = new DbProduct_BestellingManager(ConfigurationManager.ConnectionStrings["HPZBook"].ConnectionString);
                    IReadOnlyList<Product_Bestelling> productBestellingen = pbm.HaalOp(x => x.BestellingId == b.BestellingId);

                    int aantal = 0;
                    foreach (Product_Bestelling p in productBestellingen)
                    {
                        aantal += p.Aantal;
                    }
                    PbAantalProducten.Value = aantal;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                
            } 
        }

        private void SetStatusbarVisibility(bool hide)
        {
            if (hide)
            {
                PbAantalProducten.Visibility = Visibility.Hidden;
                TbStatusInformation.Visibility = Visibility.Hidden;
                TbAantalProducten.Visibility = Visibility.Hidden;
            }
            else
            {
                TbStatusInformation.Visibility = Visibility.Visible;
                PbAantalProducten.Visibility = Visibility.Visible;
                TbAantalProducten.Visibility = Visibility.Visible;
            }
        }

        private void EnableButton(Button btn)
        {
            btn.IsEnabled = true;
            btn.Opacity = 1.0;
            btn.Background = Brushes.DodgerBlue;
            btn.Foreground = Brushes.White;
        }

        private void DisableButton(Button btn)
        {
            btn.IsEnabled = false;
            btn.Background = new SolidColorBrush(Colors.Gray);
            btn.Foreground = new SolidColorBrush(Colors.Black);
            btn.Opacity = 0.5;
        }
    }
}
