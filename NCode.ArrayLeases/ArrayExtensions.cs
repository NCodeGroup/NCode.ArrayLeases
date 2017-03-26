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

namespace NCode.ArrayLeases
{
  public static class ArrayExtensions
  {
    public static IArrayLease<T> Lease<T>(this T[] array)
    {
      if (array == null) throw new ArgumentNullException(nameof(array));

      return new ArrayLease<T>(array, array.Length);
    }

    public static IArrayLease<T> Lease<T>(this T[] array, int count)
    {
      if (array == null) throw new ArgumentNullException(nameof(array));

      return new ArrayLease<T>(array, count);
    }

  }
}