using System;
using System.Collections.Generic;

namespace DfmWeb.Core.Collections
{
    public class LruCache<TKey, TValue>
    {
        private class CacheItem
        {
            public CacheItem Next     { get; set; }
            public CacheItem Previous { get; set; }
            public TKey      Key      { get; set; }
            public TValue    Value    { get; set; }
        }

        private readonly Dictionary<TKey, CacheItem> _items;
        private readonly int _capacity;

        private CacheItem _head;
        private CacheItem _tail;

        public int Count => this._items.Count;

        public LruCache(int capacity = 10)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity should be greater than zero");
            }

            this._capacity = capacity;
            this._items  = new Dictionary<TKey, CacheItem>();

            this._head = null;
        }

        public void Set(TKey key, TValue value)
        {
            if (!this._items.TryGetValue(key, out CacheItem item))
            {
                item = new CacheItem
                {
                    Key   = key,
                    Value = value
                };

                if (this._items.Count == this._capacity)
                {
                    this._items.Remove(this._tail.Key);
                    this._tail = this._tail.Previous;
                    if (this._tail != null)
                    {
                        this._tail.Next = null;
                    }
                }

                this._items.Add(key, item);
            }

            item.Value = value;
            MoveToHead(item);

            if (this._tail == null)
            {
                this._tail = this._head;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            if (!this._items.TryGetValue(key, out CacheItem item))
            {
                return false;
            }

            MoveToHead(item);
            value = item.Value;

            return true;
        }

        private void MoveToHead(CacheItem item)
        {
            if (item == this._head || item == null)
            {
                return;
            }

            CacheItem next     = item.Next;
            CacheItem previous = item.Previous;

            if (next != null)
            {
                next.Previous = item.Previous;
            }

            if (previous != null)
            {
                previous.Next = item.Next;
            }

            item.Previous = null;
            item.Next     = this._head;

            if (this._head != null)
            {
                this._head.Previous = item;
            }

            this._head = item;

            if (this._tail == item)
            {
                this._tail = previous;
            }
        }
    }
}
