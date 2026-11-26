
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Helpers;
using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Controls;

/// <summary>
/// Trang dạng bảng dùng chung.
/// Trang con có thể ghi đè LoadData / OnAdd / OnEdit / OnDelete / OnSearch.
/// Hỗ trợ phân quyền xem, thêm, sửa, xóa.
/// </summary>
public class BasePage : UserControl
{
    // ===== MÀU SẮC DÙNG CHUNG =====
    protected readonly Color Navy = Theme.Navy;
    protected readonly Color Blue = Theme.Blue;
    protected readonly Color Background = Theme.Background;

    // ===== ĐIỀU KHIỂN DÙNG CHUNG =====
    protected DataGridView Grid { get; }
    protected TextBox SearchBox { get; }

    private readonly Label statusLabel;
    private readonly Button addButton;
    private readonly Button editButton;
    private readonly Button deleteButton;

    // ===== THÔNG TIN PHÂN QUYỀN =====
    private AuthUser? permissionUser;
    private string? permissionFeature;

    /// <summary>
    /// Khởi tạo trạng thái các nút theo quyền của tài khoản.
    /// </summary>
    protected void SetCrudButtons(bool canAdd, bool canEdit, bool canDelete)
    {
        addButton.Enabled = canAdd && HasPermission("them");
        editButton.Enabled = canEdit && HasPermission("sua");
        deleteButton.Enabled = canDelete && HasPermission("xoa");
    }

    /// <summary>
    /// Kiểm tra tài khoản có quyền xem trang hay không.
    /// Nếu chưa truyền tài khoản thì giữ cách hoạt động cũ.
    /// </summary>
    protected bool CanViewPage()
    {
        return permissionUser == null
            || string.IsNullOrWhiteSpace(permissionFeature)
            || permissionUser.HasPermission(permissionFeature, "xem");
    }

    /// <summary>
    /// Kiểm tra quyền của một thao tác.
    /// Các trang chưa truyền tài khoản vẫn hoạt động như trước.
    /// </summary>
    private bool HasPermission(string action)
    {
        if (permissionUser == null ||
            string.IsNullOrWhiteSpace(permissionFeature))
        {
            return true;
        }

        return permissionUser.HasPermission(permissionFeature, "xem")
            && permissionUser.HasPermission(permissionFeature, action);
    }

    /// <summary>
    /// Kiểm tra quyền trước khi thực hiện thao tác.
    /// </summary>
    private bool CheckPermission(string action)
    {
        if (HasPermission(action))
            return true;

        ShowWarning("Tài khoản của bạn không có quyền thực hiện chức năng này.");
        return false;
    }

    /// <summary>
    /// Khởi tạo trang theo cách cũ.
    /// Dùng cho các trang chưa kết nối phân quyền.
    /// </summary>
    public BasePage(string title, string description)
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.Background;
        Padding = new Padding(22);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Theme.Background
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Controls.Add(layout);

        // ===== TIÊU ĐỀ =====
        var heading = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = Theme.HeadingFont,
            ForeColor = Theme.Navy,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var sub = new Label
        {
            Text = description,
            Dock = DockStyle.Fill,
            Font = Theme.SubHeadingFont,
            ForeColor = Theme.Muted,
            TextAlign = ContentAlignment.MiddleLeft
        };

        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(sub, 0, 1);

