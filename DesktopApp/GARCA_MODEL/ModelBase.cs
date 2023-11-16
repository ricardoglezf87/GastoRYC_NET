using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    public class ModelBase<T> : IComparable
    {
        [Key]
        public virtual T Id { set; get; }

        public ModelBase()
        {

        }

        public virtual int CompareTo(Object? obj)
        {
            return obj == null ? 0 : Id.ToString().CompareTo(((ModelBase<T>)obj).Id);
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

            return Id.Equals(((ModelBase<T>)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(ModelBase<T>? left, ModelBase<T>? right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(ModelBase<T>? left, ModelBase<T>? right)
        {
            return !(left == right);
        }

        public static bool operator <(ModelBase<T>? left, ModelBase<T>? right)
        {
            return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(ModelBase<T>? left, ModelBase<T>? right)
        {
            return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
        }

        public static bool operator >(ModelBase<T>? left, ModelBase<T>? right)
        {
            return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(ModelBase<T>? left, ModelBase<T>? right)
        {
            return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
        }

    }
}
