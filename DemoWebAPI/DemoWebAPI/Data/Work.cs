using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoWebAPI.Data
{
    [Table("Work")]
    public class Work
    {
        [Key]
        public int WorkId { get; set; }
        [Required]
        [MaxLength(100)]
        public string WorkName { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public bool IsStatus { get; set; }
        public int? Id { get; set; }
        [ForeignKey("Id")]
        public virtual User User { get; set; }
    }
}
