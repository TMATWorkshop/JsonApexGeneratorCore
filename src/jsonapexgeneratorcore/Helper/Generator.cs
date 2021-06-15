using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;
using System.Globalization;

namespace JsonApexGeneratorCore.Helper {
    public class Generator {

        private String ASSETPATH = ""; //Local
        //JSON to Variable Conversion
        //Handle Reserved Words
        public List<String> reservedWords = new List<String>() { 
                "abstract", "activate", "and", "any", "array", "as", "asc", "autonomous", 
                "begin", "bigdecimal", "blob", "boolean", "break", "bulk", "by", "byte", 
                "case", "cast", "catch", "char", "class", "collect", "commit", "const", "continue", "currency", 
                "date", "datetime", "decimal", "default", "delete", "desc", "do", "double", 
                "else", "end", "enum", "exception", "exit", "export", "extends", 
                "false", "final", "finally", "float", "for", "from", 
                "global", "goto", "group", 
                "having", "hint", 
                "if", "implements", "import", "in", "inner", "insert", "instanceof", "int", "integer", "interface", "into", 
                "join", 
                "like", "limit", "list", "long", "loop", 
                "map", "merge", 
                "new", "not", "null", "nulls", "number", 
                "object", "of", "on", "or", "outer", "override", 
                "package", "parallel", "pragma", "private", "protected", "public", 
                "retrieve", "return", "rollback", 
                "select", "set", "short", "sObject", "sort", "static", "string", "super", "switch", "synchronized", "system", 
                "testmethod", "then", "this", "throw", "time", "transaction", "trigger", "true", "try", 
                "undelete", "update", "upsert", "using", 
                "virtual", "void", 
                "webservice", "when", "where", "while"};

        //Placeholders to replace
        //Handler
        private String className = "";
        private String namedCredential = "";
        private String urlExtension = "";
        private String calloutMethod = "POST";
        private String requestJSON = "";
        private String responseJSON = "";
        private String requestParamsWithType = "";
        private String requestParams = "";
        private String requestJSONWrapperVars = "";
        private String requestJSONWrapperVarsThis = "";
        private String responseJSONWrapperVars = "";
        private String responseJSONExplicitParse = "";
        private String otherClasses = ""; //in next iteration
        private Boolean usesStringArrayMethod = false;
        private String stringArrayMethod = "";
        private Boolean usesIntegerArrayMethod = false;
        private String integerArrayMethod = "";
        private Boolean usesDoubleArrayMethod = false;
        private String doubleArrayMethod = "";
        private TextInfo enUS = new CultureInfo("en-US",false).TextInfo;

        public Generator(String assetPath, String className, String namedCredential, String urlExtension, String calloutMethod, String requestJSON, String responseJSON) {
            this.ASSETPATH = assetPath;
            this.className = className;
            this.namedCredential = namedCredential;
            this.calloutMethod = calloutMethod;
            this.requestJSON = requestJSON;
            this.responseJSON = responseJSON;
            this.urlExtension = urlExtension;
            prepareVariables();
            loadArrayMethods();
        }

        public List<Models.FileModel> generateFiles() {
            List<Models.FileModel> fileModels = new List<Models.FileModel>();

            Models.FileModel wrapper = new Models.FileModel();
            wrapper.name = this.className + "Wrapper";
            String wrapperTemplate = File.ReadAllText(ASSETPATH + "wrapper.txt");
            wrapper.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(wrapperTemplate));
            fileModels.Add(wrapper);

            Models.FileModel wrapperTest = new Models.FileModel();
            wrapperTest.name = this.className + "WrapperTest";
            String wrapperTestTemplate = File.ReadAllText(ASSETPATH + "wrapperTest.txt");
            wrapperTest.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(wrapperTestTemplate));
            fileModels.Add(wrapperTest);

