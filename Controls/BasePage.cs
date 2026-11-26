using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChungCu_SkyResidence.Helpers;

namespace QuanLyChungCu_SkyResidence.Controls;

/// <summary>
/// Trang dạng bảng dùng chung. Trang con chỉ cần:
///   1. Gọi AddColumn(...) trong constructor để khai báo cột.
///   2. Ghi đè LoadData / OnAdd / OnEdit / OnDelete / OnSearch khi cần.
/// Các trang CHƯA làm (Cư dân, Phí...) vẫn chạy như cũ với AddColumns/AddRows.
/// </summary>
public class BasePage : UserControl
{
    protected readonly Color Navy = Color.FromArgb(18, 53, 94);
    protected readonly Color Blue = Color.FromArgb(43, 125, 232);
    protected readonly Color Background = Color.FromArgb(244, 248, 255);

    protected DataGridView Grid { get; }
    protected TextBox SearchBox { get; }

    private readonly Label statusLabel;
    private readonly Button addButton;
    private readonly Button editButton;
    private readonly Button deleteButton;

    protected void SetCrudButtons(bool canAdd, bool canEdit, bool canDelete)
    {
        addButton.Enabled = canAdd;
        editButton.Enabled = canEdit;
        deleteButton.Enabled = canDelete;
    }

    public BasePage(string title, string description)
    {
        Dock = DockStyle.Fill;
        BackColor = Background;
        Padding = new Padding(22);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Background
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        Controls.Add(layout);

        var heading = new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 19, FontStyle.Bold),
            ForeColor = Navy,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var sub = new Label
        {
            Text = description,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.DimGray,
            TextAlign = ContentAlignment.MiddleLeft
        };

        layout.Controls.Add(heading, 0, 0);
        layout.Controls.Add(sub, 0, 1);

        var body = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(14)
        };
        layout.Controls.Add(body, 0, 2);

        var bodyLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        bodyLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        body.Controls.Add(bodyLayout);

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.White,
            Padding = new Padding(0, 4, 0, 4)
        };

        var search = new TextBox
        {
            Width = 260,
            Height = 32,
            Font = new Font("Segoe UI", 10),
            PlaceholderText = "Nhập từ khóa tìm kiếm..."
        };

        var searchButton = CreateButton("Tìm kiếm", Blue);
        addButton = CreateButton("+ Thêm mới", Color.FromArgb(32, 160, 100));
        editButton = CreateButton("Sửa", Color.FromArgb(235, 158, 45));
        deleteButton = CreateButton("Xóa", Color.FromArgb(210, 70, 70));
        var refreshButton = CreateButton("Làm mới", Color.FromArgb(95, 110, 130));

        toolbar.Controls.AddRange(new Control[]
        {
            search, searchButton, addButton, editButton, deleteButton, refreshButton
        });
        bodyLayout.Controls.Add(toolbar, 0, 0);

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 9.5f),
            ColumnHeadersHeight = 42,
            RowTemplate = { Height = 36 },
            EnableHeadersVisualStyles = false
        };

        grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(222, 235, 252);
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Navy;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 230, 255);
        grid.DefaultCellStyle.SelectionForeColor = Navy;
        bodyLayout.Controls.Add(grid, 0, 1);

        statusLabel = new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.DimGray,
            TextAlign = ContentAlignment.MiddleLeft
        };
        bodyLayout.Controls.Add(statusLabel, 0, 2);

        // ===== Nối sự kiện vào các hàm ảo để trang con ghi đè =====
        search.TextChanged += (_, _) => Safe(() => OnSearch(search.Text));
        searchButton.Click += (_, _) => Safe(() => OnSearch(search.Text));
        search.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Safe(() => OnSearch(search.Text));
            }
        };
        refreshButton.Click += (_, _) =>
        {
            search.Clear();
            Safe(LoadData);
        };
        addButton.Click += (_, _) => Safe(OnAdd);
        editButton.Click += (_, _) => Safe(OnEdit);
        deleteButton.Click += (_, _) => Safe(OnDelete);
        grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0)
                Safe(OnEdit);
        };

        Grid = grid;
        SearchBox = search;
    }

    // =====================================================================
    //  CÁC HÀM ẢO - TRANG CON GHI ĐÈ
    // =====================================================================

    /// <summary>Nạp dữ liệu từ CSDL. Tự được gọi một lần khi trang hiện ra và khi bấm "Làm mới".</summary>
    protected virtual void LoadData() { }

    protected virtual void OnAdd() =>
        ShowInfo("Chức năng thêm dữ liệu sẽ được kết nối ở giai đoạn tiếp theo.");

    protected virtual void OnEdit() =>
        ShowInfo("Chức năng sửa sẽ được lập trình ở giai đoạn tiếp theo.");

    protected virtual void OnDelete() =>
        ShowInfo("Chức năng xóa sẽ được lập trình ở giai đoạn tiếp theo.");

    /// <summary>Mặc định lọc dòng trên lưới chưa gắn dữ liệu. Trang gắn dữ liệu (Bind) phải ghi đè.</summary>
    protected virtual void OnSearch(string keyword) => FilterRows(Grid, keyword);

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!DesignMode)
            Safe(LoadData);
    }

    // =====================================================================
    //  TIỆN ÍCH CHO TRANG CON
    // =====================================================================

    /// <summary>Khai báo cột gắn với thuộc tính của model (dùng khi Bind).</summary>
    protected void AddColumn(string header, string dataProperty, float weight = 100,
                             bool center = false, string? format = null)
    {
        var col = new DataGridViewTextBoxColumn
        {
            Name = dataProperty,
            HeaderText = header,
            DataPropertyName = dataProperty,
            FillWeight = weight
        };
        if (center)
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

    /// <summary>Lấy đối tượng của dòng đang chọn (null nếu chưa chọn dòng nào).</summary>
    protected T? SelectedItem<T>() where T : class =>
        Grid.SelectedRows.Count == 0 ? null : Grid.SelectedRows[0].DataBoundItem as T;

    /// <summary>Chọn lại dòng thỏa điều kiện (VD: sau khi thêm/sửa xong).</summary>
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

    protected void SetStatus(string text) => statusLabel.Text = text;

    /// <summary>Chạy một hành động, nếu lỗi thì báo cho người dùng thay vì làm sập chương trình.</summary>
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
        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

    protected static void ShowWarning(string message) =>
        MessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);

    protected static void ShowError(string message) =>
        MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

    protected static bool Confirm(string message) =>
        MessageBox.Show(message, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            == DialogResult.Yes;

    // ===== Giữ nguyên cho các trang chưa làm (dữ liệu giả) =====

    protected DataGridViewColumn GetColumn(string name)
    {
        return Grid.Columns[name]
            ?? throw new InvalidOperationException($"Không tìm thấy cột: {name}");
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

    private static Button CreateButton(string text, Color color)
    {
        var button = new Button
        {
            Text = text,
            Width = 102,
            Height = 32,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Margin = new Padding(5, 2, 5, 2),
            Cursor = Cursors.Hand
        };
        button.FlatAppearance.BorderSize = 0;
        return button;
    }

    private static void FilterRows(DataGridView grid, string keyword)
    {
        // Lưới đã Bind dữ liệu thì không thể ẩn dòng, trang con phải tự lọc danh sách.
        if (grid.DataSource != null)
            return;

        foreach (DataGridViewRow row in grid.Rows)
        {
            bool visible = string.IsNullOrWhiteSpace(keyword);

            if (!visible)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (TextHelper.ContainsIgnoreAccent(cell.Value?.ToString(), keyword))
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
