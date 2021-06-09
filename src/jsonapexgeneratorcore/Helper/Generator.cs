using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings;
using System.IO;

namespace JsonApexGeneratorCore.Helper {
    public class Generator {
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
        private String calloutMethod = "POST";
        private String requestJSON = "";
        private String responseJSON = "";
        private String requestParamsWithType = "";
        private String requestParams = "";
        private String requestJSONWrapperVars = "";
        private String requestJSONWrapperVarsThis = "";
        private String responseJSONWrapperVars = "";
        private String otherClasses = ""; //in next iteration

        public Generator(String className, String namedCredential, String calloutMethod, String requestJSON, String responseJSON) {
            this.className = className;
            this.namedCredential = namedCredential;
            this.calloutMethod = calloutMethod;
            this.requestJSON = requestJSON;
            this.responseJSON = responseJSON;
            prepareVariables();
        }

        public List<Models.FileModel> generateFiles() {
            List<Models.FileModel> fileModels = new List<Models.FileModel>();
            //Templates templateHelper = new Templates();

            Models.FileModel wrapper = new Models.FileModel();
            wrapper.name = this.className + "Wrapper";
            String wrapperTemplate = File.ReadAllText("../src/jsonapexgeneratorcore/Assets/wrapper.txt");
            wrapper.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(wrapperTemplate));
            fileModels.Add(wrapper);

            Models.FileModel wrapperTest = new Models.FileModel();
            wrapperTest.name = this.className + "WrapperTest";
            //String wrapperTestTemplate = templateHelper.wrapperTemplate;
            String wrapperTestTemplate = File.ReadAllText("../src/jsonapexgeneratorcore/Assets/wrapperTest.txt");
            wrapperTest.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(wrapperTestTemplate));
            fileModels.Add(wrapperTest);

