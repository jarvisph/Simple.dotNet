using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simple.dotNet.Web.Swagger
{
    /// <summary>
    /// 设置请求头文件
    /// </summary>
    public class SwaggerHeaderParameter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters = operation.Parameters ?? new List<OpenApiParameter>();

            if (!context.ApiDescription.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
                !context.ApiDescription.HttpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase) &&
                !context.ApiDescription.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) &&
                !context.ApiDescription.HttpMethod.Equals("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var fileParameters = context.ApiDescription.ActionDescriptor.Parameters.Where(n => n.ParameterType == typeof(IFormFile)).ToList();

            if (fileParameters.Count < 0)
            {
                return;
            }

            foreach (var fileParameter in fileParameters)
            {
                if (fileParameter.BindingInfo == null)
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = fileParameter.Name,
                        Description = "文件上传",
                        Required = true,
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form
                    });
                }
                else
                {
                    var parameter = operation.Parameters.Single(n => n.Name == fileParameter.Name);
                    operation.Parameters.Remove(parameter);
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = fileParameter.Name,
                        Description = "文件上传",
                        Required = true,
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form
                    });
                }
            }

            //in query header 
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Token",
                In = ParameterLocation.Header,
                Description = "身份验证",
                Required = false,
                Style = ParameterStyle.Label
            });

        }
    }
}
