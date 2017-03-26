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
using System.Linq;
using Moq;
using Xunit;

namespace NCode.ArrayLeases.Test
{
  public class ArrayPoolTests
  {
    [Fact]
    public void MockPool()
    {
      const int length = 20;
      var buffer = new byte[length];
      new Random().NextBytes(buffer);

      var moqPool = new Mock<ArrayPool<byte>>(MockBehavior.Strict);
      moqPool.Setup(_ => _.Rent(length)).Returns((int len) => buffer).Verifiable();
      moqPool.Setup(_ => _.Return(buffer, false)).Verifiable();
      var pool = moqPool.Object;

      using (var lease = new ArrayPoolLease<byte>(pool, length))
      {
        Assert.Equal(length, lease.Count);
        Assert.Equal(buffer, lease.Array);
      }

      moqPool.Verify();
    }

    [Fact]
    public void ClearArrayOnReturnTrue()
    {
      byte[] buffer;
      using (var lease = new ArrayPoolLease<byte>(20, true))
      {
        buffer = lease.Array;
        new Random().NextBytes(lease.Array);

        var sumBefore = buffer.Sum(value => value);
        Assert.NotEqual(0, sumBefore);
      }
      var sumAfter = buffer.Sum(value => value);
      Assert.Equal(0, sumAfter);
    }

    [Fact]
    public void ClearArrayOnReturnFalse()
    {
      byte[] buffer;
      int sumBefore;
      using (var lease = new ArrayPoolLease<byte>(20, false))
      {
        buffer = lease.Array;
        new Random().NextBytes(lease.Array);

        sumBefore = buffer.Sum(value => value);
        Assert.NotEqual(0, sumBefore);
      }
      var sumAfter = buffer.Sum(value => value);
      Assert.Equal(sumBefore, sumAfter);
    }

  }
}