using BusinessLayer.Models;
using BusinessLayer.Factories;
using KlantBestellingen.WPF.Languages;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KlantBestellingen.WPF
{
    /// <summary>
    /// Interaction logic for Producten.xaml
    /// </summary>
    public partial class Products : Window
    {
        private ObservableCollection<Product> _producten;

        public Products()
        {
            InitializeComponent();
            _producten = new ObservableCollection<Product>(Controller.ProductManager.HaalOp());
            // Assign collection to datagrid
            DgProducten.ItemsSource = _producten;
            // Fire event when ObservableCollection changes
            _producten.CollectionChanged += _producten_CollectionChanged;
        }

        private OrderDetail _orderDetailWindow;
        public OrderDetail OrderDetailWindow
        {
            get => _orderDetailWindow;
            set
            {
                if (_orderDetailWindow == value)
                    return;
                _orderDetailWindow = value;
            }
        }

        /// <summary>
        /// Notify BusinessLayer that a record has been deleted from the ObservableCollection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _producten_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Product product in e.OldItems)
                {
                    Controller.ProductManager.Verwijder(product);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Product product in e.NewItems)
                {
                    product.ProductId = Controller.ProductManager.VoegToeEnGeefId(product);
                }
            }
            OrderDetailWindow.RefreshProducts();
        }

        /// <summary>
        /// Handles pressing the 'delete' key on keyboard when Product selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgProducten_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                if (!(MessageBox.Show(Translations.DeleteProduct + "?", Translations.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes))
                {
                    // Cancel delete and return
                    e.Handled = true;
                    return;
                }

                // We moeten een while gebruiken en telkens testen want met foreach treden problemen op omdat de verzameling intussen telkens wijzigt!
                while (grid.SelectedItems.Count > 0)
                {
                    var row = grid.SelectedItems[0];
                    _producten.Remove(row as Product);
                }
            }
        }

        /// <summary>
        /// Handles input (KeyUp) in the TextBox fields.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tb_KeyUp(object sender, KeyEventArgs e)
        {
            // If input not empty enable Voeg Toe button
            if (!string.IsNullOrEmpty(TbProductNaam.Text) && !string.IsNullOrEmpty(TbProductPrijs.Text))
            {
                BtnNieuwProduct.IsEnabled = true;
            }
            else
            {
                BtnNieuwProduct.IsEnabled = false;
            }
        }

        private void BtnNieuwProduct_Click(object sender, RoutedEventArgs e)
        {
            // Validate data
            if (string.IsNullOrEmpty(TbProductNaam?.Text) || string.IsNullOrEmpty(TbProductPrijs?.Text))
            {
                MessageBox.Show(Translations.ProductWarning);
                return;
            }

            // Add new Klant to ObservableCollection
            // _klanten_CollectionChanged makes sure the changes also get pushed to BusinessLayer
            Product product = ProductFactory.MaakNieuwProduct(TbProductNaam.Text, Convert.ToDecimal(TbProductPrijs.Text), 0);
            _producten.Add(product);

            // Empty TextBoxes after adding new Klant
            TbProductNaam.Text = "";
            TbProductPrijs.Text = "";
            BtnNieuwProduct.IsEnabled = false;
        }

        private void AllowNumbers(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Handles clicking the 'Delete' button next to a Product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Product p = (Product)DgProducten.SelectedItem;
            string productNaam = p.Naam;

            // Ask confirmation for deleting a Product
            if (!(MessageBox.Show($"{Translations.DeleteProduct}?", Translations.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes))
            {
                // Cancel delete and return
                e.Handled = true;
                return;
            }
            _producten.Remove(DgProducten.SelectedItem as Product);
        }
    }
}
