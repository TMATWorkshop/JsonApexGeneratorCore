using System;

namespace JsonApexGeneratorCore.Helper
{
    public class Templates
    {

        public string wrapperTemplate = "";
        public string wrapperTestTemplate = "";
        public string handlerTemplate = "";
        public string handlerTestTemplate = "";
        public string mockTemplate = "";

        public Templates()
        {
            //setWrapperTemplate();
            //setWrapperTestTemplate();
        }

        private void setWrapperTemplate() {
            this.wrapperTemplate += "/* Class Generated with JSONApexGeneratorCore */" + Environment.NewLine;
            this.wrapperTemplate += "\tpublic class {className}Wrapper {" + Environment.NewLine;
            this.wrapperTemplate += "\t\tpublic class Request {" + Environment.NewLine;
            this.wrapperTemplate += "\t\t{requestJSONWrapperVars}" + Environment.NewLine;
            this.wrapperTemplate += "" + Environment.NewLine;
            this.wrapperTemplate += "\t\t\tpublic Request ({requestParams}) {" + Environment.NewLine;
            this.wrapperTemplate += "\t\t\t\t{requestJSONWrapperVarsThis}" + Environment.NewLine;
            this.wrapperTemplate += "\t\t\t}" + Environment.NewLine;
            this.wrapperTemplate += "\t\t}" + Environment.NewLine;
            this.wrapperTemplate += "" + Environment.NewLine;
            this.wrapperTemplate += "\t\tpublic class Response {" + Environment.NewLine;
            this.wrapperTemplate += "\t\t\t{responseJSONWrapperVars}" + Environment.NewLine;
            this.wrapperTemplate += "\t\t}" + Environment.NewLine;
            this.wrapperTemplate += "" + Environment.NewLine;
            this.wrapperTemplate += "{otherClasses}" + Environment.NewLine;
            this.wrapperTemplate += "" + Environment.NewLine;
            this.wrapperTemplate += "\t\tpublic static Response parseResponse(String json) {" + Environment.NewLine;
            this.wrapperTemplate += "\t\t\treturn (Response) System.JSON.deserialize(json, Response.class);" + Environment.NewLine;
            this.wrapperTemplate += "\t\t}" + Environment.NewLine;
            this.wrapperTemplate += "\t}" + Environment.NewLine;
            this.wrapperTemplate += "}" + Environment.NewLine;
        }

        private void setWrapperTestTemplate() {
            this.wrapperTestTemplate += "/* Class Generated with JSONApexGeneratorCore */" + Environment.NewLine;
            this.wrapperTestTemplate += "@IsTest" + Environment.NewLine;
            this.wrapperTestTemplate += "public class {className}WrapperTest {" + Environment.NewLine;
            this.wrapperTestTemplate += "\t@IsTest" + Environment.NewLine;
            this.wrapperTestTemplate += "\tstatic void testParse() {" + Environment.NewLine;
            this.wrapperTestTemplate += "\t\tString json = '{responseJSON}';" + Environment.NewLine;
            this.wrapperTestTemplate += "" + Environment.NewLine;
            this.wrapperTestTemplate += "\t\tTest.startTest();" + Environment.NewLine;
            this.wrapperTestTemplate += "\t\t{className}Wrapper.Response obj = {className}Wrapper.parseResponse(json);" + Environment.NewLine;
            this.wrapperTestTemplate += "\t\tTest.stopTest();" + Environment.NewLine;
            this.wrapperTestTemplate += "\t\tSystem.assert(obj != null);" + Environment.NewLine;
            this.wrapperTestTemplate += "\t}" + Environment.NewLine;
            this.wrapperTestTemplate += "}" + Environment.NewLine;
        }
    }
}