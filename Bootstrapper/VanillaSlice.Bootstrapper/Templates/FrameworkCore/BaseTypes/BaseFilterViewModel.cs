namespace {{ProjectName}}.Framework
{
    public class BaseFilterViewModel : ObservableBase
    {
        public int CurrentIndex { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public bool UsePagination { get; set; } = true;

        private string? _searchKey;
        public string? SearchKey
        {
            get { return _searchKey; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    CurrentIndex = 1;
                SetField(ref _searchKey, value);
            }
        }
    }
}
