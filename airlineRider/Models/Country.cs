using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace airlineRider.Models;

[PrimaryKey("Name")]
public class Country
{
    public string Name { get; set; }
    public string Region { get; set; }
    [MaxLength(2)]
    public string Alpha2 { get; set; }
    [MaxLength(3)]
    public string Alpha3 { get; set; }
    [MaxLength(5)]
    public string IcaoPrefix { get; set; }
}