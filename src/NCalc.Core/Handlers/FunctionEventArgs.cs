using NCalc.Exceptions;
using NCalc.Visitors;

namespace NCalc.Handlers;

public class FunctionEventArgs(
    Guid id,
    LogicalExpressionList arguments,
    ExpressionContext context,
    ILogicalExpressionVisitor<object?> syncVisitor,
    ILogicalExpressionVisitor<Task<object?>>? asyncVisitor,
    CancellationToken cancellationToken)
    : EventArgs, IReadOnlyList<LogicalExpression>
{
    private LogicalExpressionList Arguments { get; } = arguments;
    private ILogicalExpressionVisitor<object?> SyncVisitor { get; } = syncVisitor;
    private ILogicalExpressionVisitor<Task<object?>>? AsyncVisitor { get; } = asyncVisitor;

    [Obsolete("Use Evaluate/EvaluateAsync, Count, or the indexer directly instead.")]
    public FunctionEventArgs Parameters => this;

    public Guid Id { get; } = id;

    public ExpressionContext Context { get; } = context;
    public CancellationToken CancellationToken { get; } = cancellationToken;

    public object? Result
    {
        get;
        set
        {
            field = value;
            HasResult = true;
        }
    }

    public bool HasResult { get; private set; }

    public LogicalExpression this[int index] => Arguments[index];

    public int Count => Arguments.Count;

    public object? Evaluate(int index)
    {
        return Arguments[index].Accept(SyncVisitor);
    }

    public Task<object?> EvaluateAsync(int index)
    {
        if (AsyncVisitor is null)
            throw new NCalcEvaluationException(
                "Asynchronous binary value evaluation is not available in this context.");

        return Arguments[index].Accept(AsyncVisitor);
    }

    public IEnumerator<LogicalExpression> GetEnumerator()
    {
        return Arguments.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Arguments.GetEnumerator();
    }
}
