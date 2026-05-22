using FrmdOrderManager.Data;
using FrmdOrderManager.Models;
using FrmdOrderManager.Services;
using System.Diagnostics;

namespace FrmdOrderManager.Forms;

// MainForm håller hela det grafiska gränssnittet. Den anropar services och repositories
// för all logik, så att formuläret bara hanterar knapptryckningar och visning av data.
public partial class MainForm : Form
{
    private readonly ProductService _productService;
    private readonly CustomerService _customerService;
    private readonly OrderService _orderService;

    public MainForm()
    {
        InitializeComponent();

        // JSON-filerna sparas i en undermapp i programmets körmapp.
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");

        ValidationService validationService = new ValidationService();
        _productService = new ProductService();

        // Skapar repositories med rätt fil för varje typ. JsonRepository<T> är generisk
        // så samma klass fungerar både för Customer och Order.
        JsonRepository<Customer> customerRepo = new JsonRepository<Customer>(Path.Combine(dataDirectory, "customers.json"));
        JsonRepository<Order> orderRepo = new JsonRepository<Order>(Path.Combine(dataDirectory, "orders.json"));

        _customerService = new CustomerService(customerRepo, validationService);
        _orderService = new OrderService(orderRepo, validationService);

        LoadProductsIntoUi();
        RefreshAllViews();
    }

    private void btnAddCustomer_Click(object sender, EventArgs e)
    {
        ValidationResult result = _customerService.AddCustomer(txtCustomerName.Text, txtCustomerEmail.Text, txtCustomerPhone.Text);
        if (!result.IsValid)
        {
            MessageBox.Show(result.ToMessage(), "Could not add customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtCustomerName.Clear();
        txtCustomerEmail.Clear();
        txtCustomerPhone.Clear();
        RefreshAllViews();
    }

    private void btnCreateOrder_Click(object sender, EventArgs e)
    {
        // SelectedItem kan vara null om inget är valt, så vi använder "as" och låter
        // ValidationService rapportera felet i stället för att krascha.
        Customer customer = cmbOrderCustomer.SelectedItem as Customer;
        Product product = cmbOrderProduct.SelectedItem as Product;
        int quantity = (int)numQuantity.Value;

        ValidationResult result = _orderService.CreateOrder(customer, product, quantity, txtOrderNotes.Text);
        if (!result.IsValid)
        {
            MessageBox.Show(result.ToMessage(), "Could not create order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtOrderNotes.Clear();
        numQuantity.Value = 1;
        RefreshAllViews();
        tabMain.SelectedTab = tabOrders;
    }

    private void btnUpdateStatus_Click(object sender, EventArgs e)
    {
        // Kollar att användaren har valt en rad i tabellen.
        if (gridOrders.CurrentRow == null)
        {
            MessageBox.Show("Choose an order in the table first.", "No order selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Order selectedOrder = gridOrders.CurrentRow.DataBoundItem as Order;
        if (selectedOrder == null)
        {
            return;
        }

        if (cmbStatus.SelectedItem == null)
        {
            MessageBox.Show("Choose a status first.", "No status selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        OrderStatus status = (OrderStatus)cmbStatus.SelectedItem;
        _orderService.UpdateStatus(selectedOrder, status);
        RefreshAllViews();
    }

    // Triggas varje gång man klickar på en ny rad i ordertabellen.
    private void gridOrders_SelectionChanged(object sender, EventArgs e)
    {
        if (gridOrders.CurrentRow == null)
        {
            return;
        }

        Order selectedOrder = gridOrders.CurrentRow.DataBoundItem as Order;
        if (selectedOrder == null)
        {
            return;
        }

        // Visar orderns rader, anteckningar och status i panelen till höger.
        lstOrderDetails.DataSource = null;
        lstOrderDetails.DataSource = selectedOrder.Items;

        if (string.IsNullOrWhiteSpace(selectedOrder.Notes))
        {
            txtSelectedOrderNotes.Text = "No notes for this order.";
        }
        else
        {
            txtSelectedOrderNotes.Text = selectedOrder.Notes;
        }

        cmbStatus.SelectedItem = selectedOrder.Status;
    }

    private void LoadProductsIntoUi()
    {
        // Sätter null först för att Windows Forms ska uppdatera bindningen ordentligt.
        lstProducts.DataSource = null;
        lstProducts.DataSource = _productService.GetAllProducts();

        cmbOrderProduct.DataSource = null;
        cmbOrderProduct.DataSource = new List<Product>(_productService.GetAllProducts());
    }

    private void RefreshAllViews()
    {
        RefreshCustomers();
        RefreshOrders();
        RefreshDashboard();
    }

    private void RefreshCustomers()
    {
        // Skapar en ny lista varje gång så ListBox/ComboBox uppdaterar sig.
        lstCustomers.DataSource = null;
        lstCustomers.DataSource = new List<Customer>(_customerService.GetAllCustomers());

        cmbOrderCustomer.DataSource = null;
        cmbOrderCustomer.DataSource = new List<Customer>(_customerService.GetAllCustomers());
    }

    private void RefreshOrders()
    {
        // Sorterar så att den senast skapade ordern ligger högst upp i tabellen.
        List<Order> sortedOrders = _orderService.GetAllOrders()
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        gridOrders.DataSource = null;
        gridOrders.DataSource = sortedOrders;

        // Döljer kolumner som inte behöver visas i tabellen (t.ex. interna ID:n).
        HideColumnIfExists("Id");
        HideColumnIfExists("CustomerId");
        HideColumnIfExists("Items");

        if (gridOrders.Columns["Notes"] != null)
        {
            gridOrders.Columns["Notes"].HeaderText = "Notes";
            gridOrders.Columns["Notes"].FillWeight = 160;
        }
    }

    private void HideColumnIfExists(string columnName)
    {
        if (gridOrders.Columns[columnName] != null)
        {
            gridOrders.Columns[columnName].Visible = false;
        }
    }

    private void RefreshDashboard()
    {
        List<Order> orders = _orderService.GetAllOrders();

        // Räknar hur många ordrar som är i produktion just nu.
        int inProductionCount = 0;
        foreach (Order order in orders)
        {
            if (order.Status == OrderStatus.InProduction)
            {
                inProductionCount++;
            }
        }

        lblTotalOrdersValue.Text = orders.Count.ToString();
        lblActiveOrdersValue.Text = _orderService.CountActiveOrders().ToString();
        lblInProductionValue.Text = inProductionCount.ToString();
        lblTotalSalesValue.Text = _orderService.CalculateTotalSales() + " kr";
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        this.linkLabel1.LinkVisited = true;

        System.Diagnostics.Process.Start(new ProcessStartInfo
        {
            FileName = "http://www.frmd.se",
            UseShellExecute = true
        });
    }
}
