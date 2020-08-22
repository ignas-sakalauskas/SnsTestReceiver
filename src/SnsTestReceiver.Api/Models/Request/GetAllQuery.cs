using System.ComponentModel.DataAnnotations;

namespace SnsTestReceiver.Api.Models.Request
{
    public class GetAllQuery
    {
        [MaxLength(500)]
        public string Search { get; set; }

        [Range(1, 100)]
        public int? Limit { get; set; }
    }
}
