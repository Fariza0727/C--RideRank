using System;
using System.Linq;
using System.Linq.Expressions;

namespace RR.Core
{
     public static class PredicateBuilder
     {
          public static Expression<Func<T, bool>> True<T>() { return f => true; }
          public static Expression<Func<T, bool>> False<T>() { return f => false; }
          public static Expression<Func<T, bool>> Custom<T>() { return p => true; }

          public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
          {
               var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
               return Expression.Lambda<Func<T, bool>>
                     (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
          }

          public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
          {
               var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
               return Expression.Lambda<Func<T, bool>>
                     (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
          }
     }
    
     public static class PredicateRewriter
     {
          public static Expression<Func<T, bool>> Rewrite<T>(Expression<Func<T, bool>> exp, string newParamName)
          {
               var param = Expression.Parameter(exp.Parameters[0].Type, newParamName);
               var newExpression = new PredicateRewriterVisitor(param).Visit(exp);

               return (Expression<Func<T, bool>>)newExpression;
          }

          private class PredicateRewriterVisitor : ExpressionVisitor
          {
               private readonly ParameterExpression _parameterExpression;

               public PredicateRewriterVisitor(ParameterExpression parameterExpression)
               {
                    _parameterExpression = parameterExpression;
               }

               protected override Expression VisitParameter(ParameterExpression node)
               {
                    return _parameterExpression;
               }
          }
     }

     public class ParameterToMemberExpressionRebinder : ExpressionVisitor
     {
          ParameterExpression _paramExpr;
          MemberExpression _memberExpr;

          ParameterToMemberExpressionRebinder(ParameterExpression paramExpr, MemberExpression memberExpr)
          {
               _paramExpr = paramExpr;
               _memberExpr = memberExpr;
          }

          public override Expression Visit(Expression p)
          {
               return base.Visit(p == _paramExpr ? _memberExpr : p);
          }

          public static Expression<Func<T, bool>> CombinePropertySelectorWithPredicate<T, T2>(
              Expression<Func<T, T2>> propertySelector,
              Expression<Func<T2, bool>> propertyPredicate)
          {
               var memberExpression = propertySelector.Body as MemberExpression;

               if (memberExpression == null)
               {
                    throw new ArgumentException("propertySelector");
               }

               var expr = Expression.Lambda<Func<T, bool>>(propertyPredicate.Body, propertySelector.Parameters);
               var rebinder = new ParameterToMemberExpressionRebinder(propertyPredicate.Parameters[0], memberExpression);
               expr = (Expression<Func<T, bool>>)rebinder.Visit(expr);

               return expr;
          }
     }
}