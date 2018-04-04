﻿using System;

namespace Tinja
{
    public class Component
    {
        public LifeStyle LifeStyle { get; set; }

        public Type ServiceType { get; set; }

        public Type ImplementionType { get; set; }

        public Func<IContainer, object> ImplementionFactory { get; set; }

        public override int GetHashCode()
        {
            var hashCode = ServiceType.GetHashCode();

            hashCode += (hashCode * 31) ^ (ImplementionType?.GetHashCode() ?? 0);
            hashCode += (hashCode * 31) ^ (ImplementionFactory?.GetHashCode() ?? 0);
            hashCode += (hashCode * 31) ^ LifeStyle.GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is Component component)
            {
                return
                    LifeStyle == component.LifeStyle &&
                    ServiceType == component.ServiceType &&
                    ImplementionType == component.ImplementionType &&
                    ImplementionFactory == component.ImplementionFactory;
            }

            return false;
        }

        public static bool operator ==(Component left, Component right)
        {
            if (!(left is null))
            {
                return left.Equals(right);
            }

            if (!(right is null))
            {
                return right.Equals(left);
            }

            return true;
        }

        public static bool operator !=(Component left, Component right)
        {
            return !(left == right);
        }
    }
}