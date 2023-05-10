using DAOLib.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace BOLib.Models
{
    public class Persons
    {
        public virtual int id { set; get; }
        public virtual String? name { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual Categories? category { set; get; }

        public static explicit operator Persons(PersonsDAO? v)
        {
            return new Persons()
            {
                id = v.id,
                name = v.name
            };
        }
    }
}
