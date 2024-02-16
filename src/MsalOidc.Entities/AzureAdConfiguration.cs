using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OicdDemo.Entities;

public class AzureAdConfiguration
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Audience { get; set; }
    public string Authority { get; set; }
    public string ReturnUrl { get; set; }
    public string MetadataAddress { get; set; }
}
