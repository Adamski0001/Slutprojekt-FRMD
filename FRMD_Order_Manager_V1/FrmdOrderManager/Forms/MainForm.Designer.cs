using FrmdOrderManager.Models;

namespace FrmdOrderManager.Forms;

// Designer-fil. Här ligger alla controls och layouten för MainForm.
// I ett vanligt designer-formulär hanteras detta via .resx, men jag valde
// att skapa controls i kod för att kunna styra layouten själv.
partial class MainForm
{
    private System.ComponentModel.IContainer components;
    private TabControl tabMain;
    private TabPage tabDashboard;
    private TabPage tabCustomers;
    private TabPage tabProducts;
    private TabPage tabOrders;
    private Label lblTitle;
    private Label lblTotalOrders;
    private Label lblTotalOrdersValue;
    private Label lblActiveOrders;
    private Label lblActiveOrdersValue;
    private Label lblInProduction;
    private Label lblInProductionValue;
    private Label lblTotalSales;
    private Label lblTotalSalesValue;
    private TextBox txtCustomerName;
    private TextBox txtCustomerEmail;
    private TextBox txtCustomerPhone;
    private Button btnAddCustomer;
    private ListBox lstCustomers;
    private ListBox lstProducts;
    private ComboBox cmbOrderCustomer;
    private ComboBox cmbOrderProduct;
    private NumericUpDown numQuantity;
    private TextBox txtOrderNotes;
    private Button btnCreateOrder;
    private DataGridView gridOrders;
    private ComboBox cmbStatus;
    private Button btnUpdateStatus;
    private ListBox lstOrderDetails;
    private TextBox txtSelectedOrderNotes;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        tabMain = new TabControl();
        tabDashboard = new TabPage();
        lblTitle = new Label();
        lblTotalOrders = new Label();
        lblTotalOrdersValue = new Label();
        lblActiveOrders = new Label();
        lblActiveOrdersValue = new Label();
        lblInProduction = new Label();
        lblInProductionValue = new Label();
        lblTotalSales = new Label();
        lblTotalSalesValue = new Label();
        tabCustomers = new TabPage();
        lblCustomerName = new Label();
        lblCustomerEmail = new Label();
        lblCustomerPhone = new Label();
        txtCustomerName = new TextBox();
        txtCustomerEmail = new TextBox();
        txtCustomerPhone = new TextBox();
        btnAddCustomer = new Button();
        lstCustomers = new ListBox();
        tabProducts = new TabPage();
        lblProductsHeader = new Label();
        lstProducts = new ListBox();
        tabOrders = new TabPage();
        lblCustomerPick = new Label();
        lblProductPick = new Label();
        lblQuantity = new Label();
        lblNotes = new Label();
        lblOrderDetails = new Label();
        lblSelectedNotes = new Label();
        lblStatus = new Label();
        cmbOrderCustomer = new ComboBox();
        cmbOrderProduct = new ComboBox();
        numQuantity = new NumericUpDown();
        txtOrderNotes = new TextBox();
        btnCreateOrder = new Button();
        gridOrders = new DataGridView();
        lstOrderDetails = new ListBox();
        txtSelectedOrderNotes = new TextBox();
        cmbStatus = new ComboBox();
        btnUpdateStatus = new Button();
        linkLabel1 = new LinkLabel();
        tabMain.SuspendLayout();
        tabDashboard.SuspendLayout();
        tabCustomers.SuspendLayout();
        tabProducts.SuspendLayout();
        tabOrders.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridOrders).BeginInit();
        SuspendLayout();
        // 
        // tabMain
        // 
        tabMain.Controls.Add(tabDashboard);
        tabMain.Controls.Add(tabCustomers);
        tabMain.Controls.Add(tabProducts);
        tabMain.Controls.Add(tabOrders);
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 0);
        tabMain.Name = "tabMain";
        tabMain.SelectedIndex = 0;
        tabMain.Size = new Size(1020, 650);
        tabMain.TabIndex = 0;
        // 
        // tabDashboard
        // 
        tabDashboard.Controls.Add(linkLabel1);
        tabDashboard.Controls.Add(lblTitle);
        tabDashboard.Controls.Add(lblTotalOrders);
        tabDashboard.Controls.Add(lblTotalOrdersValue);
        tabDashboard.Controls.Add(lblActiveOrders);
        tabDashboard.Controls.Add(lblActiveOrdersValue);
        tabDashboard.Controls.Add(lblInProduction);
        tabDashboard.Controls.Add(lblInProductionValue);
        tabDashboard.Controls.Add(lblTotalSales);
        tabDashboard.Controls.Add(lblTotalSalesValue);
        tabDashboard.Location = new Point(4, 29);
        tabDashboard.Name = "tabDashboard";
        tabDashboard.Size = new Size(1012, 617);
        tabDashboard.TabIndex = 0;
        tabDashboard.Text = "Dashboard";
        // 
        // lblTitle
        // 
        lblTitle.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        lblTitle.Location = new Point(30, 30);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(500, 50);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "FRMD Order Manager";
        // 
        // lblTotalOrders
        // 
        lblTotalOrders.Font = new Font("Segoe UI", 14F);
        lblTotalOrders.Location = new Point(40, 120);
        lblTotalOrders.Name = "lblTotalOrders";
        lblTotalOrders.Size = new Size(200, 35);
        lblTotalOrders.TabIndex = 1;
        lblTotalOrders.Text = "Total orders";
        // 
        // lblTotalOrdersValue
        // 
        lblTotalOrdersValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTotalOrdersValue.Location = new Point(260, 120);
        lblTotalOrdersValue.Name = "lblTotalOrdersValue";
        lblTotalOrdersValue.Size = new Size(200, 35);
        lblTotalOrdersValue.TabIndex = 2;
        lblTotalOrdersValue.Text = "0";
        // 
        // lblActiveOrders
        // 
        lblActiveOrders.Font = new Font("Segoe UI", 14F);
        lblActiveOrders.Location = new Point(40, 180);
        lblActiveOrders.Name = "lblActiveOrders";
        lblActiveOrders.Size = new Size(200, 35);
        lblActiveOrders.TabIndex = 3;
        lblActiveOrders.Text = "Active orders";
        // 
        // lblActiveOrdersValue
        // 
        lblActiveOrdersValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblActiveOrdersValue.Location = new Point(260, 180);
        lblActiveOrdersValue.Name = "lblActiveOrdersValue";
        lblActiveOrdersValue.Size = new Size(200, 35);
        lblActiveOrdersValue.TabIndex = 4;
        lblActiveOrdersValue.Text = "0";
        // 
        // lblInProduction
        // 
        lblInProduction.Font = new Font("Segoe UI", 14F);
        lblInProduction.Location = new Point(40, 240);
        lblInProduction.Name = "lblInProduction";
        lblInProduction.Size = new Size(200, 35);
        lblInProduction.TabIndex = 5;
        lblInProduction.Text = "In production";
        // 
        // lblInProductionValue
        // 
        lblInProductionValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblInProductionValue.Location = new Point(260, 240);
        lblInProductionValue.Name = "lblInProductionValue";
        lblInProductionValue.Size = new Size(200, 35);
        lblInProductionValue.TabIndex = 6;
        lblInProductionValue.Text = "0";
        // 
        // lblTotalSales
        // 
        lblTotalSales.Font = new Font("Segoe UI", 14F);
        lblTotalSales.Location = new Point(40, 300);
        lblTotalSales.Name = "lblTotalSales";
        lblTotalSales.Size = new Size(200, 35);
        lblTotalSales.TabIndex = 7;
        lblTotalSales.Text = "Total sales";
        // 
        // lblTotalSalesValue
        // 
        lblTotalSalesValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTotalSalesValue.Location = new Point(260, 300);
        lblTotalSalesValue.Name = "lblTotalSalesValue";
        lblTotalSalesValue.Size = new Size(200, 35);
        lblTotalSalesValue.TabIndex = 8;
        lblTotalSalesValue.Text = "0 kr";
        // 
        // tabCustomers
        // 
        tabCustomers.Controls.Add(lblCustomerName);
        tabCustomers.Controls.Add(lblCustomerEmail);
        tabCustomers.Controls.Add(lblCustomerPhone);
        tabCustomers.Controls.Add(txtCustomerName);
        tabCustomers.Controls.Add(txtCustomerEmail);
        tabCustomers.Controls.Add(txtCustomerPhone);
        tabCustomers.Controls.Add(btnAddCustomer);
        tabCustomers.Controls.Add(lstCustomers);
        tabCustomers.Location = new Point(4, 29);
        tabCustomers.Name = "tabCustomers";
        tabCustomers.Size = new Size(1012, 617);
        tabCustomers.TabIndex = 1;
        tabCustomers.Text = "Customers";
        // 
        // lblCustomerName
        // 
        lblCustomerName.Location = new Point(30, 30);
        lblCustomerName.Name = "lblCustomerName";
        lblCustomerName.Size = new Size(100, 23);
        lblCustomerName.TabIndex = 0;
        lblCustomerName.Text = "Name";
        // 
        // lblCustomerEmail
        // 
        lblCustomerEmail.Location = new Point(30, 70);
        lblCustomerEmail.Name = "lblCustomerEmail";
        lblCustomerEmail.Size = new Size(100, 23);
        lblCustomerEmail.TabIndex = 1;
        lblCustomerEmail.Text = "Email";
        // 
        // lblCustomerPhone
        // 
        lblCustomerPhone.Location = new Point(30, 110);
        lblCustomerPhone.Name = "lblCustomerPhone";
        lblCustomerPhone.Size = new Size(100, 23);
        lblCustomerPhone.TabIndex = 2;
        lblCustomerPhone.Text = "Phone";
        // 
        // txtCustomerName
        // 
        txtCustomerName.Location = new Point(130, 28);
        txtCustomerName.Name = "txtCustomerName";
        txtCustomerName.Size = new Size(260, 27);
        txtCustomerName.TabIndex = 3;
        // 
        // txtCustomerEmail
        // 
        txtCustomerEmail.Location = new Point(130, 68);
        txtCustomerEmail.Name = "txtCustomerEmail";
        txtCustomerEmail.Size = new Size(260, 27);
        txtCustomerEmail.TabIndex = 4;
        // 
        // txtCustomerPhone
        // 
        txtCustomerPhone.Location = new Point(130, 108);
        txtCustomerPhone.Name = "txtCustomerPhone";
        txtCustomerPhone.Size = new Size(260, 27);
        txtCustomerPhone.TabIndex = 5;
        // 
        // btnAddCustomer
        // 
        btnAddCustomer.Location = new Point(130, 150);
        btnAddCustomer.Name = "btnAddCustomer";
        btnAddCustomer.Size = new Size(130, 32);
        btnAddCustomer.TabIndex = 6;
        btnAddCustomer.Text = "Add customer";
        btnAddCustomer.Click += btnAddCustomer_Click;
        // 
        // lstCustomers
        // 
        lstCustomers.Location = new Point(430, 28);
        lstCustomers.Name = "lstCustomers";
        lstCustomers.Size = new Size(520, 504);
        lstCustomers.TabIndex = 7;
        // 
        // tabProducts
        // 
        tabProducts.Controls.Add(lblProductsHeader);
        tabProducts.Controls.Add(lstProducts);
        tabProducts.Location = new Point(4, 29);
        tabProducts.Name = "tabProducts";
        tabProducts.Size = new Size(1012, 617);
        tabProducts.TabIndex = 2;
        tabProducts.Text = "Products";
        // 
        // lblProductsHeader
        // 
        lblProductsHeader.Location = new Point(30, 30);
        lblProductsHeader.Name = "lblProductsHeader";
        lblProductsHeader.Size = new Size(200, 23);
        lblProductsHeader.TabIndex = 0;
        lblProductsHeader.Text = "Seeded FRMD products";
        // 
        // lstProducts
        // 
        lstProducts.Location = new Point(30, 65);
        lstProducts.Name = "lstProducts";
        lstProducts.Size = new Size(920, 464);
        lstProducts.TabIndex = 1;
        // 
        // tabOrders
        // 
        tabOrders.Controls.Add(lblCustomerPick);
        tabOrders.Controls.Add(lblProductPick);
        tabOrders.Controls.Add(lblQuantity);
        tabOrders.Controls.Add(lblNotes);
        tabOrders.Controls.Add(lblOrderDetails);
        tabOrders.Controls.Add(lblSelectedNotes);
        tabOrders.Controls.Add(lblStatus);
        tabOrders.Controls.Add(cmbOrderCustomer);
        tabOrders.Controls.Add(cmbOrderProduct);
        tabOrders.Controls.Add(numQuantity);
        tabOrders.Controls.Add(txtOrderNotes);
        tabOrders.Controls.Add(btnCreateOrder);
        tabOrders.Controls.Add(gridOrders);
        tabOrders.Controls.Add(lstOrderDetails);
        tabOrders.Controls.Add(txtSelectedOrderNotes);
        tabOrders.Controls.Add(cmbStatus);
        tabOrders.Controls.Add(btnUpdateStatus);
        tabOrders.Location = new Point(4, 29);
        tabOrders.Name = "tabOrders";
        tabOrders.Size = new Size(1012, 617);
        tabOrders.TabIndex = 3;
        tabOrders.Text = "Orders";
        // 
        // lblCustomerPick
        // 
        lblCustomerPick.Location = new Point(30, 30);
        lblCustomerPick.Name = "lblCustomerPick";
        lblCustomerPick.Size = new Size(100, 23);
        lblCustomerPick.TabIndex = 0;
        lblCustomerPick.Text = "Customer";
        // 
        // lblProductPick
        // 
        lblProductPick.Location = new Point(30, 70);
        lblProductPick.Name = "lblProductPick";
        lblProductPick.Size = new Size(100, 23);
        lblProductPick.TabIndex = 1;
        lblProductPick.Text = "Product";
        // 
        // lblQuantity
        // 
        lblQuantity.Location = new Point(30, 110);
        lblQuantity.Name = "lblQuantity";
        lblQuantity.Size = new Size(100, 23);
        lblQuantity.TabIndex = 2;
        lblQuantity.Text = "Quantity";
        // 
        // lblNotes
        // 
        lblNotes.Location = new Point(30, 150);
        lblNotes.Name = "lblNotes";
        lblNotes.Size = new Size(100, 23);
        lblNotes.TabIndex = 3;
        lblNotes.Text = "Notes";
        // 
        // lblOrderDetails
        // 
        lblOrderDetails.Location = new Point(430, 345);
        lblOrderDetails.Name = "lblOrderDetails";
        lblOrderDetails.Size = new Size(150, 23);
        lblOrderDetails.TabIndex = 4;
        lblOrderDetails.Text = "Order details";
        // 
        // lblSelectedNotes
        // 
        lblSelectedNotes.Location = new Point(430, 455);
        lblSelectedNotes.Name = "lblSelectedNotes";
        lblSelectedNotes.Size = new Size(100, 23);
        lblSelectedNotes.TabIndex = 5;
        lblSelectedNotes.Text = "Notes";
        // 
        // lblStatus
        // 
        lblStatus.Location = new Point(430, 550);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(100, 23);
        lblStatus.TabIndex = 6;
        lblStatus.Text = "Status";
        // 
        // cmbOrderCustomer
        // 
        cmbOrderCustomer.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbOrderCustomer.Location = new Point(130, 28);
        cmbOrderCustomer.Name = "cmbOrderCustomer";
        cmbOrderCustomer.Size = new Size(280, 28);
        cmbOrderCustomer.TabIndex = 7;
        // 
        // cmbOrderProduct
        // 
        cmbOrderProduct.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbOrderProduct.Location = new Point(130, 68);
        cmbOrderProduct.Name = "cmbOrderProduct";
        cmbOrderProduct.Size = new Size(280, 28);
        cmbOrderProduct.TabIndex = 8;
        // 
        // numQuantity
        // 
        numQuantity.Location = new Point(130, 108);
        numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numQuantity.Name = "numQuantity";
        numQuantity.Size = new Size(120, 27);
        numQuantity.TabIndex = 9;
        numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // txtOrderNotes
        // 
        txtOrderNotes.Location = new Point(130, 148);
        txtOrderNotes.Multiline = true;
        txtOrderNotes.Name = "txtOrderNotes";
        txtOrderNotes.Size = new Size(280, 70);
        txtOrderNotes.TabIndex = 10;
        // 
        // btnCreateOrder
        // 
        btnCreateOrder.Location = new Point(130, 235);
        btnCreateOrder.Name = "btnCreateOrder";
        btnCreateOrder.Size = new Size(130, 32);
        btnCreateOrder.TabIndex = 11;
        btnCreateOrder.Text = "Create order";
        btnCreateOrder.Click += btnCreateOrder_Click;
        // 
        // gridOrders
        // 
        gridOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        gridOrders.ColumnHeadersHeight = 29;
        gridOrders.Location = new Point(430, 28);
        gridOrders.MultiSelect = false;
        gridOrders.Name = "gridOrders";
        gridOrders.ReadOnly = true;
        gridOrders.RowHeadersWidth = 51;
        gridOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        gridOrders.Size = new Size(540, 300);
        gridOrders.TabIndex = 12;
        gridOrders.SelectionChanged += gridOrders_SelectionChanged;
        // 
        // lstOrderDetails
        // 
        lstOrderDetails.Location = new Point(430, 375);
        lstOrderDetails.Name = "lstOrderDetails";
        lstOrderDetails.Size = new Size(540, 64);
        lstOrderDetails.TabIndex = 13;
        // 
        // txtSelectedOrderNotes
        // 
        txtSelectedOrderNotes.BackColor = SystemColors.Window;
        txtSelectedOrderNotes.Location = new Point(430, 485);
        txtSelectedOrderNotes.Multiline = true;
        txtSelectedOrderNotes.Name = "txtSelectedOrderNotes";
        txtSelectedOrderNotes.ReadOnly = true;
        txtSelectedOrderNotes.Size = new Size(540, 50);
        txtSelectedOrderNotes.TabIndex = 14;
        // 
        // cmbStatus
        // 
        cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbStatus.Location = new Point(540, 548);
        cmbStatus.Name = "cmbStatus";
        cmbStatus.Size = new Size(170, 28);
        cmbStatus.TabIndex = 15;
        // 
        // btnUpdateStatus
        // 
        btnUpdateStatus.Location = new Point(730, 544);
        btnUpdateStatus.Name = "btnUpdateStatus";
        btnUpdateStatus.Size = new Size(130, 32);
        btnUpdateStatus.TabIndex = 16;
        btnUpdateStatus.Text = "Update status";
        btnUpdateStatus.Click += btnUpdateStatus_Click;
        // 
        // linkLabel1
        // 
        linkLabel1.AutoSize = true;
        linkLabel1.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        linkLabel1.Location = new Point(893, 30);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(90, 31);
        linkLabel1.TabIndex = 9;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "frmd.se";
        linkLabel1.LinkClicked += linkLabel1_LinkClicked;
        // 
        // MainForm
        // 
        ClientSize = new Size(1020, 650);
        Controls.Add(tabMain);
        MinimumSize = new Size(980, 620);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FRMD Order Manager";
        tabMain.ResumeLayout(false);
        tabDashboard.ResumeLayout(false);
        tabDashboard.PerformLayout();
        tabCustomers.ResumeLayout(false);
        tabCustomers.PerformLayout();
        tabProducts.ResumeLayout(false);
        tabOrders.ResumeLayout(false);
        tabOrders.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridOrders).EndInit();
        ResumeLayout(false);
    }

    private LinkLabel linkLabel1;
    private Label lblCustomerName;
    private Label lblCustomerEmail;
    private Label lblCustomerPhone;
    private Label lblProductsHeader;
    private Label lblCustomerPick;
    private Label lblProductPick;
    private Label lblQuantity;
    private Label lblNotes;
    private Label lblOrderDetails;
    private Label lblSelectedNotes;
    private Label lblStatus;
}
