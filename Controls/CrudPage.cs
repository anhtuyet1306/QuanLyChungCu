using QuanLyChungCu_SkyResidence.Services;

namespace QuanLyChungCu_SkyResidence.Controls;

/// <summary>Form con trả lại bản ghi sau khi service đã lưu thành công.</summary>
public interface IEntityEditor<T> where T : class
{
    T? Result { get; }
}

/// <summary>Quy trình thêm/sửa/xóa dùng chung; từng trang cung cấp form và nghiệp vụ riêng.</summary>
public abstract class CrudPage<T> : BasePage where T : class
{
    private readonly AuthUser _user;
    private readonly string _feature;
    private readonly string _entityName;

    protected CrudPage(string title, string description, AuthUser user, string feature, string entityName)
        : base(title, description)
    {
        _user = user;
        _feature = feature;
        _entityName = entityName;
        SetCrudButtons(CanView && user.HasPermission(feature, "them"),
            CanView && user.HasPermission(feature, "sua"), CanView && user.HasPermission(feature, "xoa"));
    }

    protected bool CanView => _user.HasPermission(_feature, "xem");

    protected abstract Form CreateEditor(T? item);
    protected abstract string GetKey(T item);
    protected abstract OperationResult DeleteItem(T item);

    private bool CanExecute(string action)
    {
        if (_user.HasPermission(_feature, "xem") && _user.HasPermission(_feature, action))
            return true;
        ShowWarning("Tài khoản của bạn không được phép thực hiện thao tác này.");
        return false;
    }

    protected sealed override void OnAdd() => EditItem(isNew: true);
    protected sealed override void OnEdit() => EditItem(isNew: false);

    private void EditItem(bool isNew)
    {
        // Kiểm tra trong hàm xử lý, nên nhấp đúp cũng không vượt qua được quyền sửa.
        if (!CanExecute(isNew ? "them" : "sua")) return;
        T? item = isNew ? null : SelectedItem<T>();
        if (!isNew && item is null)
        {
            ShowWarning($"Vui lòng chọn {_entityName} cần sửa.");
            return;
        }
        using var form = CreateEditor(item);
        if (form is not IEntityEditor<T> editor)
            throw new InvalidOperationException("Form phải triển khai IEntityEditor<T>.");
        if (form.ShowDialog(this) != DialogResult.OK || editor.Result is null) return;
        string key = GetKey(editor.Result);
        SearchBox.Clear();
        LoadData();
        SelectRow<T>(x => GetKey(x) == key);
    }

    protected sealed override void OnDelete()
    {
        if (!CanExecute("xoa")) return;
        var item = SelectedItem<T>();
        if (item is null)
        {
            ShowWarning($"Vui lòng chọn {_entityName} cần xóa.");
            return;
        }
        if (!Confirm($"Xóa bản ghi {_entityName} {GetKey(item)}?\nChỉ xóa bản ghi nhập sai chưa có dữ liệu liên quan.")) return;
        var result = DeleteItem(item);
        if (!result.Success) { ShowWarning(result.Message); return; }
        ShowInfo(result.Message);
        LoadData();
    }
}
