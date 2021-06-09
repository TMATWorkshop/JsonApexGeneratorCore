using System;

namespace JsonApexGeneratorCore.Helper
{
    public class Templates
    {

        public string wrapperTemplate = "";

        public Templates()
        {
            this.wrapperTemplate = "/* Class Generated with JSONApexGeneratorCore */" + Environment.NewLine;
            this.wrapperTemplate = "public class {className}Wrapper {" + Environment.NewLine;
            this.wrapperTemplate = "public class Request {" + Environment.NewLine;
            this.wrapperTemplate = "{requestJSONWrapperVars}" + Environment.NewLine;
            this.wrapperTemplate = "" + Environment.NewLine;
            this.wrapperTemplate = "public Request ({requestParams}) {" + Environment.NewLine;
            this.wrapperTemplate = "{requestJSONWrapperVarsThis}" + Environment.NewLine;
            this.wrapperTemplate = "}" + Environment.NewLine;
            this.wrapperTemplate = "}" + Environment.NewLine;
            this.wrapperTemplate = "" + Environment.NewLine;
            this.wrapperTemplate = "public class Response {" + Environment.NewLine;
            this.wrapperTemplate = "{responseJSONWrapperVars}" + Environment.NewLine;
            this.wrapperTemplate = "}" + Environment.NewLine;
            this.wrapperTemplate = "" + Environment.NewLine;
            this.wrapperTemplate = "{otherClasses}" + Environment.NewLine;
            this.wrapperTemplate = "" + Environment.NewLine;
            this.wrapperTemplate = "public static Response parseResponse(String json) {" + Environment.NewLine;
            this.wrapperTemplate = "return (Response) System.JSON.deserialize(json, Response.class);" + Environment.NewLine;
            this.wrapperTemplate = "}" + Environment.NewLine;
            this.wrapperTemplate = "}" + Environment.NewLine;
            this.wrapperTemplate = "}" + Environment.NewLine;
        }
    }