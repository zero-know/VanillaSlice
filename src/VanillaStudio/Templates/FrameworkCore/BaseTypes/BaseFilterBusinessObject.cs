namespace {{ProjectName}}.Framework
{
    public class BaseFilterBusinessObject
    {
        public int CurrentIndex { get; set; } = 1;
        public int RowsPerPage { get; set; } = 10;
        public bool UsePagination { get; set; } = true; 
        public string? SearchKey { get; set; }
    }
}
