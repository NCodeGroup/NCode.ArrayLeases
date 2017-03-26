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

using Xunit;

namespace NCode.ArrayLeases.Test
{
  public class ArraySegmentTests
  {
    [Fact]
    public void Length()
    {
      var buffer = new byte[1024];
      using (var lease = new ArrayLease<byte>(buffer))
      {
        var segment = lease.Segment();
        Assert.Equal(0, segment.Offset);
        Assert.Equal(1024, segment.Count);
        Assert.Same(buffer, segment.Array);
      }
    }

    [Fact]
    public void Count()
    {
      var buffer = new byte[1024];
      using (var lease = new ArrayLease<byte>(buffer, 20))
      {
        var segment = lease.Segment();
        Assert.Equal(0, segment.Offset);
        Assert.Equal(20, lease.Count);
        Assert.Equal(20, segment.Count);
        Assert.Same(buffer, segment.Array);
      }
    }

    [Fact]
    public void Offset()
    {
      var buffer = new byte[1024];
      using (var lease = new ArrayLease<byte>(buffer, 30))
      {
        var segment = lease.Segment(10, 20);
        Assert.Equal(10, segment.Offset);
        Assert.Equal(20, segment.Count);
        Assert.Same(buffer, segment.Array);
      }
    }

  }
}