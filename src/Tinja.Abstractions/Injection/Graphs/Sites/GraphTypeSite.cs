﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tinja.Abstractions.Injection.Graphs.Sites
{
    /// <inheritdoc />
    /// AddSingleton(typeof(Service),typeof(Service));
    public class GraphTypeSite : GraphSite
    {
        public Type ImplementationType { get; set; }

        public ConstructorInfo ConstructorInfo { get; set; }

        public Dictionary<PropertyInfo, GraphSite> PropertySites { get; set; }

        public Dictionary<ParameterInfo, GraphSite> ParameterSites { get; set; }

        public override TVisitResult Accept<TVisitResult>(GraphSiteVisitor<TVisitResult> visitor)
        {
            return visitor.VisitType(this);
        }
    }
}
