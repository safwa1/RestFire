namespace RestFire.Types;

public sealed record DataChangeEventArg<T>(DbActionType ActionType, T Entry);