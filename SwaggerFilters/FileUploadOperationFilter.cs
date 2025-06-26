using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System;
using CantineAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CantineAPI.SwaggerFilters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formFileParameters = context.ApiDescription.ActionDescriptor.Parameters
                .Where(p => p.ParameterType == typeof(IFormFile))
                .ToList();

            if (formFileParameters.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties =
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary",
                                        Description = "Fichier à télécharger"
                                    }
                                }
                            }
                        }
                    }
                };

                foreach (var parameter in formFileParameters)
                {
                    var parameterToRemove = operation.Parameters.FirstOrDefault(p => p.Name == parameter.Name);
                    if (parameterToRemove != null)
                    {
                        operation.Parameters.Remove(parameterToRemove);
                    }
                }
            }
        }
    }
}