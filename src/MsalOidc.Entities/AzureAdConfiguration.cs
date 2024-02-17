using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OicdDemo.Entities;

public class AzureAdConfiguration
{
    public string Audience { get; set; }
    public string Authority { get; set; }
    public string Issuer { get; set; }
}
