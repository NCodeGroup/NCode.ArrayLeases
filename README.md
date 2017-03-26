# NCode.ArrayLeases
> PM> Install-Package NCode.ArrayLeases

This library provides provides `IDisposable` leases for Microsoft's new [ArrayPool] class from the [System.Buffers] package. These `IDisposable` implementations will automatically return the array back to the `ArrayPool` when the lease is disposed.

## Usage
### Before
```csharp
public static void Main(string[] args)
{
  var pool = ArrayPool<byte>.Shared;
  var buffer = pool.Rent(4096);
  try
  {
    // use the buffer...
  }
  finally
  {
    pool.Return(buffer);
  }
}
```
### After
```csharp
public static void Main(string[] args)
{
  var pool = ArrayPool<byte>.Shared;
  using (var lease = pool.Lease(4096))
  {
    // use the buffer...
  }
}
```

## IArrayLease
Represents a lease by encapsulating it's array and item count. The item `Count` property is assignable to any value less than or equal to the array length.

```csharp
public interface IArrayLease<out T> : IDisposable
{
  int Count { get; set; }
  T[] Array { get; }
}
```

### Count Property
The impetus for allowing to assign the item `Count` is because arrays returned from the pool are not the exact length requested. The `Count` property is initially set to the requested length but can be assigned to any value less than or equal to the array length so that consumers may know how much data to consume. See the [ArrayPool] API for additional details about buffer sizes.

## ArrayLease
Base class for `IArrayLease<T>` and provides a default implementation for it's properties. Derived classes must override the following method to relinquish the lease. The default implementation for `Return` does nothing.

```csharp
public class ArrayLease<T>
{
  public ArrayLease(T[] array) { /* ... */ }
  public ArrayLease(T[] array, int count) { /* ... */ }

  // other members ...

  protected virtual void Return(T[] array)
  {
    // override for custom behavior
  }
}
```

## ArrayPoolLease
Contains an implementation of `ArrayLease<T>` that will `Rent` an array from `ArrayPool<T>` and then `Return` the array after the lease is disposed.

```csharp
public class ArrayPoolLease<T> : ArrayLease<T>
{
  // uses static 'ArrayPool<T>.Shared' instance:
  public ArrayPoolLease(
    int count,
    bool clearArrayOnReturn = false
  );

  // specify any other 'ArrayPool<T>' instance:
  public ArrayPoolLease(
    ArrayPool<T> pool,
    int count,
    bool clearArrayOnReturn = false
  );

  protected override void Return(T[] array)
  {
    _pool.Return(array);
  }
}
```

## Extension Methods
Various extension methods exist to create leases for multiple use-cases.

```csharp
// From an array to a lease:
// The Dispose implementation on these leases perform nothing.
public static IArrayLease<T> Lease<T>(this T[] array);
public static IArrayLease<T> Lease<T>(this T[] array, int count);

// From an array pool to a lease:
// The Dispose implementation on these leases will return the array back to the pool.
public static IArrayLease<T> Lease<T>(this ArrayPool<T> pool, int count, bool clearArrayOnReturn = false);

// From a lease to an array segment:
public static ArraySegment<T> Segment<T>(this IArrayLease<T> lease);
public static ArraySegment<T> Segment<T>(this IArrayLease<T> lease, int offset, int count);
```

## Release Notes
* v1.0.0 - Initial Release

## Feedback
Please provide any feedback, comments, or issues to this GitHub project [here][issues].

[issues]: https://github.com/NCodeGroup/NCode.ArrayPoolLeases/issues
[ArrayPool]: https://github.com/dotnet/corefx/issues/4547
[System.Buffers]: https://www.nuget.org/packages/System.Buffers
