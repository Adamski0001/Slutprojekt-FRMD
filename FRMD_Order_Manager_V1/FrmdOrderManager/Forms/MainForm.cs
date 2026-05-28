using FrmdOrderManager.Data;
using FrmdOrderManager.Models;
using FrmdOrderManager.Services;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace FrmdOrderManager.Forms;

// Huvudfönstret med alla flikar. Anropar services för all logik så att formuläret
// bara hanterar knapptryck och visning av data.
public partial class MainForm : Form
{
    private readonly ProductService _productService;
    private readonly CustomerService _customerService;
    private readonly OrderService _orderService;
    private readonly ToolTip _toolTip = new ToolTip();

    // sv-SE används för att få "1 234,50 kr" oavsett vilken culture programmet kör med.
    private static readonly CultureInfo SwedishCulture = new CultureInfo("sv-SE");

    // Skapar fönstret, kopplar upp services och fyller alla flikar med innehåll.
    public MainForm()
    {
        InitializeComponent();

        // JSON-filerna läggs i en undermapp i programmets körmapp.
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");

        ValidationService validationService = new ValidationService();
        _productService = new ProductService();

        // JsonRepository<T> är generisk så samma klass fungerar för Customer och Order.
        JsonRepository<Customer> customerRepo = new JsonRepository<Customer>(Path.Combine(dataDirectory, "customers.json"));
        JsonRepository<Order> orderRepo = new JsonRepository<Order>(Path.Combine(dataDirectory, "orders.json"));

        _customerService = new CustomerService(customerRepo, validationService);
        _orderService = new OrderService(orderRepo, validationService);

        ApplyTheme();
        LoadProductsIntoUi();
        RefreshAllViews();
    }

    // Lägger på färger, KPI-kort och tabellstyling efter att Designer-filen ritat upp controlsen.
    private void ApplyTheme()
    {
        BackColor = Color.White;

        lblTitle.ForeColor = UiTheme.Accent;
        linkLabel1.LinkColor = UiTheme.Accent;
        linkLabel1.ActiveLinkColor = UiTheme.AccentSoft;

        StyleKpiCard(lblTotalOrders, lblTotalOrdersValue);
        StyleKpiCard(lblActiveOrders, lblActiveOrdersValue);
        StyleKpiCard(lblInProduction, lblInProductionValue);
        StyleKpiCard(lblTotalSales, lblTotalSalesValue);

        StyleButton(btnAddCustomer);
        StyleButton(btnCreateOrder);
        StyleButton(btnUpdateStatus);

        StyleGridOrders();
        ConfigureToolTips();
    }

    // Lägger en färgad panel bakom en KPI-rad så att den ser ut som ett kort.
    private void StyleKpiCard(Label nameLabel, Label valueLabel)
    {
        Control parent = nameLabel.Parent ?? tabDashboard;

        int top = Math.Min(nameLabel.Top, valueLabel.Top);
        int bottom = Math.Max(nameLabel.Bottom, valueLabel.Bottom);

        Panel card = new Panel
        {
            Location = new Point(30, top - 10),
            Size = new Size(440, bottom - top + 20),
            BackColor = UiTheme.CardBackground,
            BorderStyle = BorderStyle.FixedSingle
        };
        parent.Controls.Add(card);
        card.SendToBack();

        // Matcha labelns bakgrund mot kortet, annars syns en ljus rektangel ovanpå.
        nameLabel.BackColor = UiTheme.CardBackground;
        valueLabel.BackColor = UiTheme.CardBackground;
        nameLabel.ForeColor = UiTheme.TextDark;
        valueLabel.ForeColor = UiTheme.Accent;
    }

