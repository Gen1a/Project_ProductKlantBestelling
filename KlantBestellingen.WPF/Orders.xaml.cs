using BusinessLayer.Models;
using KlantBestellingen.WPF.Languages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    /// Interaction logic for Orders.xaml
    /// </summary>
    public partial class Orders : Window
    {
        private ObservableCollection<Bestelling> _bestellingen;

        private MainWindow _mainWindow;
        public MainWindow MainWindow
        {
            get => _mainWindow;
            set
            {
                if (_mainWindow == value)
                    return;
                _mainWindow = value;
            }
        }

        public Orders()
        {
            InitializeComponent();
            _bestellingen = new ObservableCollection<Bestelling>(Controller.BestellingManager.HaalOp());
            // Assign collection to datagrid
            DgBestellingen.ItemsSource = _bestellingen;
            // Fire event when ObservableCollection changes
            _bestellingen.CollectionChanged += _bestellingen_CollectionChanged;
        }

        /// <summary>
        /// Notify BusinessLayer that a record has been deleted from the ObservableCollection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _bestellingen_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (Bestelling bestelling in e.OldItems)
                {
                    Controller.BestellingManager.Verwijder(bestelling);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (Bestelling bestelling in e.NewItems)
                {
                    bestelling.BestellingId = Controller.BestellingManager.VoegToeEnGeefId(bestelling);
                }
            }
            _mainWindow.Refresh();
        }

        /// <summary>
        /// Handles pressing the 'delete' key on keyboard when Bestelling selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgBestellingen_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (DataGrid)sender;
            if (Key.Delete == e.Key)
            {
                if (!(MessageBox.Show(Translations.DeleteOrder + "?", Translations.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes))
                {
                    // Cancel delete and return
                    e.Handled = true;
                    return;
                }

                // We moeten een while gebruiken en telkens testen want met foreach treden problemen op omdat de verzameling intussen telkens wijzigt!
                while (grid.SelectedItems.Count > 0)
                {
                    var row = grid.SelectedItems[0];
                    _bestellingen.Remove(row as Bestelling);
                }
            }
        }

        /// <summary>
        /// Handles clicking the 'Delete' button next to a Bestelling.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Bestelling b = (Bestelling)DgBestellingen.SelectedItem;

            // Ask confirmation for deleting a Product
            if (!(MessageBox.Show($"{Translations.DeleteOrder}?", Translations.Confirmation, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes))
            {
                // Cancel delete and return
                e.Handled = true;
                return;
            }
            _bestellingen.Remove(DgBestellingen.SelectedItem as Bestelling);
        }

        private void Row_Bestelling_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Check if actual row was double clicked (and not empty space)
            var row = ItemsControl.ContainerFromElement(DgBestellingen, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null)
            {
                e.Handled = true;
                return;
            }
            else
            {
                Bestelling b = (Bestelling)row.Item;
                Klant k = Controller.KlantManager.HaalOp(b.Klant.KlantId);
                MainWindow.OrderDetail.Klant = k;
                MainWindow.OrderDetail.Bestelling = b;
                MainWindow.OrderDetail.Show();
                MainWindow.OrderDetail.MainWindow = MainWindow;
            }
        }

        public void RefreshBestellingen()
        {
            _bestellingen = new ObservableCollection<Bestelling>(Controller.BestellingManager.HaalOp());
            DgBestellingen.ItemsSource = _bestellingen;
        }
    }
}
