using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OicdDemo.Entities;

[Table("files")]
public class FileDto
{
    [Key]
    [Column("id")]
    public int? Id { get; set; }
    public string? FriendlyName { get; set; }
    public string? HashedName { get; set; }
}
