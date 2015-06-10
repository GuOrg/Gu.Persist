namespace Gu.Settings
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;

    internal class MinMaxSetting<T> : ValidatingSetting<T>
        where T : struct, IComparable<T>
    {
        public T? Min { get; set; }
       
        public T? Max { get; set; }

        protected override bool IsValidCore(T value)
        {
            if (Min.HasValue && Comparer<T>.Default.Compare(value, Min.Value) < 0)
            {
                return false;
            }

            if (Max.HasValue && Comparer<T>.Default.Compare(value, Max.Value) > 0)
            {
                return false;
            }
            return true;
        }
    }
}