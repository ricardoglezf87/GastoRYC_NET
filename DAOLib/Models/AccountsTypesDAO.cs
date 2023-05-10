using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DAOLib.Models
{
    [Table("AccountsTypes")]
    public class AccountsTypesDAO : IModelDAO
    {
        public virtual String? description { set; get; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual List<AccountsDAO>? accounts { set; get; }
    }
}
