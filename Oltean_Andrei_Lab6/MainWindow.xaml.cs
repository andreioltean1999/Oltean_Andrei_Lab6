using AutoLotModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oltean_Andrei_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource customerOrdersViewSource;
       // CollectionViewSource inventoryViewSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //  customerOrdersViewSource.Source = ctx.Orders.Local;
            BindDataGrid();
         //   inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
          //  inventoryViewSource.Source = ctx.Inventories.Local;
            ctx.Orders.Load();
            ctx.Inventories.Load();

            cmbCustomers.ItemsSource = ctx.Customers.Local;
          //  cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";

            cmbInventory.ItemsSource = ctx.Inventories.Local;
          //  cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            ctx.Customers.Load();
            System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName"); 
            lastNameValidationBinding.NotifyOnValidationError = true; 
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged; 
            //string min length validator 
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValid()); 
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);
            //setare binding nou 



        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                  
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;


                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
              //  lstPhones.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
               // txtPhoneNumber.IsEnabled = false;
              //  txtSubscriber.IsEnabled = false;
            }
            else
            {
                if (action == ActionState.Edit)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        customer.FirstName = firstNameTextBox.Text.Trim();
                        customer.LastName = lastNameTextBox.Text.Trim();
                    }
                    catch (DataException ex)
                    {
                       
                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                    customerViewSource.View.MoveCurrentTo(customer);
                    btnNew.IsEnabled = true; btnEdit.IsEnabled = true;
                    btnDelete.IsEnabled = true; btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false; 
                    btnPrevious.IsEnabled = true; btnNext.IsEnabled = true;
                  
                }
                else
                    if (action == ActionState.Delete)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        ctx.Customers.Remove(customer);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                      
                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                    btnNew.IsEnabled = true; btnEdit.IsEnabled = true; btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = false; btnCancel.IsEnabled = false; 
                  
                    btnPrevious.IsEnabled = true; btnNext.IsEnabled = true;
                   
                   
                }
            }

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextI_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnSaveI_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;

            if (action == ActionState.New)
            {
                try
                {
                    inventory = new Inventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };
                    ctx.Inventories.Add(inventory);
                    customerViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;


                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                //  lstPhones.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                // txtPhoneNumber.IsEnabled = false;
                //  txtSubscriber.IsEnabled = false;
            }
            else
            {
                if (action == ActionState.Edit)
                {
                    try
                    {
                        inventory = (Inventory)customerDataGrid.SelectedItem;

                        inventory.Make = makeTextBox.Text.Trim();
                        inventory.Color = colorTextBox.Text.Trim();

                    }
                    catch (DataException ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                    customerViewSource.View.MoveCurrentTo(inventory);
                    btnNew.IsEnabled = true; btnEdit.IsEnabled = true;
                    btnDelete.IsEnabled = true; btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    btnPrevious.IsEnabled = true; btnNext.IsEnabled = true;

                }
                else
                    if (action == ActionState.Delete)
                {
                    try
                    {
                        inventory = (Inventory)customerDataGrid.SelectedItem;
                        ctx.Inventories.Remove(inventory);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                    btnNew.IsEnabled = true; btnEdit.IsEnabled = true; btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = false; btnCancel.IsEnabled = false;

                    btnPrevious.IsEnabled = true; btnNext.IsEnabled = true;


                }
            }
        }

        private void btnPreviousI_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            

            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;

                   order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {

                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;


                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                //  lstPhones.IsEnabled = true;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                // txtPhoneNumber.IsEnabled = false;
                //  txtSubscriber.IsEnabled = false;
            }
            else
            {
                if (action == ActionState.Edit)
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    try
                    {
                        int curr_id = selectedOrder.OrderId;
                        var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                        if(editedOrder != null)
                        {
                            editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                            editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());

                            ctx.SaveChanges();
                        }
                   //     order = (Order)customerDataGrid.SelectedItem;

                      //  order.CustId = custIdTextBox.Text.Trim();
                      //  order.CarId = carIdTextBox.Text.Trim();

                    }
                    catch (DataException ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                    BindDataGrid();
                    customerViewSource.View.MoveCurrentTo(selectedOrder);
                 //   customerViewSource.View.Refresh();
                  //  customerViewSource.View.MoveCurrentTo(order);
                  //  btnNew.IsEnabled = true; btnEdit.IsEnabled = true;
                  //  btnDelete.IsEnabled = true; btnSave.IsEnabled = false;
                  //  btnCancel.IsEnabled = false;
                  //  btnPrevious.IsEnabled = true; btnNext.IsEnabled = true;

                }
                else
                    if (action == ActionState.Delete)
                {
                    try
                    {
                        dynamic selectedOrder = ordersDataGrid.SelectedItem;

                        int curr_id = selectedOrder.OrderId;
                        var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                        if(deletedOrder != null)
                        {
                            ctx.Orders.Remove(deletedOrder);
                            ctx.SaveChanges();
                            MessageBox.Show("Order Deleted Successfully", "Message");
                            BindDataGrid();
                        }
                      //  order = (Order)customerDataGrid.SelectedItem;
                      //  ctx.Orders.Remove(order);
                       // ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                  //  customerViewSource.View.Refresh();
                  //  btnNew.IsEnabled = true; btnEdit.IsEnabled = true; btnDelete.IsEnabled = true;
                  //  btnSave.IsEnabled = false; btnCancel.IsEnabled = false;

                  //  btnPrevious.IsEnabled = true; btnNext.IsEnabled = true;


                }
            }
        }

        private void btnNextO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnPreviousO_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }
    }
}