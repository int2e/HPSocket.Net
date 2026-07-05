using System;
#if NET20 || NET30 || NET35
using System.Collections.Generic;
#else
using System.Collections.Concurrent;
#endif
namespace HPSocket
{
#if NET20 || NET30 || NET35
    public class ExtraData<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dict = new Dictionary<TKey, TValue>();

        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            lock (_dict)
            {
                return _dict.ContainsKey(key);
            }
        }

        /// <summary>
        /// 获取附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            lock (_dict)
            {
                try
                {
                    return _dict.TryGetValue(key, out var value) ? value : default;
                }
                catch (ArgumentNullException)
                {
                    return default;
                }
            }

        }

        /// <summary>
        /// 获取所有附加数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<TKey, TValue> GetAll()
        {
            return new Dictionary<TKey, TValue>(_dict);
        }

        /// <summary>
        /// 设置附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool Set(TKey key, TValue newValue)
        {
            lock (_dict)
            {
                try
                {
                    _dict[key] = newValue;
                    return true;
                }
                catch (OverflowException)
                {
                    // 字典数目超过int.Max
                    return false;
                }
                catch (ArgumentNullException)
                {
                    // 参数为空
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            lock (_dict)
            {
                try
                {
                    return _dict.Remove(key);
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 删除附加数据并返回被删除的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TKey key, out TValue value)
        {
            lock (_dict)
            {
                try
                {
                    if (_dict.TryGetValue(key, out value))
                    {
                        _dict.Remove(key);
                        return true;
                    }
                    value = default;
                    return false;
                }
                catch (ArgumentNullException)
                {
                    value = default;
                    return false;
                }
            }
        }

        /// <summary>
        /// 当 key 当前值与指定值一致时删除附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TKey key, TValue value)
        {
            lock (_dict)
            {
                try
                {
                    if (_dict.TryGetValue(key, out var existingValue) &&
                        EqualityComparer<TValue>.Default.Equals(existingValue, value))
                    {
                        _dict.Remove(key);
                        return true;
                    }

                    return false;
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        internal void Clear()
        {
            lock (_dict)
            {
                _dict.Clear();
            }
        }
    }
#else
    public class ExtraData<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dict = new ConcurrentDictionary<TKey, TValue>();

        /// <summary>
        /// 是否包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// 获取附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            try
            {
                return _dict.TryGetValue(key, out var value) ? value : default;
            }
            catch (ArgumentNullException)
            {
                return default;
            }
        }

        /// <summary>
        /// 获取所有附加数据
        /// </summary>
        /// <returns></returns>
        public ConcurrentDictionary<TKey, TValue> GetAll()
        {
            return new ConcurrentDictionary<TKey, TValue>(_dict);
        }

        /// <summary>
        /// 设置附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool Set(TKey key, TValue newValue)
        {
            try
            {
                _dict.AddOrUpdate(key, newValue, (tKey, existingVal) => newValue);
                return true;
            }
            catch (OverflowException)
            {
                // 字典数目超过int.Max
                return false;
            }
            catch (ArgumentNullException)
            {
                // 参数为空
                return false;
            }
        }

        /// <summary>
        /// 删除附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            try
            {
                return _dict.TryRemove(key, out _);
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除附加数据并返回被删除的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TKey key, out TValue value)
        {
            try
            {
                return _dict.TryRemove(key, out value);
            }
            catch (ArgumentNullException)
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// 当 key 当前值与指定值一致时删除附加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TKey key, TValue value)
        {
            try
            {
                return ((System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)_dict)
                    .Remove(new System.Collections.Generic.KeyValuePair<TKey, TValue>(key, value));
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        internal void Clear()
        {
            _dict.Clear();
        }
    }
#endif
}
