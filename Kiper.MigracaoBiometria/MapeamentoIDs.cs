using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kiper.MigracaoBiometria
{
    public class MapeamentoIDs
    {
        [JsonPropertyName("idSigma")]

        public long IdSigma { get; set; }
        [JsonPropertyName("idMonitoring")]
        public long IdMonitoring { get; set; }
        [JsonPropertyName("condominiumId")]
        public int CondominiumId { get; set; }

    }
}
