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
using System.Buffers;

namespace NCode.ArrayLeases
{
  public class ArrayPoolLease<T> : ArrayLease<T>
  {
    private readonly ArrayPool<T> _pool;
    private readonly bool _clearArrayOnReturn;

    public ArrayPoolLease(int count, bool clearArrayOnReturn = false)
      : this(ArrayPool<T>.Shared, count, clearArrayOnReturn)
    {
      // nothing
    }

    public ArrayPoolLease(ArrayPool<T> pool, int count, bool clearArrayOnReturn = false)
      : base(Rent(pool, count), count)
    {
      _pool = pool;
      _clearArrayOnReturn = clearArrayOnReturn;
    }

    protected override void Return(T[] array)
    {
      _pool.Return(array, _clearArrayOnReturn);
    }

    private static T[] Rent(ArrayPool<T> pool, int count)
    {
      if (pool == null) throw new ArgumentNullException(nameof(pool));
      return pool.Rent(count);
    }

  }
}