using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace jjwebapicore.Models
{
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactId { get; set; }
        public string FullName { get; set; }
     }
}
