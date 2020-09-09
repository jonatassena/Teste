using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teste.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string AuditDetails { get; set; }

        public string UserEmail { get; set; }
    }
}
