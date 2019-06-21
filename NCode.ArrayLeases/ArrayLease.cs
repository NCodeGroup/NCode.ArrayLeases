#region Copyright Preamble
// 
//    Copyright © 2017 NCode Group
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NCode.ArrayLeases
{
  public interface IArrayLease<out T> : IDisposable
  {
    int Count { get; set; }
    T[] Array { get; }
  }

  public class ArrayLease<T> : IArrayLease<T>
  {
    private readonly T[] _array;
    private int _count;
    private int _disposed;

    public ArrayLease(T[] array)
    {
      _array = array ?? throw new ArgumentNullException(nameof(array));
      _count = array.Length;
    }

    public ArrayLease(T[] array, int count)
    {
      if (array == null) throw new ArgumentNullException(nameof(array));
      if (count > array.Length) throw new ArgumentOutOfRangeException(nameof(count));

      _array = array;
      _count = count;
    }

    public int Count
    {
      get { return GetOrThrowIfDisposed(_count); }
      set
      {
        if (value > Array.Length) throw new ArgumentOutOfRangeException(nameof(value));

        _count = value;
      }
    }

    public T[] Array => GetOrThrowIfDisposed(_array);

    protected virtual void Return(T[] array)
    {
      // nothing
    }

    #region IDisposable Members

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~ArrayLease()
    {
      Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 1) return;
      Return(_array);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
      if (Interlocked.CompareExchange(ref _disposed, 0, 0) == 1)
        throw new ObjectDisposedException(GetType().FullName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected TValue GetOrThrowIfDisposed<TValue>(TValue value)
    {
      ThrowIfDisposed();
      return value;
    }

    #endregion

  }
}