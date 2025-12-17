using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LightProto.Generator;

internal sealed class ExpressionEmitter
{
    private readonly SemanticModel _semanticModel;

    public ExpressionEmitter(SemanticModel semanticModel)
    {
        _semanticModel = semanticModel;
    }

    public string Emit(ExpressionSyntax expr)
    {
        if (expr is LiteralExpressionSyntax)
        {
            return expr.ToFullString();
        }

        if (expr is DefaultExpressionSyntax def)
        {
            var type = _semanticModel.GetTypeInfo(def.Type).Type;
            return type is null ? expr.ToFullString() : $"default({Fqn(type)})";
        }

        if (expr is CollectionExpressionSyntax collection)
        {
            return EmitCollectionExpression(collection);
        }

        if (expr is ObjectCreationExpressionSyntax creation)
        {
            return EmitObjectCreation(creation);
        }

        if (expr is ImplicitObjectCreationExpressionSyntax implicitNew)
        {
            return EmitImplicitObjectCreation(implicitNew);
        }
        if (expr is ImplicitArrayCreationExpressionSyntax implicitArrayNew)
        {
            return EmitImplicitArrayCreation(implicitArrayNew);
        }

        if (
            expr is InvocationExpressionSyntax invocation
            && invocation.Expression is IdentifierNameSyntax id
        )
        {
            return $"{id}{EmitArgumentsExpression(invocation.ArgumentList)}";
        }
        if (_semanticModel.GetSymbolInfo(expr).Symbol is { } symbol)
        {
            return EmitSymbolExpression(expr, symbol);
        }

        return expr.ToFullString();
    }

    // ---------------- helpers ----------------

    private string EmitArgumentsExpression(ArgumentListSyntax? argumentListSyntax)
    {
        return argumentListSyntax is null
            ? "()"
            : EmitArgumentsExpression(argumentListSyntax.Arguments.Select(x => x.Expression));
    }

    private string EmitArgumentsExpression(IEnumerable<ExpressionSyntax> expressionSyntaxes)
    {
        return $"({string.Join(",", expressionSyntaxes.Select(Emit))})";
    }

    private string EmitInitializerExpression(
        InitializerExpressionSyntax? initializerExpressionSyntax
    )
    {
        return initializerExpressionSyntax is null
            ? ""
            : "{"
                + string.Join(",", initializerExpressionSyntax.Expressions.Select(x => Emit(x)))
                + "}";
    }

    private string EmitObjectCreation(ObjectCreationExpressionSyntax creation)
    {
        var type = (_semanticModel.GetSymbolInfo(creation.Type).Symbol as ITypeSymbol);

        if (type is null)
        {
            return creation.ToFullString();
        }
        var typeName = Fqn(type);
        var args = EmitArgumentsExpression(creation.ArgumentList);
        var init = EmitInitializerExpression(creation.Initializer);

        return $"new {typeName}{args}{init}";
    }

    private string EmitImplicitObjectCreation(ImplicitObjectCreationExpressionSyntax creation)
    {
        var type = _semanticModel.GetTypeInfo(creation).Type;

        if (type is null)
        {
            return creation.ToFullString();
        }
        var typeName = Fqn(type);
        var args = EmitArgumentsExpression(creation.ArgumentList);
        var init = EmitInitializerExpression(creation.Initializer);

        return $"new {typeName}{args}{init}";
    }

    private string EmitImplicitArrayCreation(ImplicitArrayCreationExpressionSyntax creation)
    {
        var init = EmitInitializerExpression(creation.Initializer);
        return $"new []{init}";
    }

    private string EmitCollectionExpression(CollectionExpressionSyntax collection)
    {
        if (collection.Elements.Count == 0)
        {
            return collection.ToFullString();
        }

        var elements = string.Join(
            ", ",
            collection.Elements.Select(e =>
            {
                if (e is ExpressionElementSyntax ee)
                    return Emit(ee.Expression);
                return e.ToFullString();
            })
        );

        return $"[{elements}]";
    }

    static readonly SymbolDisplayFormat FullyQualifiedMemberFormat = SymbolDisplayFormat
        .FullyQualifiedFormat.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType)
        .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters);

    private string EmitSymbolExpression(ExpressionSyntax expr, ISymbol symbol)
    {
        if (expr is InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
            {
                var method = symbol as IMethodSymbol;
                if (method is null)
                {
                    return expr.ToFullString();
                }

                var reducedMethod = method;
                if (method.MethodKind is MethodKind.ReducedExtension)
                {
                    reducedMethod = method.ReducedFrom!;
                }

                var name = reducedMethod.IsStatic
                    ? reducedMethod.ContainingType.ToDisplayString(
                        SymbolDisplayFormat.FullyQualifiedFormat
                    )
                        + "."
                        + reducedMethod.Name
                    : reducedMethod.Name;

                if (method.IsGenericMethod)
                {
                    name +=
                        "<"
                        + string.Join(
                            ", ",
                            method.TypeArguments.Select(t =>
                                t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                            )
                        )
                        + ">";
                }
                string args;
                if (reducedMethod.IsStatic)
                {
                    if (method.MethodKind is MethodKind.ReducedExtension)
                    {
                        args = EmitArgumentsExpression(
                            Enumerable
                                .Repeat(memberAccessExpressionSyntax.Expression, 1)
                                .Concat(invocation.ArgumentList.Arguments.Select(x => x.Expression))
                        );
                    }
                    else
                    {
                        args = EmitArgumentsExpression(invocation.ArgumentList);
                    }
                    return $"{name}{args}";
                }
                else
                {
                    args = EmitArgumentsExpression(invocation.ArgumentList);
                    name = Emit(memberAccessExpressionSyntax.Expression) + "." + name;
                    return $"{name}{args}";
                }
            }
            else
            {
                return expr.ToFullString();
            }
        }
        else if (expr is PostfixUnaryExpressionSyntax postfixUnary)
        {
            var name = Emit(postfixUnary.Operand);
            return $"{name}{postfixUnary.OperatorToken}";
        }
        else if (expr is PrefixUnaryExpressionSyntax prefixUnary)
        {
            var name = Emit(prefixUnary.Operand);
            return $"{prefixUnary.OperatorToken}{name}";
        }
        else if (expr is MemberAccessExpressionSyntax memberAccess)
        {
            return symbol.ToDisplayString(FullyQualifiedMemberFormat);
        }

        return symbol.ToDisplayString(FullyQualifiedMemberFormat);
    }

    private static string Fqn(ITypeSymbol type)
    {
        return type.ToDisplayString(FullyQualifiedMemberFormat);
    }
}
