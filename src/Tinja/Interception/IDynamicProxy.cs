﻿namespace Tinja.Interception
{
    public interface IDynamicProxy
    {
        object Target { get; }

        IInterceptor[] GetInterceptors();
    }
}