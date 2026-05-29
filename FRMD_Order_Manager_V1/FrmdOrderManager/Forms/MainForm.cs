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

    // sv-SE används för att få "1 234,50 kr" oavsett vad.
    private static readonly CultureInfo SwedishCulture = new CultureInfo("sv-SE");

    // Skapar fönstret, kopplar upp services och fyller alla flikar med innehåll.
    public MainForm()
    {
        InitializeComponent();

        // JSON-filerna läggs i en undermapp i programmets körmapp.
        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles");

        ValidationService validationService = new ValidationService();

        // JsonRepository<T> är generisk så samma klass fungerar för Customer, Order och Product.
        JsonRepository<Customer> customerRepo = new JsonRepository<Customer>(Path.Combine(dataDirectory, "customers.json"));
        JsonRepository<Order> orderRepo = new JsonRepository<Order>(Path.Combine(dataDirectory, "orders.json"));
        JsonRepository<Product> productRepo = new JsonRepository<Product>(Path.Combine(dataDirectory, "products.json"));

        _productService = new ProductService(productRepo, validationService);
        _customerService = new CustomerService(customerRepo, validationService);
        _orderService = new OrderService(orderRepo, validationService);

        ApplyTheme();
        InitializeProductForm();
        InitializeOrderForm();
        InitializeCustomerForm();
        LoadProductsIntoUi();
        RefreshAllViews();

        // ListBox-bindningen autoväljer första kunden - nollställ formuläret så användaren
        // möts av en tom "ny kund"-form i stället för en redigeringsvy vid start.
        ClearCustomerForm();
    }

    // Kopplar upp timern som rensar status-meddelandet på Customers-fliken,
    // samt ESC-tangenten som avbryter ett pågående kundredigeringsläge.
    private void InitializeCustomerForm()
    {
        _customerMessageTimer.Interval = 4000;
        _customerMessageTimer.Tick += (_, _) =>
        {
            _customerMessageTimer.Stop();
            lblCustomerStatusMessage.Text = string.Empty;
        };

        // KeyPreview gör att formuläret ser tangenttryck innan kontrollerna får dem.
        // Det är så vi kan avbryta en pågående redigering med ESC oavsett var fokus ligger.
        KeyPreview = true;
        KeyDown += MainForm_KeyDown;
    }

    // ESC i Customers-fliken stänger redigeringsläget. På övriga flikar är ESC en no-op
    // så vi inte stör t.ex. dropdownmenyer eller modaler.
    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Escape)
        {
            return;
        }

        if (tabMain.SelectedTab != tabCustomers)
        {
            return;
        }

        // Vi är bara i redigeringsläge när Update-knappen är aktiverad - då finns det
        // någonting att avbryta. I "lägg till ny"-läget gör ESC ingenting.
        if (!btnUpdateCustomer.Enabled)
        {
            return;
        }

        e.Handled = true;
        e.SuppressKeyPress = true;
        ClearCustomerForm();
        ShowCustomerStatusMessage("Edit cancelled. Form ready for a new customer.", UiTheme.TextDark);
    }

    // Timern som rensar "Order created"-meddelandet några sekunder efter en lyckad skapande.
    private readonly System.Windows.Forms.Timer _statusMessageTimer = new System.Windows.Forms.Timer();

    // Fyller statusdropdownen i Orders-fliken med alla värden från OrderStatus-enumen och
    // kopplar upp event-handlers för det live-uppdaterade totalpriset.
    private void InitializeOrderForm()
    {
        cmbStatus.DataSource = Enum.GetValues(typeof(OrderStatus));

        // Totalpriset i förhandsvisningen räknas om när användaren byter produkt eller antal.
        cmbOrderProduct.SelectedIndexChanged += (_, _) => UpdateOrderTotalPreview();
        numQuantity.ValueChanged += (_, _) => UpdateOrderTotalPreview();
        UpdateOrderTotalPreview();

        // Status-meddelandet ("Order created", "Status updated" osv) försvinner automatiskt
        // efter en stund så att UI inte fylls med inaktuell info.
        _statusMessageTimer.Interval = 4000;
        _statusMessageTimer.Tick += (_, _) =>
        {
            _statusMessageTimer.Stop();
            lblOrderStatusMessage.Text = string.Empty;
        };
    }

    // Räknar ihop priset för valt produkt × antal och visar i förhandsvisningsetiketten.
    // Anropas vid varje förändring i produkt-droppdown eller antal-räknaren.
    private void UpdateOrderTotalPreview()
    {
        Product product = cmbOrderProduct.SelectedItem as Product;
        int quantity = (int)numQuantity.Value;

        if (product == null || quantity < 1)
        {
            lblOrderTotalValue.Text = FormatMoney(0);
            return;
        }

        decimal total = product.CalculatePrice() * quantity;
        lblOrderTotalValue.Text = FormatMoney(total);
    }

    // Visar ett kort, färgkodat statusmeddelande på Orders-fliken som försvinner av sig självt.
    private void ShowOrderStatusMessage(string text, Color color)
    {
        lblOrderStatusMessage.ForeColor = color;
        lblOrderStatusMessage.Text = text;
        _statusMessageTimer.Stop();
        _statusMessageTimer.Start();
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
        StyleButton(btnUpdateCustomer);
        StyleButton(btnClearCustomerForm);
        StyleButton(btnCreateOrder);
        StyleButton(btnUpdateStatus);
        StyleButton(btnAddProduct);
        StyleButton(btnRemoveProduct);

        StyleGridOrders();
        ConfigureToolTips();

        // Live-total och rubriken över detalj-panelen får accentfärg så de syns tydligt.
        lblOrderTotalValue.ForeColor = UiTheme.Accent;
        lblOrderDetails.ForeColor = UiTheme.TextDark;

        // Hint-texterna "Tip: press Delete" hålls dämpade så de inte konkurrerar med innehållet.
        lblOrdersDeleteHint.ForeColor = UiTheme.TextDark;
        lblProductsDeleteHint.ForeColor = UiTheme.TextDark;
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

    // Standardstil på en primärknapp - fyllning, ramfärg och hand-cursor.
    private static void StyleButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = UiTheme.Accent;
        button.ForeColor = Color.White;
        button.FlatAppearance.BorderSize = 0;
        button.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        button.Cursor = Cursors.Hand;
    }

    // Styling på ordertabellen - accentheader, varannan rad färgad och CellFormatting.
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
        gridOrders.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        gridOrders.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
        gridOrders.AlternatingRowsDefaultCellStyle.BackColor = UiTheme.CardBackground;

        gridOrders.CellFormatting += GridOrders_CellFormatting;
        gridOrders.KeyDown += GridOrders_KeyDown;
    }

    // Tar bort den markerade ordern när användaren trycker Delete-tangenten. Frågar först om bekräftelse
    // så att man inte råkar radera en order av misstag.
    private void GridOrders_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Delete)
        {
            return;
        }

        if (gridOrders.CurrentRow == null)
        {
            return;
        }

        Order selectedOrder = gridOrders.CurrentRow.DataBoundItem as Order;
        if (selectedOrder == null)
        {
            return;
        }

        // Förhindrar att grid'en själv tolkar Delete-tangenten på något annat sätt.
        e.Handled = true;
        e.SuppressKeyPress = true;

        string message = $"Delete order {selectedOrder.ShortId} for {selectedOrder.CustomerName}?";
        DialogResult choice = MessageBox.Show(message, "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (choice != DialogResult.Yes)
        {
            return;
        }

        _orderService.DeleteOrder(selectedOrder);
        RefreshAllViews();

        ShowOrderStatusMessage($"✓ Deleted order #{selectedOrder.ShortId}", UiTheme.Warning);
    }

    // Lägger svenska tooltips på inmatningsfält och knappar.
    private void ConfigureToolTips()
    {
        _toolTip.SetToolTip(txtCustomerName, "The customer's full name.");
        _toolTip.SetToolTip(txtCustomerEmail, "Email address. Also used for validation.");
        _toolTip.SetToolTip(txtCustomerPhone, "Phone number (optional).");
        _toolTip.SetToolTip(btnAddCustomer, "Save the new customer.");

        _toolTip.SetToolTip(cmbOrderCustomer, "Pick which customer the order is for.");
        _toolTip.SetToolTip(cmbOrderProduct, "Pick which product to order.");
        _toolTip.SetToolTip(numQuantity, "Number of units of the product.");
        _toolTip.SetToolTip(txtOrderNotes, "Any special requests for production.");
        _toolTip.SetToolTip(btnCreateOrder, "Create a new order from the selected values.");

        _toolTip.SetToolTip(cmbStatus, "The new status for the selected order.");
        _toolTip.SetToolTip(btnUpdateStatus, "Save the status change on the selected order.");

        _toolTip.SetToolTip(cmbProductCategory, "Which type of product to add?");
        _toolTip.SetToolTip(txtProductLocation, "Location the product is for, e.g. \"Åre\".");
        _toolTip.SetToolTip(numProductBasePrice, "Base price in kronor.");
        _toolTip.SetToolTip(cmbProductSize, "Map frame size. Medium adds +100 kr, Large adds +250 kr.");
        _toolTip.SetToolTip(chkProductGiftWrap, "Adds 25 kr to the price.");
        _toolTip.SetToolTip(numProductComplexity, "1 = simple, 5 = very detailed. Each step adds +150 kr.");
        _toolTip.SetToolTip(btnAddProduct, "Add the product to the catalog.");
        _toolTip.SetToolTip(btnRemoveProduct, "Remove the selected product from the catalog.");
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

    // Timern som rensar status-meddelandet på Customers-fliken efter en stund.
    private readonly System.Windows.Forms.Timer _customerMessageTimer = new System.Windows.Forms.Timer();

    // Hjälp-metod för att visa ett färgkodat, självsläckande meddelande på Customers-fliken.
    private void ShowCustomerStatusMessage(string text, Color color)
    {
        lblCustomerStatusMessage.ForeColor = color;
        lblCustomerStatusMessage.Text = text;
        _customerMessageTimer.Stop();
        _customerMessageTimer.Start();
    }

    // Validerar och lägger till en ny kund från fälten i Customers-fliken.
    private void btnAddCustomer_Click(object sender, EventArgs e)
    {
        try
        {
            _customerService.AddCustomer(txtCustomerName.Text, txtCustomerEmail.Text, txtCustomerPhone.Text);
        }
        catch (ValidationException ex)
        {
            MessageBox.Show(ex.Message, "Could not add customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string addedName = txtCustomerName.Text.Trim();
        // OBS: ClearCustomerForm() MÅSTE ske EFTER RefreshAllViews(). När listan får ny DataSource
        // autoväljer ListBox första kunden och fyller formuläret igen - så vi rensar i sista steget.
        RefreshAllViews();
        ClearCustomerForm();
        ShowCustomerStatusMessage($"✓ Customer \"{addedName}\" added", UiTheme.Success);
    }

    // Sparar ändringar på den kund som ligger i fälten just nu (vald via listan till höger).
    // En typfelad e-post kan på så vis rättas i stället för att vara permanent.
    private void btnUpdateCustomer_Click(object sender, EventArgs e)
    {
        Customer selected = lstCustomers.SelectedItem as Customer;
        if (selected == null)
        {
            return;
        }

        try
        {
            _customerService.UpdateCustomer(selected, txtCustomerName.Text, txtCustomerEmail.Text, txtCustomerPhone.Text);
        }
        catch (ValidationException ex)
        {
            MessageBox.Show(ex.Message, "Could not update customer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string updatedName = selected.Name;
        // Samma ordning som vid Add: refresha först, rensa sen - annars återväljs kunden via ListBox-bindningen.
        RefreshAllViews();
        ClearCustomerForm();
        ShowCustomerStatusMessage($"✓ Customer \"{updatedName}\" updated", UiTheme.Success);
    }

    // Avmarkerar kunden i listan, tömmer fälten och stänger redigeringsläget.
    private void btnClearCustomerForm_Click(object sender, EventArgs e)
    {
        ClearCustomerForm();
    }

    // När användaren klickar på en kund i listan fyller vi formuläret med kundens värden och
    // växlar knapparna så att "Save changes" och "Clear / new" blir tillgängliga.
    private void lstCustomers_SelectedIndexChanged(object sender, EventArgs e)
    {
        Customer selected = lstCustomers.SelectedItem as Customer;
        if (selected == null)
        {
            return;
        }

        txtCustomerName.Text = selected.Name;
        txtCustomerEmail.Text = selected.Email;
        txtCustomerPhone.Text = selected.Phone;

        btnUpdateCustomer.Enabled = true;
        btnClearCustomerForm.Enabled = true;
        btnAddCustomer.Enabled = false;
    }

    // Återställer fälten och knapparna till "lägg till ny kund"-läget.
    private void ClearCustomerForm()
    {
        txtCustomerName.Clear();
        txtCustomerEmail.Clear();
        txtCustomerPhone.Clear();
        lstCustomers.ClearSelected();

        btnUpdateCustomer.Enabled = false;
        btnClearCustomerForm.Enabled = false;
        btnAddCustomer.Enabled = true;
    }

    // Skapar en ny order av valda värden i Orders-fliken.
    private void btnCreateOrder_Click(object sender, EventArgs e)
    {
        // SelectedItem kan vara null om inget är valt - "as" ger då null som ValidationService
        // upptäcker och kastar ett ValidationException för.
        Customer customer = cmbOrderCustomer.SelectedItem as Customer;
        Product product = cmbOrderProduct.SelectedItem as Product;
        int quantity = (int)numQuantity.Value;

        try
        {
            _orderService.CreateOrder(customer, product, quantity, txtOrderNotes.Text);
        }
        catch (ValidationException ex)
        {
            MessageBox.Show(ex.Message, "Could not create order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Bygg en kort sammanfattning för bekräftelsemeddelandet ("Order created: 2 x Åre frame - 1 798 kr").
        decimal lineTotal = product.CalculatePrice() * quantity;
        string summary = $"✓ Order created: {quantity} × {product.Name} for {FormatMoney(lineTotal)}";

        txtOrderNotes.Clear();
        numQuantity.Value = 1;
        RefreshAllViews();
        tabMain.SelectedTab = tabOrders;

        // Listan sorteras nyast först i RefreshOrders, så den nyskapade ordern hamnar alltid överst.
        // Vi markerar den så att användaren direkt ser sin nya order både i tabellen och i detalj-panelen.
        if (gridOrders.Rows.Count > 0)
        {
            gridOrders.ClearSelection();
            gridOrders.Rows[0].Selected = true;
            gridOrders.CurrentCell = gridOrders.Rows[0].Cells[gridOrders.Columns.GetFirstColumn(DataGridViewElementStates.Visible).Index];
        }

        ShowOrderStatusMessage(summary, UiTheme.Success);
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

        ShowOrderStatusMessage($"✓ Status updated to {status} for order #{selectedOrder.ShortId}", UiTheme.Success);
    }

    // Triggas när man klickar på en ny rad i ordertabellen. Visar orderdetaljerna.
    private void gridOrders_SelectionChanged(object sender, EventArgs e)
    {
        if (gridOrders.CurrentRow == null)
        {
            // Inga ordrar markerade - nollställ detaljpanelen så den inte visar gammal data.
            lblOrderDetails.Text = "Order details";
            lstOrderDetails.DataSource = null;
            txtSelectedOrderNotes.Text = string.Empty;
            return;
        }

        Order selectedOrder = gridOrders.CurrentRow.DataBoundItem as Order;
        if (selectedOrder == null)
        {
            return;
        }

        // Rubriken visar både ordernumret (ShortId) och kundens namn så att man tydligt
        // ser vilken order detalj-panelen, anteckningarna och statusen hör till.
        lblOrderDetails.Text = $"Order details for #{selectedOrder.ShortId}  ({selectedOrder.CustomerName})";

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

    // Fyller dropdownsen i Products-fliken som inte ändras under körningen.
    // Kör en gång vid start så att kategorin och storleksvalen finns redo.
    private void InitializeProductForm()
    {
        cmbProductCategory.DataSource = Enum.GetValues(typeof(ProductCategory));
        cmbProductSize.DataSource = new List<string> { "Small", "Medium", "Large" };
        cmbProductSize.SelectedItem = "Medium";

        // Sätter SelectedIndex sist så att SelectedIndexChanged kör och visar rätt fält.
        cmbProductCategory.SelectedIndex = 0;
    }

    // Visar bara de inmatningsfält som hör till den valda kategorin.
    private void cmbProductCategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbProductCategory.SelectedItem == null)
        {
            return;
        }

        ProductCategory category = (ProductCategory)cmbProductCategory.SelectedItem;

        bool isMapFrame = category == ProductCategory.MapFrame;
        bool isKeyring = category == ProductCategory.Keyring;
        bool isCustomMap = category == ProductCategory.CustomMap;

        lblProductSize.Visible = isMapFrame;
        cmbProductSize.Visible = isMapFrame;

        lblProductGiftWrap.Visible = isKeyring;
        chkProductGiftWrap.Visible = isKeyring;

        lblProductComplexity.Visible = isCustomMap;
        numProductComplexity.Visible = isCustomMap;

        // CustomMap använder begreppet "requested location" i stället för "location".
        lblProductLocation.Text = isCustomMap ? "Requested location" : "Location";
    }

    // Skapar rätt subklass utifrån kategorin och försöker lägga den i katalogen.
    private void btnAddProduct_Click(object sender, EventArgs e)
    {
        if (cmbProductCategory.SelectedItem == null)
        {
            MessageBox.Show("Choose a category first.", "Missing category", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ProductCategory category = (ProductCategory)cmbProductCategory.SelectedItem;
        string location = txtProductLocation.Text.Trim();
        decimal basePrice = numProductBasePrice.Value;

        Product product;
        switch (category)
        {
            case ProductCategory.MapFrame:
                string size = cmbProductSize.SelectedItem as string ?? "Medium";
                product = new MapFrame(location, size, basePrice);
                break;
            case ProductCategory.Keyring:
                product = new Keyring(location, basePrice, chkProductGiftWrap.Checked);
                break;
            case ProductCategory.CustomMap:
                int complexity = (int)numProductComplexity.Value;
                product = new CustomMapProduct(location, complexity, basePrice);
                break;
            default:
                return;
        }

        try
        {
            _productService.AddProduct(product);
        }
        catch (ValidationException ex)
        {
            MessageBox.Show(ex.Message, "Could not add product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        txtProductLocation.Clear();
        numProductBasePrice.Value = 0;
        chkProductGiftWrap.Checked = false;
        numProductComplexity.Value = 1;
        cmbProductSize.SelectedItem = "Medium";

        LoadProductsIntoUi();
    }

    // Tar bort den markerade produkten efter en bekräftelseruta.
    private void btnRemoveProduct_Click(object sender, EventArgs e)
    {
        RemoveSelectedProduct();
    }

    // Tar bort produkten även när användaren trycker Delete med listan markerad.
    // SuppressKeyPress hindrar att ListBox tolkar tangenten på något annat sätt.
    private void lstProducts_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Delete)
        {
            return;
        }

        e.Handled = true;
        e.SuppressKeyPress = true;
        RemoveSelectedProduct();
    }

    // Gemensam bortagningslogik som anropas både av Remove-knappen och Delete-tangenten.
    // Order-historiken påverkas inte - OrderItem har en kopia av produktens namn och pris.
    private void RemoveSelectedProduct()
    {
        Product selected = lstProducts.SelectedItem as Product;
        if (selected == null)
        {
            MessageBox.Show("Choose a product in the list first.", "No product selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        DialogResult confirm = MessageBox.Show(
            $"Remove \"{selected.Name}\" from the catalog?\n\nExisting orders that used this product stay intact. Only future orders are affected.",
            "Confirm removal",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _productService.RemoveProduct(selected);
        LoadProductsIntoUi();
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

    // Uppdaterar ordertabellen - sorterar nyast först och döljer interna kolumner.
    private void RefreshOrders()
    {
        List<Order> sortedOrders = _orderService.GetAllOrders()
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        // Stäng tillfälligt av Fill-läget medan vi konfigurerar kolumnerna. Annars triggar
        // varje MinimumWidth-ändring en omräkning som kan kasta NullReferenceException när
        // formuläret inte är fullt layoutat ännu (t.ex. första anropet från konstruktorn).
        gridOrders.SuspendLayout();
        gridOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

        gridOrders.DataSource = null;
        gridOrders.DataSource = sortedOrders;

        HideColumnIfExists("Id");
        HideColumnIfExists("CustomerId");
        HideColumnIfExists("Items");
        HideColumnIfExists("IsActive");
        // Order-id (ShortId) syns inte längre i tabellen - den visas i stället i rubriken över
        // detalj-panelen ("Order details for #42DF3 (Adam Hessel)") där den faktiskt behövs.
        HideColumnIfExists("ShortId");

        // Kolumnerna delar på tabellens bredd via FillWeight så att alla syns utan vågrät rullning.
        // MinimumWidth + radbrytning på cellnivå gör att längre värden bryts istället för att klippas.
        // Minsta bredderna är medvetet återhållsamma så att alla 5 kolumner får plats även
        // när formuläret krymps ned till MinimumSize - Notes/Total föll annars av till höger.
        ConfigureOrderColumn("CustomerName", "Customer", weight: 26, minWidth: 95);
        ConfigureOrderColumn("CreatedAt", "Created", weight: 24, minWidth: 100);
        ConfigureOrderColumn("Status", "Status", weight: 18, minWidth: 90);
        ConfigureOrderColumn("Notes", "Notes", weight: 18, minWidth: 80);
        ConfigureOrderColumn("TotalPrice", "Total", weight: 14, minWidth: 75);

        // Slå på Fill igen så kolumnerna delar på den synliga bredden.
        gridOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        gridOrders.ResumeLayout();
    }

    // Konfigurerar en kolumn i ordertabellen: rubrik, andel av bredden och minsta bredd.
    // OBS: ordningen är medvetet vald - MinimumWidth sätts INNAN AutoSizeMode = Fill för att
    // undvika en intern NullReferenceException i DataGridView's layout-motor vid uppstart.
    private void ConfigureOrderColumn(string columnName, string headerText, int weight, int minWidth)
    {
        DataGridViewColumn column = gridOrders.Columns[columnName];
        if (column == null)
        {
            return;
        }

        column.HeaderText = headerText;
        column.MinimumWidth = minWidth;
        column.FillWeight = weight;
        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        column.Resizable = DataGridViewTriState.False;
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

        // Grafen ritas om med aktuella ordrar. Att en kund eller produkt har tagits bort spelar
        // ingen roll - Order/OrderItem har egna kopior av all nödvändig data, så historiken
        // i diagrammet förändras bara när en order skapas, ändras eller raderas.
        pnlRevenueChart.SetOrders(orders);
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

