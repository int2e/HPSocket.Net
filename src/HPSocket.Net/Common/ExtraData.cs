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
        /// 清除
        /// </summary>
        internal void Clear()
        {
            _dict.Clear();
        }
    }
#endif
}