        // ===== KHU VỰC NỘI DUNG =====
        var body = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Theme.White,
            Padding = new Padding(14)
        };

        layout.Controls.Add(body, 0, 2);

        var bodyLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Theme.White
        };

        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));

        body.Controls.Add(bodyLayout);

        // ===== THANH CÔNG CỤ =====
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Theme.White,
            Padding = new Padding(0, 4, 0, 4)
        };

        var search = new TextBox
        {
            Width = 260,
            Height = 32,
            Font = Theme.NormalFont,
            PlaceholderText = "Nhập từ khóa tìm kiếm..."
        };

        var searchButton = CreateButton("Tìm kiếm", Theme.Blue);
        addButton = CreateButton("+ Thêm mới", Theme.Green);
        editButton = CreateButton("Sửa", Theme.Orange);
        deleteButton = CreateButton("Xóa", Theme.Red);
        var refreshButton = CreateButton("Làm mới", Theme.Muted);

        toolbar.Controls.AddRange(new Control[]
        {
            search,
            searchButton,
            addButton,
            editButton,
            deleteButton,
            refreshButton
        });

        bodyLayout.Controls.Add(toolbar, 0, 0);

        // ===== BẢNG DỮ LIỆU =====
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Theme.White,
            BorderStyle = BorderStyle.None,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = Theme.NormalFont,
            ColumnHeadersHeight = 42,
            RowTemplate = { Height = 36 },
            EnableHeadersVisualStyles = false
        };

        Theme.ApplyGrid(grid);

        bodyLayout.Controls.Add(grid, 0, 1);

        // ===== DÒNG TRẠNG THÁI =====
        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = Theme.SubHeadingFont,
            ForeColor = Theme.Muted,
            TextAlign = ContentAlignment.MiddleLeft
        };

        bodyLayout.Controls.Add(statusLabel, 0, 2);

        // ===== GÁN SỰ KIỆN =====

        search.TextChanged += (_, _) =>
        {
            if (CanViewPage())
                Safe(() => OnSearch(search.Text));
        };

        searchButton.Click += (_, _) =>
        {
            if (CanViewPage())
                Safe(() => OnSearch(search.Text));
            else
                ShowWarning("Bạn không có quyền xem dữ liệu của trang này.");
        };

        search.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (CanViewPage())
                    Safe(() => OnSearch(search.Text));
                else
                    ShowWarning("Bạn không có quyền xem dữ liệu của trang này.");
            }
        };

        refreshButton.Click += (_, _) =>
        {
            if (!CanViewPage())
            {
                ShowWarning("Bạn không có quyền xem dữ liệu của trang này.");
                return;
            }

            search.Clear();
            Safe(LoadData);
        };

        addButton.Click += (_, _) =>
        {
            if (CheckPermission("them"))
                Safe(OnAdd);
        };

        editButton.Click += (_, _) =>
        {
            if (CheckPermission("sua"))
                Safe(OnEdit);
        };

        deleteButton.Click += (_, _) =>
        {
            if (CheckPermission("xoa"))
                Safe(OnDelete);
        };

        grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0 && CheckPermission("sua"))
                Safe(OnEdit);
        };

        Grid = grid;
        SearchBox = search;
    }

    /// <summary>
    /// Khởi tạo trang có phân quyền.
    /// feature phải trùng mã chức năng trong bảng PhanQuyen.
    /// </summary>
    public BasePage(
        string title,
        string description,
        AuthUser user,
        string feature
    ) : this(title, description)
    {
        permissionUser = user
            ?? throw new ArgumentNullException(nameof(user));

        permissionFeature = feature;

        SetCrudButtons(true, true, true);

        if (!CanViewPage())
        {
            Grid.DataSource = null;
            Grid.Rows.Clear();
            SetStatus("Tài khoản không có quyền xem chức năng này.");
        }
    }

    // =====================================================================
    // CÁC HÀM ẢO - TRANG CON GHI ĐÈ
    // =====================================================================

    /// <summary>Nạp dữ liệu từ cơ sở dữ liệu.</summary>
    protected virtual void LoadData() { }

    protected virtual void OnAdd() =>
        ShowInfo("Chức năng thêm dữ liệu chưa được kết nối.");

    protected virtual void OnEdit() =>
        ShowInfo("Chức năng sửa dữ liệu chưa được kết nối.");

    protected virtual void OnDelete() =>
        ShowInfo("Chức năng xóa dữ liệu chưa được kết nối.");

    /// <summary>Lọc dữ liệu.</summary>
    protected virtual void OnSearch(string keyword) =>
        FilterRows(Grid, keyword);

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!DesignMode && CanViewPage())
            Safe(LoadData);
    }

    // =====================================================================
    // TIỆN ÍCH CHO TRANG CON
    // =====================================================================

    /// <summary>Khai báo cột gắn với thuộc tính của model.</summary>
    protected void AddColumn(
        string header,
        string dataProperty,
        float weight = 100,
        bool center = false,
        string? format = null)
    {
        var col = new DataGridViewTextBoxColumn
        {
            Name = dataProperty,
            HeaderText = header,
            DataPropertyName = dataProperty,
            FillWeight = weight
        };

        if (center)
            col.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

        if (format != null)
            col.DefaultCellStyle.Format = format;

        Grid.Columns.Add(col);
    }

    /// <summary>Đổ danh sách đối tượng lên lưới.</summary>
    protected void Bind<T>(IList<T> items, string? statusText = null)
    {
        Grid.DataSource = items;
        Grid.ClearSelection();
        SetStatus(statusText ?? $"Tổng: {items.Count} bản ghi");
    }

    /// <summary>Lấy đối tượng của dòng đang chọn.</summary>
    protected T? SelectedItem<T>() where T : class =>
        Grid.SelectedRows.Count == 0
            ? null
            : Grid.SelectedRows[0].DataBoundItem as T;

    /// <summary>Chọn lại dòng thỏa điều kiện.</summary>
    protected void SelectRow<T>(Func<T, bool> match) where T : class
    {
        foreach (DataGridViewRow row in Grid.Rows)
        {
            if (row.DataBoundItem is T item && match(item))
            {
                Grid.CurrentCell = row.Cells[0];
                row.Selected = true;
                return;
            }
        }
    }

    protected void SetStatus(string text) =>
        statusLabel.Text = text;

    /// <summary>Chạy hành động và hiển thị lỗi nếu có.</summary>
    protected void Safe(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            ShowError("Đã xảy ra lỗi:\n" + ex.Message);
        }
    }

    protected static void ShowInfo(string message) =>
        MessageBox.Show(
            message,
            "Thông báo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

    protected static void ShowWarning(string message) =>
        MessageBox.Show(
            message,
            "Thông báo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);

    protected static void ShowError(string message) =>
        MessageBox.Show(
            message,
            "Lỗi",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);

    protected static bool Confirm(string message) =>
        MessageBox.Show(
            message,
            "Xác nhận",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)
        == DialogResult.Yes;

    // ===== HỖ TRỢ CÁC TRANG DỮ LIỆU GIẢ =====

    protected DataGridViewColumn GetColumn(string name)
    {
        return Grid.Columns[name]
            ?? throw new InvalidOperationException(
                $"Không tìm thấy cột: {name}");
    }

    protected void AddColumns(params string[] columns)
    {
        foreach (string column in columns)
            Grid.Columns.Add(column, column);
    }

    protected void AddRows(params string[][] rows)
    {
        foreach (string[] row in rows)
            Grid.Rows.Add(row);
    }

    // =====================================================================
    // HÀM TẠO NÚT VÀ LỌC DỮ LIỆU
    // =====================================================================

    private static Button CreateButton(string text, Color color)
    {
        var button = new Button
        {
            Text = text,
            Width = 102,
            Height = 32,
            BackColor = color,
            ForeColor = Theme.White,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.ButtonFont,
            Margin = new Padding(5, 2, 5, 2),
            Cursor = Cursors.Hand
        };

        button.FlatAppearance.BorderSize = 0;

        return button;
    }

    private static void FilterRows(
        DataGridView grid,
        string keyword)
    {
        if (grid.DataSource != null)
            return;

        foreach (DataGridViewRow row in grid.Rows)
        {
            bool visible = string.IsNullOrWhiteSpace(keyword);

            if (!visible)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (TextHelper.ContainsIgnoreAccent(
                        cell.Value?.ToString(),
                        keyword))
                    {
                        visible = true;
                        break;
                    }
                }
            }

            row.Visible = visible;
        }
    }
}
