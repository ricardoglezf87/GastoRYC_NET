using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Utils.Data
{
    public static class DataUtils
    {
        public static Expression<Func<Q, bool>> ConvertToLambda<Q>(string lambdaString)
        {
            // Puedes agregar más manejo de errores para cadenas inválidas
            var parameters = new[] { Expression.Parameter(typeof(Q), "x") };
            var body = DynamicExpressionParser.ParseLambda(parameters, null, lambdaString).Body;

            return Expression.Lambda<Func<Q, bool>>(body, parameters);
        }
    }
}