            Models.FileModel handler = new Models.FileModel();
            handler.name = this.className + "Handler";
            String handlerTemplate = File.ReadAllText(ASSETPATH + "handler.txt");
            handler.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(handlerTemplate));
            fileModels.Add(handler);

            Models.FileModel handlerTest = new Models.FileModel();
            handlerTest.name = this.className + "HandlerTest";
            String handlerTestTemplate = File.ReadAllText(ASSETPATH + "handlerTest.txt");
            handlerTest.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(handlerTestTemplate));
            fileModels.Add(handlerTest);

            Models.FileModel mock = new Models.FileModel();
            mock.name = this.className + "Mock";
            String mockTemplate = File.ReadAllText(ASSETPATH + "mock.txt");
            mock.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(mockTemplate));
            fileModels.Add(mock);

            return fileModels;
        }

        private void prepareVariables() {
            String requestSubClassTemplate = File.ReadAllText(ASSETPATH + "requestsubclass.txt");
            List<JSONPropertyParseResult> requestResults = jsonToClass(this.requestJSON, JSONType.Request);
            for (int i = 0; i < requestResults.Count; i++ ) {
                this.requestParamsWithType += requestResults[i].parameterization;
                this.requestParams += requestResults[i].paramName;
                if (i != requestResults.Count -1) {
                    this.requestParamsWithType +=  ", ";
                    this.requestParams += ", ";
                }
                this.requestJSONWrapperVars += requestResults[i].variableDeclaration + Environment.NewLine;
                this.requestJSONWrapperVarsThis += requestResults[i].thisParameterization + Environment.NewLine;
            }
            String responseSubClassTemplate = File.ReadAllText(ASSETPATH + "responsesubclass.txt");
            List<JSONPropertyParseResult> responseResults = jsonToClass(this.responseJSON, JSONType.Response);
            for (int i = 0; i < responseResults.Count; i++ ) {
                this.responseJSONWrapperVars += responseResults[i].variableDeclaration + Environment.NewLine;
                if (i > 0) {
                    this.responseJSONExplicitParse += "else ";
                }
                this.responseJSONExplicitParse += responseResults[i].explicitParse + Environment.NewLine;
            }
        }

        private void loadArrayMethods() {
            if (this.usesDoubleArrayMethod) {
                this.doubleArrayMethod = File.ReadAllText(ASSETPATH + "doubleArrayMethod.txt");
            }

            if (this.usesIntegerArrayMethod) {
                this.doubleArrayMethod = File.ReadAllText(ASSETPATH + "integerArrayMethod.txt");
            }

            if (this.usesStringArrayMethod) {
                this.doubleArrayMethod = File.ReadAllText(ASSETPATH + "stringArrayMethod.txt");
            }
        }

        //Handle Sub Branches
        private List<JSONPropertyParseResult> jsonToClass(String jsonStr, JSONType jsonType) {
            List<JSONPropertyParseResult> lstResult = new List<JSONPropertyParseResult>();
            String subClassTemplate = "";
            if (jsonType == JSONType.Request) {
                subClassTemplate = File.ReadAllText(ASSETPATH + "requestsubclass.txt");
            }
            if (jsonType == JSONType.Response) {
                subClassTemplate = File.ReadAllText(ASSETPATH + "responsesubclass.txt");
            }
            //Parse JSON
            using(JsonDocument parsedJSON = JsonDocument.Parse(jsonStr)) {
                JsonElement root = parsedJSON.RootElement;

                foreach(JsonProperty property in root.EnumerateObject()) {
                    JSONPropertyParseResult result;
                    if (property.Value.ValueKind == JsonValueKind.Object) {
                        result = parseVarType(property, subClassTemplate, jsonType);
                    }
                    else {
                        result = parseVarType(property, "", jsonType);
                    }
                    if (result.parsed) {
                        lstResult.Add(result);
                    }
                }
            }
            return lstResult;
        }
        
        //This will keep the Mapping between JSONType and Apex Primitives
        private JSONPropertyParseResult parseVarType (JsonProperty property, String subClassTemplate, JSONType jsonType) {
            String propertyName = property.Name;
            propertyName = JsonNamingPolicy.CamelCase.ConvertName(propertyName);
            if (reservedWords.Contains(propertyName)) {
                propertyName = "_" + propertyName;
            }
            JSONPropertyParseResult result = new JSONPropertyParseResult(propertyName);

            switch (property.Value.ValueKind) {
                case JsonValueKind.String:
                    result.variableDeclaration = "String " + propertyName + " { get; set; }";
                    result.parameterization = "String " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = parser.getText(); }"; 
                    result.parsed = true;
                    break;
                case JsonValueKind.Number:
                    if (property.Value.ToString().Contains(".")) {
                        result.variableDeclaration = "Decimal " + propertyName + " { get; set; }";
                        result.parameterization = "Decimal " + propertyName;
                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                        result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = parser.getDoubleValue(); }";
                        result.parsed = true;
                    }
                    else {
                        result.variableDeclaration = "Integer " + propertyName + " { get; set; }";
                        result.parameterization = "Integer " + propertyName;
                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                        result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = parser.getIntegerValue(); }";
                        result.parsed = true;
                    }
                    break;
                case JsonValueKind.Array:
                    if (property.Value.GetArrayLength() > 0) {
                        foreach (JsonElement arrayValue in property.Value.EnumerateArray()) {
                            switch (arrayValue.ValueKind) {
                                case JsonValueKind.String:
                                    this.usesStringArrayMethod = true;
                                    result.variableDeclaration = "List<String> " + propertyName + " { get; set; }";
                                    result.parameterization = "List<String> " + propertyName;
                                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                                    result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = arrayOfString(parser); ); }";
                                    result.parsed = true;
                                    break;
                                case JsonValueKind.Number:
                                    if (arrayValue.ToString().Contains(".")) {
                                        this.usesDoubleArrayMethod = true;
                                        result.variableDeclaration = "List<Decimal> " + propertyName + " { get; set; }";
                                        result.parameterization = "List<Decimal> " + propertyName;
                                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                                    result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = arrayOfDouble(parser); ); }";
                                        result.parsed = true;
                                    }
                                    else {
                                        this.usesIntegerArrayMethod = true;
                                        result.variableDeclaration = "List<Integer> " + propertyName + " { get; set; }";
                                        result.parameterization = "List<Integer> " + propertyName;
                                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                                        result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = arrayOfInteger(parser); ); }";
                                        result.parsed = true;
                                    }
                                    break;
                            }
                            break;
                        }
                    }
                    else {
                        this.usesStringArrayMethod = true;
                        result.variableDeclaration = "List<String> " + propertyName + " { get; set; }";
                        result.parameterization = "List<String> " + propertyName;
                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                        result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = arrayOfString(parser); ); }";
                        result.parsed = true;
                    }
                    break;
                case JsonValueKind.True:
                    result.variableDeclaration =  "Boolean " + propertyName + " { get; set; }";
                    result.parameterization = "Boolean " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = parser.getBooleanValue(); }";
                    result.parsed = true;
                    break;
                case JsonValueKind.False:
                    result.variableDeclaration =  "Boolean " + propertyName + " { get; set; }";
                    result.parameterization = "Boolean " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = parser.getBooleanValue(); }";
                    result.parsed = true;
                    break;
                case JsonValueKind.Object:
                    String subclassName = propertyName.ToLowerInvariant();
                    subclassName = enUS.ToTitleCase(subclassName).Replace(" ", String.Empty);
                    createSubClass(property, subclassName, subClassTemplate, jsonType);
                    result.variableDeclaration = subclassName + " " + propertyName + " { get; set; }";
                    result.parameterization = subclassName + " " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = new " + subclassName + "();";
                    result.explicitParse = "if (text == '" + property.Name + "') { " + propertyName + " = new " + propertyName + "(parser); }";
                    result.parsed = true;
                    break;
            }
            
            return result;
        }

        private String replaceTemplate(String templateString) {
            templateString = templateString.Replace("{className}", this.className);
            templateString = templateString.Replace("{namedCredential}", this.namedCredential);
            templateString = templateString.Replace("{calloutMethod}", this.calloutMethod);
            templateString = templateString.Replace("{urlExtension}", this.urlExtension);

            templateString = templateString.Replace("{requestParamsWithType}", this.requestParamsWithType);
            templateString = templateString.Replace("{requestParams}", this.requestParams);
            //templateString = templateString.Replace("{requestJSON}", this.requestJSON.Replace(Environment.NewLine, " "));
            templateString = templateString.Replace("{requestJSON}", Regex.Replace(this.requestJSON, @"\r\n?|\n", " "));
            templateString = templateString.Replace("{requestJSONWrapperVars}", this.requestJSONWrapperVars);
            templateString = templateString.Replace("{requestJSONWrapperVarsThis}", this.requestJSONWrapperVarsThis);
            //templateString = templateString.Replace("{responseJSON}", this.responseJSON.Replace(Environment.NewLine, " "));
            templateString = templateString.Replace("{responseJSON}", Regex.Replace(this.responseJSON, @"\r\n?|\n", " "));
            templateString = templateString.Replace("{responseJSONWrapperVars}", this.responseJSONWrapperVars);
            templateString = templateString.Replace("{parseCode}", this.responseJSONExplicitParse);
            String arrayMethod = "";
            if (usesDoubleArrayMethod) {
                arrayMethod += doubleArrayMethod + Environment.NewLine + Environment.NewLine;
            }
            if (usesIntegerArrayMethod) {
                arrayMethod += integerArrayMethod + Environment.NewLine + Environment.NewLine;
            }
            if (usesStringArrayMethod) {
                arrayMethod += stringArrayMethod + Environment.NewLine + Environment.NewLine;

            }
            if (arrayMethod.Length > 0) {
                templateString = templateString.Replace("{arrayMethods}", arrayMethod);
            }
            else {
                templateString = templateString.Replace("{arrayMethods}", "");
            }

            templateString = templateString.Replace("{otherClasses}", this.otherClasses);
            return templateString;
        }

        //Return Sub Class Name
        private void createSubClass(JsonProperty property, String subClassName, String template, JSONType jsonType) {
            List<JSONPropertyParseResult> results = jsonToClass(property.Value.ToString(), jsonType);

            String subClassResult = replaceSubClassVariables(template, subClassName, results);
            this.otherClasses += (this.otherClasses.Length > 0) ? Environment.NewLine + subClassResult : subClassResult;
        }

        private String replaceSubClassVariables(String template, String subClassName, List<JSONPropertyParseResult> results) {
            String wrapperVars = "";
            String parseCode = "";
            String methodParams = "";
            String thisWrapperVars = "";

            for (int i = 0; i < results.Count; i++ ) {
                wrapperVars += results[i].variableDeclaration + Environment.NewLine;
                thisWrapperVars += results[i].thisParameterization + Environment.NewLine;

                methodParams += results[i].parameterization;
                if (i != results.Count -1) {
                    methodParams += ", ";
                }
                if (i > 0) {
                    parseCode += "else ";
                }
                parseCode += results[i].explicitParse + Environment.NewLine;
            }

            template = template.Replace("{subClassName}", subClassName);

            //Response
            template = template.Replace("{responseJSONWrapperVars}", wrapperVars);
            template = template.Replace("{parseCode}", parseCode);

            //Request
            template = template.Replace("{requestJSONWrapperVars}", wrapperVars);
            template = template.Replace("{requestParams}", methodParams);
            template = template.Replace("{requestJSONWrapperVarsThis}", thisWrapperVars);

            return template;
        }

        public class JSONPropertyParseResult {
            public String paramName;
            public String variableDeclaration;
            public String parameterization;
            public String thisParameterization;
            public String explicitParse;
            public Boolean parsed = false;

            public JSONPropertyParseResult (String paramName) {
                this.paramName = paramName;
            }
        }

        enum JSONType {
            Request,
            Response
        }
    }
    
}