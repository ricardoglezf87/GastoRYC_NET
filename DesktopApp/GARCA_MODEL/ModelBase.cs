using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    public class ModelBase : IComparable
    {
        [Key]
        [Column("id")]
        public virtual int Id { set; get; }

        public virtual int CompareTo(object? obj)
        {
            return obj == null ? 0 : Id.CompareTo(((ModelBase)obj).Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null) || obj.GetType() != this.GetType())
            {
                return false;
            }                        
            
            return Id.Equals(((ModelBase)obj).Id) ;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ModelBase? left, ModelBase? right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(ModelBase? left, ModelBase? right)
        {
            return !(left == right);
        }

        public static bool operator <(ModelBase? left, ModelBase? right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ModelBase? left, ModelBase? right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(ModelBase? left, ModelBase? right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(ModelBase? left, ModelBase? right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }
    }
}
