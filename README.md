# Swagger.Plugin

Swagger.Plugin is a *OperationFilter* for **Swagger**. It can easily modify Swagger Json file to have _[x-amazon-apigateway-integration](https://docs.aws.amazon.com/apigateway/latest/developerguide/api-gateway-swagger-extensions-integration.html)_ and your Swagger Json file is ready to import into AWS API Gateway.

## Dependencies

-   [Swashbuckle](https://www.nuget.org/packages/Swashbuckle/)  (>= 5.5.3)
-   [Swashbuckle.Core](https://www.nuget.org/packages/Swashbuckle.Core/)  (>= 5.5.3)

## How to use

Install from Nuget, use below command to get reference  

    Install-Package Swagger.Plugin -Version 1.0.2

Open SwaggerConfig.cs file in _~/App_Start_ Use below code to have _x-amazon-apigateway-integration_ in swagger Json file.
```javascript
	GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
	                  c.OperationFilter<ApiGatewayIntegrationFilter>();  
                    });
  
```
## Integration Settings

You can create object of _ApiGatewayIntegrationSettings_ class to apply configure  *Request | Response headers, Method Request, Method Response, parameters and query strings*.

You can also add **extra response codes** which are not mentioned on you webapi action methods.
```javascript
	// For example you have to put 409 response for all you API you need not to 
	// modify you code jut use below setting and this will add into your swagger Json file
	var integrationSettings = new ApiGatewayIntegrationSettings();
	integrationSettings.AddResponse("409", new Swashbuckle.Swagger.Response());
	
	GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
					  ApiGatewayIntegrationFilter.IntegrationSettings = integrationSettings;                    
	                  c.OperationFilter<ApiGatewayIntegrationFilter>();  
                    });
```
Your Swagger file will look like this.

```json
"paths": {
	"/api/Account/UserInfo": {
		"get": {
			"tags": ["Account"],
			"operationId": "Account_GetUserInfo",
			"consumes": [],
			"produces": ["application/json",
			"text/json",
			"application/xml",
			"text/xml"],
			"parameters": [],
			"responses": {
				"200": {
					"description": "200",
					"schema": {
						"$ref": "#/definitions/UserInfoViewModel"
					},
					"headers": {
						
					}
				},
				"409": {
					"description": "409",
					"headers": {
						
					}
				}
			},
			"x-amazon-apigateway-integration": {
				"type": "http",
				"httpMethod": "GET",
				"uri": "http: //localhost/api/Account/UserInfo",
				"responses": {
					"200": {
						"statusCode": "200",
						"responseParameters": {
							
						}
					},
					"409": {
						"statusCode": "409",
						"responseParameters": {
							
						}
					},
					"default": {
						"statusCode": "200",
						"responseParameters": {
							
						}
					}
				},
				"requestParameters": {
					
				}
			}
		}
	}
```
Adding common parameters(Header,QueryString, etc.) by using below code.
```javascript
ApiGatewayIntegrationFilter.IntegrationSettings.AddGlobalActionParameter(new Parameter() {
                            name = "x-api-your-custom-header",
                            @in = "header",
                            type = "string",
                            required = false
                        });

```
```json
"x-amazon-apigateway-integration": {
	"type": "http",
	"httpMethod": "GET",
	"uri": "http://localhost/api/Account/UserInfo",
	"responses": {
		"200": {
			"statusCode": "200",
			"responseParameters": {
				
			}
		},
		"409": {
			"statusCode": "409",
			"responseParameters": {
				
			}
		},
		"default": {
			"statusCode": "200",
			"responseParameters": {
				
			}
		}
	},
	"requestParameters": {
		"integration.request.header.Authorization": "method.request.header.Authorization",
		"integration.request.header.x-api-your-custom-header": "method.request.header.x-api-your-custom-header"
	}
}
``` 

Access control settings can be enabled by below code.

```javascript
ApiGatewayIntegrationFilter.IntegrationSettings
                            .AccessControlSettings()
                            .AllowHeadersWtihValue("Content-Type,X-Amz-Date,Authorization,x-api-custom,X-Api-Key")
                            .AllowMethodWtihValue("*")
                            .AllowOrigenWtihValue("*");
```

Now you swagger file *x-amazon-apigateway-integration* property  should look like this.

```json
"x-amazon-apigateway-integration": {
	"type": "http",
	"httpMethod": "GET",
	"uri": "http://localhost/api/Account/UserInfo",
	"responses": {
		"200": {
			"statusCode": "200",
			"responseParameters": {
				"method.response.header.Access-Control-Allow-Headers": "'Content-Type,X-Amz-Date,Authorization,x-api-custom,X-Api-Key'",
				"method.response.header.Access-Control-Allow-Methods": "'*'",
				"method.response.header.Access-Control-Allow-Origin": "'*'"
			}
		},
		"409": {
			"statusCode": "409",
			"responseParameters": {
				"method.response.header.Access-Control-Allow-Headers": "'Content-Type,X-Amz-Date,Authorization,x-api-custom,X-Api-Key'",
				"method.response.header.Access-Control-Allow-Methods": "'*'",
				"method.response.header.Access-Control-Allow-Origin": "'*'"
			}
		},
		"default": {
			"statusCode": "200",
			"responseParameters": {
				"method.response.header.Access-Control-Allow-Headers": "'Content-Type,X-Amz-Date,Authorization,x-api-custom,X-Api-Key'",
				"method.response.header.Access-Control-Allow-Methods": "'*'",
				"method.response.header.Access-Control-Allow-Origin": "'*'"
			}
		}
	},
	"requestParameters": {
		"integration.request.header.Authorization": "method.request.header.Authorization",
		"integration.request.header.x-api-your-custom-header": "method.request.header.x-api-your-custom-header"
	}
}
}
```
You can now save your swagger file and import it to AWS API Gateway.

