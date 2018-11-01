using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rodolfo.Domain.Models
{
    public class MessageEntity : TableEntity
    {
        public string caption { get; set; }

        public string description { get; set; }

        public string imageUrl { get; set; }

        public string created_time { get; set; }

        public string location { get; set; }
    }
}
