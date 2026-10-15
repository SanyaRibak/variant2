namespace MoiMarket.Services;

public class CategoryConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        return values[routeKey]?.ToString() is "вентиляторы" or "мультиварки" or "мантоварки";
    }
}
