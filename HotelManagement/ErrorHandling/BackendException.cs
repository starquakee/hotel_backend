using System.Text.Json.Nodes;

namespace HotelManagement.ErrorHandling
{
    public class BackendException : Exception
    {
        public int Code { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public JsonObject ToJson() 
        { 

        var jsonObject = new JsonObject
        {
            { "code", Code },
            { "message", ErrorMessage }
        };
        return jsonObject;

        }    
    }
}
