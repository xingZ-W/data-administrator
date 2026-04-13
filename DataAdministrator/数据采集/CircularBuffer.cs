using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAdministrator.数据采集
{
    /// <summary>
    /// 环形缓冲区 - 用于高频数据采集
    /// 线程安全，支持高效写入和读取
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class CircularBuffer<T>
    {
        private readonly T[] _buffer;
        private int _head; // 写入位置
        private int _tail; // 读取位置
        private int _count;
        private readonly object _lock = new object();
        
        /// <summary>
        /// 缓冲区容量
        /// </summary>
        public int Capacity => _buffer.Length;
        
        /// <summary>
        /// 当前元素数量
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _count;
                }
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public CircularBuffer(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("容量必须大于 0", nameof(capacity));
            
            _buffer = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }
        
        /// <summary>
        /// 写入单个元素
        /// </summary>
        public void Write(T item)
        {
            lock (_lock)
            {
                _buffer[_head] = item;
                _head = (_head + 1) % Capacity;
                
                if (_count < Capacity)
                {
                    _count++;
                }
                else
                {
                    // 缓冲区满时，移动 tail（丢弃最旧数据）
                    _tail = (_tail + 1) % Capacity;
                }
            }
        }
        
        /// <summary>
        /// 批量写入
        /// </summary>
        public void Write(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Write(item);
            }
        }
        
        /// <summary>
        /// 读取并删除最旧的元素
        /// </summary>
        public T Read()
        {
            lock (_lock)
            {
                if (_count == 0)
                    throw new InvalidOperationException("缓冲区为空");
                
                var item = _buffer[_tail];
                _tail = (_tail + 1) % Capacity;
                _count--;
                return item;
            }
        }
        
        /// <summary>
        /// 读取最后 N 个元素（不删除）
        /// </summary>
        public List<T> ReadLast(int count)
        {
            lock (_lock)
            {
                if (count <= 0)
                    return new List<T>();
                
                var actualCount = Math.Min(count, _count);
                var result = new List<T>(actualCount);
                
                var startIndex = (_head - actualCount + Capacity) % Capacity;
                
                for (int i = 0; i < actualCount; i++)
                {
                    var index = (startIndex + i) % Capacity;
                    result.Add(_buffer[index]);
                }
                
                return result;
            }
        }
        
        /// <summary>
        /// 读取所有元素（不删除）
        /// </summary>
        public List<T> ReadAll()
        {
            lock (_lock)
            {
                return ReadLast(_count);
            }
        }
        
        /// <summary>
        /// 清空缓冲区
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                Array.Clear(_buffer, 0, _buffer.Length);
                _head = 0;
                _tail = 0;
                _count = 0;
            }
        }
        
        /// <summary>
        /// 尝试读取（不抛异常）
        /// </summary>
        public bool TryRead(out T item)
        {
            lock (_lock)
            {
                if (_count == 0)
                {
                    item = default(T);
                    return false;
                }
                
                item = Read();
                return true;
            }
        }
        
        /// <summary>
        /// 获取指定索引的元素（只读）
        /// </summary>
        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    if (index < 0 || index >= _count)
                        throw new ArgumentOutOfRangeException(nameof(index));
                    
                    var actualIndex = (_tail + index) % Capacity;
                    return _buffer[actualIndex];
                }
            }
        }
        
        /// <summary>
        /// 转为数组
        /// </summary>
        public T[] ToArray()
        {
            lock (_lock)
            {
                return ReadAll().ToArray();
            }
        }
    }
}
