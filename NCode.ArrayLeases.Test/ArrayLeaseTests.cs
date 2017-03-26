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
using Xunit;

namespace NCode.ArrayLeases.Test
{
  public class ArrayLeaseTests
  {
    [Fact]
    public void ArrayIsIdentical()
    {
      var buffer = new byte[1024];
      new Random().NextBytes(buffer);

      var copy = new byte[buffer.Length];
      Buffer.BlockCopy(buffer, 0, copy, 0, buffer.Length);

      using (var lease = new ArrayLease<byte>(buffer))
      {
        Assert.Equal(copy, lease.Array);
      }
    }

    [Fact]
    public void DisposeDoesNotModify()
    {
      var buffer = new byte[1024];
      new Random().NextBytes(buffer);

      var copy = new byte[buffer.Length];
      Buffer.BlockCopy(buffer, 0, copy, 0, buffer.Length);

      new ArrayLease<byte>(buffer).Dispose();

      Assert.Equal(copy, buffer);
    }

    [Fact]
    public void SetCountLessThanArray()
    {
      var buffer = new byte[1024];
      new Random().NextBytes(buffer);

      using (var lease = new ArrayLease<byte>(buffer, 20))
      {
        Assert.Equal(20, lease.Count);
        lease.Count = 40;
        Assert.Equal(40, lease.Count);
      }
    }

    [Fact]
    public void SetCountEqualToArray()
    {
      var buffer = new byte[1024];
      new Random().NextBytes(buffer);

      using (var lease = new ArrayLease<byte>(buffer, 20))
      {
        Assert.Equal(20, lease.Count);
        lease.Count = buffer.Length;
        Assert.Equal(buffer.Length, lease.Count);
      }
    }

    [Fact]
    public void SetCountGreaterThanArrayThrows()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayLease<byte>(new byte[20]).Count = 200);
    }

    [Fact]
    public void CreateCountGreaterThanArrayThrows()
    {
      Assert.Throws<ArgumentOutOfRangeException>(() => new ArrayLease<byte>(new byte[20], 100));
    }

    [Fact]
    public void DisposeThenGetArrayThrows()
    {
      Assert.Throws<ObjectDisposedException>(() =>
      {
        var lease = new ArrayLease<byte>(new byte[20]);
        lease.Dispose();
        return lease.Array;
      });
    }

    [Fact]
    public void DisposeThenGetCountThrows()
    {
      Assert.Throws<ObjectDisposedException>(() =>
      {
        var lease = new ArrayLease<byte>(new byte[20]);
        lease.Dispose();
        return lease.Count;
      });
    }

  }
}