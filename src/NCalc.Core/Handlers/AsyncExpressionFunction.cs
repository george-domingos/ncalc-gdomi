namespace NCalc.Handlers;

public delegate Task<object?> AsyncExpressionFunction(FunctionEventArgs parameters);
