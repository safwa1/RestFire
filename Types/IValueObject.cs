namespace RestFire.Types;

public interface IValueObject<out T>
{ 
    T Value { get; }
}