    // Standardstil på en primärknapp – fyllning, ramfärg och hand-cursor.
    private static void StyleButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = UiTheme.Accent;
        button.ForeColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        button.Cursor = Cursors.Hand;
    }

    // Styling på ordertabellen – accentheader, varannan rad färgad och CellFormatting.
    private void StyleGridOrders()
    {
        gridOrders.EnableHeadersVisualStyles = false;
        gridOrders.RowHeadersVisible = false;
        gridOrders.BorderStyle = BorderStyle.FixedSingle;
        gridOrders.BackgroundColor = Color.White;
        gridOrders.GridColor = UiTheme.CardBorder;
        gridOrders.ColumnHeadersHeight = 32;

        gridOrders.ColumnHeadersDefaultCellStyle.BackColor = UiTheme.Accent;
        gridOrders.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        gridOrders.ColumnHeadersDefaultCellStyle.SelectionBackColor = UiTheme.Accent;
        gridOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        gridOrders.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        gridOrders.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

        gridOrders.DefaultCellStyle.SelectionBackColor = UiTheme.AccentSoft;
        gridOrders.DefaultCellStyle.SelectionForeColor = Color.White;
        gridOrders.AlternatingRowsDefaultCellStyle.BackColor = UiTheme.CardBackground;

        gridOrders.CellFormatting += GridOrders_CellFormatting;
    }

    // Lägger svenska tooltips på inmatningsfält och knappar.
    private void ConfigureToolTips()
    {
        _toolTip.SetToolTip(txtCustomerName, "Kundens fullständiga namn.");
        _toolTip.SetToolTip(txtCustomerEmail, "E-postadress – används också i valideringen.");
        _toolTip.SetToolTip(txtCustomerPhone, "Telefonnummer (frivilligt).");
        _toolTip.SetToolTip(btnAddCustomer, "Spara den nya kunden.");

        _toolTip.SetToolTip(cmbOrderCustomer, "Välj vilken kund ordern gäller.");
        _toolTip.SetToolTip(cmbOrderProduct, "Välj vilken produkt som ska beställas.");
        _toolTip.SetToolTip(numQuantity, "Antal enheter av produkten.");
        _toolTip.SetToolTip(txtOrderNotes, "Eventuella specialönskemål till tillverkningen.");
        _toolTip.SetToolTip(btnCreateOrder, "Skapa en ny order av valda värden.");

        _toolTip.SetToolTip(cmbStatus, "Den nya statusen för den valda ordern.");
        _toolTip.SetToolTip(btnUpdateStatus, "Spara statusbytet på den markerade ordern.");
    }

    // Formaterar ett belopp som svensk valuta, t.ex. "1 234,50 kr".
    private static string FormatMoney(decimal value)
    {
        return value.ToString("C", SwedishCulture);
    }

    // Mappar en orderstatus till en accentfärg.
    private static Color GetStatusColor(OrderStatus status)
    {
        switch (status)
        {
            case OrderStatus.New:
                return UiTheme.Accent;
            case OrderStatus.InProduction:
                return UiTheme.InProgress;
            case OrderStatus.Completed:
            case OrderStatus.Shipped:
                return UiTheme.Success;
            case OrderStatus.Cancelled:
                return UiTheme.Warning;
            default:
                return UiTheme.TextDark;
        }
    }

    // Formaterar pris- och datumkolumner och färglägger statuskolumnen.
    private void GridOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex >= gridOrders.Columns.Count)
        {
            return;
        }

        string columnName = gridOrders.Columns[e.ColumnIndex].Name;

        if (columnName == "TotalPrice" && e.Value is decimal totalPrice)
        {
            e.Value = FormatMoney(totalPrice);
            e.FormattingApplied = true;
        }
        else if (columnName == "CreatedAt" && e.Value is DateTime createdAt)
        {
            e.Value = createdAt.ToString("yyyy-MM-dd HH:mm", SwedishCulture);
            e.FormattingApplied = true;
        }

        if (columnName == "Status" && gridOrders.Rows[e.RowIndex].DataBoundItem is Order order)
        {
            e.CellStyle.ForeColor = GetStatusColor(order.Status);
            e.CellStyle.SelectionForeColor = Color.White;
            e.CellStyle.Font = new Font(gridOrders.Font, FontStyle.Bold);
        }
    }

    // Validerar och lägger till en ny kund från fälten i Customers-fliken.
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

    // Skapar en ny order av valda värden i Orders-fliken.
    private void btnCreateOrder_Click(object sender, EventArgs e)
    {
        // SelectedItem kan vara null om inget är valt – "as" låter ValidationService hantera felet.
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

    // Sätter den valda statusen på den markerade ordern i tabellen.
    private void btnUpdateStatus_Click(object sender, EventArgs e)
    {
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

    // Triggas när man klickar på en ny rad i ordertabellen. Visar orderdetaljerna.
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

    // Fyller produktlistan och produktdropdownen från ProductService.
    private void LoadProductsIntoUi()
    {
        // Sätter null först så att Windows Forms släpper bindningen och tar fram ny data.
        lstProducts.DataSource = null;
        lstProducts.DataSource = _productService.GetAllProducts();

        cmbOrderProduct.DataSource = null;
        cmbOrderProduct.DataSource = new List<Product>(_productService.GetAllProducts());
    }

    // Uppdaterar kunder, ordrar och dashboard på en gång.
    private void RefreshAllViews()
    {
        RefreshCustomers();
        RefreshOrders();
        RefreshDashboard();
    }

    // Uppdaterar kundlistan och kunddropdownen så de visar aktuella kunder.
    private void RefreshCustomers()
    {
        // En ny lista varje gång så att ListBox/ComboBox märker att data ändrats.
        lstCustomers.DataSource = null;
        lstCustomers.DataSource = new List<Customer>(_customerService.GetAllCustomers());

        cmbOrderCustomer.DataSource = null;
        cmbOrderCustomer.DataSource = new List<Customer>(_customerService.GetAllCustomers());
    }

    // Uppdaterar ordertabellen – sorterar nyast först och döljer interna kolumner.
    private void RefreshOrders()
    {
        List<Order> sortedOrders = _orderService.GetAllOrders()
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        gridOrders.DataSource = null;
        gridOrders.DataSource = sortedOrders;

        HideColumnIfExists("Id");
        HideColumnIfExists("CustomerId");
        HideColumnIfExists("Items");
        HideColumnIfExists("IsActive");

        if (gridOrders.Columns["Notes"] != null)
        {
            gridOrders.Columns["Notes"].HeaderText = "Notes";
            gridOrders.Columns["Notes"].FillWeight = 160;
        }
    }

    // Döljer en kolumn i ordertabellen om den finns.
    private void HideColumnIfExists(string columnName)
    {
        if (gridOrders.Columns[columnName] != null)
        {
            gridOrders.Columns[columnName].Visible = false;
        }
    }

    // Räknar fram KPI-värdena och skriver in dem på dashboarden.
    private void RefreshDashboard()
    {
        List<Order> orders = _orderService.GetAllOrders();

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
        lblTotalSalesValue.Text = FormatMoney(_orderService.CalculateTotalSales());
    }

    // Öppnar FRMD:s webbplats i användarens standardwebbläsare.
    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        linkLabel1.LinkVisited = true;

        Process.Start(new ProcessStartInfo
        {
            FileName = "http://www.frmd.se",
            UseShellExecute = true
        });
    }
}