            Models.FileModel handler = new Models.FileModel();
            handler.name = this.className + "Handler";
            //String handlerTemplate = templateHelper.wrapperTemplate;
            String handlerTemplate = File.ReadAllText("../src/jsonapexgeneratorcore/Assets/handler.txt");
            handler.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(handlerTemplate));
            fileModels.Add(handler);

            Models.FileModel handlerTest = new Models.FileModel();
            handlerTest.name = this.className + "HandlerTest";
            //String handlerTestTemplate = templateHelper.wrapperTemplate;
            String handlerTestTemplate = File.ReadAllText("../src/jsonapexgeneratorcore/Assets/handlerTest.txt");
            handlerTest.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(handlerTestTemplate));
            fileModels.Add(handlerTest);

            Models.FileModel mock = new Models.FileModel();
            mock.name = this.className + "Mock";
            //String mockTemplate = templateHelper.wrapperTemplate;
            String mockTemplate = File.ReadAllText("../src/jsonapexgeneratorcore/Assets/mock.txt");
            mock.body = System.Text.Encoding.ASCII.GetBytes(replaceTemplate(mockTemplate));
            fileModels.Add(mock);

            return fileModels;
        }

        private void prepareVariables() {
            List<JSONParseResult> requestResults = jsonToClass(this.requestJSON);
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
            List<JSONParseResult> responseResults = jsonToClass(this.responseJSON);
            for (int i = 0; i < responseResults.Count; i++ ) {
                this.responseJSONWrapperVars += responseResults[i].thisParameterization + Environment.NewLine;
            }
        }

        //Handle Sub Branches
        private List<JSONParseResult> jsonToClass(String jsonStr) {
            List<JSONParseResult> lstResult = new List<JSONParseResult>();
            
            //Parse JSON
            using(JsonDocument parsedJSON = JsonDocument.Parse(jsonStr)) {
                JsonElement root = parsedJSON.RootElement;

                foreach(JsonProperty property in root.EnumerateObject()) {
                    if (property.Value.ValueKind == JsonValueKind.Object) {
                        foreach(JsonProperty propertySub in property.Value.EnumerateObject()) {	
                            //TODO:
                        }
                    }
                    else {
                        JSONParseResult result = parseVarType(property);
                        if (result.parsed) {
                            lstResult.Add(result);
                        }
                    }
                }
            }
            return lstResult;
        }
        
        //This will keep the Mapping between JSONType and Apex Primitives
        private JSONParseResult parseVarType (JsonProperty property) {
            String propertyName = property.Name;
            if (reservedWords.Contains(propertyName)) {
                propertyName = "_" + propertyName;
            }
            JSONParseResult result = new JSONParseResult(propertyName);

            switch (property.Value.ValueKind) {
                case JsonValueKind.String:
                    result.variableDeclaration = "String " + propertyName + " { get; set; }";
                    result.parameterization = "String " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.parsed = true;
                    break;
                case JsonValueKind.Number:
                    if (property.Value.ToString().Contains(".")) {
                        result.variableDeclaration = "Decimal " + propertyName + " { get; set; }";
                        result.parameterization = "Decimal " + propertyName;
                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                        result.parsed = true;
                    }
                    else {
                        result.variableDeclaration = "Integer " + propertyName + " { get; set; }";
                        result.parameterization = "Integer " + propertyName;
                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                        result.parsed = true;
                    }
                    break;
                case JsonValueKind.Array:
                    if (property.Value.GetArrayLength() > 0) {
                        foreach (JsonElement arrayValue in property.Value.EnumerateArray()) {
                            switch (arrayValue.ValueKind) {
                                case JsonValueKind.String:
                                    result.variableDeclaration = "List<String> " + propertyName + " { get; set; }";
                                    result.parameterization = "List<String> " + propertyName;
                                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                                    result.parsed = true;
                                    break;
                                case JsonValueKind.Number:
                                    if (arrayValue.ToString().Contains(".")) {
                                        result.variableDeclaration = "List<Decimal> " + propertyName + " { get; set; }";
                                        result.parameterization = "List<Decimal> " + propertyName;
                                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                                        result.parsed = true;
                                    }
                                    else {
                                        result.variableDeclaration = "List<Integer> " + propertyName + " { get; set; }";
                                        result.parameterization = "List<Integer> " + propertyName;
                                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                                        result.parsed = true;
                                    }
                                    break;
                            }
                            break;
                        }
                    }
                    else {
                        result.variableDeclaration = "List<String> " + propertyName + " { get; set; }";
                        result.parameterization = "List<String> " + propertyName;
                        result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                        result.parsed = true;
                    }
                    break;
                case JsonValueKind.True:
                    result.variableDeclaration =  "Boolean " + propertyName + " { get; set; }";
                    result.parameterization = "Boolean " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.parsed = true;
                    break;
                case JsonValueKind.False:
                    result.variableDeclaration =  "Boolean " + propertyName + " { get; set; }";
                    result.parameterization = "Boolean " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.parsed = true;
                    break;
                case JsonValueKind.Object:
                    //On Next Version to handle multi-level 
                    result.variableDeclaration = "String " + propertyName + " { get; set; } //On next iteration of JSONAPEXGenerator";
                    result.parameterization = "String " + propertyName;
                    result.thisParameterization = "this." + propertyName + " = " + propertyName + ";";
                    result.parsed = true;
                    break;
                
            }
            
            return result;
        }

        private String replaceTemplate(String templateString) {
            templateString = templateString.Replace("{className}", this.className);
            templateString = templateString.Replace("{namedCredential}", this.namedCredential);
            templateString = templateString.Replace("{calloutMethod}", this.calloutMethod);
            templateString = templateString.Replace("{requestParamsWithType}", this.requestParamsWithType);
            templateString = templateString.Replace("{requestParams}", this.requestParams);
            templateString = templateString.Replace("{requestJSON}", this.requestJSON);
            templateString = templateString.Replace("{requestJSONWrapperVars}", this.requestJSONWrapperVars);
            templateString = templateString.Replace("{requestJSONWrapperVarsThis}", this.requestJSONWrapperVarsThis);
            templateString = templateString.Replace("{responseJSON}", this.responseJSON);
            templateString = templateString.Replace("{responseJSONWrapperVars}", this.responseJSONWrapperVars);
            templateString = templateString.Replace("{otherClasses}", "");

            return templateString;
        }

        //Return Sub Class Name
        private String createSubClass(JsonProperty property) {
            String className = property.Name;

            return className;
        }

        public class JSONParseResult {
            public String paramName;
            public String variableDeclaration;
            public String parameterization;
            public String thisParameterization;
            public Boolean parsed = false;

            public JSONParseResult (String paramName) {
                this.paramName = paramName;
            }
        }
    }
    
}