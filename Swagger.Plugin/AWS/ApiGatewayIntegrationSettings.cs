using System;
using System.Collections.Generic;
using Swashbuckle.Swagger;

namespace Swagger.Plugin.AWS {
    public class ApiGatewayIntegrationSettings {

        /// <summary>
        /// The extra responses
        /// </summary>
        internal IDictionary<string, Response> extraResponses;


        /// <summary>
        /// Gets or sets the global action parameters.
        /// </summary>
        /// <value>
        /// The global action parameters.
        /// </value>
        /// <example>Authorization</example>
        internal IList<Parameter> globalActionParameters { get; set; }

        /// <summary>
        /// The allowed access control
        /// </summary>
        internal AccessControl allowedAccessControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiGatewayIntegrationSettings"/> class.
        /// </summary>
        public ApiGatewayIntegrationSettings() {
            extraResponses = new Dictionary<string, Response>();
            allowedAccessControl = new AccessControl();
            globalActionParameters = new List<Parameter>();
        }

        /// <summary>
        /// Adds the response. Add only those responses which are not decorated on action methods.
        /// </summary>
        /// <param name="responseCode">The response code.</param>
        /// <param name="response">The response.</param>
        /// <exception cref="ArgumentException">response can not be empty!</exception>
        public void AddResponse(string responseCode, Response response) {
            if (string.IsNullOrWhiteSpace(responseCode))
                throw new ArgumentException("response can not be empty!");

            if (!extraResponses.ContainsKey(responseCode))
                extraResponses.Add(responseCode, response ?? new Response());
        }

        /// <summary>
        /// Adds the global action parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentException">response can not be empty!</exception>
        public void AddGlobalActionParameter(Parameter parameter) {
            if (parameter != null)
                globalActionParameters.Add(parameter);
        }

        public AccessControl AccessControlSettings()
        {
            return allowedAccessControl;
        }

        /// <summary>
        /// 
        /// </summary>
        public class AccessControl {
            /// <summary>
            /// The access control allow headers
            /// </summary>
            internal const string ACCESS_CONTROL_ALLOW_HEADERS = "Access-Control-Allow-Headers";
            /// <summary>
            /// The access control allow methods
            /// </summary>
            internal const string ACCESS_CONTROL_ALLOW_METHODS = "Access-Control-Allow-Methods";
            /// <summary>
            /// The access control allow origin
            /// </summary>
            internal const string ACCESS_CONTROL_ALLOW_ORIGIN = "Access-Control-Allow-Origin";

            /// <summary>
            /// Gets or sets a value indicating whether [allow headers].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [Access-Control-Allow-Headers]; otherwise, <c>false</c>.
            /// </value>
            internal bool AllowHeaders { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [allow methods].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [Access-Control-Allow-Methods]; otherwise, <c>false</c>.
            /// </value>
            internal bool AllowMethods { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether [allow origin].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [Access-Control-Allow-Origin]; otherwise, <c>false</c>.
            /// </value>
            internal bool AllowOrigin { get; set; }

            /// <summary>
            /// Gets or sets the header value.
            /// </summary>
            /// <value>
            /// The header value.
            /// </value>
            internal string HeaderValue { get; set; }
            /// <summary>
            /// Gets or sets the header methods.
            /// </summary>
            /// <value>
            /// The header methods.
            /// </value>
            internal string MethodValue { get; set; }
            /// <summary>
            /// Gets or sets the header origin.
            /// </summary>
            /// <value>
            /// The header origin.
            /// </value>
            internal string OriginValue { get; set; }

            /// <summary>
            /// Gets the allowed access controls.
            /// </summary>
            /// <returns></returns>
            /// 
            internal IList<string> GetAllowedAccessControls() {
                var allowedAccessControl = new List<string>();

                if (AllowOrigin)
                    allowedAccessControl.Add(ACCESS_CONTROL_ALLOW_ORIGIN);
                if (AllowMethods)
                    allowedAccessControl.Add(ACCESS_CONTROL_ALLOW_METHODS);
                if (AllowHeaders)
                    allowedAccessControl.Add(ACCESS_CONTROL_ALLOW_HEADERS);

                return allowedAccessControl;
            }

            /// <summary>
            /// Headerses the wtih value.
            /// </summary>
            /// <param name="headerValue">The header value.</param>
            /// <example>Content-Type,X-Amz-Date,Authorization,X-Api-Key</example>
            /// <returns></returns>
            public AccessControl AllowHeadersWtihValue(string headerValue)
            {
                AllowHeaders = true;
                HeaderValue = headerValue;
                return this;
            }

            /// <summary>
            /// Allows the method wtih value.
            /// </summary>
            /// <param name="methodValue">The method value.</param>
            /// <example>*</example>
            /// <returns></returns>
            public AccessControl AllowMethodWtihValue(string methodValue) {
                AllowMethods = true;
                MethodValue = methodValue;
                return this;
            }

            /// <summary>
            /// Allows the origen wtih value.
            /// </summary>
            /// <param name="origenValue">The origen value.</param>
            /// <example>*</example>
            /// <returns></returns>
            public AccessControl AllowOrigenWtihValue(string origenValue) {
                AllowOrigin = true;
                OriginValue = origenValue;
                return this;
            }

        }
    }
}
