namespace Gu.Settings
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class SettingCollection : ICollection<ISetting>
    {
        public IEnumerator<ISetting> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISetting item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(ISetting item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(ISetting[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(ISetting item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
    }
}
