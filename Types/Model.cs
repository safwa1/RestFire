using RestFire.Attribute;

namespace RestFire.Types;

public abstract class Model
{
    [Key]
    public string Id { get; set; }
}

public abstract record BaseModel
{
    [Key]
    public string Id { get; set; }
}