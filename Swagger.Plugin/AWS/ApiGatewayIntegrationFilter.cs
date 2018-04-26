using System.Collections.Generic;
using System.Web;
using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Swagger.Plugin.AWS {
    public class ApiGatewayIntegrationFilter : IOperationFilter {

        /// <summary>
        /// Gets or sets the integration settings.
        /// </summary>
        /// <value>
        /// The integration settings.
        /// </value>
        public static ApiGatewayIntegrationSettings IntegrationSettings { get; set; }

        /// <summary>
        /// Applies the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="schemaRegistry">The schema registry.</param>
        /// <param name="apiDescription">The API description.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription) {
            if (apiDescription.HttpMethod == System.Net.Http.HttpMethod.Options) {
                CreateOptionsMethod(operation);
                return;
            }

            //Add extra response for every opration
            foreach (var extraResponse in IntegrationSettings.extraResponses) {
                if (!operation.responses.ContainsKey(extraResponse.Key)) {
                    operation.responses.Add(extraResponse);
                }
            }

            if (operation.parameters == null) {
                operation.parameters = new List<Parameter>();
            }

            foreach (var parameter in IntegrationSettings.globalActionParameters) {
                operation.parameters.Add(parameter);
            }

            foreach (var response in operation.responses) {

                if (response.Value.headers == null) {
                    response.Value.headers = new Dictionary<string, Header>();
                }

                if (response.Value.vendorExtensions == null) {
                    response.Value.vendorExtensions = new Dictionary<string, object>();
                }

                response.Value.description = response.Key;

                foreach (var allowedAccessControl in IntegrationSettings.allowedAccessControl.GetAllowedAccessControls()) {
                    if (!response.Value.headers.ContainsKey(allowedAccessControl))
                        response.Value.headers.Add(allowedAccessControl, new Header() { type = "string" });
                }

            }

            operation.vendorExtensions.Add("x-amazon-apigateway-integration", new
            {
                type = "http",
                httpMethod = apiDescription.HttpMethod.Method.ToUpper(),
                uri = $"{HttpContext.Current.Request.Url.Scheme}://{HttpContext.Current.Request.Url.Host}{System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.TrimEnd('/')}/{apiDescription.RelativePathSansQueryString()}",
                responses = GetAWSResponse(operation),
                requestParameters = GetAWSRequest(operation)
            });
        }

        /// <summary>
        /// Gets the aws request.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        private object GetAWSRequest(Operation operation) {
            var requestParameters = new Dictionary<string, string>();
            var methodParameterFormat = "method.request.{0}.{1}";
            var integrationParameterFormat = "integration.request.{0}.{1}";

            if (operation.parameters == null || operation.parameters.Count <= 0) return requestParameters;
            foreach (var parameter in operation.parameters) {
                switch (parameter.@in) {
                    case "query":
                        requestParameters.Add(string.Format(integrationParameterFormat, "querystring", parameter.name),
                            string.Format(methodParameterFormat, "querystring", parameter.name));
                        break;
                    case "header":
                        requestParameters.Add(string.Format(integrationParameterFormat, "header", parameter.name),
                            string.Format(methodParameterFormat, "header", parameter.name));
                        break;
                }
            }

            return requestParameters;
        }

        private object GetAWSResponse(Operation operation) {
            var awsResponses = new Dictionary<string, object>();
            var responseParameters = new Dictionary<string, string>();

            if (IntegrationSettings.allowedAccessControl.AllowHeaders)
                responseParameters.Add("method.response.header.Access-Control-Allow-Headers", $"'{IntegrationSettings.allowedAccessControl.HeaderValue}'");

            if (IntegrationSettings.allowedAccessControl.AllowMethods)
                responseParameters.Add("method.response.header.Access-Control-Allow-Methods", $"'{IntegrationSettings.allowedAccessControl.MethodValue}'");

            if (IntegrationSettings.allowedAccessControl.AllowOrigin)
                responseParameters.Add("method.response.header.Access-Control-Allow-Origin", $"'{IntegrationSettings.allowedAccessControl.OriginValue}'");

            foreach (var response in operation.responses) {
                awsResponses.Add(response.Key, new
                {
                    statusCode = response.Key,
                    responseParameters = responseParameters
                });
            }

            awsResponses.Add("default", new
            {
                statusCode = "200",
                responseParameters = responseParameters
            });

            return awsResponses;
        }

        private void CreateOptionsMethod(Operation operation) {
            foreach (var response in operation.responses) {

                if (response.Value.headers == null) {
                    response.Value.headers = new Dictionary<string, Header>();
                }

                if (response.Value.vendorExtensions == null) {
                    response.Value.vendorExtensions = new Dictionary<string, object>();
                }

                foreach (var allowedAccessControl in IntegrationSettings.allowedAccessControl.GetAllowedAccessControls()) {
                    response.Value.headers.Add(allowedAccessControl, new Header() { type = "string" });
                }
            }

            var responseParameters = new Dictionary<string, string>();
            var awsResponses = new Dictionary<string, object>();
            var requestTemplates = new Dictionary<string, string>();

            operation.consumes.Add("application/json");
            operation.consumes.Add("text/json");

            operation.operationId = null;

            requestTemplates.Add("application/json", "{\"statusCode\": 200}");

            if (IntegrationSettings.allowedAccessControl.AllowHeaders)
                responseParameters.Add("method.response.header.Access-Control-Allow-Headers", $"'{IntegrationSettings.allowedAccessControl.HeaderValue}'");

            if (IntegrationSettings.allowedAccessControl.AllowMethods)
                responseParameters.Add("method.response.header.Access-Control-Allow-Methods", $"'{IntegrationSettings.allowedAccessControl.MethodValue}'");

            if (IntegrationSettings.allowedAccessControl.AllowOrigin)
                responseParameters.Add("method.response.header.Access-Control-Allow-Origin", $"'{IntegrationSettings.allowedAccessControl.OriginValue}'");

            awsResponses.Add("default", new
            {
                statusCode = "200",
                responseParameters = responseParameters
            });

            operation.vendorExtensions.Add("x-amazon-apigateway-integration", new
            {
                type = "mock",
                responses = awsResponses,
                passthroughBehavior = "when_no_match",
                requestTemplates = requestTemplates
            });
        }
    }
}
