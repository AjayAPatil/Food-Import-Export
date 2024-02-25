namespace Food.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class KeyValueModel
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
