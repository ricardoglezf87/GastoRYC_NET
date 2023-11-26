using System.ComponentModel.DataAnnotations;

namespace GARCA.Models
{
    public class ModelBase : IComparable
    {
        [Key]
        public virtual int Id { set; get; }

        public virtual int CompareTo(Object? obj)
        {
            return obj == null ? 0 : Id.ToString().CompareTo(((ModelBase)obj).Id);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is not null && obj.GetType() == this.GetType() && Id.Equals(((ModelBase)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ModelBase? left, ModelBase? right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(ModelBase? left, ModelBase? right)
        {
            return !(left == right);
        }

        public static bool operator <(ModelBase? left, ModelBase? right)
        {
            return left is null ? right is not null : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ModelBase? left, ModelBase? right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(ModelBase? left, ModelBase? right)
        {
            return left is not null && left.CompareTo(right) > 0;
        }

        public static bool operator >=(ModelBase? left, ModelBase? right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

    }
}
